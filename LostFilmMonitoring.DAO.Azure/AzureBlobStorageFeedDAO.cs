// <copyright file="AzureBlobStorageFeedDAO.cs" company="Alexander Panfilenok">
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
    /// Implements <see cref="IFeedDao"/> for Azure Blob Storage.
    /// </summary>
    public class AzureBlobStorageFeedDao : IFeedDao
    {
        private static readonly string ConteinerName = "rssfeeds";
        private static readonly string BaseFeedName = "baseFeed.xml";
        private readonly IAzureBlobStorageClient azureBlobStorageClient;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobStorageFeedDao"/> class.
        /// </summary>
        /// <param name="azureBlobStorageClient">Instance of AzureBlobStorageClient.</param>
        /// <param name="logger">Instance of ILogger.</param>
        public AzureBlobStorageFeedDao(IAzureBlobStorageClient azureBlobStorageClient, ILogger logger)
        {
            this.azureBlobStorageClient = azureBlobStorageClient;
            this.logger = logger.CreateScope(nameof(AzureBlobStorageFeedDao));
        }

        /// <inheritdoc/>
        public Task DeleteAsync(string userId)
        {
            this.logger.Info($"Call: {nameof(this.DeleteAsync)}('{userId}')");
            return this.azureBlobStorageClient.DeleteAsync(ConteinerName, userId);
        }

        /// <inheritdoc/>
        public async Task<SortedSet<FeedItem>> LoadBaseFeedAsync()
        {
            this.logger.Info($"Call: {nameof(this.LoadBaseFeedAsync)}()");
            return ToSortedSet(await this.azureBlobStorageClient.DownloadStringAsync(ConteinerName, BaseFeedName));
        }

        /// <inheritdoc/>
        public Task<string?> LoadFeedRawAsync(string userId)
        {
            this.logger.Info($"Call: {nameof(this.LoadFeedRawAsync)}('{userId}')");
            return this.azureBlobStorageClient.DownloadStringAsync(ConteinerName, GetUserFeedFileName(userId));
        }

        /// <inheritdoc/>
        public async Task<SortedSet<FeedItem>> LoadUserFeedAsync(string userId)
        {
            this.logger.Info($"Call: {nameof(this.LoadUserFeedAsync)}('{userId}')");
            return ToSortedSet(await this.azureBlobStorageClient.DownloadStringAsync(ConteinerName, GetUserFeedFileName(userId)));
        }

        /// <inheritdoc/>
        public Task SaveBaseFeedAsync(FeedItem[] items)
        {
            this.logger.Info($"Call: {nameof(this.SaveBaseFeedAsync)}(FeedItem[])");
            return this.azureBlobStorageClient.UploadAsync(ConteinerName, BaseFeedName, items.GenerateXml(), "text/xml");
        }

        /// <inheritdoc/>
        public Task SaveUserFeedAsync(string userId, FeedItem[] items)
        {
            this.logger.Info($"Call: {nameof(this.SaveUserFeedAsync)}('{userId}', FeedItem[])");
            return this.azureBlobStorageClient.UploadAsync(ConteinerName, GetUserFeedFileName(userId), items.GenerateXml(), "public, max-age=300");
        }

        private static string GetUserFeedFileName(string userId) => $"{userId}.xml";

        private static SortedSet<FeedItem> ToSortedSet(string? xml)
            => xml == null
                ? new SortedSet<FeedItem>()
                : XDocument.Parse(xml).GetItems();
    }
}
