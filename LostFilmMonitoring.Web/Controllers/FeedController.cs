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

using System;
using System.Text;
using System.Threading.Tasks;
using LostFilmMonitoring.BLL;
using LostFilmMonitoring.Common;
using Microsoft.AspNetCore.Mvc;

namespace LostFilmMonitoring.Web.Controllers
{
    public class FeedController : Controller
    {
        private readonly RssFeedService feedService;

        public FeedController(ICurrentUserProvider currentUserProvider, ILogger logger)
        {
            this.feedService = new RssFeedService(currentUserProvider, logger);
        }

        [HttpGet, Route("Feed")]
        public async Task<IActionResult> Feed()
        {
            var model = await feedService.GetFeedViewModel();
            if (model == null) return RedirectToAction("index");
            return View(model);
        }

        [HttpGet, Route("Rss/{userId}")]
        public async Task<IActionResult> Rss(Guid userId)
        {
            var rssData = await feedService.GetRss(userId);
            if (rssData == null) return new NotFoundResult();
            return File(Encoding.UTF8.GetBytes(rssData), "application/rss+xml", userId + ".xml");
        }

        [HttpGet, Route("Rss/{userId}/{id}")]
        public async Task<IActionResult> RssItem(int id, Guid userId)
        {
            var result = await feedService.GetRssItem(userId, id);
            if (result == null) return new NotFoundResult();
            return File(result.TorrentFileBody, result.ContentType, result.TorrentFileName);
        }
    }
}
