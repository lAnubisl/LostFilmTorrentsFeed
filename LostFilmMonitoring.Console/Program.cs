using LostFilmMonitoring.BLL.Implementations;
using LostFilmMonitoring.DAO.DAO;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace LostFilmMonitoring.Console
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; private set; }
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            try
            {
                var configurationService = new ConfigurationService();
                var feedService = new FeedService(configurationService, null);
                var userDao = new UserDAO(configurationService.GetConnectionString());
                var feedDao = new FeedDAO(configurationService.GetBasePath());
                var deletedUserIds = userDao.DeleteOldUsersAsync().Result;
                foreach (var userId in deletedUserIds)
                {
                    feedDao.Delete(userId);
                }

                feedService.Update().Wait();
                System.Console.WriteLine("Done!");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }
    }
}