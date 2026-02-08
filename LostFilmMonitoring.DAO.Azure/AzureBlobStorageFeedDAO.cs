namespace LostFilmMonitoring.DAO.Azure;

/// <summary>
/// Implements <see cref="IFeedDao"/> for Azure Blob Storage.
/// </summary>
public class AzureBlobStorageFeedDao : IFeedDao
{
    private static readonly string ConteinerName = Constants.MetadataStorageContainerRssFeeds;
    private static readonly string BaseFeedName = "baseFeed.xml";
    private readonly IAzureBlobStorageClient azureBlobStorageClient;
    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureBlobStorageFeedDao"/> class.
    /// </summary>
    /// <param name="azureBlobStorageClient">Instance of AzureBlobStorageClient.</param>
    /// <param name="logger">Instance of ILogger.</param>
    public AzureBlobStorageFeedDao(IAzureBlobStorageClient azureBlobStorageClient, ILogger logger)
    {
        this.azureBlobStorageClient = azureBlobStorageClient;
        this.logger = logger.CreateScope(nameof(AzureBlobStorageFeedDao));
    }

    /// <inheritdoc/>
    public Task DeleteAsync(string userId)
    {
        this.logger.Info($"Call: {nameof(this.DeleteAsync)}('{userId}')");
        return this.azureBlobStorageClient.DeleteAsync(ConteinerName, userId);
    }

    /// <inheritdoc/>
    public async Task<SortedSet<FeedItem>> LoadBaseFeedAsync()
    {
        this.logger.Info($"Call: {nameof(this.LoadBaseFeedAsync)}()");
        return ToSortedSet(await this.azureBlobStorageClient.DownloadStringAsync(ConteinerName, BaseFeedName));
    }

    /// <inheritdoc/>
    public Task<string?> LoadFeedRawAsync(string userId)
    {
        this.logger.Info($"Call: {nameof(this.LoadFeedRawAsync)}('{userId}')");
        return this.azureBlobStorageClient.DownloadStringAsync(ConteinerName, GetUserFeedFileName(userId));
    }

    /// <inheritdoc/>
    public async Task<SortedSet<FeedItem>> LoadUserFeedAsync(string userId)
    {
        this.logger.Info($"Call: {nameof(this.LoadUserFeedAsync)}('{userId}')");
        return ToSortedSet(await this.azureBlobStorageClient.DownloadStringAsync(ConteinerName, GetUserFeedFileName(userId)));
    }

    /// <inheritdoc/>
    public Task SaveBaseFeedAsync(FeedItem[] items)
    {
        this.logger.Info($"Call: {nameof(this.SaveBaseFeedAsync)}(FeedItem[])");
        return this.azureBlobStorageClient.UploadAsync(ConteinerName, BaseFeedName, items.GenerateXml(), "text/xml");
    }

    /// <inheritdoc/>
    public Task SaveUserFeedAsync(string userId, FeedItem[] items)
    {
        this.logger.Info($"Call: {nameof(this.SaveUserFeedAsync)}('{userId}', FeedItem[])");
        return this.azureBlobStorageClient.UploadAsync(ConteinerName, GetUserFeedFileName(userId), items.GenerateXml(), "public, max-age=300");
    }

    private static string GetUserFeedFileName(string userId) => $"{userId}.xml";

    private static SortedSet<FeedItem> ToSortedSet(string? xml)
        => xml == null
            ? new SortedSet<FeedItem>()
            : XDocument.Parse(xml).GetItems();
}
