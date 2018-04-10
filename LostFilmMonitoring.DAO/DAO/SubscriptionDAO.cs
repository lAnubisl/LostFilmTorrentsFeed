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
    }
}