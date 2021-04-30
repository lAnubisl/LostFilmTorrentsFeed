// <copyright file="BaseRssFeed.cs" company="Alexander Panfilenok">
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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using LostFilmMonitoring.Common;
    using LostFilmTV.Client;
    using LostFilmTV.Client.Exceptions;
    using LostFilmTV.Client.Response;

    /// <summary>
    /// Represents base rss feed.
    /// </summary>
    public abstract class BaseRssFeed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRssFeed"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public BaseRssFeed(ILogger logger)
        {
            this.Logger = logger;
        }

        /// <summary>
        /// Gets Logger.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Downloads content by URL.
        /// </summary>
        /// <param name="rssUri">URL.</param>
        /// <returns>Content.</returns>
        protected async Task<string> DownloadRssText(string rssUri)
        {
            this.Logger.Info($"Call: {nameof(this.DownloadRssText)}({rssUri})");
            using (var client = new HttpClient())
            {
                try
                {
                    var rssText = await client.GetStringAsync(rssUri);
                    this.Logger.Debug(rssText);
                    return rssText;
                }
                catch (TaskCanceledException ex)
                {
                    this.Logger.Log(ex);
                    throw new RemoteServiceUnavailableException();
                }
                catch (IOException ex)
                {
                    this.Logger.Log(ex);
                    throw new RemoteServiceUnavailableException();
                }
                catch (Exception ex)
                {
                    this.Logger.Log(ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Reads Feed item objects from RSS content.
        /// </summary>
        /// <param name="rssText">RSS content.</param>
        /// <returns>Feed item objects.</returns>
        protected SortedSet<FeedItemResponse> GetItems(string rssText)
        {
            this.Logger.Info($"Call: {nameof(this.GetItems)}(rssText)");
            XDocument document;
            try
            {
                document = LostFilmTV.Client.Extensions.Parse(rssText);
            }
            catch (Exception ex)
            {
                this.Logger.Log(ex);
                return new SortedSet<FeedItemResponse>();
            }

            return document.GetItems();
        }
    }
}
