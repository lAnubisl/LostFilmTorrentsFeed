namespace LostFilmMonitoring.Common;

/// <summary>
/// Contains all activity source names for an application.
/// </summary>
public static class ActivitySourceNames
{
    /// <summary>
    /// Name of the activity source for Azure Blob Storage.
    /// </summary>
    public static readonly string BlobStorage = "BlobStorage";

    /// <summary>
    /// Name of the activity source for Azure Table Storage.
    /// </summary>
    public static readonly string TableStorage = "TableStorage";

    /// <summary>
    /// Command for updating feeds.
    /// </summary>
    public static readonly string UpdateFeedsCommand = "UpdateFeedsCommand";

    /// <summary>
    /// Name of the activity source for RSS feeds.
    /// </summary>
    public static readonly string RssFeed = "RssFeed";

    /// <summary>
    /// Gets an array of activity source names used for monitoring and logging purposes.
    /// </summary>
    /// <value>
    /// An array of strings representing the names of activity sources, such as BlobStorage, TableStorage, and UpdateFeedsCommand.
    /// </value>
    public static string[] ActivitySources =>
    [
        BlobStorage, TableStorage, UpdateFeedsCommand, RssFeed
    ];
}
