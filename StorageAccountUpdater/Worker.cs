// <copyright file="Worker.cs" company="Alexander Panfilenok">
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
using LostFilmMonitoring.DAO.Interfaces.DomainModels;
using LostFilmTV.Client.Response;

namespace StorageAccountUpdater
{
    internal class Worker
    {
        private readonly TableServiceClient tableServiceClient;
        private readonly BlobServiceClient blobServiceClient;
        private readonly AzureBlobStorageClient azureBlobStorageClient;
        private readonly ConsoleLogger logger;

        public Worker(string connectionString)
        {
            logger = new ConsoleLogger("root");
            tableServiceClient = new TableServiceClient(connectionString);
            blobServiceClient = new BlobServiceClient(connectionString);
            azureBlobStorageClient = new AzureBlobStorageClient(blobServiceClient, logger);
        }

        public async Task DoWork()
        {
            await DeleteImages();
            //await CopyThumbnails();
            //await RenameSeries();
            //await RenameSubscriptions();
            //await RenameSubscriptionModels();
            //await RenameIndexModel();
        }

        private async Task DeleteImages()
        {
            AzureTableStorageSeriesDao tableStorageSeriesDao = new AzureTableStorageSeriesDao(tableServiceClient, logger);
            AzureBlobStorageClient blobStorageClient = new AzureBlobStorageClient(blobServiceClient, logger);
            var allSeries = await tableStorageSeriesDao.LoadAsync();
            foreach (var series in allSeries)
            {
                var fileName = EscapePath(series.Name) + "..jpg";
                await blobStorageClient.DeleteAsync("images", fileName);
            }
        }

        private async Task RenameIndexModel()
        {
            AzureBlobStorageClient blobStorageClient = new AzureBlobStorageClient(blobServiceClient, logger);
            AzureBlobStorageModelPersister modelPersister = new AzureBlobStorageModelPersister(blobStorageClient, logger);

            var index = await modelPersister.LoadAsync<IndexViewModel>("index");
            index.Last24HoursItems = Filter(index.Last24HoursItems).ToArray();
            index.Last7DaysItems = Filter(index.Last7DaysItems).ToArray();
            index.OlderItems = Filter(index.OlderItems).ToArray();
            await modelPersister.PersistAsync("index", index);
        }

        private IEnumerable<string> Filter(IEnumerable<string> source)
        {
            foreach(var item in source)
            {
                if (item.EndsWith('.')) yield return item[..^1];
                else yield return item;
            }
        }

        private async Task RenameSubscriptionModels()
        {
            AzureBlobStorageClient blobStorageClient = new AzureBlobStorageClient(blobServiceClient, logger);
            AzureBlobStorageModelPersister modelPersister = new AzureBlobStorageModelPersister(blobStorageClient, logger);
            AzureTableStorageUserDAO azureTableStorageUserDAO = new AzureTableStorageUserDAO(tableServiceClient, logger);
            var users = await azureTableStorageUserDAO.LoadAsync();
            foreach(var user in users)
            {
                var items = await modelPersister.LoadAsync<SubscriptionItem[]>($"subscription_{user.Id}");
                foreach(var item in items)
                {
                    if (item.SeriesName.EndsWith('.'))
                    {
                        item.SeriesName = item.SeriesName[..^1];
                    }
                }

                await modelPersister.PersistAsync($"subscription_{user.Id}", items);
            }
        }

        private async Task RenameSubscriptions()
        {
            AzureTableStorageSubscriptionDAO azureTableStorageSubscriptionDao = new AzureTableStorageSubscriptionDAO(tableServiceClient, logger);
            AzureTableStorageUserDAO azureTableStorageUserDAO = new AzureTableStorageUserDAO(tableServiceClient, logger);

            var users = await azureTableStorageUserDAO.LoadAsync();
            foreach(var user in users)
            {
                var subscriptions = await azureTableStorageSubscriptionDao.LoadAsync(user.Id);
                var newSubscriptions = new List<Subscription>();
                foreach(var subscription in subscriptions)
                {
                    if (subscription.SeriesName.EndsWith('.'))
                    {
                        newSubscriptions.Add(new Subscription(subscription.SeriesName[..^1], subscription.Quality));
                    }
                    else
                    {
                        newSubscriptions.Add(subscription);
                    }
                }

                await azureTableStorageSubscriptionDao.SaveAsync(user.Id, newSubscriptions.ToArray());
            }
        }

        private async Task DeleteSeries()
        {
            AzureTableStorageSeriesDao azureTableStorageSeriesDao = new AzureTableStorageSeriesDao(tableServiceClient, logger);
            var allSeries = await azureTableStorageSeriesDao.LoadAsync();
            foreach (var series in allSeries)
            {
                if (series.Name.EndsWith('.'))
                {
                    var newSeries = new Series(
                        series.Name[..^1],
                        series.LastEpisode,
                        series.LastEpisodeName,
                        series.LastEpisodeTorrentLinkSD,
                        series.LastEpisodeTorrentLinkMP4,
                        series.LastEpisodeTorrentLink1080,
                        series.Q1080SeasonNumber,
                        series.QMP4SeasonNumber,
                        series.QSDSeasonNumber,
                        series.Q1080EpisodeNumber,
                        series.QMP4EpisodeNumber,
                        series.QSDEpisodeNumber
                        );
                    await azureTableStorageSeriesDao.DeleteAsync(series);
                }
            }
        }

        private async Task RenameSeries()
        {
            AzureTableStorageSeriesDao azureTableStorageSeriesDao = new AzureTableStorageSeriesDao(tableServiceClient, logger);
            var allSeries = await azureTableStorageSeriesDao.LoadAsync();
            foreach (var series in allSeries)
            {
                if (series.Name.EndsWith('.'))
                {
                    var newSeries = new Series(
                        series.Name[..^1],
                        series.LastEpisode,
                        series.LastEpisodeName,
                        series.LastEpisodeTorrentLinkSD,
                        series.LastEpisodeTorrentLinkMP4,
                        series.LastEpisodeTorrentLink1080,
                        series.Q1080SeasonNumber,
                        series.QMP4SeasonNumber,
                        series.QSDSeasonNumber,
                        series.Q1080EpisodeNumber,
                        series.QMP4EpisodeNumber,
                        series.QSDEpisodeNumber
                        );
                    await azureTableStorageSeriesDao.DeleteAsync(series);
                    await azureTableStorageSeriesDao.SaveAsync(newSeries);
                }
            }
        }

        private async Task CopyThumbnails()
        {
            AzureTableStorageSeriesDao azureTableStorageSeriesDao = new AzureTableStorageSeriesDao(tableServiceClient, logger);

            var allSeries = await azureTableStorageSeriesDao.LoadAsync();
            foreach(var series in allSeries.Where(x => x.Name.EndsWith('.')))
            {
                var fileName = EscapePath(series.Name) + ".jpg";
                var st = await azureBlobStorageClient.DownloadAsync("images", fileName);
                if (st == null)
                {
                    continue;
                }

                fileName = EscapePath(series.Name[..^1]) + ".jpg";

                await azureBlobStorageClient.UploadAsync("images", fileName, st, "public, max-age=15552000");
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
