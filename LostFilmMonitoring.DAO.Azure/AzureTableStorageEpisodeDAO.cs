namespace LostFilmMonitoring.DAO.Azure;

/// <summary>
/// Implements <see cref="IEpisodeDao"/> for Azure Table Storage.
/// </summary>
public class AzureTableStorageEpisodeDao : BaseAzureTableStorageDao, IEpisodeDao
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureTableStorageEpisodeDao"/> class.
    /// </summary>
    /// <param name="tableServiceClient">Instance of Azure.Data.Tables.TableServiceClient.</param>
    /// <param name="logger">Instance of Logger.</param>
    public AzureTableStorageEpisodeDao(TableServiceClient tableServiceClient, ILogger? logger)
        : base(tableServiceClient, Constants.MetadataStorageTableNameEpisodes, logger)
    {
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(string seriesName, int seasonNumber, int episideNumber, string quality)
    {
        this.Logger.Info($"Call: {nameof(this.ExistsAsync)}('{seriesName}', {seasonNumber}, {episideNumber}, '{quality}')");
        return (await this.TryCountAsync(tc =>
        {
            var query = tc.QueryAsync<EpisodeTableEntity>(entity =>
                entity.PartitionKey == EscapeKey(seriesName) &&
                entity.Quality == quality &&
                entity.SeasonNumber == seasonNumber &&
                entity.EpisodeNumber == episideNumber);
            return CountAsync(query);
        })) > 0;
    }

    /// <inheritdoc/>
    public async Task<Episode[]> LoadAsync()
    {
        this.Logger.Info($"Call: {nameof(this.LoadAsync)}()");
        return await this.TryGetEntityAsync(tc =>
        {
            var query = tc.QueryAsync<EpisodeTableEntity>();
            return IterateAsync(query, Mapper.Map);
        }) ?? [];
    }

    /// <inheritdoc/>
    public async Task SaveAsync(Episode episode)
    {
        this.Logger.Info($"Call: {nameof(this.SaveAsync)}(Episode episode)");
        try
        {
            await this.TryExecuteAsync(c => c.UpsertEntityAsync(Mapper.Map(episode)));
        }
        catch (ExternalServiceUnavailableException)
        {
            this.Logger.Fatal($"Error while saving episode: '{JsonSerializer.Serialize(episode, CommonSerializationOptions.Default)}'");
            throw;
        }
    }
}