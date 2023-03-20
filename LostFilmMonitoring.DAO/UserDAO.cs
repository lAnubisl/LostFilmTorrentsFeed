// <copyright file="UserDAO.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2023 Alexander Panfilenok
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

namespace LostFilmMonitoring.DAO.Sql
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using LostFilmMonitoring.Common;
    using LostFilmMonitoring.DAO.Interfaces;
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
            : base(configuration.SqlServerConnectionString)
        {
        }

        public async Task<Interfaces.DomainModels.User[]> LoadAsync()
        {
            using var ctx = this.OpenContext();
            return (await ctx.Users.ToArrayAsync()).Select(Mapper.Map).ToArray();
        }

        /// <inheritdoc/>
        public async Task<Interfaces.DomainModels.User> LoadAsync(string userId)
        {
            var id = Guid.Parse(userId);
            using (var ctx = this.OpenContext())
            {
                return Mapper.Map(await ctx.Users.FirstOrDefaultAsync(u => u.Id == id));
            }
        }

        /// <inheritdoc/>
        public async Task SaveAsync(Interfaces.DomainModels.User user)
        {
            var id = Guid.Parse(user.Id);
            using (var ctx = this.OpenContext())
            {
                var entity = await ctx.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    ctx.Add(user);
                }
                else
                {
                    ctx.Entry(entity).CurrentValues.SetValues(user);
                }

                await ctx.SaveChangesAsync();
            }
        }
    }
}
