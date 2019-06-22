using LostFilmMonitoring.Common;
using LostFilmMonitoring.DAO;
using LostFilmMonitoring.DAO.DomainModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LostFilmMonitoring.BLL.Implementations.RssFeedService
{
    public abstract class BaseRssFeedService
    {
        protected readonly ILogger _logger;
        public BaseRssFeedService(ILogger logger)
        {
            _logger = logger;
        }

        protected async Task<string> DownloadRssText(string rssUri)
        {
            _logger.Info($"Call: {nameof(DownloadRssText)}({rssUri})");
            using (var client = new HttpClient())
            {
                try
                {
                    var rssText = await client.GetStringAsync(rssUri);
                    _logger.Debug(rssText);
                    return rssText;
                }
                catch (Exception ex)
                {
                    _logger.Log(ex);
                    throw ex;
                }
            }
        }

        protected SortedSet<FeedItem> GetItems(string rssText)
        {
            _logger.Info($"Call: {nameof(GetItems)}(rssText)");
            XDocument document;
            try
            {
                document = XDocument.Parse(rssText);
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                return null;
            }

            return document.GetItems();
        }
    }
}