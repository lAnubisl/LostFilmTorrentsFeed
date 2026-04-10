namespace LostFilmMonitoring.BLL.Interfaces;

/// <summary>
/// Represents a parsed torrent file.
/// </summary>
public interface IParsedTorrent
{
    /// <summary>
    /// Gets the display name of the torrent.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Converts the torrent to a <see cref="TorrentFile"/> with the specified tracker announce URLs.
    /// </summary>
    /// <param name="announces">Array of tracker announce URLs.</param>
    /// <returns>A <see cref="TorrentFile"/> instance.</returns>
    TorrentFile ToTorrentFile(string[] announces);
}
