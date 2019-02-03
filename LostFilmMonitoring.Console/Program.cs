using LostFilmMonitoring.BLL.Implementations;
using LostFilmMonitoring.Common;
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
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Production.json", true);
            Configuration = builder.Build();
            LoggerConfiguration.ConfigureLogger("LostFilmFeed.Console", Configuration.GetConnectionString("log"));
            var logger = new Logger(nameof(Program));
            try
            {
                var configurationService = new ConfigurationService();
                var feedService = new FeedService(configurationService, null, logger);
                var userDao = new UserDAO(configurationService.GetConnectionString());
                var feedDao = new FeedDAO(configurationService.GetBasePath(), logger);
                var deletedUserIds = userDao.DeleteOldUsersAsync().Result;
                foreach (var userId in deletedUserIds)
                {
                    feedDao.Delete(userId);
                }

                feedService.Update().Wait();
            }
            catch (Exception ex)
            {
                logger.Log(ex);
                if (ex.InnerException != null)
                {
                    logger.Log(ex.InnerException);
                }
            }
        }
    }
}