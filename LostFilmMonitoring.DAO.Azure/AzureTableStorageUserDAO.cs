namespace LostFilmMonitoring.DAO.Azure;

/// <summary>
/// Implements <see cref="IUserDao"/> for Azure Table Storage.
/// </summary>
public class AzureTableStorageUserDao : BaseAzureTableStorageDao, IUserDao
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureTableStorageUserDao"/> class.
    /// </summary>
    /// <param name="tableServiceClient">Instance of Azure.Data.Tables.TableServiceClient.</param>
    /// <param name="logger">Instance of Logger.</param>
    public AzureTableStorageUserDao(TableServiceClient tableServiceClient, ILogger logger)
        : base(tableServiceClient, Constants.MetadataStorageTableNameUsers, logger?.CreateScope(nameof(AzureTableStorageUserDao)))
    {
    }

    /// <inheritdoc/>
    public Task<User?> LoadAsync(string userId)
    {
        this.Logger.Info($"Call: {nameof(this.LoadAsync)}('{userId}')");
        return this.TryGetEntityAsync(async (tc) =>
        {
            var response = await tc.GetEntityAsync<UserTableEntity>(userId, userId);
            return Mapper.Map(response.Value);
        });
    }

    /// <inheritdoc/>`
    public async Task<User[]> LoadAsync()
    {
        this.Logger.Info($"Call: {nameof(this.LoadAsync)}()");
        return await this.TryGetEntityAsync(tc =>
        {
            var query = tc.QueryAsync<UserTableEntity>();
            return IterateAsync(query, Mapper.Map);
        }) ?? [];
    }

    /// <inheritdoc/>
    public async Task SaveAsync(User user)
    {
        this.Logger.Info($"Call: {nameof(this.SaveAsync)}(User user)");
        try
        {
            await this.TryExecuteAsync(c => c.UpsertEntityAsync(Mapper.Map(user)));
        }
        catch (ExternalServiceUnavailableException)
        {
            this.Logger.Fatal($"Error while saving user: '{JsonSerializer.Serialize(user, CommonSerializationOptions.Default)}'");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(string userId)
    {
        this.Logger.Info($"Call: {nameof(this.DeleteAsync)}('{userId}')");
        await this.TryExecuteAsync(c => c.DeleteEntityAsync(EscapeKey(userId), userId));
    }
}