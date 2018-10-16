using LostFilmMonitoring.BLL.Interfaces;
using LostFilmMonitoring.BLL.Models;
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
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IConfigurationService _configurationService;

        public FeedService(IConfigurationService configurationService, ICurrentUserProvider currentUserProvider)
        {
            var connectionString = configurationService.GetConnectionString();
            _configurationService = configurationService;
            _serialDao = new SerialDAO(connectionString);
            _feedDAO = new FeedDAO(configurationService.GetBasePath());
            _subscriptionDAO = new SubscriptionDAO(connectionString);
            _userDAO = new UserDAO(connectionString);
            _serialCoverService = new SerialCoverService(configurationService.GetImagesDirectory());
            _currentUserProvider = currentUserProvider;
        }

        public async Task<Stream> GetRss(Guid userId)
        {
            if (!await _userDAO.UpdateLastActivity(userId)) return null;
            return _feedDAO.LoadFeedRawAsync(userId.ToString());
        }

        public async Task<SortedSet<FeedItem>> GetItems()
        {
            var userId = _currentUserProvider.GetCurrentUserId();
            if (!await _userDAO.UpdateLastActivity(userId)) return null;
            return await _feedDAO.LoadUserFeedAsync(userId);
        }

        public async Task Update()
        {
            var newItems = LostFilmFeedService.Load();
            await UpdateSerialList(newItems);
            var oldItems = await _feedDAO.LoadBaseFeedAsync();
            foreach (var item in newItems)
            {
                if (!oldItems.Contains(item))
                {
                    oldItems.Add(item);
                    await PrepareFeeds(item);
                }
            }

            await _feedDAO.SaveBaseFeedAsync(oldItems.Take(15).ToArray());
        }

        private async Task PrepareFeeds(FeedItem item)
        {
            var subscriptions = await _subscriptionDAO.LoadAsync(item.Serial());
            foreach (var subscription in subscriptions)
            {
                var link = await TorrentFilePathService.GetTorrentLink(
                    item.Link, subscription.User.Cookie, subscription.Quality);
                if (link == null) continue;
                var userFeedItem = new FeedItem(item, link);
                var userFeed = await _feedDAO.LoadUserFeedAsync(subscription.User.Id);
                userFeed.Add(userFeedItem);
                await _feedDAO.SaveUserFeedAsync(subscription.User.Id, userFeed.Take(15).ToArray());
            }
        }

        public async Task UpdateUserFeed(SelectedFeedItem[] selectedItems)
        {
            var userId = _currentUserProvider.GetCurrentUserId();
            var user = await _userDAO.LoadAsync(userId);
            var userFeedItems = await _feedDAO.LoadUserFeedAsync(userId);
            var newItems = selectedItems.Where(i => userFeedItems.All(f => !f.Title.StartsWith(i.Serial, StringComparison.OrdinalIgnoreCase))).ToList();
            foreach(var newItem in newItems)
            {
                var serial = await _serialDao.LoadAsync(newItem.Serial);
                var torrentLink = await TorrentFilePathService.GetTorrentLink(serial.LastEpisodeLink, user.Cookie, newItem.Quality);
                if (torrentLink == null) continue;
                var userFeedItem = new FeedItem() {
                    Link = torrentLink,
                    Title = newItem.Serial,
                    PublishDateParsed = DateTime.UtcNow,
                    PublishDate = DateTime.UtcNow.ToString()
                };
                userFeedItems.Add(userFeedItem);
            }

            await _feedDAO.SaveUserFeedAsync(userId, userFeedItems.Take(15).ToArray());
        }

        private async Task UpdateSerialList(IEnumerable<FeedItem> feedItems)
        {
            var oldSerials = await _serialDao.LoadAsync();
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

                var oldSerial = oldSerials.FirstOrDefault(s => s.Name == serial.Name);
                if (oldSerial != null && oldSerial.LastEpisode >= serial.LastEpisode) continue;
                serial.LastEpisodeTorrentLinkSD = await TorrentFilePathService.GetTorrentLink(serial.LastEpisodeLink, baseFeedCookie, "SD");
                serial.LastEpisodeTorrentLink1080 = await TorrentFilePathService.GetTorrentLink(serial.LastEpisodeLink, baseFeedCookie, "1080");
                serial.LastEpisodeTorrentLinkMP4 = await TorrentFilePathService.GetTorrentLink(serial.LastEpisodeLink, baseFeedCookie, "MP4");
                await _serialDao.SaveAsync(serial);
                if (oldSerial == null) oldSerials.Add(serial);
                if (oldSerial.LastEpisode < serial.LastEpisode) oldSerial.LastEpisode = serial.LastEpisode;
            }
        }
    }
}