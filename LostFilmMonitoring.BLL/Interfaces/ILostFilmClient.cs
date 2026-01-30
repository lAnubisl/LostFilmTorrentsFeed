namespace LostFilmMonitoring.BLL.Interfaces;

/// <summary>
/// LostFilmTV client interface.
/// </summary>
public interface ILostFilmClient
{
    /// <summary>
    /// Downloads the torrent file asynchronous.
    /// </summary>
    /// <param name="uid">The uid.</param>
    /// <param name="usess">The usess.</param>
    /// <param name="torrentFileId">The torrent file identifier.</param>
    /// <returns>Torrent file response container.</returns>
    Task<ITorrentFileResponse?> DownloadTorrentFileAsync(string uid, string usess, string torrentFileId);
}
