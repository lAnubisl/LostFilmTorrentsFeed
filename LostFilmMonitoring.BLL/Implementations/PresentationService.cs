using LostFilmMonitoring.BLL.Interfaces;
using LostFilmMonitoring.BLL.Models;
using LostFilmMonitoring.DAO.DAO;
using LostFilmMonitoring.DAO.DomainModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LostFilmMonitoring.BLL.Implementations
{
    public class PresentationService
    {
        private readonly SerialDAO _serialDAO;
        private readonly UserDAO _userDAO;
        private readonly FeedDAO _feedDAO;

        public PresentationService(IConfigurationService configurationService)
        {
            var connectionString = configurationService.GetConnectionString();
            _serialDAO = new SerialDAO(connectionString);
            _userDAO = new UserDAO(connectionString);
            _feedDAO = new FeedDAO(configurationService.GetBasePath());
        }

        public async Task<RegistrationViewModel> GetRegistrationViewModel(Guid userId)
        {
            var serials = await _serialDAO.LoadAsync();
            var user = await _userDAO.LoadAsync(userId);
            return new RegistrationViewModel(serials, user);
        }

        public Task RemoveOldUsers()
        {
            return _userDAO.DeleteOldUsersAsync();
        }

        public async Task<Guid> Register(RegistrationViewModel model)
        {
            var user = new User
            {
                Id = model.UserId,
                Cookie = model.Cookie,
                LastActivity = DateTime.UtcNow,
                Subscriptions = model.SelectedItems.Select(i => new Subscription
                {
                    UserId = model.UserId,
                    Serial = i.Serial,
                    Quality = i.Quality
                }).ToList()
            };
            
            var userId = await _userDAO.SaveAsync(user);
            if (model.UserId == Guid.Empty)
            {
                await _feedDAO.SaveUserFeedAsync(userId, new FeedItem[0]);
            }
            return userId;
        }
    }
}