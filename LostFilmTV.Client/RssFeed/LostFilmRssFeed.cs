// <copyright file="LostFilmRssFeed.cs" company="Alexander Panfilenok">
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
    /// <summary>
    /// LostFilmRssFeed.
    /// </summary>
    public class LostFilmRssFeed : BaseRssFeed, IRssFeed
    {
        private const string RssUrl = "https://www.lostfilm.tv/rss.xml";

        /// <summary>
        /// Initializes a new instance of the <see cref="LostFilmRssFeed"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="httpClientFactory">httpClientFactory.</param>
        /// <param name="configuration">configuration.</param>
        public LostFilmRssFeed(ILogger logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(logger.CreateScope(nameof(ReteOrgRssFeed)), httpClientFactory)
        {
        }

        /// <summary>
        /// Reads Feed items.
        /// </summary>
        /// <returns>Feed items.</returns>
        public async Task<SortedSet<FeedItemResponse>> LoadFeedItemsAsync()
        {
            var requestHeaders = new Dictionary<string, string>
            {
                { "user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36" },
            };

            string rssText;
            try
            {
                rssText = await this.DownloadRssTextAsync(RssUrl, requestHeaders);
            }
            catch (RemoteServiceUnavailableException)
            {
                return new SortedSet<FeedItemResponse>();
            }

            rssText = FixAmpBug(rssText);
            return this.GetItems(rssText);
        }

        private static string FixAmpBug(string rss)
        {
            var ampIndex = rss.IndexOf('&');
            if (ampIndex == -1)
            {
                return rss;
            }

            var escapedAmpIndex = rss.IndexOf("&amp;");
            if (ampIndex == escapedAmpIndex)
            {
                return rss[.. (ampIndex + 1)] + FixAmpBug(rss[(ampIndex + 1) ..]);
            }

            rss = rss.Insert(ampIndex + 1, "amp;");
            return FixAmpBug(rss);
        }
    }
}
