namespace LostFilmMonitoring.DAO.Azure;

/// <summary>
/// Implements <see cref="ISeriesDao"/> for Azure Table Storage.
/// </summary>
public class AzureTableStorageSeriesDao : BaseAzureTableStorageDao, ISeriesDao
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureTableStorageSeriesDao"/> class.
    /// </summary>
    /// <param name="tableServiceClient">Instance of Azure.Data.Tables.TableServiceClient.</param>
    /// <param name="logger">Instance of Logger.</param>
    public AzureTableStorageSeriesDao(TableServiceClient tableServiceClient, ILogger logger)
        : base(tableServiceClient, Constants.MetadataStorageTableNameSeries, logger?.CreateScope(nameof(AzureTableStorageUserDao)))
    {
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Series? series)
    {
        this.Logger.Info($"Call: {nameof(this.DeleteAsync)}(series)");

        if (series == null)
        {
            return;
        }

        try
        {
            await this.TryExecuteAsync(c => c.DeleteEntityAsync(EscapeKey(series.Name), EscapeKey(series.Name)));
        }
        catch (ExternalServiceUnavailableException ex)
        {
            this.Logger.Log($"Error deleting series (Name='{series.Name}')", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<Series?> LoadAsync(string name)
    {
        this.Logger.Info($"Call: {nameof(this.LoadAsync)}('{name}')");
        return await this.TryGetEntityAsync(async (tc) =>
        {
            var response = await tc.GetEntityAsync<SeriesTableEntity>(name, name);
            return Mapper.Map(response.Value);
        });
    }

    /// <inheritdoc/>
    public async Task<Series[]> LoadAsync()
    {
        this.Logger.Info($"Call: {nameof(this.LoadAsync)}()");
        return await this.TryGetEntityAsync(async (tc) =>
        {
            var result = new List<Series>();
            await foreach (var item in tc.QueryAsync<SeriesTableEntity>())
            {
                result.Add(Mapper.Map(item));
            }

            return result.ToArray();
        }) ?? [];
    }

    /// <inheritdoc/>
    public async Task<Guid> SaveAsync(Series series)
    {
        this.Logger.Info($"Call: {nameof(this.SaveAsync)}(Series series)");
        try
        {
            var entity = Mapper.Map(series);
            await this.TryExecuteAsync(c => c.UpsertEntityAsync(entity));
            return entity.Id;
        }
        catch (ExternalServiceUnavailableException)
        {
            this.Logger.Error($"Error saving series (Name='{series.Name}')");
            throw;
        }
    }
}