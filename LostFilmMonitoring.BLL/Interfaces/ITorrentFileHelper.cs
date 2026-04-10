namespace LostFilmMonitoring.BLL.Interfaces;

/// <summary>
/// Helper for working with torrent files.
/// </summary>
public interface ITorrentFileHelper
{
    /// <summary>
    /// Parses a torrent file from a stream.
    /// </summary>
    /// <param name="stream">Stream containing torrent file data.</param>
    /// <returns>A parsed torrent representation.</returns>
    IParsedTorrent Parse(Stream stream);
}
