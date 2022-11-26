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
    /// <summary>
    /// Represents base rss feed.
    /// </summary>
    public abstract class BaseRssFeed
    {
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRssFeed"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="httpClientFactory">IHttpClientFactory.</param>
        protected BaseRssFeed(ILogger logger, IHttpClientFactory httpClientFactory)
        {
            this.Logger = logger;
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        /// <summary>
        /// Gets Logger.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Downloads content by URL.
        /// </summary>
        /// <param name="rssUri">URL.</param>
        /// <param name="requestHeaders">Additional headers to add for the request to external service.</param>
        /// <returns>Content.</returns>
        protected async Task<string> DownloadRssTextAsync(string rssUri, Dictionary<string, string>? requestHeaders = null)
        {
            this.Logger.Info($"Call: {nameof(this.DownloadRssTextAsync)}({rssUri})");
            using var client = this.httpClientFactory.CreateClient();
            var message = new HttpRequestMessage(HttpMethod.Get, rssUri);
            if (requestHeaders != null)
            {
                foreach (var header in requestHeaders)
                {
                    message.Headers.Add(header.Key, header.Value);
                }
            }

            try
            {
                var response = await client.SendAsync(message);
                var rssText = await response.Content.ReadAsStringAsync();
                this.Logger.Debug(rssText);
                return rssText;
            }
            catch (TaskCanceledException ex)
            {
                this.Logger.Log(ex);
                throw new RemoteServiceUnavailableException(ex);
            }
            catch (IOException ex)
            {
                this.Logger.Log(ex);
                throw new RemoteServiceUnavailableException(ex);
            }
            catch (Exception ex)
            {
                this.Logger.Log(ex);
                throw new RemoteServiceUnavailableException(ex);
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
            if (string.IsNullOrWhiteSpace(rssText))
            {
                this.Logger.Error("RSS content is empty.");
                return new SortedSet<FeedItemResponse>();
            }

            XDocument document;
            try
            {
                document = Parse(rssText);
            }
            catch (Exception ex)
            {
                this.Logger.Log($"Error parsing RSS data: {Environment.NewLine}'{rssText}'", ex);
                return new SortedSet<FeedItemResponse>();
            }

            return GetItems(document);
        }

        /// <summary>
        /// Generages XDocument from input string.
        /// </summary>
        /// <param name="rssString">Input string.</param>
        /// <returns>XDocument.</returns>
        private static XDocument Parse(string rssString)
        {
            string pattern = "(?<start>>)(?<content>.+?(?<!>))(?<end><)|(?<start>\")(?<content>.+?)(?<end>\")";
            string result = Regex.Replace(rssString, pattern, m =>
                        m.Groups["start"].Value +
                        HttpUtility.HtmlEncode(HttpUtility.HtmlDecode(m.Groups["content"].Value)) +
                        m.Groups["end"].Value);
            try
            {
                return XDocument.Parse(result);
            }
            catch
            {
                return XDocument.Parse(rssString);
            }
        }

        /// <summary>
        /// Read feed items from XDocument.
        /// </summary>
        /// <param name="doc">XDocument.</param>
        /// <returns>Set of FeedItemResponse.</returns>
        private static SortedSet<FeedItemResponse> GetItems(XDocument doc)
        {
            var entries = from item in doc.Root?.Descendants()
                          .First(i => i.Name.LocalName == "channel")
                          .Elements()
                          .Where(i => i.Name.LocalName == "item")
                          select new FeedItemResponse(item);
            return new SortedSet<FeedItemResponse>(entries);
        }
    }
}
