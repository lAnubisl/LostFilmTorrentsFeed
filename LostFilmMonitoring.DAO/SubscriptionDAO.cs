// <copyright file="SubscriptionDAO.cs" company="Alexander Panfilenok">
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
    using System.Linq;
    using System.Threading.Tasks;
    using LostFilmMonitoring.Common;
    using LostFilmMonitoring.DAO.Interfaces;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Provides functionality for managing subscription in storage.
    /// </summary>
    public class SubscriptionDAO : BaseDAO, ISubscriptionDAO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionDAO"/> class.
        /// </summary>
        /// <param name="configuration">IConfiguration.</param>
        public SubscriptionDAO(IConfiguration configuration)
            : base(configuration.SqlServerConnectionString)
        {
        }

        /// <inheritdoc/>
        public async Task<string[]> LoadUsersIdsAsync(string seriesName, string quality)
        {
            using var ctx = this.OpenContext();
            return await ctx.Subscriptions.Where(s => s.SeriesName == seriesName && s.Quality == quality).Select(s => s.UserId.ToString()).ToArrayAsync();
        }

        /// <inheritdoc/>
        public async Task SaveAsync(string userId, Interfaces.DomainModels.Subscription[] subscriptions)
        {
            var id = Guid.Parse(userId);
            using var ctx = this.OpenContext();
            var existingSubscriptions = ctx.Subscriptions.Where(s => s.UserId == id).ToList();
            ctx.RemoveRange(existingSubscriptions.Where(e => subscriptions.All(s => s.SeriesName != e.SeriesName)));
            foreach (var subscription in subscriptions)
            {
                AddOrUpdate(ctx, subscription, existingSubscriptions, id);
            }

            await ctx.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<Interfaces.DomainModels.Subscription[]> LoadAsync(string userId)
        {
            var id = Guid.Parse(userId);
            using var ctx = this.OpenContext();
            return (await ctx.Subscriptions.Where(s => s.UserId == id).ToArrayAsync()).Select(Mapper.Map).ToArray();
        }

        private static void AddOrUpdate(LostFilmDbContext ctx, Interfaces.DomainModels.Subscription subscription, List<DomainModels.Subscription> existingSubscriptions, Guid userId)
        {
            var existingSubscription = existingSubscriptions.FirstOrDefault(s => s.SeriesName == subscription.SeriesName);
            if (existingSubscription == null)
            {
                ctx.Add(Mapper.Map(subscription, userId));
            }
            else
            {
                existingSubscription.Quality = subscription.Quality;
            }
        }
    }
}
