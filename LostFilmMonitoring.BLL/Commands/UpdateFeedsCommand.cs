// <copyright file="UpdateFeedsCommand.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.BLL.Commands
{
    /// <summary>
    /// Update all feeds.
    /// </summary>
    public class UpdateFeedsCommand : ICommand
    {
        private static readonly object SeriesLocker = new ();
        private static readonly object TorrentFileLocker = new ();
        private readonly ILogger logger;
        private readonly IDAL dal;
        private readonly IRssFeed rssFeed;
        private readonly IConfiguration configuration;
        private readonly IModelPersister modelPersister;
        private readonly ILostFilmClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateFeedsCommand"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="rssFeed">ReteOrgRssFeed.</param>
        /// <param name="dal">dal.</param>
        /// <param name="configuration">IConfiguration.</param>
        /// <param name="modelPersister">modelPersister.</param>
        /// <param name="client">client.</param>
        public UpdateFeedsCommand(
            ILogger logger,
            IRssFeed rssFeed,
            IDAL dal,
            IConfiguration configuration,
            IModelPersister modelPersister,
            ILostFilmClient client)
        {
            this.logger = logger != null ? logger.CreateScope(nameof(UpdateFeedsCommand)) : throw new ArgumentNullException(nameof(logger));
            this.dal = dal ?? throw new ArgumentNullException(nameof(dal));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.rssFeed = rssFeed ?? throw new ArgumentNullException(nameof(rssFeed));
            this.modelPersister = modelPersister ?? throw new ArgumentNullException(nameof(modelPersister));
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <inheritdoc/>
        public async Task ExecuteAsync()
        {
            this.logger.Info($"Call: {nameof(this.ExecuteAsync)}()");
            var feedItemsResponse = await this.LoadFeedUpdatesAsync();
            var persistedItemsRespone = await this.LoadLastFeedUpdatesAsync();
            if (!FeedItemResponse.HasUpdates(feedItemsResponse, persistedItemsRespone))
            {
                this.logger.Info("No updates.");
                return;
            }

            var feedItems = ClearSeasonTorrents(feedItemsResponse);

            if (!feedItems.Any())
            {
                this.logger.Error("No feed items were found.");
                return;
            }

            var success = await this.ProcessFeedItemsAsync(feedItems);
            if (success)
            {
                await this.SaveFeedUpdatesAsync(feedItemsResponse);
            }
        }

        private static Episode ToEpisode(FeedItemResponse feedItem)
            => new (
                feedItem.SeriesName,
                feedItem.EpisodeName,
                feedItem.SeasonNumber,
                feedItem.EpisodeNumber,
                feedItem.GetTorrentId(),
                feedItem.Quality);

        private static Series ToSeries(FeedItemResponse feedItem)
            => new (
                feedItem.SeriesName,
                feedItem.PublishDateParsed,
                $"{feedItem.SeriesName}. {feedItem.EpisodeName} (S{feedItem.SeasonNumber:D2}E{feedItem.EpisodeNumber:D2}) ",
                ParseLink(feedItem, Quality.SD),
                ParseLink(feedItem, Quality.H720),
                ParseLink(feedItem, Quality.H1080),
                ParseSeasonNumber(feedItem, Quality.H1080),
                ParseSeasonNumber(feedItem, Quality.H720),
                ParseSeasonNumber(feedItem, Quality.SD),
                ParseEpisodeNumber(feedItem, Quality.H1080),
                ParseEpisodeNumber(feedItem, Quality.H720),
                ParseEpisodeNumber(feedItem, Quality.SD));

        private static BencodeNET.Torrents.Torrent ToTorrentDataStructure(TorrentFileResponse torrentFileResponse)
            => torrentFileResponse.Content.ToTorrentDataStructure();

        private static int? ParseEpisodeNumber(FeedItemResponse feedItem, string quality)
            => feedItem.Quality == quality ? feedItem.EpisodeNumber : null;

        private static int? ParseSeasonNumber(FeedItemResponse feedItem, string quality)
            => feedItem.Quality == quality ? feedItem.SeasonNumber : null;

        private static string? ParseLink(FeedItemResponse feedItem, string quality)
            => feedItem.Quality == quality ? feedItem.Link : null;

        private static TorrentFile ToTorrentFile(TorrentFileResponse x)
            => new (x.FileName, x.Content);

        private static FeedItem ToFeedItem(FeedItemResponse x, string link)
            => new (x.Title, link, x.PublishDateParsed);

        private static bool HasUpdatesComparedTo(Series newOne, Series oldOne) =>
            HasUpdatesComparedTo(newOne, oldOne, x => x.LastEpisodeTorrentLink1080) ||
            HasUpdatesComparedTo(newOne, oldOne, x => x.LastEpisodeTorrentLinkMP4) ||
            HasUpdatesComparedTo(newOne, oldOne, x => x.LastEpisodeTorrentLinkSD);

        private static bool HasUpdatesComparedTo(Series newOne, Series oldOne, Func<Series, string?> propertySelectorFunc) =>
            !string.Equals(propertySelectorFunc(oldOne), propertySelectorFunc(newOne));

        private static SortedSet<FeedItemResponse> ClearSeasonTorrents(SortedSet<FeedItemResponse> set)
            => new (set.Where(x => !x.Title.Contains("E999")));

        private static Series? GetSeriesToUpdate(List<Series> existingSeries, Series newSeries)
        {
            lock (SeriesLocker)
            {
                var existing = existingSeries.FirstOrDefault(s => s.Name == newSeries.Name);
                if (existing != null && existing.LastEpisodeName == newSeries.LastEpisodeName && !HasUpdatesComparedTo(newSeries, existing))
                {
                    return null;
                }

                if (existing == null)
                {
                    existingSeries.Add(newSeries);
                    return newSeries;
                }

                existing.MergeFrom(newSeries);
                return existing;
            }
        }

        private async Task<bool> ProcessFeedItemsAsync(SortedSet<FeedItemResponse> feedItems)
        {
            var allSeries = await this.LoadSeriesAsync();
            var results = await Task.WhenAll(feedItems.Select(x => this.ProcessFeedItemAsync(x, allSeries)));
            await this.UpdateIndexViewModelAsync(allSeries);
            return results.All(x => x == true);
        }

        private async Task<bool> ProcessFeedItemAsync(FeedItemResponse feedItem, List<Series> series)
        {
            var seriesToUpdate = GetSeriesToUpdate(series, ToSeries(feedItem));
            if (seriesToUpdate == null)
            {
                return true;
            }

            var torrent = await this.GetTorrentAsync(feedItem);
            if (torrent == null)
            {
                return false;
            }

            await this.SaveEpisodeAsync(feedItem);
            await this.UpdateAllSubscribedUsersAsync(feedItem, torrent);
            await this.dal.Series.SaveAsync(seriesToUpdate);
            return true;
        }

        private async Task SaveEpisodeAsync(FeedItemResponse feedItem)
        {
            var episode = ToEpisode(feedItem);
            if (episode.SeasonNumber != null && episode.EpisodeNumber != null)
            {
                await this.dal.Episode.SaveAsync(episode);
            }
        }

        private async Task<SortedSet<FeedItemResponse>> LoadFeedUpdatesAsync()
        {
            return (await this.rssFeed.LoadFeedItemsAsync()) ?? new SortedSet<FeedItemResponse>();
        }

        private async Task<SortedSet<FeedItemResponse>> LoadLastFeedUpdatesAsync()
            => (await this.modelPersister.LoadAsync<SortedSet<FeedItemResponse>>("ReteOrgItems")) ?? new SortedSet<FeedItemResponse>();

        private Task SaveFeedUpdatesAsync(SortedSet<FeedItemResponse> feedItemsResponse)
            => this.modelPersister.PersistAsync("ReteOrgItems", feedItemsResponse);

        private async Task<List<Series>> LoadSeriesAsync()
            => new List<Series>(await this.dal.Series.LoadAsync());

        private Task UpdateIndexViewModelAsync(List<Series> existingSeries)
            => this.modelPersister.PersistAsync("index", new IndexViewModel(existingSeries));

        private async Task<BencodeNET.Torrents.Torrent?> GetTorrentAsync(FeedItemResponse feedResponseItem)
        {
            var torrentId = feedResponseItem.GetTorrentId();
            var torrentFileResponse = await this.client.DownloadTorrentFileAsync(this.configuration.BaseUID, this.configuration.BaseUSESS, torrentId);
            if (torrentFileResponse == null)
            {
                return null;
            }

            var torrent = ToTorrentDataStructure(torrentFileResponse);
            await this.dal.TorrentFile.SaveBaseFileAsync(torrentId, ToTorrentFile(torrentFileResponse));
            return torrent;
        }

        private async Task UpdateAllSubscribedUsersAsync(FeedItemResponse feedResponseItem, BencodeNET.Torrents.Torrent torrent)
        {
            var userIds = await this.dal.Subscription.LoadUsersIdsAsync(feedResponseItem.SeriesName, feedResponseItem.Quality);
            await this.UpdateAllSubscribedUsersAsync(userIds, feedResponseItem, torrent);
        }

        private Task UpdateAllSubscribedUsersAsync(string[] userIds, FeedItemResponse feedResponseItem, BencodeNET.Torrents.Torrent torrent)
            => Task.WhenAll(userIds.Select(x => this.UpdateSubscribedUserAsync(feedResponseItem, torrent, x)));

        private async Task UpdateSubscribedUserAsync(FeedItemResponse feedResponseItem, BencodeNET.Torrents.Torrent torrent, string userId)
        {
            if (await this.SaveTorrentFileForUserAsync(userId, torrent))
            {
                await this.UpdateUserFeedAsync(userId, feedResponseItem, torrent.DisplayNameUtf8 ?? torrent.DisplayName);
            }
        }

        private async Task<bool> SaveTorrentFileForUserAsync(string userId, BencodeNET.Torrents.Torrent torrent)
        {
            var user = await this.dal.User.LoadAsync(userId);
            if (user == null)
            {
                this.logger.Error($"User '{userId}' not found.");
                return false;
            }

            TorrentFile torrentFile;
            lock (TorrentFileLocker)
            {
                torrent.FixTrackers(this.configuration.GetTorrentAnnounceList(user.TrackerId));
                torrentFile = torrent.ToTorrentFile();
            }

            await this.dal.TorrentFile.SaveUserFileAsync(userId, torrentFile);
            return true;
        }

        private async Task UpdateUserFeedAsync(string userId, FeedItemResponse item, string torrentFileName)
        {
            string link = Extensions.GenerateTorrentLink(this.configuration.BaseUrl, userId, torrentFileName);
            var userFeedItem = ToFeedItem(item, link);
            var userFeed = (await this.dal.Feed.LoadUserFeedAsync(userId)) ?? new SortedSet<FeedItem>();
            userFeed.Add(userFeedItem);
            await this.dal.Feed.SaveUserFeedAsync(userId, userFeed.Take(15).ToArray());
            await this.CleanupOldRssFilesAsync(userId, userFeed.Skip(15).ToArray());
        }

        private async Task CleanupOldRssFileAsync(string userId, FeedItem item)
        {
            if (item == null)
            {
                return;
            }

            this.logger.Info($"Call: {nameof(this.CleanupOldRssFileAsync)}('{userId}', '{item.Link}')");
            try
            {
                var fileName = item.GetUserFileName(userId);
                if (fileName == null)
                {
                    this.logger.Error($"Cannot get fileName from FeedItem '{item.Link}'.");
                    return;
                }

                await this.dal.TorrentFile.DeleteUserFileAsync(userId, fileName);
            }
            catch (Exception ex)
            {
                this.logger.Log($"Error deleting FeedItem '{item.Link}' for user '{userId}'.", ex);
            }
        }

        private Task CleanupOldRssFilesAsync(string userId, FeedItem[] oldRssFeedItems)
            => Task.WhenAll(oldRssFeedItems.Select(i => this.CleanupOldRssFileAsync(userId, i)));
    }
}
