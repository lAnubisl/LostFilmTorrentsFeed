// <copyright file="SignInCommand.cs" company="Alexander Panfilenok">
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
    /// Sign in user.
    /// </summary>
    public class SignInCommand : ICommand<SignInRequestModel, SignInResponseModel>
    {
        private readonly ILogger logger;
        private readonly IUserDAO userDao;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignInCommand"/> class.
        /// </summary>
        /// <param name="userDao">Instance of <see cref="IUserDAO"/>.</param>
        /// <param name="logger">Instance of <see cref="ILogger"/>.</param>
        public SignInCommand(IUserDAO userDao, ILogger logger)
        {
            this.userDao = userDao ?? throw new ArgumentNullException(nameof(userDao));
            this.logger = logger?.CreateScope(nameof(SignInCommand)) ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<SignInResponseModel> ExecuteAsync(SignInRequestModel? request)
        {
            this.logger.Info($"Call: {nameof(this.ExecuteAsync)}(SingInRequestModel request);");
            if (request == null)
            {
                return new SignInResponseModel { Success = false };
            }

            if (request.UserId == null)
            {
                return new SignInResponseModel { Success = false };
            }

            var user = await this.userDao.LoadAsync(request.UserId);
            return new SignInResponseModel { Success = user != null };
        }
    }
}
