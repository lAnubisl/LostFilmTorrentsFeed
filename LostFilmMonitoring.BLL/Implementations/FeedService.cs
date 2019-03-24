using LostFilmMonitoring.BLL.Interfaces;
using LostFilmMonitoring.BLL.Models;
using LostFilmMonitoring.Common;
using LostFilmMonitoring.DAO.DAO;
using LostFilmMonitoring.DAO.DomainModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly TorrentFilePathService _torrentFilePathService;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IConfigurationService _configurationService;
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
            _torrentFilePathService = new TorrentFilePathService(logger);
        }

        public async Task<Stream> GetRss(Guid userId)
        {
            if (!await _userDAO.UpdateLastActivity(userId)) return null;
            return _feedDAO.LoadFeedRawAsync(userId);
        }

        public async Task Update()
        {
            var newItems = LostFilmFeedService.Load();
            await UpdateSerialList(newItems);
            var existingItems = await _feedDAO.LoadBaseFeedAsync();
            foreach (var item in newItems)
            {
                if (!existingItems.Contains(item))
                {
                    existingItems.Add(item);
                    await PrepareFeeds(item);
                }
            }

            await _feedDAO.SaveBaseFeedAsync(existingItems.Take(15).ToArray());
            _logger.Info("Base feed updated");
        }

        private async Task PrepareFeeds(FeedItem item)
        {
            var subscriptions = await _subscriptionDAO.LoadAsync(item.Serial());
            _logger.Info($"{subscriptions.Count()} subscriptions should be updated for serial {item.Serial()}");
            foreach (var subscription in subscriptions)
            {
                var link = await _torrentFilePathService.GetTorrentLink(
                    item.Link, subscription.User.Cookie, subscription.Quality);
                if (link == null) continue;
                var userFeedItem = new FeedItem(item, link);
                var userFeed = await _feedDAO.LoadUserFeedAsync(subscription.User.Id);
                userFeed.Add(userFeedItem);
                await _feedDAO.SaveUserFeedAsync(subscription.User.Id, userFeed.Take(15).ToArray());
                _logger.Info($"Feed for user {subscription.User.Id} updated.");
            }
        }

        public async Task UpdateUserFeed(SelectedFeedItem[] selectedItems)
        {
            if (selectedItems == null) return;
            var userId = _currentUserProvider.GetCurrentUserId();
            if (userId == Guid.Empty) return;
            var user = await _userDAO.LoadAsync(userId);
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
                var newFeedItem = new FeedItem()
                {
                    Link = GetTorrentLink(newItem, serial),
                    Title = serial.LastEpisodeName,
                    PublishDateParsed = DateTime.UtcNow,
                    PublishDate = DateTime.UtcNow.ToString()
                };
                userFeedItems.RemoveWhere(i => string.Equals(i.Title, newFeedItem.Title));
                userFeedItems.Add(newFeedItem);
            }

            await _feedDAO.SaveUserFeedAsync(userId, userFeedItems.Take(15).ToArray());
        }

        private static string GetTorrentLink(SelectedFeedItem item, Serial serial)
        {
            switch (item.Quality)
            {
                case "SD": return serial.LastEpisodeTorrentLinkSD;
                case "1080": return serial.LastEpisodeTorrentLink1080;
                case "MP4": return serial.LastEpisodeTorrentLinkMP4;
                default: throw new InvalidOperationException("Quality not supported");
            }
        }

        private async Task UpdateSerialList(IEnumerable<FeedItem> feedItems)
        {
            var existingSerials = await _serialDao.LoadAsync();
            var baseFeedCookie = _configurationService.BaseFeedCookie();
            foreach (var feedItem in feedItems)
            {
                await _serialCoverService.EnsureImageDownloaded(feedItem);
                var serial = new Serial()
                {
                    Name = feedItem.Serial(),
                    LastEpisodeName = feedItem.Title,
                    LastEpisode = feedItem.PublishDateParsed,
                    LastEpisodeLink = feedItem.Link
                };

                var existingSerial = existingSerials.FirstOrDefault(s => s.Name == serial.Name);
                if (existingSerial != null && existingSerial.LastEpisode >= serial.LastEpisode) continue;
                serial.LastEpisodeTorrentLinkSD = await _torrentFilePathService.GetTorrentLink(serial.LastEpisodeLink, baseFeedCookie, "SD");
                serial.LastEpisodeTorrentLink1080 = await _torrentFilePathService.GetTorrentLink(serial.LastEpisodeLink, baseFeedCookie, "1080");
                serial.LastEpisodeTorrentLinkMP4 = await _torrentFilePathService.GetTorrentLink(serial.LastEpisodeLink, baseFeedCookie, "MP4");
                await _serialDao.SaveAsync(serial);

                
                if (existingSerial == null)
                {
                    _logger.Info($"New serial detected: {serial.Name}");
                    existingSerials.Add(serial);
                }

                if (existingSerial != null && existingSerial.LastEpisode < serial.LastEpisode)
                {
                    _logger.Info($"New episode detected: {serial.Name}");
                    existingSerial.LastEpisode = serial.LastEpisode;
                }
            }
        }

        public async Task<FeedViewModel> GetFeedViewModel()
        {
            var userId = _currentUserProvider.GetCurrentUserId();
            if (!await _userDAO.UpdateLastActivity(userId)) return null;
            return new FeedViewModel(await _feedDAO.LoadUserFeedAsync(userId), userId);
        }
    }
}