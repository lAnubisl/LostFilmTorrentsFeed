// <copyright file="ITorrentFileDAO.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2021 Alexander Panfilenok
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

namespace LostFilmMonitoring.DAO.Interfaces
{
    using System.IO;
    using System.Threading.Tasks;
    using LostFilmMonitoring.DAO.Interfaces.DomainModels;

    /// <summary>
    /// Provides functionality for managing torrent files on disk.
    /// </summary>
    public interface ITorrentFileDAO
    {
        /// <summary>
        /// Save torrent file on disk.
        /// </summary>
        /// <param name="fileName">FileName.</param>
        /// <param name="fileContentStream">FileContentStream.</param>
        /// <param name="torrentId">TorrentId.</param>
        /// <returns>Awaitable task.</returns>
        Task SaveAsync(string fileName, Stream fileContentStream, int torrentId);

        /// <summary>
        /// Tries to find torrent file by torrent id.
        /// </summary>
        /// <param name="torrentId">Torrent id.</param>
        /// <returns>TorrentFile if it is found. Otherwise Null.</returns>
        TorrentFile TryFind(int torrentId);
    }
}
