namespace LostFilmMonitoring.DAO.Azure;

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
        : base(tableServiceClient, Constants.MetadataStorageTableNameSubscriptions, logger?.CreateScope(nameof(AzureTableStorageSubscriptionDao)))
    {
    }

    /// <inheritdoc/>
    public async Task<Subscription[]> LoadAsync(string userId)
    {
        this.Logger.Info($"Call: {nameof(this.LoadAsync)}('{userId}')");
        if (string.IsNullOrEmpty(userId))
        {
            return [];
        }

        return await this.TryGetEntityAsync(tc =>
        {
            var query = tc.QueryAsync<SubscriptionTableEntity>(entity => entity.RowKey == userId);
            return IterateAsync(query, Mapper.Map);
        }) ?? [];
    }

    /// <inheritdoc/>
    public async Task<string[]> LoadUsersIdsAsync(string seriesName, string quality)
    {
        this.Logger.Info($"Call: {nameof(this.LoadUsersIdsAsync)}('{seriesName}', '{quality}')");
        if (string.IsNullOrEmpty(seriesName) || string.IsNullOrEmpty(quality))
        {
            return [];
        }

        return await this.TryGetEntityAsync(tc =>
        {
            var query = tc.QueryAsync<SubscriptionTableEntity>(entity => entity.PartitionKey == seriesName && entity.Quality == quality);
            return IterateAsync(query, item => item.RowKey);
        }) ?? [];
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
