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
            using (var client = new HttpClient())
            {
                try
                {
                    return await client.GetStringAsync(rssUri);
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