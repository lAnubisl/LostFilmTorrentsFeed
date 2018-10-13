using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LostFilmMonitoring.DAO.DomainModels;

namespace LostFilmMonitoring.BLL.Interfaces
{
    public interface IFeedService
    {
        Task<SortedSet<FeedItem>> GetItems(Guid userId);
        Task<Stream> GetRss(Guid userId);
        Task Update();
    }
}