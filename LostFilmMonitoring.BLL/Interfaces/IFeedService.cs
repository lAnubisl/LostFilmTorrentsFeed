using System;
using System.IO;
using System.Threading.Tasks;
using LostFilmMonitoring.BLL.Models;

namespace LostFilmMonitoring.BLL.Interfaces
{
    public interface IFeedService
    {
        Task<FeedViewModel> GetFeedViewModel();
        Task<Stream> GetRss(Guid userId);
        Task Update();
        Task UpdateUserFeed(SelectedFeedItem[] selectedItems);
    }
}