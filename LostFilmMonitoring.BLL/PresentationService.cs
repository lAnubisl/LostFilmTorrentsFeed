// <copyright file="PresentationService.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2021 Alexander Panfilenok
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the 'Software'), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>

namespace LostFilmMonitoring.BLL
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using LostFilmMonitoring.BLL.Models;
    using LostFilmMonitoring.Common;
    using LostFilmMonitoring.DAO.DAO;
    using LostFilmMonitoring.DAO.DomainModels;

    /// <summary>
    /// Manages user presentation.
    /// </summary>
    public class PresentationService
    {
        private readonly SeriesDAO serialDAO;
        private readonly UserDAO userDAO;
        private readonly SubscriptionDAO subscriptionDAO;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly RssFeedService feedService;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PresentationService"/> class.
        /// </summary>
        /// <param name="currentUserProvider">CurrentUserProvider.</param>
        /// <param name="logger">Logger.</param>
        public PresentationService(ICurrentUserProvider currentUserProvider, ILogger logger)
        {
            var connectionString = Configuration.GetConnectionString();
            this.serialDAO = new SeriesDAO(connectionString);
            this.userDAO = new UserDAO(connectionString);
            this.feedService = new RssFeedService(currentUserProvider, logger);
            this.subscriptionDAO = new SubscriptionDAO(connectionString);
            this.currentUserProvider = currentUserProvider;
            this.logger = logger.CreateScope(nameof(PresentationService));
        }

        /// <summary>
        /// Gets home page model.
        /// </summary>
        /// <returns>Home Page model.</returns>
        public async Task<IndexViewModel> GetIndexModelAsync()
        {
            var serials = await this.serialDAO.LoadAsync();
            var currentUserId = this.currentUserProvider.GetCurrentUserId();
            var user = currentUserId == Guid.Empty ? null : await this.userDAO.LoadWithSubscriptionsAsync(currentUserId);
            return new IndexViewModel(serials, user);
        }

        /// <summary>
        /// Registers new user.
        /// </summary>
        /// <param name="model">Registration model.</param>
        /// <returns>Awaitable task.</returns>
        public async Task RegisterAsync(RegistrationModel model)
        {
            var user = new User
            {
                Cookie = model.If_session,
                LastActivity = DateTime.UtcNow,
                Usess = model.Usess,
                Uid = model.Uid,
            };

            var userId = await this.userDAO.CreateAsync(user);
            this.currentUserProvider.SetCurrentUserId(userId);
            this.logger.Info("New user registered.");
        }

        /// <summary>
        /// Updates user subscription based on new user choice.
        /// </summary>
        /// <param name="selectedItems">Selected series.</param>
        /// <returns>Awaitable task.</returns>
        public async Task UpdateSubscriptionsAsync(SelectedFeedItem[] selectedItems)
        {
            var currentUserId = this.currentUserProvider.GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                return;
            }

            await this.feedService.UpdateUserSubscrptionAsync(selectedItems);
            await this.subscriptionDAO.SaveAsync(
                currentUserId,
                selectedItems?.Select(s => new Subscription() { Quality = s.Quality, SeriesName = s.SeriesName }).ToArray());
            this.logger.Info($"Subscriptions updated for user {currentUserId}");
        }

        /// <summary>
        /// Authenticates user.
        /// </summary>
        /// <param name="userId">UserId.</param>
        /// <returns>True - Authentication succeeded. Otherwise false.</returns>
        public async Task<bool> Authenticate(Guid userId)
        {
            var user = await this.userDAO.LoadWithSubscriptionsAsync(userId);
            if (user == null)
            {
                return false;
            }

            this.currentUserProvider.SetCurrentUserId(userId);
            return true;
        }
    }
}
