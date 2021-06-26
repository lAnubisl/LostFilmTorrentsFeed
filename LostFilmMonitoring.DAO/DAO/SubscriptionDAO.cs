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

namespace LostFilmMonitoring.DAO.DAO
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using LostFilmMonitoring.Common;
    using LostFilmMonitoring.DAO.DomainModels;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Provides functionality for managing subscription in storage.
    /// </summary>
    public class SubscriptionDAO : BaseDAO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionDAO"/> class.
        /// </summary>
        /// <param name="configuration">IConfiguration.</param>
        public SubscriptionDAO(IConfiguration configuration)
            : base(configuration.ConnectionString)
        {
        }

        /// <summary>
        /// Load subscriptions by series name and quality.
        /// </summary>
        /// <param name="seriesName">SeriesName.</param>
        /// <param name="quality">Quality.</param>
        /// <returns>Subscriptions.</returns>
        public async Task<Subscription[]> LoadAsync(string seriesName, string quality)
        {
            using (var ctx = this.OpenContext())
            {
                return await ctx.Subscriptions.Where(s => s.SeriesName == seriesName && s.Quality == quality).ToArrayAsync();
            }
        }

        /// <summary>
        /// Save subscription.
        /// </summary>
        /// <param name="userId">UserId.</param>
        /// <param name="subscriptions">Subscriptions.</param>
        /// <returns>Awaitable task.</returns>
        public async Task SaveAsync(Guid userId, Subscription[] subscriptions)
        {
            if (subscriptions == null)
            {
                subscriptions = new Subscription[0];
            }

            using (var ctx = this.OpenContext())
            {
                var existingSubscriptions = ctx.Subscriptions.Where(s => s.UserId == userId).ToList();
                ctx.RemoveRange(existingSubscriptions.Where(e => subscriptions.All(s => s.SeriesName != e.SeriesName)));
                foreach (var subscription in subscriptions)
                {
                    subscription.UserId = userId;
                    var existingSubscription = existingSubscriptions.FirstOrDefault(s => s.SeriesName == subscription.SeriesName);
                    if (existingSubscription == null)
                    {
                        ctx.Add(subscription);
                    }
                    else
                    {
                        existingSubscription.Quality = subscription.Quality;
                    }
                }

                await ctx.SaveChangesAsync();
            }
        }
    }
}
