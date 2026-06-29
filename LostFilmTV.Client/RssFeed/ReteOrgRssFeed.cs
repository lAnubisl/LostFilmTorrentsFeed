namespace LostFilmTV.Client.RssFeed;

/// <summary>
/// ReteOrgRssFeedService.
/// </summary>
public class ReteOrgRssFeed : BaseRssFeed, IRssFeed
{
    private readonly string rssUrl;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReteOrgRssFeed"/> class.
    /// </summary>
    /// <param name="logger">Logger.</param>
    /// <param name="httpClientFactory">httpClientFactory.</param>
    /// <param name="configuration">Configuration provider.</param>
    public ReteOrgRssFeed(ILogger logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        : base(logger?.CreateScope(nameof(ReteOrgRssFeed)) ?? throw new ArgumentNullException(nameof(logger)), httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        this.rssUrl = configuration.RssFeedUrl;
    }

    /// <summary>
    /// Get FeedItemResponse from n.retre.org.
    /// </summary>
    /// <returns>Set of FeedItemResponse.</returns>
    public async Task<SortedSet<FeedItemResponse>?> LoadFeedItemsAsync()
    {
        this.Logger.Info($"Call: {nameof(this.LoadFeedItemsAsync)}()");
        string rss = string.Empty;
        try
        {
            rss = await this.DownloadRssTextAsync(this.rssUrl);
        }
        catch (RemoteServiceUnavailableException)
        {
            return [];
        }

        return this.GetItems(rss);
    }
}
