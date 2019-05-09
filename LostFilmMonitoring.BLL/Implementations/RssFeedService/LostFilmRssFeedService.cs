using LostFilmMonitoring.BLL.Interfaces;
using LostFilmMonitoring.Common;
using LostFilmMonitoring.DAO.DomainModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LostFilmMonitoring.BLL.Implementations.RssFeedService
{
    public class LostFilmRssFeedService : BaseRssFeedService, IRssFeedService
    {
        public LostFilmRssFeedService(ILogger logger) : base(logger.CreateScope(nameof(ReteOrgRssFeedService)))
        {
        }

        public async Task<SortedSet<FeedItem>> LoadFeedItems()
        {
            var rssText = await DownloadRssText("http://www.lostfilm.tv/rss.xml");
            rssText = FixAmpBug(rssText);
            return GetItems(rssText);
        }

        private static string FixAmpBug(string rss)
        {
            var ampIndex = rss.IndexOf('&');
            if (ampIndex == -1) return rss;

            var escapedAmpIndex = rss.IndexOf("&amp;");
            if (ampIndex == escapedAmpIndex)
            {
                return rss.Substring(0, ampIndex + 1) + FixAmpBug(rss.Substring(ampIndex + 1));
            }

            rss = rss.Insert(ampIndex + 1, "amp;");
            return FixAmpBug(rss);
        }
    }
}