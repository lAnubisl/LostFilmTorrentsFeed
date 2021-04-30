// <copyright file="TorrentFileDownloader.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.BLL
{
    using System;
    using System.Threading.Tasks;
    using LostFilmMonitoring.Common;
    using LostFilmMonitoring.DAO.DAO;
    using LostFilmMonitoring.DAO.DomainModels;
    using LostFilmTV.Client;

    /// <summary>
    /// TorrentFileDownloader.
    /// </summary>
    internal sealed class TorrentFileDownloader
    {
        private readonly TorrentFileDAO torrentFileDAO;
        private readonly Client client;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TorrentFileDownloader"/> class.
        /// </summary>
        /// <param name="torrentFileDAO">TorrentFileDAO.</param>
        /// <param name="logger">Logger.</param>
        internal TorrentFileDownloader(TorrentFileDAO torrentFileDAO, ILogger logger)
        {
            this.logger = logger != null ? logger.CreateScope(nameof(TorrentFileDownloader)) : throw new ArgumentNullException(nameof(logger));
            this.client = new Client(logger);
            this.torrentFileDAO = torrentFileDAO ?? throw new ArgumentNullException(nameof(torrentFileDAO));
        }

        /// <summary>
        /// Downloads torrent file by id for particular user.
        /// </summary>
        /// <param name="user">Current user.</param>
        /// <param name="torrentFileId">Torrent file id.</param>
        /// <returns>Torrent file.</returns>
        internal async Task<TorrentFile> DownloadAsync(User user, int torrentFileId)
        {
            var cachedTorrent = this.torrentFileDAO.TryFind(torrentFileId);
            if (cachedTorrent != null)
            {
                return cachedTorrent;
            }

            await this.DownloadInternal(user, torrentFileId);

            cachedTorrent = this.torrentFileDAO.TryFind(torrentFileId);
            if (cachedTorrent == null)
            {
                this.logger.Error("Cannot download torrent file");
            }

            return cachedTorrent;
        }

        private async Task DownloadInternal(User user, int torrentFileId)
        {
            var torrentFile = await this.client.DownloadTorrentFile(user.Uid, user.Usess, torrentFileId);
            if (torrentFile == null)
            {
                return;
            }

            await this.torrentFileDAO.SaveAsync(torrentFile.FileName, torrentFile.Content, torrentFileId);
        }
    }
}
