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
    /// Command for downloading cover images.
    /// </summary>
    public static readonly string DownloadCoverImageCommand = "DownloadCoverImageCommand";

    /// <summary>
    /// Command for downloading cover images for multiple series.
    /// </summary>
    public static readonly string DownloadCoverImagesCommand = "DownloadCoverImagesCommand";

    /// <summary>
    /// Command for getting user information.
    /// </summary>
    public static readonly string GetUserCommand = "GetUserCommand";

    /// <summary>
    /// Command for saving subscription.
    /// </summary>
    public static readonly string SaveSubscriptionCommand = "SaveSubscriptionCommand";

    /// <summary>
    /// Command for saving user.
    /// </summary>
    public static readonly string SaveUserCommand = "SaveUserCommand";

    /// <summary>
    /// Command for signing in user.
    /// </summary>
    public static readonly string SignInCommand = "SignInCommand";

    /// <summary>
    /// Command for updating user feed.
    /// </summary>
    public static readonly string UpdateUserFeedCommand = "UpdateUserFeedCommand";

    /// <summary>
    /// Name of the activity source for RSS feeds.
    /// </summary>
    public static readonly string RssFeed = "RssFeed";

    /// <summary>
    /// Name of the activity source for Downloading Torrent File.
    /// </summary>
    public static readonly string DownloadTorrent = "DownloadTorrent";

    /// <summary>
    /// Gets an array of activity source names used for monitoring and logging purposes.
    /// </summary>
    /// <value>
    /// An array of strings representing the names of activity sources, such as BlobStorage, TableStorage, and UpdateFeedsCommand.
    /// </value>
    public static string[] ActivitySources =>
    [
        BlobStorage,
        TableStorage,
        UpdateFeedsCommand,
        DownloadCoverImageCommand,
        DownloadCoverImagesCommand,
        GetUserCommand,
        SaveSubscriptionCommand,
        SaveUserCommand,
        SignInCommand,
        UpdateUserFeedCommand,
        RssFeed
    ];
}
