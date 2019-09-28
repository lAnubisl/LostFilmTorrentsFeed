using LostFilmMonitoring.BLL.Interfaces;
using LostFilmMonitoring.Common;
using LostFilmMonitoring.DAO.DomainModels;
using LostFilmMonitoring.DAO.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostFilmMonitoring.BLL
{
    public sealed class LostFilmUpdateChecker
    {
        private readonly ILogger _logger;
        private readonly IRssFeedService _rssFeedService;
        private readonly ISerialDAO _serialDao;
        private readonly IConfigurationService _configurationService;
        private readonly ISerialCoverService _serialCoverService;
        public async Task CheckUpdatesAsync()
        {
            _logger.Info($"Call: {nameof(CheckUpdatesAsync)}()");
            var newItems = await _rssFeedService.LoadFeedItems();
            if (!newItems.Any())
            {
                _logger.Error($"IRssFeedService returned 0 items.");
                return;
            }

            await UpdateSerialList(newItems);
        }

        private async Task UpdateSerialList(IEnumerable<FeedItem> feedItems)
        {
            var existingSerials = await _serialDao.LoadAsync();
            var baseFeedCookie = _configurationService.BaseFeedCookie();
            foreach (var feedItem in feedItems)
            {
                var serial = feedItem.ParseSerial();
                await _serialCoverService.EnsureImageDownloaded(feedItem);
                var existingSerial = existingSerials.FirstOrDefault(s => s.Name == serial.Name);
                if (existingSerial != null) continue;
                _logger.Info($"New serial detected: {serial.Name}");
                existingSerials.Add(serial);
                await _serialDao.SaveAsync(serial);
            }
        }
    }
}