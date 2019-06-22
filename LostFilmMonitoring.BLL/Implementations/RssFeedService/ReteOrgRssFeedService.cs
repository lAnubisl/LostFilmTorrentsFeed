using LostFilmMonitoring.BLL.Exceptions;
using LostFilmMonitoring.BLL.Interfaces;
using LostFilmMonitoring.Common;
using LostFilmMonitoring.DAO.DomainModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LostFilmMonitoring.BLL.Implementations.RssFeedService
{
    public class ReteOrgRssFeedService : BaseRssFeedService, IRssFeedService
    {
        public ReteOrgRssFeedService(ILogger logger) : base(logger.CreateScope(nameof(ReteOrgRssFeedService)))
        {
        }

        public async Task<SortedSet<FeedItem>> LoadFeedItems()
        {
            _logger.Info($"Call: {nameof(LoadFeedItems)}()");
            string rss;
            try
            {
                rss = await DownloadRssText("http://a.retre.org/rssdd.xml");
            }
            catch (RemoteServiceUnavailableException)
            {
                return new SortedSet<FeedItem>();
            }
            
            return GetItems(rss);
        }
    }
}