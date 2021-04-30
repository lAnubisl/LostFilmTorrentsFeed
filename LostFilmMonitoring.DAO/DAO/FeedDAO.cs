using LostFilmMonitoring.DAO.DomainModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LostFilmMonitoring.DAO.DAO
{
    public class FeedDAO : BaseDAO
    {
        public FeedDAO(string connectionString) : base(connectionString)
        {
        }

        public async Task<string> LoadFeedRawAsync(Guid userId)
        {
            using (var ctx = OpenContext())
            {
                return (await ctx.Feeds.FirstOrDefaultAsync(f => f.Id == userId))?.Data;
            }
        }

        private async Task<SortedSet<FeedItem>> LoadFeedAsync(Guid userId)
        {
            string data = await this.LoadFeedRawAsync(userId);
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }

            var document = XDocument.Parse(data);
            return document.GetItems();
        }

        public async Task DeleteAsync(Guid userId)
        {
            using (var ctx = OpenContext())
            {
                var feed = await ctx.Feeds.FirstOrDefaultAsync(f => f.Id == userId);
                if (feed == null)
                {
                    return;
                }

                ctx.Feeds.Remove(feed);
                ctx.SaveChanges();
            }
        }

        private async Task SaveFeedAsync(Guid userId, FeedItem[] items)
        {
            using (var ctx = OpenContext())
            {
                var entity = await ctx.Feeds.FirstOrDefaultAsync(f => f.Id == userId);
                if (entity == null)
                {
                    entity = new Feed()
                    {
                        Id = userId
                    };
                    ctx.Feeds.Add(entity);
                }

                entity.Data = items.GenerateXml();
                ctx.SaveChanges();
            }
        }

        public Task<SortedSet<FeedItem>> LoadUserFeedAsync(Guid userId)
        {
            return LoadFeedAsync(userId);
        }

        public Task<SortedSet<FeedItem>> LoadBaseFeedAsync()
        {
            return LoadFeedAsync(Guid.Empty);
        }

        public Task SaveUserFeedAsync(Guid userId, FeedItem[] items)
        {
            return SaveFeedAsync(userId, items);
        }

        public Task SaveBaseFeedAsync(FeedItem[] items)
        {
            return SaveFeedAsync(Guid.Empty, items);
        }
    }
}
