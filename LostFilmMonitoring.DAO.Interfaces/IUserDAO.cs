// <copyright file="IUserDAO.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.Interfaces
{
    using LostFilmMonitoring.DAO.Interfaces.DomainModels;

    /// <summary>
    /// Provides functionality for managing users in storage.
    /// </summary>
    public interface IUserDAO
    {
        /// <summary>
        /// Load user.
        /// </summary>
        /// <param name="userId">UserId.</param>
        /// <returns>User.</returns>
        Task<User?> LoadAsync(string userId);

        /// <summary>
        /// Create new user.
        /// </summary>
        /// <param name="user">User to create.</param>
        /// <returns>New user GUID.</returns>
        Task SaveAsync(User user);

        /// <summary>
        /// Load users.
        /// </summary>
        /// <returns>All users.</returns>
        Task<User[]> LoadAsync();
    }
}
