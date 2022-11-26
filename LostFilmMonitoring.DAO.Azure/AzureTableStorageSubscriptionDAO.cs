// <copyright file="AzureTableStorageSubscriptionDAO.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.Azure
{
    /// <summary>
    /// Implements <see cref="ISubscriptionDao"/> for Azure Table Storage.
    /// </summary>
    public class AzureTableStorageSubscriptionDao : BaseAzureTableStorageDao, ISubscriptionDao
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTableStorageSubscriptionDao"/> class.
        /// </summary>
        /// <param name="tableServiceClient">Instance of Azure.Data.Tables.TableServiceClient.</param>
        /// <param name="logger">Instance of Logger.</param>
        public AzureTableStorageSubscriptionDao(TableServiceClient tableServiceClient, ILogger logger)
            : base(tableServiceClient, "subscription", logger?.CreateScope(nameof(AzureTableStorageSubscriptionDao)))
        {
        }

        /// <inheritdoc/>
        public async Task<Subscription[]> LoadAsync(string userId)
        {
            this.Logger.Info($"Call: {nameof(this.LoadAsync)}('{userId}')");
            if (string.IsNullOrEmpty(userId))
            {
                return Array.Empty<Subscription>();
            }

            return await this.TryGetEntityAsync(tc =>
            {
                var query = tc.QueryAsync<SubscriptionTableEntity>(entity => entity.RowKey == userId);
                return IterateAsync(query, Mapper.Map);
            }) ?? Array.Empty<Subscription>();
        }

        /// <inheritdoc/>
        public async Task<string[]> LoadUsersIdsAsync(string seriesName, string quality)
        {
            this.Logger.Info($"Call: {nameof(this.LoadUsersIdsAsync)}('{seriesName}', '{quality}')");
            if (string.IsNullOrEmpty(seriesName) || string.IsNullOrEmpty(quality))
            {
                return Array.Empty<string>();
            }

            return await this.TryGetEntityAsync(tc =>
            {
                var query = tc.QueryAsync<SubscriptionTableEntity>(entity => entity.PartitionKey == seriesName && entity.Quality == quality);
                return IterateAsync(query, item => item.RowKey);
            }) ?? Array.Empty<string>();
        }

        /// <inheritdoc/>
        public async Task SaveAsync(string userId, Subscription[] subscriptions)
        {
            this.Logger.Info($"Call: {nameof(this.SaveAsync)}('{userId}', Subscription[] subscriptions)");
            var storedSubscriptions = await this.LoadAsync(userId);
            var subscriptionsToDelete = storedSubscriptions.Where(ss => subscriptions.All(s => !string.Equals(s.SeriesName, ss.SeriesName, StringComparison.OrdinalIgnoreCase)));
            await Task.WhenAll(subscriptionsToDelete.Select(ss => this.DeleteAsync(userId, ss)));
            await Task.WhenAll(subscriptions.Select(s => this.SaveAsync(userId, s)));
        }

        private async Task DeleteAsync(string userId, Subscription subscription)
        {
            try
            {
                await this.TryExecuteAsync(c => c.DeleteEntityAsync(EscapeKey(subscription.SeriesName), userId));
            }
            catch (ExternalServiceUnavailableException ex)
            {
                this.Logger.Log($"Error deleting subscription (SeriesName='{subscription.SeriesName}', Quality='{subscription.Quality}') for user '{userId}'", ex);
            }
        }

        private async Task SaveAsync(string userId, Subscription subscription)
        {
            if (subscription == null)
            {
                return;
            }

            try
            {
                await this.TryExecuteAsync(c => c.UpsertEntityAsync(Mapper.Map(subscription, userId)));
            }
            catch (ExternalServiceUnavailableException ex)
            {
                this.Logger.Log($"Error saving subscription (SeriesName='{subscription.SeriesName}', Quality='{subscription.Quality}') for user '{userId}'", ex);
            }
        }
    }
}
