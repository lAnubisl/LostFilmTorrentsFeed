namespace LostFilmMonitoring.DAO.Interfaces;

/// <summary>
/// Provides functionality for managing user's feeds in storage.
/// </summary>
public interface IFeedDao
{
    /// <summary>
    /// Loads users rss feed.
    /// </summary>
    /// <param name="userId">User Id.</param>
    /// <returns>Rss feed content.</returns>
    Task<string?> LoadFeedRawAsync(string userId);

    /// <summary>
    /// Delete users rss feed.
    /// </summary>
    /// <param name="userId">UserId.</param>
    /// <returns>Awaitable task.</returns>
    Task DeleteAsync(string userId);

    /// <summary>
    /// Loads user's rss feed in form of items.
    /// </summary>
    /// <param name="userId">UserId.</param>
    /// <returns>Set of FeedItems.</returns>
    Task<SortedSet<FeedItem>> LoadUserFeedAsync(string userId);

    /// <summary>
    /// Loads base rss feed.
    /// </summary>
    /// <returns>Set of FeedItems.</returns>
    Task<SortedSet<FeedItem>> LoadBaseFeedAsync();

    /// <summary>
    /// Save user's feed.
    /// </summary>
    /// <param name="userId">UserId.</param>
    /// <param name="items">FeedItems to save.</param>
    /// <returns>Awaitable task.</returns>
    Task SaveUserFeedAsync(string userId, FeedItem[] items);

    /// <summary>
    /// Save base feed.
    /// </summary>
    /// <param name="items">FeedItems to save.</param>
    /// <returns>Awaitable task.</returns>
    Task SaveBaseFeedAsync(FeedItem[] items);
}
