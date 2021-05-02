// <copyright file="RssFeedService.cs" company="Alexander Panfilenok">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LostFilmMonitoring.BLL.Interfaces.Models;
    using LostFilmMonitoring.BLL.Models;
    using LostFilmMonitoring.Common;
    using LostFilmMonitoring.DAO.DAO;
    using LostFilmMonitoring.DAO.DomainModels;

    /// <summary>
    /// Manages user feed.
    /// </summary>
    public class RssFeedService
    {
        private readonly UserDAO userDAO;
        private readonly FeedDAO feedDAO;
        private readonly SeriesDAO seriesDAO;
        private readonly TorrentFileDownloader torrentFileDownloader;
        private readonly ICurrentUserProvider currentUserProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RssFeedService"/> class.
        /// </summary>
        /// <param name="currentUserProvider">CurrentUserProvider.</param>
        /// <param name="logger">Logger.</param>
        public RssFeedService(ICurrentUserProvider currentUserProvider, ILogger logger)
        {
            this.seriesDAO = new SeriesDAO(Configuration.GetConnectionString());
            this.feedDAO = new FeedDAO(Configuration.GetConnectionString());
            this.userDAO = new UserDAO(Configuration.GetConnectionString());
            this.currentUserProvider = currentUserProvider;
            this.torrentFileDownloader = new TorrentFileDownloader(new TorrentFileDAO(Configuration.GetTorrentPath(), logger), logger);
        }

        /// <summary>
        /// Gets user feed rss content by user id.
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <returns>RSS feed content.</returns>
        public async Task<string> GetRss(Guid userId)
        {
            if (!await this.userDAO.UpdateLastActivity(userId))
            {
                return null;
            }

            return await this.feedDAO.LoadFeedRawAsync(userId);
        }

        /// <summary>
        /// Update user's subscription.
        /// </summary>
        /// <param name="selectedItems">Selected subscription items.</param>
        /// <returns>Awaitable task.</returns>
        public async Task UpdateUserSubscrptionAsync(SelectedFeedItem[] selectedItems)
        {
            if (selectedItems == null)
            {
                return;
            }

            var userId = this.currentUserProvider.GetCurrentUserId();
            var user = await this.userDAO.LoadWithSubscriptionsAsync(userId);
            if (user == null)
            {
                return;
            }

            var userFeedItems = await this.feedDAO.LoadUserFeedAsync(userId);
            if (userFeedItems == null)
            {
                userFeedItems = new SortedSet<FeedItem>();
            }

            if (user.Subscriptions == null)
            {
                user.Subscriptions = new List<Subscription>();
            }

            var newItems = selectedItems.Where(
                i => user.Subscriptions.All(
                    s => !string.Equals(s.SeriesName, i.SeriesName, StringComparison.OrdinalIgnoreCase) || s.Quality != i.Quality))
                .ToList();

            foreach (var newItem in newItems)
            {
                var series = await this.seriesDAO.LoadAsync(newItem.SeriesName);
                var torrentId = this.GetTorrentId(series, newItem.Quality);
                if (torrentId == null)
                {
                    continue;
                }

                var newFeedItem = new FeedItem()
                {
                    Link = Extensions.GenerateTorrentLink(userId, torrentId),
                    Title = series.LastEpisodeName,
                    PublishDateParsed = DateTime.UtcNow,
                };
                userFeedItems.RemoveWhere(i => string.Equals(i.Title, newFeedItem.Title));
                userFeedItems.Add(newFeedItem);
            }

            await this.feedDAO.SaveUserFeedAsync(userId, userFeedItems.Take(15).ToArray());
        }

        /// <summary>
        /// Gets the FeedViewModel.
        /// </summary>
        /// <returns>FeedViewModel.</returns>
        public async Task<FeedViewModel> GetFeedViewModel()
        {
            var userId = this.currentUserProvider.GetCurrentUserId();
            if (!await this.userDAO.UpdateLastActivity(userId))
            {
                this.currentUserProvider.SetCurrentUserId(Guid.Empty);
                return null;
            }

            return new FeedViewModel(await this.feedDAO.LoadUserFeedAsync(userId), userId);
        }

        /// <summary>
        /// Gets RssItemViewModel which contains torrent file data.
        /// </summary>
        /// <param name="userId">userId.</param>
        /// <param name="torrentFileId">torrentFileId.</param>
        /// <returns>RssItemViewModel.</returns>
        public async Task<RssItemViewModel> GetRssItem(Guid userId, int torrentFileId)
        {
            var user = await this.userDAO.LoadAsync(userId);
            if (user == null)
            {
                return null;
            }

            var torrentFile = await this.torrentFileDownloader.DownloadAsync(user, torrentFileId);
            if (torrentFile == null)
            {
                return null;
            }

            return new RssItemViewModel(torrentFile);
        }

        private string GetTorrentId(Series series, string quality)
        {
            return quality switch
            {
                Quality.SD => LostFilmTV.Client.Extensions.GetTorrentId(series.LastEpisodeTorrentLinkSD),
                Quality.H1080 => LostFilmTV.Client.Extensions.GetTorrentId(series.LastEpisodeTorrentLink1080),
                Quality.H720 => LostFilmTV.Client.Extensions.GetTorrentId(series.LastEpisodeTorrentLinkMP4),
                _ => throw new InvalidOperationException("Quality not supported"),
            };
        }
    }
}
