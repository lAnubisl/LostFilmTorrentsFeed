// <copyright file="ITorrentFileDAO.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2023 Alexander Panfilenok
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the 'Software'), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>

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
