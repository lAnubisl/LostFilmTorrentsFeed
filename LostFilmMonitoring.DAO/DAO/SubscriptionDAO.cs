using LostFilmMonitoring.DAO.DomainModels;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace LostFilmMonitoring.DAO.DAO
{
    public class SubscriptionDAO : BaseDAO
    {
        public SubscriptionDAO(string connectionString) : base(connectionString)
        {
        }

        public async Task<Subscription[]> LoadAsync(string serial)
        {
            using (var ctx = OpenContext())
            {
                return await ctx.Subscriptions.Include(s => s.User).Where(s => s.Serial == serial).ToArrayAsync();
            }
        }

        public async Task SaveAsync(Guid userId, Subscription[] subscriptions)
        {
            if (subscriptions == null) subscriptions = new Subscription[0];
            using (var ctx = OpenContext())
            {
                var existingSubscriptions = ctx.Subscriptions.Where(s => s.UserId == userId).ToList();
                ctx.RemoveRange(existingSubscriptions.Where(e => subscriptions.All(s => s.Serial != e.Serial)));
                foreach(var subscription in subscriptions)
                {
                    subscription.UserId = userId;
                    var existingSubscription = existingSubscriptions.FirstOrDefault(s => s.Serial == subscription.Serial);
                    if (existingSubscription == null)
                    {
                        ctx.Add(subscription);
                    }
                    else
                    {
                        existingSubscription.Quality = subscription.Quality;
                    }
                }

                await ctx.SaveChangesAsync();
            }
        }
    }
}