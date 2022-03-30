// <copyright file="UpdateFeedsJobRunner.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2021 Alexander Panfilenok
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

namespace LostFilmMonitoring.Updater
{
    using System;
    using System.Threading.Tasks;
    using LostFilmMonitoring.BLL;
    using LostFilmMonitoring.Common;
    using LostFilmMonitoring.DAO.Interfaces;

    /// <summary>
    /// UpdateFeedsJobRunner.
    /// </summary>
    public static class UpdateFeedsJobRunner
    {
        /// <summary>
        /// Schedules recurring synchronization with LostFilm.tv.
        /// </summary>
        /// <param name="serviceProvider">IServiceProvider.</param>
        public static void Schedule(IServiceProvider serviceProvider)
        {
            Task.Factory.StartNew(async () =>
            {
                var logger = (ILogger)serviceProvider.GetService(typeof(ILogger));
                logger.CreateScope(nameof(UpdateFeedsJobRunner));
                var feedService = (RssFeedUpdaterService)serviceProvider.GetService(typeof(RssFeedUpdaterService));
                var userDao = (IUserDAO)serviceProvider.GetService(typeof(IUserDAO));
                var feedDao = (IFeedDAO)serviceProvider.GetService(typeof(IFeedDAO));
                while (true)
                {
                    await RunUpdateProcess(userDao, feedDao, feedService, logger);
                    await Task.Delay(TimeSpan.FromMinutes(10));
                }
            });
        }

        public static async Task RunUpdateProcess(IUserDAO userDao, IFeedDAO feedDao, RssFeedUpdaterService feedService, ILogger logger)
        {
            try
            {
                var deletedUserIds = await userDao.DeleteOldUsersAsync();
                foreach (var userId in deletedUserIds)
                {
                    await feedDao.DeleteAsync(userId);
                }

                await feedService.UpdateAsync();
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
