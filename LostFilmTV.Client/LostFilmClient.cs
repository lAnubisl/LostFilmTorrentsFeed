// <copyright file="LostFilmClient.cs" company="Alexander Panfilenok">
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

namespace LostFilmTV.Client
{
    /// <summary>
    /// Client for LostFilm.TV
    /// This class is responsible for all interactions with lostfilm.tv website.
    /// </summary>
    public class LostFilmClient : ILostFilmClient
    {
        private readonly ILogger logger;
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="LostFilmClient"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="httpClientFactory">IHttpClientFactory.</param>
        public LostFilmClient(ILogger logger, IHttpClientFactory httpClientFactory)
        {
            this.logger = logger?.CreateScope(nameof(LostFilmClient)) ?? throw new ArgumentNullException(nameof(logger));
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        /// <inheritdoc/>
        public async Task<Stream?> DownloadImageAsync(string lostFilmId)
        {
            using var client = this.httpClientFactory.CreateClient();
            try
            {
                var stream = await client.GetStreamAsync($"https://static.lostfilm.top/Images/{lostFilmId}/Posters/shmoster_s1.jpg");
                return stream;
            }
            catch (Exception ex)
            {
                this.logger.Log(ex);
                return null;
            }
        }

        /// <summary>
        /// Get torrent file for user.
        /// </summary>
        /// <param name="uid">User Id.</param>
        /// <param name="usess">User ss key.</param>
        /// <param name="torrentFileId">Torrent file Id.</param>
        /// <returns>TorrentFile object which contain file name and content stream.</returns>
        public async Task<TorrentFileResponse?> DownloadTorrentFileAsync(string uid, string usess, string torrentFileId)
        {
            var client = this.httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://n.tracktor.site/rssdownloader.php?id={torrentFileId}");
            request.Headers.Add("Cookie", $"uid={uid};usess={usess};");
            HttpResponseMessage response;

            try
            {
                response = await client.SendAsync(request);
            }
            catch (Exception ex)
            {
                this.logger.Log(ex);
                return null;
            }

            if (response?.Content?.Headers?.ContentType == null)
            {
                this.logger.Error("response?.Content?.Headers?.ContentType == null");
                return null;
            }

            if (response.Content.Headers.ContentType.MediaType != "application/x-bittorrent")
            {
                string? responseBody = null;
                if (response.Content.Headers.ContentType.MediaType == "text/html")
                {
                    responseBody = await response.Content.ReadAsStringAsync();
                }

                this.logger.Error($"contentType is not 'application/x-bittorrent' it is '{response.Content.Headers.ContentType.MediaType}'. Response content is: '{responseBody}'. TorrentFileId is: '{torrentFileId}'.");
                return null;
            }

            response.Content.Headers.TryGetValues("Content-Disposition", out IEnumerable<string>? cd);
            var fileName = cd?.FirstOrDefault()?[("attachment;filename=\"".Length + 1)..];
            if (fileName == null)
            {
                this.logger.Error("Something wrong with 'Content-Disposition' header of the response.");
                return null;
            }

            fileName = fileName[0..^1];
            var stream = await response.Content.ReadAsStreamAsync();
            return new TorrentFileResponse(fileName, stream);
        }
    }
}
