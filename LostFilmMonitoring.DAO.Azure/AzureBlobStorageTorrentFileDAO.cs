namespace LostFilmMonitoring.DAO.Azure;

/// <summary>
/// Implements <see cref="ITorrentFileDao"/> for Azure Blob Storage.
/// </summary>
public class AzureBlobStorageTorrentFileDao : ITorrentFileDao
{
    private readonly IAzureBlobStorageClient azureBlobStorageClient;
    private readonly ILogger logger;
    private readonly string baseTorrentsDirectory = Constants.MetadataStorageContainerBaseTorrents;
    private readonly string userTorrentsDirectory = Constants.MetadataStorageContainerUserTorrents;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureBlobStorageTorrentFileDao"/> class.
    /// </summary>
    /// <param name="azureBlobStorageClient">Instance of AzureBlobStorageClient.</param>
    /// <param name="logger">Instance of ILogger.</param>
    public AzureBlobStorageTorrentFileDao(IAzureBlobStorageClient azureBlobStorageClient, ILogger logger)
    {
        this.azureBlobStorageClient = azureBlobStorageClient;
        this.logger = logger.CreateScope(nameof(AzureBlobStorageTorrentFileDao));
    }

    /// <inheritdoc/>
    public Task DeleteUserFileAsync(string userId, string torrentFileName)
    {
        this.logger.Info($"Call: {nameof(this.DeleteUserFileAsync)}('{userId}', '{torrentFileName}')");
        return this.azureBlobStorageClient.DeleteAsync(this.userTorrentsDirectory, userId, torrentFileName);
    }

    /// <inheritdoc/>
    public async Task<TorrentFile?> LoadBaseFileAsync(string torrentId)
    {
        this.logger.Info($"Call: {nameof(this.LoadBaseFileAsync)}('{torrentId}')");
        var name = GetBaseTorrentFileName(torrentId);
        var fileStream = await this.azureBlobStorageClient.DownloadAsync(this.baseTorrentsDirectory, name);
        return fileStream == null ? null : new TorrentFile(name, fileStream);
    }

    /// <inheritdoc/>
    public Task SaveBaseFileAsync(string torrentId, TorrentFile torrentFile)
    {
        this.logger.Info($"Call: {nameof(this.SaveBaseFileAsync)}('{torrentId}', TorrentFile torrentFile)");
        return this.azureBlobStorageClient.UploadAsync(this.baseTorrentsDirectory, GetBaseTorrentFileName(torrentId), torrentFile.Stream, "applications/x-bittorrent");
    }

    /// <inheritdoc/>
    public Task SaveUserFileAsync(string userId, TorrentFile torrentFile)
    {
        this.logger.Info($"Call: {nameof(this.SaveUserFileAsync)}('{userId}', TorrentFile torrentFile)");
        return this.azureBlobStorageClient.UploadAsync(this.userTorrentsDirectory, userId, $"{torrentFile.FileName}.torrent", torrentFile.Stream, "applications/x-bittorrent");
    }

    private static string GetBaseTorrentFileName(string torrentId) => $"{torrentId}.torrent";
}
