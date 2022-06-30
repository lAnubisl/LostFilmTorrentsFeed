// <copyright file="GetUserCommand.cs" company="Alexander Panfilenok">
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
    /// Get user information.
    /// </summary>
    public class GetUserCommand : ICommand<GetUserRequestModel, GetUserResponseModel>
    {
        private readonly IUserDAO userDao;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUserCommand"/> class.
        /// </summary>
        /// <param name="userDao">Instance of <see cref="IUserDAO"/>.</param>
        /// <param name="logger">Instance of <see cref="ILogger"/>.</param>
        public GetUserCommand(IUserDAO userDao, ILogger logger)
        {
            this.logger = logger?.CreateScope(nameof(GetUserCommand)) ?? throw new ArgumentNullException(nameof(logger));
            this.userDao = userDao ?? throw new ArgumentNullException(nameof(userDao));
        }

        /// <inheritdoc/>
        public async Task<GetUserResponseModel> ExecuteAsync(GetUserRequestModel? request)
        {
            this.logger.Info($"Call: {nameof(this.ExecuteAsync)}(GetUserRequestModel request)");
            if (request == null)
            {
                return new GetUserResponseModel(ValidationResult.Fail(ErrorMessages.RequestNull));
            }

            if (request.UserId == null)
            {
                return new GetUserResponseModel(ValidationResult.Fail(nameof(GetUserRequestModel.UserId), ErrorMessages.FieldEmpty));
            }

            var user = await this.userDao.LoadAsync(request.UserId);
            if (user == null)
            {
                return new GetUserResponseModel(ValidationResult.Fail(nameof(GetUserRequestModel.UserId), ErrorMessages.UserDoesNotExist, request.UserId));
            }

            return new GetUserResponseModel(user);
        }
    }
}
