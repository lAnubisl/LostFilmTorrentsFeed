namespace LostFilmMonitoring.DAO.Azure;

/// <summary>
/// Implements <see cref="IModelPersister"/> for Azure Blob Storage.
/// </summary>
public class AzureBlobStorageModelPersister : IModelPersister
{
    private readonly IAzureBlobStorageClient azureBlobStorageClient;
    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureBlobStorageModelPersister"/> class.
    /// </summary>
    /// <param name="azureBlobStorageClient">Instance of AzureBlobStorageClient.</param>
    /// <param name="logger">Instance of Logger.</param>
    public AzureBlobStorageModelPersister(IAzureBlobStorageClient azureBlobStorageClient, ILogger logger)
    {
        this.azureBlobStorageClient = azureBlobStorageClient ?? throw new ArgumentNullException(nameof(azureBlobStorageClient));
        this.logger = logger?.CreateScope(nameof(AzureBlobStorageModelPersister)) ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<T?> LoadAsync<T>(string modelName)
        where T : class
    {
        this.logger.Info($"Call: {nameof(this.LoadAsync)}('{modelName}')");
        try
        {
            var json = await this.azureBlobStorageClient.DownloadStringAsync("models", modelName + ".json");
            if (json == null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<T>(json);
        }
        catch (ExternalServiceUnavailableException ex)
        {
            this.logger.Log($"Cannot load model '{modelName}'.", ex);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task PersistAsync<T>(string modelName, T model)
    {
        this.logger.Info($"Call: {nameof(this.PersistAsync)}('{modelName}', model)");
        try
        {
            await this.azureBlobStorageClient.UploadAsync("models", $"{modelName}.json", JsonSerializer.Serialize(model, CommonSerializationOptions.Default), "application/json");
        }
        catch (ExternalServiceUnavailableException ex)
        {
            this.logger.Log($"Cannot persist model '{modelName}'.", ex);
        }
    }
}
