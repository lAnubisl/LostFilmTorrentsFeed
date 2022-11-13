// <copyright file="AzureBlobStorageTorrentFileDAO.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.Azure
{
    /// <summary>
    /// Implements <see cref="ITorrentFileDao"/> for Azure Blob Storage.
    /// </summary>
    public class AzureBlobStorageTorrentFileDao : ITorrentFileDao
    {
        private readonly IAzureBlobStorageClient azureBlobStorageClient;
        private readonly ILogger logger;
        private readonly string baseTorrentsDirectory = "basetorrents";
        private readonly string userTorrentsDirectory = "usertorrents";

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobStorageTorrentFileDao"/> class.
        /// </summary>
        /// <param name="azureBlobStorageClient">Instance of AzureBlobStorageClient.</param>
        /// <param name="logger">Instance of ILogger.</param>
        public AzureBlobStorageTorrentFileDao(IAzureBlobStorageClient azureBlobStorageClient, ILogger logger)
        {
            this.azureBlobStorageClient = azureBlobStorageClient;
            this.logger = logger.CreateScope(nameof(AzureBlobStorageTorrentFileDao));
        }

        /// <inheritdoc/>
        public Task DeleteUserFileAsync(string userId, string torrentFileName)
        {
            this.logger.Info($"Call: {nameof(this.DeleteUserFileAsync)}('{userId}', '{torrentFileName}')");
            return this.azureBlobStorageClient.DeleteAsync(this.userTorrentsDirectory, userId, torrentFileName);
        }

        /// <inheritdoc/>
        public async Task<TorrentFile?> LoadBaseFileAsync(string torrentId)
        {
            this.logger.Info($"Call: {nameof(this.LoadBaseFileAsync)}('{torrentId}')");
            var name = GetBaseTorrentFileName(torrentId);
            var fileStream = await this.azureBlobStorageClient.DownloadAsync(this.baseTorrentsDirectory, name);
            return fileStream == null ? null : new TorrentFile(name, fileStream);
        }

        /// <inheritdoc/>
        public Task SaveBaseFileAsync(string torrentId, TorrentFile torrentFile)
        {
            this.logger.Info($"Call: {nameof(this.SaveBaseFileAsync)}('{torrentId}', TorrentFile torrentFile)");
            return this.azureBlobStorageClient.UploadAsync(this.baseTorrentsDirectory, GetBaseTorrentFileName(torrentId), torrentFile.Stream, "applications/x-bittorrent");
        }

        /// <inheritdoc/>
        public Task SaveUserFileAsync(string userId, TorrentFile torrentFile)
        {
            this.logger.Info($"Call: {nameof(this.SaveUserFileAsync)}('{userId}', TorrentFile torrentFile)");
            return this.azureBlobStorageClient.UploadAsync(this.userTorrentsDirectory, userId, $"{torrentFile.FileName}.torrent", torrentFile.Stream, "applications/x-bittorrent");
        }

        private static string GetBaseTorrentFileName(string torrentId) => $"{torrentId}.torrent";
    }
}
