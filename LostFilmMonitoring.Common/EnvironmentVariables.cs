namespace LostFilmMonitoring.Common;

/// <summary>
/// Constants.
/// </summary>
public static class EnvironmentVariables
{
    /// <summary>
    /// Metadata Storage Account Name.
    /// </summary>
    public static readonly string MetadataStorageAccountName = "MetadataStorageAccountName";

    /// <summary>
    /// Metadata Storage Account Key.
    /// </summary>
    public static readonly string MetadataStorageAccountKey = "MetadataStorageAccountKey";

    /// <summary>
    /// TMDB Api Key.
    /// </summary>
    public static readonly string TmdbApiKey = "TMDB_API_KEY";

    /// <summary>
    /// Base URI for website.
    /// </summary>
    public static readonly string BaseUrl = "BASEURL";

    /// <summary>
    /// LostFilm Base Cookie.
    /// </summary>
    public static readonly string BaseFeedCookie = "BASEFEEDCOOKIE";

    /// <summary>
    /// LostFilm Base User Identifier.
    /// </summary>
    public static readonly string BaseLinkUID = "BASELINKUID";

    /// <summary>
    /// List of torrent trackers.
    /// </summary>
    public static readonly string TorrentTrackers = "TORRENTTRACKERS";
}
