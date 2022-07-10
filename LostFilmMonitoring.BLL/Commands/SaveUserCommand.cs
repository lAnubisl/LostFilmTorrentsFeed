// <copyright file="SaveUserCommand.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.BLL.Commands
{
    /// <summary>
    /// Saves user.
    /// </summary>
    public class SaveUserCommand : ICommand<EditUserRequestModel, EditUserResponseModel>
    {
        private readonly IUserDao userDAO;
        private readonly IFeedDao feedDAO;
        private readonly ILogger logger;
        private readonly IModelPersister persister;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveUserCommand"/> class.
        /// </summary>
        /// <param name="userDao">Instance of <see cref="IUserDao"/>.</param>
        /// <param name="logger">Instance of <see cref="ILogger"/>.</param>
        /// <param name="persister">Instance of <see cref="IModelPersister"/>.</param>
        /// <param name="feedDao">Instance of <see cref="IFeedDao"/>.</param>
        public SaveUserCommand(IUserDao userDao, ILogger logger, IModelPersister persister, IFeedDao feedDao)
        {
            this.userDAO = userDao ?? throw new ArgumentNullException(nameof(userDao));
            this.logger = logger?.CreateScope(nameof(SaveUserCommand)) ?? throw new ArgumentNullException(nameof(logger));
            this.persister = persister ?? throw new ArgumentNullException(nameof(persister));
            this.feedDAO = feedDao ?? throw new ArgumentNullException(nameof(feedDao));
        }

        /// <summary>
        /// Registers new user.
        /// </summary>
        /// <param name="request">Registration model.</param>
        /// <returns>Awaitable task.</returns>
        public async Task<EditUserResponseModel> ExecuteAsync(EditUserRequestModel? request)
        {
            this.logger.Info($"Call: {nameof(this.ExecuteAsync)}(EditUserRequestModel model)");
            if (request == null)
            {
                return new EditUserResponseModel(ValidationResult.Fail(ErrorMessages.RequestNull));
            }

            if (request.TrackerId == null)
            {
                return new EditUserResponseModel(ValidationResult.Fail(nameof(EditUserRequestModel.TrackerId), ErrorMessages.FieldEmpty));
            }

            var userId = string.IsNullOrWhiteSpace(request.UserId) ? Guid.NewGuid().ToString() : request.UserId;
            var user = new User(userId, request.TrackerId);
            await this.userDAO.SaveAsync(user);
            await this.persister.PersistAsync($"subscription_{user.Id}", Array.Empty<SubscriptionItem>());
            await this.feedDAO.SaveUserFeedAsync(user.Id, Array.Empty<FeedItem>());
            return new EditUserResponseModel(user.Id);
        }
    }
}
