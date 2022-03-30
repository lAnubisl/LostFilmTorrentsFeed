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

namespace LostFilmMonitoring.DAO
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using LostFilmMonitoring.Common;
    using LostFilmMonitoring.DAO.Interfaces;
    using LostFilmMonitoring.DAO.Interfaces.DomainModels;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Provides functionality for managing users in storage.
    /// </summary>
    public class UserDAO : BaseDAO, IUserDAO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDAO"/> class.
        /// </summary>
        /// <param name="configuration">IConfiguration.</param>
        public UserDAO(IConfiguration configuration)
            : base(configuration.ConnectionString)
        {
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public async Task<User> LoadWithSubscriptionsAsync(Guid userId)
        {
            using (var ctx = this.OpenContext())
            {
                return await ctx.Users.Include(u => u.Subscriptions).FirstOrDefaultAsync(u => u.Id == userId);
            }
        }

        /// <inheritdoc/>
        public async Task<User> LoadAsync(Guid userId)
        {
            using (var ctx = this.OpenContext())
            {
                return await ctx.Users.FirstOrDefaultAsync(u => u.Id == userId);
            }
        }

        /// <inheritdoc/>
        public async Task<Guid> EditAsync(User user)
        {
            using (var ctx = this.OpenContext())
            {
                if (user.Id == default)
                {
                    ctx.Add(user);
                }
                else
                {
                    var entity = await ctx.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
                    if (entity == null)
                    {
                        return user.Id;
                    }

                    ctx.Entry(entity).CurrentValues.SetValues(user);
                }

                await ctx.SaveChangesAsync();
                return user.Id;
            }
        }
    }
}
