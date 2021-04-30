using LostFilmMonitoring.BLL;
using LostFilmMonitoring.Common;
using LostFilmMonitoring.DAO.DAO;
using System;

namespace LostFilmMonitoring.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigureLogger();
            var logger = new Logger(nameof(Program));
            try
            {
                var feedService = new RssFeedUpdaterService(logger);
                var userDao = new UserDAO(Configuration.GetConnectionString());
                var feedDao = new FeedDAO(Configuration.GetConnectionString());
                var deletedUserIds = userDao.DeleteOldUsersAsync().Result;
                foreach (var userId in deletedUserIds)
                {
                    feedDao.DeleteAsync(userId).Wait();
                }

                feedService.UpdateAsync().Wait();
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

        private static void ConfigureLogger()
        {
            var minLogLevel = "Debug";
            var maxLogLevel = "Fatal";
            LoggerConfiguration.ConfigureLogger(minLogLevel, maxLogLevel);
        }
    }
}
