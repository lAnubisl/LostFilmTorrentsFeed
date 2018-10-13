using LostFilmMonitoring.DAO.DomainModels;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace LostFilmMonitoring.DAO.DAO
{
    public class UserDAO : BaseDAO
    {
        public UserDAO(string connectionString) : base(connectionString)
        {
        }

        public async Task<Guid[]> DeleteOldUsersAsync()
        {
            var result = new Collection<Guid>();
            using (var ctx = OpenContext())
            {
                var oldUsers = await ctx.Users.Where(u => u.LastActivity < DateTime.UtcNow.AddMonths(-1)).ToArrayAsync();
                foreach(var user in oldUsers)
                {
                    result.Add(user.Id);
                    ctx.Remove(user);
                }

                await ctx.SaveChangesAsync();
            }

            return result.ToArray();
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

        public async Task<Guid> CreateAsync(User user)
        {
            using (var ctx = OpenContext())
            {
                user.Id = new Guid();
                ctx.Add(user);
                await ctx.SaveChangesAsync();
                return user.Id;
            }
        }
    }
}