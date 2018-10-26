using LostFilmMonitoring.BLL.Interfaces;
using LostFilmMonitoring.BLL.Models;
using LostFilmMonitoring.Common;
using LostFilmMonitoring.DAO.DAO;
using LostFilmMonitoring.DAO.DomainModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LostFilmMonitoring.BLL.Implementations
{
    public class PresentationService : IPresentationService
    {
        private readonly SerialDAO _serialDAO;
        private readonly UserDAO _userDAO;
        private readonly SubscriptionDAO _subscriptionDAO;
        private readonly ILostFilmRegistrationService _registrationService;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IFeedService _feedService;
        private readonly ILogger _logger;

        public PresentationService(IConfigurationService configurationService, ILostFilmRegistrationService registrationService, ICurrentUserProvider currentUserProvider, IFeedService feedService, ILogger logger)
        {
            var connectionString = configurationService.GetConnectionString();
            _serialDAO = new SerialDAO(connectionString);
            _userDAO = new UserDAO(connectionString);
            _feedService = feedService;
            _subscriptionDAO = new SubscriptionDAO(connectionString);
            _registrationService = registrationService;
            _currentUserProvider = currentUserProvider;
            _logger = logger.CreateScope(nameof(PresentationService));
        }

        public async Task<IndexModel> GetIndexModel()
        {
            var serials = await _serialDAO.LoadAsync();
            var currentUserId = _currentUserProvider.GetCurrentUserId();
            var user = currentUserId == Guid.Empty ? null : await _userDAO.LoadAsync(currentUserId);
            return new IndexModel(serials, user);
        }

        public async Task<RegistrationResultModel> Register(string captcha, string captchaCookie)
        {
            var registrationResult = await _registrationService.Register(captchaCookie, captcha);
            if (!registrationResult.Success) return registrationResult;
            var user = new User
            {
                Cookie = registrationResult.Cookie,
                LastActivity = DateTime.UtcNow,
            };

            var userId = await _userDAO.CreateAsync(user);
            _currentUserProvider.SetCurrentUserId(userId);
            _logger.Info("New user registered.");
            return registrationResult;
        }

        public async Task UpdateSubscriptions(SelectedFeedItem[] selectedItems)
        {
            var currentUserId = _currentUserProvider.GetCurrentUserId();
            if (currentUserId == Guid.Empty) return;
            await _feedService.UpdateUserFeed(selectedItems);
            await _subscriptionDAO.SaveAsync(
                currentUserId, 
                selectedItems?
                .Select(s => new Subscription() { Quality = s.Quality, Serial = s.Serial })
                .ToArray());
            _logger.Info($"Subscriptions updated for user {currentUserId}");
        }

        public async Task<bool> Authenticate(Guid userId)
        {
            var user = await _userDAO.LoadAsync(userId);
            if (user == null) return false;
            _currentUserProvider.SetCurrentUserId(userId);
            return true;
        }
    }
}