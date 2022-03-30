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

using LostFilmMonitoring.Common;
using LostFilmMonitoring.DAO.Interfaces;
using LostFilmMonitoring.DAO.Interfaces.DomainModels;

namespace LostFilmMonitoring.DAO.Azure
{
    public class AzureBlobStorageFeedDAO : IFeedDAO
    {
        private readonly AzureBlobStorageClient azureBlobStorageClient;
        private readonly ILogger logger;
        private static readonly string conteinerName = "rssfeeds";
        private static readonly Guid BaseFeedId = new Guid("96AFC9E2-C64D-4DDD-9003-016974D32100");

        public AzureBlobStorageFeedDAO(AzureBlobStorageClient azureBlobStorageClient, ILogger logger)
        {
            this.azureBlobStorageClient = azureBlobStorageClient;
            this.logger = logger.CreateScope(nameof(AzureBlobStorageFeedDAO));
        }

        public Task DeleteAsync(Guid userId)
        {
            this.logger.Info($"Call: {nameof(DeleteAsync)}('{userId}')");
            return this.azureBlobStorageClient.DeleteAsync(conteinerName, userId.ToString());
        }

        public async Task<SortedSet<FeedItem>> LoadBaseFeedAsync()
        {
            this.logger.Info($"Call: {nameof(LoadBaseFeedAsync)}()");
            string content = await this.azureBlobStorageClient.DownloadStringAsync(conteinerName, BaseFeedId.ToString());
            return FeedItem.GetItems(content);
        }

        public Task<string> LoadFeedRawAsync(Guid userId)
        {
            this.logger.Info($"Call: {nameof(LoadFeedRawAsync)}('{userId}')");
            return this.azureBlobStorageClient.DownloadStringAsync(conteinerName, userId.ToString());
        }

        public async Task<SortedSet<FeedItem>> LoadUserFeedAsync(Guid userId)
        {
            this.logger.Info($"Call: {nameof(LoadUserFeedAsync)}('{userId}')");
            string content = await this.azureBlobStorageClient.DownloadStringAsync(conteinerName, userId.ToString());
            return FeedItem.GetItems(content);
        }

        public Task SaveBaseFeedAsync(FeedItem[] items)
        {
            this.logger.Info($"Call: {nameof(SaveBaseFeedAsync)}(FeedItem[])");
            return this.azureBlobStorageClient.UploadAsync(conteinerName, BaseFeedId.ToString(), FeedItem.GenerateXml(items));
        }

        public Task SaveUserFeedAsync(Guid userId, FeedItem[] items)
        {
            this.logger.Info($"Call: {nameof(SaveUserFeedAsync)}('{userId}', FeedItem[])");
            return this.azureBlobStorageClient.UploadAsync(conteinerName, userId.ToString(), FeedItem.GenerateXml(items));
        }
    }
}
