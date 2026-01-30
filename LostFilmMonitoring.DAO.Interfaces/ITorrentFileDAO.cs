namespace LostFilmMonitoring.DAO.Interfaces;

using System.Threading.Tasks;
using LostFilmMonitoring.DAO.Interfaces.DomainModels;

/// <summary>
/// Provides functionality for managing torrent files on disk.
/// </summary>
public interface ITorrentFileDao
{
    /// <summary>
    /// Save user torrent file to persistent storage.
    /// </summary>
    /// <param name="userId">User Id.</param>
    /// <param name="torrentFile">Torrent File.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task SaveUserFileAsync(string userId, TorrentFile torrentFile);

    /// <summary>
    /// Delete user torrent file from persistent storage.
    /// </summary>
    /// <param name="userId">User Id.</param>
    /// <param name="torrentFileName">Torrent File Name.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task DeleteUserFileAsync(string userId, string torrentFileName);

    /// <summary>
    /// Save base torrent file to persistent storage.
    /// </summary>
    /// <param name="torrentId">TorrentId.</param>
    /// <param name="torrentFile">Torrent File.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task SaveBaseFileAsync(string torrentId, TorrentFile torrentFile);

    /// <summary>
    /// Loaf base torrent file from persistent storage.
    /// </summary>
    /// <param name="torrentId">Torrent id.</param>
    /// <returns>TorrentFile if it is found. Otherwise Null.</returns>
    Task<TorrentFile?> LoadBaseFileAsync(string torrentId);
}
