// <copyright file="Cloner.cs" company="Alexander Panfilenok">
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using LostFilmMonitoring.BLL.Models;
using LostFilmMonitoring.BLL.Models.ViewModel;
using LostFilmMonitoring.Common;
using LostFilmMonitoring.DAO.Azure;
using LostFilmTV.Client.Response;

namespace StorageAccountUpdater
{
    internal class Cloner
    {
        private readonly TableServiceClient sourceTableServiceClient, targetTableStorageClient;
        private readonly BlobServiceClient sourceBlobServiceClient, targetBlobServiceClient;
        private readonly ConsoleLogger logger;

        public Cloner(string sourceConnectionString, string targetConnectionString)
        {
            logger = new ConsoleLogger("root");
            sourceTableServiceClient = new TableServiceClient(sourceConnectionString);
            targetTableStorageClient = new TableServiceClient(targetConnectionString);
            sourceBlobServiceClient = new BlobServiceClient(sourceConnectionString);
            targetBlobServiceClient = new BlobServiceClient(targetConnectionString);
        }

        public async Task Clone()
        {
            //await CopyImages();
            //await CopySeries();
            //await CopyUsers();
            //await CopySubscriptions();
            //await CopyRssFeeds();
            //await CopyModels();
            //await CopyBaseModels();
        }

        private async Task CopyBaseModels()
        {
            AzureBlobStorageClient sourceBlobStorageClient = new AzureBlobStorageClient(sourceBlobServiceClient, logger);
            AzureBlobStorageClient targetBlobStorageClient = new AzureBlobStorageClient(targetBlobServiceClient, logger);
            AzureBlobStorageModelPersister sourceModelPersister = new AzureBlobStorageModelPersister(sourceBlobStorageClient, logger); ;
            AzureBlobStorageModelPersister targetModelPersister = new AzureBlobStorageModelPersister(targetBlobStorageClient, logger);

            var reteOrgItems = await sourceModelPersister.LoadAsync<SortedSet<FeedItemResponse>>("ReteOrgItems");
            await targetModelPersister.PersistAsync("ReteOrgItems", reteOrgItems);

            var index = await sourceModelPersister.LoadAsync<IndexViewModel>("index");
            await targetModelPersister.PersistAsync("index", index);
        }

        private async Task CopyModels()
        {
            AzureBlobStorageClient sourceBlobStorageClient = new AzureBlobStorageClient(sourceBlobServiceClient, logger);
            AzureBlobStorageClient targetBlobStorageClient = new AzureBlobStorageClient(targetBlobServiceClient, logger);
            AzureTableStorageUserDAO targetTableStorageUserDao = new AzureTableStorageUserDAO(targetTableStorageClient, logger);
            AzureBlobStorageModelPersister sourceModelPersister = new AzureBlobStorageModelPersister(sourceBlobStorageClient, logger); ;
            AzureBlobStorageModelPersister targetModelPersister = new AzureBlobStorageModelPersister(targetBlobStorageClient, logger);


            var users = await targetTableStorageUserDao.LoadAsync();
            foreach (var user in users)
            {
                var feed = await sourceModelPersister.LoadAsync<SubscriptionItem[]>($"subscription_{user.Id}");
                await targetModelPersister.PersistAsync($"subscription_{user.Id}", feed);
            }
        }

        private async Task CopyRssFeeds()
        {
            AzureBlobStorageClient sourceBlobStorageClient = new AzureBlobStorageClient(sourceBlobServiceClient, logger);
            AzureBlobStorageClient targetBlobStorageClient = new AzureBlobStorageClient(targetBlobServiceClient, logger);
            AzureTableStorageUserDAO targetTableStorageUserDao = new AzureTableStorageUserDAO(targetTableStorageClient, logger);
            AzureBlobStorageFeedDAO sourceBlobStorageFeedDao = new AzureBlobStorageFeedDAO(sourceBlobStorageClient, logger);
            AzureBlobStorageFeedDAO targetBlobStorageFeedDao = new AzureBlobStorageFeedDAO(targetBlobStorageClient, logger);

            var users = await targetTableStorageUserDao.LoadAsync();
            foreach (var user in users)
            {
                var feed = await sourceBlobStorageFeedDao.LoadUserFeedAsync(user.Id);
                await targetBlobStorageFeedDao.SaveUserFeedAsync(user.Id, feed.ToArray());
            }
        }

        private async Task CopySubscriptions()
        {
            AzureTableStorageSubscriptionDAO sourceTableStorageSubscriptionDao = new AzureTableStorageSubscriptionDAO(sourceTableServiceClient, logger);
            AzureTableStorageSubscriptionDAO targetTableStorageSubscriptionDao = new AzureTableStorageSubscriptionDAO(targetTableStorageClient, logger);

            AzureTableStorageUserDAO targetTableStorageUserDao = new AzureTableStorageUserDAO(targetTableStorageClient, logger);
            var users = await targetTableStorageUserDao.LoadAsync();

            foreach (var user in users)
            {
                var subscriptions = await sourceTableStorageSubscriptionDao.LoadAsync(user.Id);
                await targetTableStorageSubscriptionDao.SaveAsync(user.Id, subscriptions);
            }
        }

        private async Task CopyUsers()
        {
            AzureTableStorageUserDAO sourceTableStorageUserDao = new AzureTableStorageUserDAO(sourceTableServiceClient, logger);
            AzureTableStorageUserDAO targetTableStorageUserDao = new AzureTableStorageUserDAO(targetTableStorageClient, logger);

            var users = await sourceTableStorageUserDao.LoadAsync();
            foreach (var item in users)
            {
                await targetTableStorageUserDao.SaveAsync(item);
            }
        }

        private async Task CopySeries()
        {
            AzureTableStorageSeriesDao sourceTableStorageSeriesDao = new AzureTableStorageSeriesDao(sourceTableServiceClient, logger);
            AzureTableStorageSeriesDao targetTableStorageSeriesDao = new AzureTableStorageSeriesDao(targetTableStorageClient, logger);

            var series = await sourceTableStorageSeriesDao.LoadAsync();
            foreach (var item in series)
            {
                await targetTableStorageSeriesDao.SaveAsync(item);
            }
        }

        private async Task CopyImages()
        {
            AzureTableStorageSeriesDao sourceTableStorageSeriesDao = new AzureTableStorageSeriesDao(sourceTableServiceClient, logger);
            AzureBlobStorageClient sourceBlobStorageClient = new AzureBlobStorageClient(sourceBlobServiceClient, logger);
            AzureBlobStorageClient targetBlobStorageClient = new AzureBlobStorageClient(targetBlobServiceClient, logger);
            var allSeries = await sourceTableStorageSeriesDao.LoadAsync();
            foreach (var series in allSeries)
            {
                var fileName = EscapePath(series.Name) + ".jpg";
                var st = await sourceBlobStorageClient.DownloadAsync("images", fileName);
                if (st == null)
                {
                    continue;
                }

                await targetBlobStorageClient.UploadAsync("images", fileName, st, "public, max-age=15552000");
            }
        }

        private static string EscapePath(string path)
        {
            return path
                .Replace(":", "_")
                .Replace("*", "_")
                .Replace("\"", "_")
                .Replace("/", "_")
                .Replace("?", "_")
                .Replace(">", "_")
                .Replace("<", "_")
                .Replace("|", "_");
        }
    }
}
