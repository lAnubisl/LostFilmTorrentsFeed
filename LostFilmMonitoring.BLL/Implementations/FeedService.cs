using LostFilmMonitoring.BLL.Interfaces;
using LostFilmMonitoring.DAO.DAO;
using LostFilmMonitoring.DAO.DomainModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LostFilmMonitoring.BLL.Implementations
{
    public class FeedService
    {
        private readonly UserDAO _userDAO;
        private readonly FeedDAO _feedDAO;
        private readonly SerialDAO _serialDao;
        private readonly SubscriptionDAO _subscriptionDAO;

        public FeedService(IConfigurationService configurationService)
        {
            var connectionString = configurationService.GetConnectionString();
            _serialDao = new SerialDAO(connectionString);
            _feedDAO = new FeedDAO(configurationService.GetBasePath());
            _subscriptionDAO = new SubscriptionDAO(connectionString);
            _userDAO = new UserDAO(connectionString);
        }

        public async Task<Stream> GetRss(Guid userId)
        {
            if (!await _userDAO.UpdateLastActivity(userId)) return null;
            return _feedDAO.LoadFeedRawAsync(userId.ToString());
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
                var userFeedItem = new FeedItem
                {
                    Title = item.Title,
                    PublishDate = item.PublishDate,
                };
                userFeedItem.Link = await TorrentFilePathService.GetTorrentLink(
                    item.Link, subscription.User.Cookie, subscription.Quality);
                if (userFeedItem.Link == null) continue;
                var userFeed = await _feedDAO.LoadUserFeedAsync(subscription.User.Id);
                userFeed.Add(userFeedItem);
                await _feedDAO.SaveUserFeedAsync(subscription.User.Id, userFeed.Take(15).ToArray());
            }
        }

        private async Task UpdateSerialList(IEnumerable<FeedItem> feedItems)
        {
            var oldSerials = await _serialDao.LoadAsync();
            foreach (var feedItem in feedItems)
            {
                var serial = feedItem.Serial();
                if (!oldSerials.Any(s => s.Name == serial))
                {
                    var newSerial = new Serial() { Name = serial };
                    await _serialDao.SaveAsync(newSerial);
                    oldSerials.Add(newSerial);
                }
            }
        }
    }
}