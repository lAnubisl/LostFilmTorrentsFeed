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

namespace LostFilmMonitoring.DAO.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using LostFilmMonitoring.Common;
    using LostFilmMonitoring.DAO.Interfaces;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Provides functionality for managing user's feeds in storage.
    /// </summary>
    public class FeedDAO : BaseDAO, IFeedDAO
    {
        private static readonly string BaseFeedId = "96AFC9E2-C64D-4DDD-9003-016974D32100";

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedDAO"/> class.
        /// </summary>
        /// <param name="configuration">IConfiguration.</param>
        public FeedDAO(IConfiguration configuration)
            : base(configuration.SqlServerConnectionString)
        {
        }

        public async Task<string> LoadFeedRawAsync(string userId)
        {
            var id = Guid.Parse(userId);
            using var ctx = this.OpenContext();
            return (await ctx.Feeds.FirstOrDefaultAsync(f => f.UserId == id))?.Data;
        }

        public async Task DeleteAsync(string userId)
        {
            var id = Guid.Parse(userId);
            using var ctx = this.OpenContext();
            var feed = await ctx.Feeds.FirstOrDefaultAsync(f => f.UserId == id);
            if (feed == null)
            {
                return;
            }

            ctx.Feeds.Remove(feed);
            await ctx.SaveChangesAsync();
        }

        public Task<SortedSet<Interfaces.DomainModels.FeedItem>> LoadUserFeedAsync(string userId)
        {
            return this.LoadFeedAsync(userId);
        }

        public Task<SortedSet<Interfaces.DomainModels.FeedItem>> LoadBaseFeedAsync()
        {
            return this.LoadFeedAsync(BaseFeedId);
        }

        public Task SaveUserFeedAsync(string userId, Interfaces.DomainModels.FeedItem[] items)
        {
            return this.SaveFeedAsync(userId, items);
        }

        public Task SaveBaseFeedAsync(Interfaces.DomainModels.FeedItem[] items)
        {
            return this.SaveFeedAsync(BaseFeedId, items);
        }

        private async Task<SortedSet<Interfaces.DomainModels.FeedItem>> LoadFeedAsync(string userId)
        {
            var data = await this.LoadFeedRawAsync(userId);
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }

            var document = XDocument.Parse(data);
            return document.GetItems();
        }

        private async Task SaveFeedAsync(string userId, Interfaces.DomainModels.FeedItem[] items)
        {
            var id = Guid.Parse(userId);
            using (var ctx = this.OpenContext())
            {
                var entity = await ctx.Feeds.FirstOrDefaultAsync(f => f.UserId == id);
                if (entity == null)
                {
                    entity = new DomainModels.Feed()
                    {
                        UserId = id,
                    };
                    ctx.Feeds.Add(entity);
                }

                entity.Data = items.GenerateXml();
                ctx.SaveChanges();
            }
        }
    }
}
