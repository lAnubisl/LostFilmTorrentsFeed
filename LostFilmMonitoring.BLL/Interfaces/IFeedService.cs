using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LostFilmMonitoring.BLL.Models;
using LostFilmMonitoring.DAO.DomainModels;

namespace LostFilmMonitoring.BLL.Interfaces
{
    public interface IFeedService
    {
        Task<SortedSet<FeedItem>> GetItems();
        Task<Stream> GetRss(Guid userId);
        Task Update();
        Task UpdateUserFeed(SelectedFeedItem[] selectedItems);
    }
}