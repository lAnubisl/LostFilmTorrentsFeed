using LostFilmMonitoring.BLL.Interfaces;
using LostFilmMonitoring.Common;
using LostFilmMonitoring.DAO;
using LostFilmMonitoring.DAO.DomainModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LostFilmMonitoring.BLL.Implementations
{
    public class LostFilmFeedService : ILostFilmFeedService
    {
        private readonly ILogger _logger;
        public LostFilmFeedService(ILogger logger)
        {
            _logger = logger.CreateScope(nameof(LostFilmFeedService));
        }

        public async Task<SortedSet<FeedItem>> LoadFeedItems()
        {
            var rss = await DownloadRssText();
            rss = FixAmpBug(rss);
            XDocument document = null;
            try
            {
                document = XDocument.Parse(rss);
            }
            catch(Exception ex)
            {
                _logger.Log(ex);
            }

            return document.GetItems();
        }

        private async Task<string> DownloadRssText()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    return await client.GetStringAsync("http://www.lostfilm.tv/rss.xml");
                }
                catch(Exception ex)
                {
                    _logger.Log(ex);
                    throw ex;
                }
            }
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
