using System;
using System.IO;
using System.Threading.Tasks;
using LostFilmMonitoring.BLL.Interfaces.Models;
using LostFilmMonitoring.BLL.Models;

namespace LostFilmMonitoring.BLL.Interfaces
{
    public interface IFeedService
    {
        Task<FeedViewModel> GetFeedViewModel();
        Task<Stream> GetRss(Guid userId);
        Task<RssItemViewModel> GetRssItem(Guid userId, int id);
        Task Update();
        Task UpdateUserFeed(SelectedFeedItem[] selectedItems);
    }
}