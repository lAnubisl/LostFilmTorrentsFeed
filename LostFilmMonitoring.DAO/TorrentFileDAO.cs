// <copyright file="TorrentFileDAO.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.Sql
{
    using System.IO;
    using System.Threading.Tasks;
    using LostFilmMonitoring.Common;
    using LostFilmMonitoring.DAO.Interfaces;

    /// <summary>
    /// Provides functionality for managing torrent files on disk.
    /// </summary>
    public class TorrentFileDAO : ITorrentFileDAO
    {
        private readonly string torrentFilesDirectoryPath;
        private readonly ILogger logger;
        private readonly string baseTorrentsDirectory = "basetorrents";
        private readonly string userTorrentsDirectory = "usertorrents";

        /// <summary>
        /// Initializes a new instance of the <see cref="TorrentFileDAO"/> class.
        /// </summary>
        /// <param name="configuration">IConfiguration.</param>
        /// <param name="logger">Logger.</param>
        public TorrentFileDAO(IConfiguration configuration, ILogger logger)
        {
            this.torrentFilesDirectoryPath = configuration.TorrentsDirectory;
            this.logger = logger.CreateScope(nameof(TorrentFileDAO));
        }

        /// <inheritdoc/>
        public Task<Interfaces.DomainModels.TorrentFile> LoadBaseFileAsync(string torrentId)
        {
            this.logger.Info($"Call: {nameof(this.LoadBaseFileAsync)}('{torrentId}')");
            var fileName = GetBaseTorrentFileName(torrentId);
            var filePath = Path.Combine(this.torrentFilesDirectoryPath, this.baseTorrentsDirectory, fileName);
            return Task.FromResult(File.Exists(filePath)
                ? new Interfaces.DomainModels.TorrentFile(fileName, File.OpenRead(filePath))
                : null);
        }

        /// <inheritdoc/>
        public async Task SaveBaseFileAsync(string torrentId, Interfaces.DomainModels.TorrentFile torrentFile)
        {
            var fileName = GetBaseTorrentFileName(torrentId);
            var filePath = Path.Combine(this.torrentFilesDirectoryPath, this.baseTorrentsDirectory, fileName);
            if (!File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
            await torrentFile.Stream.CopyToAsync(fs);
        }

        /// <inheritdoc/>
        public async Task SaveUserFileAsync(string userId, Interfaces.DomainModels.TorrentFile torrentFile)
        {
            var filePath = Path.Combine(this.torrentFilesDirectoryPath, this.userTorrentsDirectory, userId, $"{torrentFile.FileName}.torrent");
            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
            await torrentFile.Stream.CopyToAsync(fs);
        }

        private static string GetBaseTorrentFileName(string torrentId) => $"{torrentId}.torrent";

        public Task DeleteUserFileAsync(string userId, string torrentFileName)
        {
            var filePath = Path.Combine(this.torrentFilesDirectoryPath, this.userTorrentsDirectory, userId, torrentFileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return Task.CompletedTask;
        }
    }
}
