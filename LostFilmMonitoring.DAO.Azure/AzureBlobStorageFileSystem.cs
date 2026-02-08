namespace LostFilmMonitoring.DAO.Azure;

/// <summary>
/// Implements <see cref="IFileSystem"/> for Azure Blob Storage.
/// </summary>
public class AzureBlobStorageFileSystem : IFileSystem
{
    private readonly IAzureBlobStorageClient azureBlobStorageClient;
    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureBlobStorageFileSystem"/> class.
    /// </summary>
    /// <param name="azureBlobStorageClient">Instance of AzureBlobStorageClient.</param>
    /// <param name="logger">Instance of Logger.</param>
    public AzureBlobStorageFileSystem(IAzureBlobStorageClient azureBlobStorageClient, ILogger logger)
    {
        this.azureBlobStorageClient = azureBlobStorageClient ?? throw new ArgumentNullException(nameof(azureBlobStorageClient));
        this.logger = logger?.CreateScope(nameof(AzureBlobStorageFileSystem)) ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public Task<bool> ExistsAsync(string directory, string fileName)
    {
        this.logger.Info($"Call: {nameof(this.ExistsAsync)}({directory}, {fileName})");
        return this.azureBlobStorageClient.ExistsAsync(directory, fileName);
    }

    /// <inheritdoc/>
    public Task SaveAsync(string directory, string fileName, string contentType, Stream contentStream)
    {
        this.logger.Info($"Call: {nameof(this.SaveAsync)}({directory}, {fileName}, {contentStream})");
        return this.azureBlobStorageClient.UploadAsync(directory, fileName, contentStream, contentType);
    }
}
