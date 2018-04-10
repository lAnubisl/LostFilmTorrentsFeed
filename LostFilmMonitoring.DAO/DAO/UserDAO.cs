using LostFilmMonitoring.DAO.DomainModels;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace LostFilmMonitoring.DAO.DAO
{
    public class UserDAO : BaseDAO
    {
        public UserDAO(string connectionString) : base(connectionString)
        {
        }

        public async Task DeleteOldUsersAsync()
        {
            using (var ctx = OpenContext())
            {
                var oldUsers = await ctx.Users.Where(u => u.LastActivity < DateTime.UtcNow.AddMonths(-1)).ToArrayAsync();
                foreach(var user in oldUsers)
                {
                    ctx.Remove(user);
                }

                await ctx.SaveChangesAsync();
            }
        }

        public async Task<bool> UpdateLastActivity(Guid userId)
        {
            using (var ctx = OpenContext())
            {
                var user = await ctx.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null) return false;
                user.LastActivity = DateTime.UtcNow;
                ctx.Update(user);
                await ctx.SaveChangesAsync();
                return true;
            }
        }

        public async Task<User> LoadAsync(Guid userId)
        {
            using (var ctx = OpenContext())
            {
                return await ctx.Users.Include(u => u.Subscriptions).FirstOrDefaultAsync(u => u.Id == userId);
            }
        }

        public async Task<Guid> SaveAsync(User user)
        {
            using (var ctx = OpenContext())
            {
                if (user.Id != Guid.Empty)
                {
                    var existingUser = await ctx.Users.Include(u => u.Subscriptions)
                        .FirstOrDefaultAsync(u => u.Id == user.Id);
                    if (existingUser == null) return Guid.Empty;
                    existingUser.Cookie = user.Cookie;
                    existingUser.LastActivity = DateTime.UtcNow;
                    var subscriptionsToAdd = user.Subscriptions
                        .Where(s => !existingUser.Subscriptions.Any(es => es.Serial == s.Serial));
                    existingUser.Subscriptions.AddRange(subscriptionsToAdd);

                    foreach(var subscriptionToUpdate in new List<Subscription>(existingUser.Subscriptions))
                    {
                        var update = user.Subscriptions
                            .FirstOrDefault(s => subscriptionToUpdate.Serial == s.Serial);
                        if(update == null)
                        {
                            existingUser.Subscriptions.Remove(subscriptionToUpdate);
                        }
                        else
                        {
                            subscriptionToUpdate.Quality = update.Quality;
                        }
                    }

                    ctx.Update(existingUser);
                }
                else
                {
                    user.Id = new Guid();
                    ctx.Add(user);
                    ctx.AddRange(user.Subscriptions);
                }

                await ctx.SaveChangesAsync();
                return user.Id;
            }
        }
    }
}