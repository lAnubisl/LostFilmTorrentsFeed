// <copyright file="IUserDao.cs" company="Alexander Panfilenok">
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

using LostFilmMonitoring.DAO.Interfaces.DomainModels;

namespace LostFilmMonitoring.DAO.Interfaces
{
    public interface IUserDAO
    {
        /// <summary>
        /// Delete users which were not be active for 1 month.
        /// </summary>
        /// <returns>User Ids which were deleted.</returns>
        Task<Guid[]> DeleteOldUsersAsync();

        /// <summary>
        /// Update user's last activity.
        /// </summary>
        /// <param name="userId">UserId.</param>
        /// <returns>True if user was found and updated. Otherwise false.</returns>
        Task<bool> UpdateLastActivity(Guid userId);

        /// <summary>
        /// Load user with subscriptions.
        /// </summary>
        /// <param name="userId">UserId.</param>
        /// <returns>User with subscriptions preloaded.</returns>
        Task<User> LoadWithSubscriptionsAsync(Guid userId);

        /// <summary>
        /// Load user.
        /// </summary>
        /// <param name="userId">UserId.</param>
        /// <returns>User.</returns>
        Task<User> LoadAsync(Guid userId);

        /// <summary>
        /// Create new user.
        /// </summary>
        /// <param name="user">User to create.</param>
        /// <returns>New user GUID.</returns>
        Task<Guid> EditAsync(User user);
    }
}
