namespace LostFilmMonitoring.BLL.Interfaces;

/// <summary>
/// Interface that provides access to RSS feed.
/// </summary>
public interface IRssFeed
{
    /// <summary>
    /// Get RSS feed items.
    /// </summary>
    /// <returns>Awaitable task with RSS feed items.</returns>
    Task<SortedSet<FeedItemResponse>?> LoadFeedItemsAsync();
}
