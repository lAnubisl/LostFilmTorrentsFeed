using LostFilmMonitoring.DAO.DomainModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LostFilmMonitoring.BLL.Interfaces
{
    public interface IRssFeedService
    {
        Task<SortedSet<FeedItem>> LoadFeedItems();
    }
}