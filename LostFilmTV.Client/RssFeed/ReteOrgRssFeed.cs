namespace LostFilmTV.Client.RssFeed;

/// <summary>
/// ReteOrgRssFeedService.
/// </summary>
public class ReteOrgRssFeed : BaseRssFeed, IRssFeed
{
    private const string RssUrl = "https://insearch.site/rssdd.xml";

    /// <summary>
    /// Initializes a new instance of the <see cref="ReteOrgRssFeed"/> class.
    /// </summary>
    /// <param name="logger">Logger.</param>
    /// <param name="httpClientFactory">httpClientFactory.</param>
    public ReteOrgRssFeed(ILogger logger, IHttpClientFactory httpClientFactory)
        : base(logger?.CreateScope(nameof(ReteOrgRssFeed)) ?? throw new ArgumentNullException(nameof(logger)), httpClientFactory)
    {
    }

    /// <summary>
    /// Get FeedItemResponse from n.retre.org.
    /// </summary>
    /// <returns>Set of FeedItemResponse.</returns>
    public async Task<SortedSet<FeedItemResponse>?> LoadFeedItemsAsync()
    {
        this.Logger.Info($"Call: {nameof(this.LoadFeedItemsAsync)}()");
        string rss;
        try
        {
            rss = await this.DownloadRssTextAsync(RssUrl);
        }
        catch (RemoteServiceUnavailableException)
        {
            return new SortedSet<FeedItemResponse>();
        }

        return this.GetItems(rss);
    }
}
