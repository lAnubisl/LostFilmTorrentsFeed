// <copyright file="FeedController.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.Web.Controllers
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using LostFilmMonitoring.BLL;
    using LostFilmMonitoring.Common;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Feed controller.
    /// </summary>
    public class FeedController : Controller
    {
        private readonly RssFeedService feedService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedController"/> class.
        /// </summary>
        /// <param name="currentUserProvider">currentUserProvider.</param>
        /// <param name="logger">logger.</param>
        public FeedController(ICurrentUserProvider currentUserProvider, ILogger logger)
        {
            this.feedService = new RssFeedService(currentUserProvider, logger);
        }

        /// <summary>
        /// Feed.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [Route("Feed")]
        public async Task<IActionResult> Feed()
        {
            var model = await this.feedService.GetFeedViewModel();
            if (model == null)
            {
                return this.RedirectToAction("index", "Home");
            }

            return this.View(model);
        }

        /// <summary>
        /// Rss.
        /// </summary>
        /// <param name="userId">userId.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [Route("Rss/{userId}")]
        public async Task<IActionResult> Rss(Guid userId)
        {
            var rssData = await this.feedService.GetRss(userId);
            if (rssData == null)
            {
                return new NotFoundResult();
            }

            return this.File(Encoding.UTF8.GetBytes(rssData), "application/rss+xml", userId + ".xml");
        }

        /// <summary>
        /// RssItem.
        /// </summary>
        /// <param name="id">id.</param>
        /// <param name="userId">userId.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [Route("Rss/{userId}/{id}")]
        public async Task<IActionResult> RssItem(int id, Guid userId)
        {
            var result = await this.feedService.GetRssItem(userId, id);
            if (result == null)
            {
                return new NotFoundResult();
            }

            return this.File(result.TorrentFileBody, result.ContentType, result.TorrentFileName);
        }
    }
}
