// <copyright file="RssFeedUpdaterService.cs" company="Alexander Panfilenok">
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
    using LostFilmMonitoring.Common;
    using LostFilmMonitoring.DAO.DAO;
    using LostFilmMonitoring.DAO.DomainModels;
    using LostFilmTV.Client.Response;
    using LostFilmTV.Client.RssFeed;

    /// <summary>
    /// Responsible for rss feeds update process.
    /// </summary>
    public class RssFeedUpdaterService
    {
        private readonly ILogger logger;
        private readonly FeedDAO feedDAO;
        private readonly ReteOrgRssFeed reteOrgRssFeed;
        private readonly SeriesDAO serialDao;
        private readonly SeriesCoverService serialCoverService;
        private readonly SubscriptionDAO subscriptionDAO;

        /// <summary>
        /// Initializes a new instance of the <see cref="RssFeedUpdaterService"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public RssFeedUpdaterService(ILogger logger)
        {
            var connectionString = Configuration.GetConnectionString();
            this.logger = logger != null ? logger.CreateScope(nameof(RssFeedUpdaterService)) : throw new ArgumentNullException(nameof(logger));
            this.reteOrgRssFeed = new ReteOrgRssFeed(logger);
            this.feedDAO = new FeedDAO(connectionString);
            this.serialCoverService = new SeriesCoverService(logger);
            this.serialDao = new SeriesDAO(connectionString);
            this.subscriptionDAO = new SubscriptionDAO(connectionString);
        }

        /// <summary>
        /// Update rss feeds.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        public async Task UpdateAsync()
        {
            this.logger.Info($"Call: {nameof(this.UpdateAsync)}()");
            var feedItemResponses = await this.reteOrgRssFeed.LoadFeedItemsAsync();
            if (!feedItemResponses.Any())
            {
                return;
            }

            await this.UpdateSerialList(feedItemResponses);
            var feedItems = await this.feedDAO.LoadBaseFeedAsync();
            if (feedItems == null)
            {
                feedItems = new SortedSet<FeedItem>();
            }

            foreach (var feedItemResponse in feedItemResponses)
            {
                var feedItem = new FeedItem(feedItemResponse);
                if (!feedItems.Contains(feedItem))
                {
                    feedItems.Add(feedItem);
                    await this.PrepareUserFeeds(feedItemResponse);
                }
            }

            await this.feedDAO.SaveBaseFeedAsync(feedItems.Take(30).ToArray());
            this.logger.Info("Base feed updated");
        }

        private async Task UpdateSerialList(IEnumerable<FeedItemResponse> feedItems)
        {
            var existingSerials = await this.serialDao.LoadAsync();
            var baseFeedCookie = Configuration.BaseFeedCookie();
            foreach (var feedItem in feedItems)
            {
                var series = this.ParseSeries(feedItem);
                await this.serialCoverService.EnsureCoverDownloadedAsync(series.Name);
                var existingSeries = existingSerials.FirstOrDefault(s => s.Name == series.Name);
                if (existingSeries != null && existingSeries.LastEpisodeName == series.LastEpisodeName && !this.HasUpdatesComparedTo(series, existingSeries))
                {
                    continue;
                }

                if (existingSeries == null)
                {
                    this.logger.Info($"New series detected: {series.Name}");
                    existingSerials.Add(series);
                    await this.serialDao.SaveAsync(series);
                    continue;
                }

                this.Merge(existingSeries, series);
                await this.serialDao.SaveAsync(existingSeries);
            }
        }

        private bool HasUpdatesComparedTo(Series newOne, Series oldOne)
        {
            return (newOne.LastEpisodeTorrentLink1080 != null && oldOne.LastEpisodeTorrentLink1080 == null) ||
                   (newOne.LastEpisodeTorrentLinkMP4 != null && oldOne.LastEpisodeTorrentLinkMP4 == null) ||
                   (newOne.LastEpisodeTorrentLinkSD != null && oldOne.LastEpisodeTorrentLinkSD == null);
        }

        private Series ParseSeries(FeedItemResponse feedItem)
        {
            var serial = new Series()
            {
                Name = feedItem.GetSerialName(),
                LastEpisodeName = feedItem.GetEpisodeName(),
                LastEpisode = feedItem.PublishDateParsed,
            };

            var quality = feedItem.GetQuality();
            switch (quality)
            {
                case Quality.H1080:
                    serial.LastEpisodeTorrentLink1080 = feedItem.Link;
                    break;
                case Quality.H720:
                    serial.LastEpisodeTorrentLinkMP4 = feedItem.Link;
                    break;
                case Quality.SD:
                    serial.LastEpisodeTorrentLinkSD = feedItem.Link;
                    break;
            }

            return serial;
        }

        private void Merge(Series to, Series from)
        {
            if (from.LastEpisodeName != to.LastEpisodeName || from.LastEpisodeTorrentLink1080 != null)
            {
                to.LastEpisodeTorrentLink1080 = from.LastEpisodeTorrentLink1080;
            }

            if (from.LastEpisodeName != to.LastEpisodeName || from.LastEpisodeTorrentLinkMP4 != null)
            {
                to.LastEpisodeTorrentLinkMP4 = from.LastEpisodeTorrentLinkMP4;
            }

            if (from.LastEpisodeName != to.LastEpisodeName || from.LastEpisodeTorrentLinkSD != null)
            {
                to.LastEpisodeTorrentLinkSD = from.LastEpisodeTorrentLinkSD;
            }

            to.LastEpisodeName = from.LastEpisodeName;
            to.LastEpisode = from.LastEpisode;
        }

        private async Task PrepareUserFeeds(FeedItemResponse item)
        {
            var serial = item.GetSerialName();
            var quality = item.GetQuality();
            var subscriptions = await this.subscriptionDAO.LoadAsync(serial, quality);
            this.logger.Info($"{subscriptions.Count()} subscriptions should be updated for serial '{serial}' with quality '{quality}'");
            foreach (var subscription in subscriptions)
            {
                var torrentId = item.GetTorrentId();
                var link = Extensions.GenerateTorrentLink(subscription.UserId, torrentId);
                var userFeedItem = new FeedItem(item, link);
                var userFeed = await this.feedDAO.LoadUserFeedAsync(subscription.UserId);
                userFeed.Add(userFeedItem);
                await this.feedDAO.SaveUserFeedAsync(subscription.UserId, userFeed.Take(15).ToArray());
                this.logger.Info($"Feed for user {subscription.UserId} updated.");
            }
        }
    }
}
