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
        private readonly SeriesDAO seriesDAO;
        private readonly SeriesCoverService seriesCoverService;
        private readonly SubscriptionDAO subscriptionDAO;

        /// <summary>
        /// Initializes a new instance of the <see cref="RssFeedUpdaterService"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="reteOrgRssFeed">ReteOrgRssFeed.</param>
        /// <param name="feedDAO">FeedDAO.</param>
        /// <param name="seriesDAO">SeriesDAO.</param>
        /// <param name="subscriptionDAO">SubscriptionDAO.</param>
        /// <param name="seriesCoverService">SeriesCoverService.</param>
        public RssFeedUpdaterService(ILogger logger, ReteOrgRssFeed reteOrgRssFeed, FeedDAO feedDAO, SeriesDAO seriesDAO, SubscriptionDAO subscriptionDAO, SeriesCoverService seriesCoverService)
        {
            this.logger = logger != null ? logger.CreateScope(nameof(RssFeedUpdaterService)) : throw new ArgumentNullException(nameof(logger));
            this.reteOrgRssFeed = reteOrgRssFeed ?? throw new ArgumentNullException(nameof(reteOrgRssFeed));
            this.feedDAO = feedDAO ?? throw new ArgumentNullException(nameof(feedDAO));
            this.seriesDAO = seriesDAO ?? throw new ArgumentNullException(nameof(seriesDAO));
            this.subscriptionDAO = subscriptionDAO ?? throw new ArgumentNullException(nameof(subscriptionDAO));
            this.seriesCoverService = seriesCoverService ?? throw new ArgumentNullException(nameof(seriesCoverService));
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

            await this.UpdateSeriesList(feedItemResponses);
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

        private async Task UpdateSeriesList(IEnumerable<FeedItemResponse> feedItems)
        {
            var allSeries = await this.seriesDAO.LoadAsync();
            var baseFeedCookie = Configuration.BaseFeedCookie();
            foreach (var feedItem in feedItems)
            {
                var series = this.ParseSeries(feedItem);
                await this.seriesCoverService.EnsureCoverDownloadedAsync(series.Name);
                var existing = allSeries.FirstOrDefault(s => s.Name == series.Name);
                if (existing != null && existing.LastEpisodeName == series.LastEpisodeName && !this.HasUpdatesComparedTo(series, existing))
                {
                    continue;
                }

                if (existing == null)
                {
                    this.logger.Info($"New series detected: {series.Name}");
                    allSeries.Add(series);
                    await this.seriesDAO.SaveAsync(series);
                    continue;
                }

                this.Merge(existing, series);
                await this.seriesDAO.SaveAsync(existing);
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
            var series = new Series()
            {
                Name = feedItem.GetSeriesName(),
                LastEpisodeName = feedItem.GetEpisodeName(),
                LastEpisode = feedItem.PublishDateParsed,
            };

            var quality = feedItem.GetQuality();
            switch (quality)
            {
                case Quality.H1080:
                    series.LastEpisodeTorrentLink1080 = feedItem.Link;
                    break;
                case Quality.H720:
                    series.LastEpisodeTorrentLinkMP4 = feedItem.Link;
                    break;
                case Quality.SD:
                    series.LastEpisodeTorrentLinkSD = feedItem.Link;
                    break;
            }

            return series;
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
            var seriesName = item.GetSeriesName();
            var quality = item.GetQuality();
            var subscriptions = await this.subscriptionDAO.LoadAsync(seriesName, quality);
            this.logger.Info($"{subscriptions.Count()} subscriptions should be updated for series '{seriesName}' with quality '{quality}'");
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
