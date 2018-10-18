using LostFilmMonitoring.DAO.DomainModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LostFilmMonitoring.DAO.DAO
{
    public class FeedDAO
    {
        private readonly string _basePath;
        private const string baseFeed = "base_feed";

        public FeedDAO(string basePath)
        {
            _basePath = basePath;
        }

        private string GetPath(string fileName)
        {
            return Path.Combine(_basePath, fileName + ".xml");
        }

        public Stream LoadFeedRawAsync(Guid userId)
        {
            var feedPath = GetPath(userId.ToString());
            if (!File.Exists(feedPath)) return null;
            return new FileStream(feedPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize: 4096, useAsync: true);
        }

        private async Task<SortedSet<FeedItem>> LoadFeedAsync(string fileName)
        {
            var feedPath = GetPath(fileName);
            if (!File.Exists(feedPath)) return new SortedSet<FeedItem>();
            using (var reader = File.OpenText(feedPath))
            {
                var xml = await reader.ReadToEndAsync();
                var document = XDocument.Parse(xml);
                return document.GetItems();
            }
        }

        public void Delete(Guid userId)
        {
            File.Delete(GetPath(userId.ToString()));
        }

        private async Task SaveFeedAsync(string fileName, FeedItem[] items)
        {
            var xml = items.GenerateXml();
            byte[] bytes = Encoding.UTF8.GetBytes(xml);
            using (var fs = new FileStream(
                GetPath(fileName),
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                useAsync: true))
            {
                await fs.WriteAsync(bytes, 0, bytes.Length);
            }
        }

        public Task<SortedSet<FeedItem>> LoadUserFeedAsync(Guid userId)
        {
            return LoadFeedAsync(userId.ToString());
        }

        public Task<SortedSet<FeedItem>> LoadBaseFeedAsync()
        {
            return LoadFeedAsync(baseFeed);
        }

        public Task SaveUserFeedAsync(Guid userId, FeedItem[] items)
        {
            return SaveFeedAsync(userId.ToString(), items);
        }

        public Task SaveBaseFeedAsync(FeedItem[] items)
        {
            return SaveFeedAsync(baseFeed, items);
        }
    }
}