using LostFilmMonitoring.BLL.Implementations.RssFeedService;
using LostFilmMonitoring.BLL.Interfaces;
using LostFilmMonitoring.BLL.Interfaces.Models;
using LostFilmMonitoring.BLL.Models;
using LostFilmMonitoring.Common;
using LostFilmMonitoring.DAO.DAO;
using LostFilmMonitoring.DAO.DomainModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LostFilmMonitoring.BLL.Implementations
{
    public class FeedService : IFeedService
    {
        private readonly UserDAO _userDAO;
        private readonly FeedDAO _feedDAO;
        private readonly SerialDAO _serialDao;
        private readonly SubscriptionDAO _subscriptionDAO;
        private readonly SerialCoverService _serialCoverService;
        private readonly TorrentFileDownloader _torrentFileDownloader;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IConfigurationService _configurationService;
        private readonly IRssFeedService _rssFeedService;
        private readonly ILogger _logger;

        public FeedService(IConfigurationService configurationService, ICurrentUserProvider currentUserProvider, ILogger logger)
        {
            var connectionString = configurationService.GetConnectionString();
            _configurationService = configurationService;
            _serialDao = new SerialDAO(connectionString);
            _logger = logger.CreateScope(nameof(FeedService));
            _feedDAO = new FeedDAO(configurationService.GetBasePath(), logger);
            _subscriptionDAO = new SubscriptionDAO(connectionString);
            _userDAO = new UserDAO(connectionString);
            _serialCoverService = new SerialCoverService(configurationService.GetImagesDirectory());
            _currentUserProvider = currentUserProvider;
            _rssFeedService = new ReteOrgRssFeedService(logger);
            _torrentFileDownloader = new TorrentFileDownloader(new TorrentFileDAO(configurationService.GetTorrentCachePath(), logger), logger);
        }

        public async Task<Stream> GetRss(Guid userId)
        {
            if (!await _userDAO.UpdateLastActivity(userId)) return null;
            return _feedDAO.LoadFeedRawAsync(userId);
        }

        public async Task Update()
        {
            _logger.Info($"Call: {nameof(Update)}()");
            var newItems = await _rssFeedService.LoadFeedItems();
            if (!newItems.Any()) return;
            await UpdateSerialList(newItems);
            var existingItems = await _feedDAO.LoadBaseFeedAsync();
            foreach (var item in newItems)
            {
                if (!existingItems.Contains(item))
                {
                    existingItems.Add(item);
                    await PrepareUserFeeds(item);
                }
            }

            await _feedDAO.SaveBaseFeedAsync(existingItems.Take(30).ToArray());
            _logger.Info("Base feed updated");
        }

        private async Task PrepareUserFeeds(FeedItem item)
        {
            var serial = item.ParseSerial();
            var quality = item.ParseQuality();
            var subscriptions = await _subscriptionDAO.LoadAsync(serial.Name, quality);
            _logger.Info($"{subscriptions.Count()} subscriptions should be updated for serial '{serial.Name}' with quality '{quality}'");
            foreach (var subscription in subscriptions)
            {
                await PrepareUserFeed(item, subscription);
            }
        }

        private async Task PrepareUserFeed(FeedItem item, Subscription subscription)
        {
            var torrentId = item.ParseId();
            var link = Extensions.GenerateTorrentLink(subscription.UserId, torrentId);
            var userFeedItem = new FeedItem(item, link);
            var userFeed = await _feedDAO.LoadUserFeedAsync(subscription.UserId);
            userFeed.Add(userFeedItem);
            await _feedDAO.SaveUserFeedAsync(subscription.UserId, userFeed.Take(15).ToArray());
            _logger.Info($"Feed for user {subscription.UserId} updated.");
        }

        public async Task UpdateUserFeed(SelectedFeedItem[] selectedItems)
        {
            if (selectedItems == null) return;
            var userId = _currentUserProvider.GetCurrentUserId();
            if (userId == Guid.Empty) return;
            var user = await _userDAO.LoadWithSubscriptionsAsync(userId);
            if (user == null) return;
            var userFeedItems = await _feedDAO.LoadUserFeedAsync(userId);
            if (userFeedItems == null) return;
            if (user.Subscriptions == null) user.Subscriptions = new List<Subscription>();
            var newItems = selectedItems.Where(
                i => user.Subscriptions.All(
                    s => !string.Equals(s.Serial, i.Serial, StringComparison.OrdinalIgnoreCase) || s.Quality != i.Quality)
                ).ToList();
            foreach (var newItem in newItems)
            {
                var serial = await _serialDao.LoadAsync(newItem.Serial);
                var torrentId = serial.GetTorrentId(newItem.Quality);
                if (torrentId == null)
                {
                    continue;
                }

                var newFeedItem = new FeedItem()
                {
                    Link = Extensions.GenerateTorrentLink(userId, torrentId),
                    Title = serial.LastEpisodeName,
                    PublishDateParsed = DateTime.UtcNow,
                    PublishDate = DateTime.UtcNow.ToString()
                };
                userFeedItems.RemoveWhere(i => string.Equals(i.Title, newFeedItem.Title));
                userFeedItems.Add(newFeedItem);
            }

            await _feedDAO.SaveUserFeedAsync(userId, userFeedItems.Take(15).ToArray());
        }

        private async Task UpdateSerialList(IEnumerable<FeedItem> feedItems)
        {
            var existingSerials = await _serialDao.LoadAsync();
            var baseFeedCookie = _configurationService.BaseFeedCookie();
            foreach (var feedItem in feedItems)
            {
                var serial = feedItem.ParseSerial();
                await _serialCoverService.EnsureImageDownloaded(serial.Name);
                var existingSerial = existingSerials.FirstOrDefault(s => s.Name == serial.Name);
                if (existingSerial != null && existingSerial.LastEpisodeName == serial.LastEpisodeName && !serial.HasUpdatesComparedTo(existingSerial))
                {
                    continue;
                }
                          
                if (existingSerial == null)
                {
                    _logger.Info($"New serial detected: {serial.Name}");
                    existingSerials.Add(serial);
                    await _serialDao.SaveAsync(serial);
                    continue;
                }

                existingSerial.Merge(serial);
                await _serialDao.SaveAsync(existingSerial);
            }
        }

        public async Task<FeedViewModel> GetFeedViewModel()
        {
            var userId = _currentUserProvider.GetCurrentUserId();
            if (!await _userDAO.UpdateLastActivity(userId))
            {
                _currentUserProvider.SetCurrentUserId(Guid.Empty);
                return null;
            }

            return new FeedViewModel(await _feedDAO.LoadUserFeedAsync(userId), userId);
        }

        public async Task<RssItemViewModel> GetRssItem(Guid userId, int id)
        {
            var user = await _userDAO.LoadAsync(userId);
            if (user == null) return null;
            var torrentFile = await _torrentFileDownloader.Download(user, id);
            if (torrentFile == null) return null;
            return new RssItemViewModel(torrentFile);
        }
    }
}