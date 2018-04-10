using LostFilmMonitoring.DAO;
using LostFilmMonitoring.DAO.DomainModels;
using System.Collections.Generic;
using System.Xml.Linq;

namespace LostFilmMonitoring.BLL
{
    public class LostFilmFeedService
    {
        public static SortedSet<FeedItem> Load()
        {
            var document = XDocument.Load("http://www.lostfilm.tv/rss.xml");
            return document.GetItems();
        }
    }
}
