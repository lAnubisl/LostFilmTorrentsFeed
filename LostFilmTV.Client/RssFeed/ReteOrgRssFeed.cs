// <copyright file="ReteOrgRssFeed.cs" company="Alexander Panfilenok">
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

namespace LostFilmTV.Client.RssFeed
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using LostFilmMonitoring.Common;
    using LostFilmTV.Client;
    using LostFilmTV.Client.Exceptions;
    using LostFilmTV.Client.Response;

    /// <summary>
    /// ReteOrgRssFeedService.
    /// </summary>
    public class ReteOrgRssFeed : BaseRssFeed, IRssFeed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReteOrgRssFeed"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="httpClientFactory">httpClientFactory.</param>
        public ReteOrgRssFeed(ILogger logger, IHttpClientFactory httpClientFactory)
            : base(logger.CreateScope(nameof(ReteOrgRssFeed)), httpClientFactory)
        {
        }

        /// <summary>
        /// Get FeedItemResponse from n.retre.org.
        /// </summary>
        /// <returns>Set of FeedItemResponse.</returns>
        public async Task<SortedSet<FeedItemResponse>> LoadFeedItemsAsync()
        {
            this.Logger.Info($"Call: {nameof(this.LoadFeedItemsAsync)}()");
            string rss;
            try
            {
                rss = await this.DownloadRssTextAsync("http://insearch.site/rssdd.xml");
            }
            catch (RemoteServiceUnavailableException)
            {
                return new SortedSet<FeedItemResponse>();
            }

            return this.GetItems(rss);
        }
    }
}
