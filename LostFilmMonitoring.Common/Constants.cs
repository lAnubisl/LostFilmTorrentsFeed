namespace LostFilmMonitoring.Common;

/// <summary>
/// Constants.
/// </summary>
public static class Constants
{
    /// <summary>
    /// Table Storage dictionary table name.
    /// </summary>
    public static readonly string MetadataStorageTableNameDictionary = "dictionary";

    /// <summary>
    /// Table Storage episodes table name.
    /// </summary>
    public static readonly string MetadataStorageTableNameEpisodes = "episodes";

    /// <summary>
    /// Table Storage series table name.
    /// </summary>
    public static readonly string MetadataStorageTableNameSeries = "series";

    /// <summary>
    /// Table Storage subscriptions table name.
    /// </summary>
    public static readonly string MetadataStorageTableNameSubscriptions = "subscriptions";

    /// <summary>
    /// Table Storage users table name.
    /// </summary>
    public static readonly string MetadataStorageTableNameUsers = "users";

    /// <summary>
    /// Blob Storage container name for images.
    /// </summary>
    public static readonly string MetadataStorageContainerImages = "images";

    /// <summary>
    /// Blob Storage container name for base torrents.
    /// </summary>
    public static readonly string MetadataStorageContainerBaseTorrents = "basetorrents";

    /// <summary>
    /// Blob Storage container name for models.
    /// </summary>
    public static readonly string MetadataStorageContainerModels = "models";

    /// <summary>
    /// Blob Storage container name for RSS feeds.
    /// </summary>
    public static readonly string MetadataStorageContainerRssFeeds = "rssfeeds";

    /// <summary>
    /// Blob Storage container name for user torrents.
    /// </summary>
    public static readonly string MetadataStorageContainerUserTorrents = "usertorrents";
}