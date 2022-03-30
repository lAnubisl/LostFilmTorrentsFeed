// <copyright file="FeedDAO.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2021 Alexander Panfilenok
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the 'Software'), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>

namespace LostFilmMonitoring.DAO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using LostFilmMonitoring.Common;
    using LostFilmMonitoring.DAO.Interfaces;
    using LostFilmMonitoring.DAO.Interfaces.DomainModels;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Provides functionality for managing user's feeds in storage.
    /// </summary>
    public class FeedDAO : BaseDAO, IFeedDAO
    {
        private static readonly Guid BaseFeedId = new Guid("96AFC9E2-C64D-4DDD-9003-016974D32100");

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedDAO"/> class.
        /// </summary>
        /// <param name="configuration">IConfiguration.</param>
        public FeedDAO(IConfiguration configuration)
            : base(configuration.ConnectionString)
        {
        }

        /// <inheritdoc/>
        public async Task<string> LoadFeedRawAsync(Guid userId)
        {
            using (var ctx = this.OpenContext())
            {
                return (await ctx.Feeds.FirstOrDefaultAsync(f => f.UserId == userId))?.Data;
            }
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(Guid userId)
        {
            using (var ctx = this.OpenContext())
            {
                var feed = await ctx.Feeds.FirstOrDefaultAsync(f => f.UserId == userId);
                if (feed == null)
                {
                    return;
                }

                ctx.Feeds.Remove(feed);
                ctx.SaveChanges();
            }
        }

        /// <inheritdoc/>
        public Task<SortedSet<FeedItem>> LoadUserFeedAsync(Guid userId)
        {
            return this.LoadFeedAsync(userId);
        }

        /// <inheritdoc/>
        public Task<SortedSet<FeedItem>> LoadBaseFeedAsync()
        {
            return this.LoadFeedAsync(BaseFeedId);
        }

        /// <inheritdoc/>
        public Task SaveUserFeedAsync(Guid userId, FeedItem[] items)
        {
            return this.SaveFeedAsync(userId, items);
        }

        /// <inheritdoc/>
        public Task SaveBaseFeedAsync(FeedItem[] items)
        {
            return this.SaveFeedAsync(BaseFeedId, items);
        }

        private async Task<SortedSet<FeedItem>> LoadFeedAsync(Guid userId)
        {
            string data = await this.LoadFeedRawAsync(userId);
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }

            var document = XDocument.Parse(data);
            return FeedItem.GetItems(document);
        }

        private async Task SaveFeedAsync(Guid userId, FeedItem[] items)
        {
            using (var ctx = this.OpenContext())
            {
                var entity = await ctx.Feeds.FirstOrDefaultAsync(f => f.UserId == userId);
                if (entity == null)
                {
                    entity = new Feed()
                    {
                        UserId = userId,
                    };
                    ctx.Feeds.Add(entity);
                }

                entity.Data = FeedItem.GenerateXml(items);
                ctx.SaveChanges();
            }
        }
    }
}
