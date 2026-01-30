namespace LostFilmMonitoring.Common;

/// <summary>
/// Configuration.
/// </summary>
public interface IConfiguration
{
    /// <summary>
    /// Gets Base torrent tracker user identifier.
    /// </summary>
    /// <returns>Base torrent tracker user identifier.</returns>
    string BaseUID { get; }

    /// <summary>
    /// Gets BaseFeedCookie.
    /// </summary>
    /// <returns>BaseFeedCookie.</returns>
    string BaseUSESS { get; }

    /// <summary>
    /// Gets base url where website is hosted.
    /// </summary>
    /// <returns>Base url where website is hosted.</returns>
    string BaseUrl { get; }

    /// <summary>
    /// Get list of torrent trackers for torrent file.
    /// </summary>
    /// <param name="link_uid">Torrent tracker user identifier.</param>
    /// <returns>List of torrent trackers for torrent file.</returns>
    string[] GetTorrentAnnounceList(string link_uid);
}
