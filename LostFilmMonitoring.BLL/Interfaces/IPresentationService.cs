using System;
using System.Threading.Tasks;
using LostFilmMonitoring.BLL.Models;

namespace LostFilmMonitoring.BLL.Interfaces
{
    public interface IPresentationService
    {
        Task<IndexModel> GetIndexModel();
        Task<bool> Authenticate(Guid userId);
        Task<RegistrationResultModel> Register(string captcha, string captchaCookie);
        Task RemoveOldUsers();
        Task UpdateSubscriptions(SelectedFeedItem[] selectedItems);
    }
}