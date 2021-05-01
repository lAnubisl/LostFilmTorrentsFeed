// <copyright file="UserDAO.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.DAO
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using LostFilmMonitoring.DAO.DomainModels;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Provides functionality for managing users in storage.
    /// </summary>
    public class UserDAO : BaseDAO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDAO"/> class.
        /// </summary>
        /// <param name="connectionString">Database connection string.</param>
        public UserDAO(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>
        /// Delete users which were not be active for 1 month.
        /// </summary>
        /// <returns>User Ids which were deleted.</returns>
        public async Task<Guid[]> DeleteOldUsersAsync()
        {
            var deleted = new Collection<Guid>();
            using (var ctx = this.OpenContext())
            {
                var oldUsers = await ctx.Users.Where(u => u.LastActivity < DateTime.UtcNow.AddMonths(-1)).ToArrayAsync();
                foreach (var user in oldUsers)
                {
                    deleted.Add(user.Id);
                    ctx.Remove(user);
                }

                await ctx.SaveChangesAsync();
            }

            return deleted.ToArray();
        }

        /// <summary>
        /// Update user's last activity.
        /// </summary>
        /// <param name="userId">UserId.</param>
        /// <returns>True if user was found and updated. Otherwise false.</returns>
        public async Task<bool> UpdateLastActivity(Guid userId)
        {
            using (var ctx = this.OpenContext())
            {
                var user = await ctx.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return false;
                }

                user.LastActivity = DateTime.UtcNow;
                ctx.Update(user);
                await ctx.SaveChangesAsync();
                return true;
            }
        }

        /// <summary>
        /// Load user with subscriptions.
        /// </summary>
        /// <param name="userId">UserId.</param>
        /// <returns>User with subscriptions preloaded.</returns>
        public async Task<User> LoadWithSubscriptionsAsync(Guid userId)
        {
            using (var ctx = this.OpenContext())
            {
                return await ctx.Users.Include(u => u.Subscriptions).FirstOrDefaultAsync(u => u.Id == userId);
            }
        }

        /// <summary>
        /// Load user.
        /// </summary>
        /// <param name="userId">UserId.</param>
        /// <returns>User.</returns>
        public async Task<User> LoadAsync(Guid userId)
        {
            using (var ctx = this.OpenContext())
            {
                return await ctx.Users.FirstOrDefaultAsync(u => u.Id == userId);
            }
        }

        /// <summary>
        /// Create new user.
        /// </summary>
        /// <param name="user">User to create.</param>
        /// <returns>New user GUID.</returns>
        public async Task<Guid> CreateAsync(User user)
        {
            using (var ctx = this.OpenContext())
            {
                user.Id = default;
                ctx.Add(user);
                await ctx.SaveChangesAsync();
                return user.Id;
            }
        }
    }
}
