// <copyright file="Client.cs" company="Alexander Panfilenok">
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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using LostFilmMonitoring.Common;
    using LostFilmTV.Client.Response;
    using Newtonsoft.Json;

    /// <summary>
    /// Client for LostFilm.TV
    /// This class is responsible for all interactions with lostfilm.tv website.
    /// </summary>
    public class Client
    {
        private const string BaseUrl = "https://www.lostfilm.tv";
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public Client(ILogger logger)
        {
            this.logger = logger.CreateScope(nameof(Client));
        }

        /// <summary>
        /// Returns captcha for new user registration.
        /// </summary>
        /// <returns>Captcha object which contains captcha cookie and image.</returns>
        public async Task<CaptchaResponse> GetRegistrationCaptcha()
        {
            using (var client = new HttpClient())
            {
                var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/simple_captcha.php"));
                return await CaptchaResponse.Build(response);
            }
        }

        /// <summary>
        /// Creates new account on lostfilm.tv website.
        /// </summary>
        /// <param name="captchaCookie">Captcha cookie you got from <see cref="GetRegistrationCaptcha"/>.</param>
        /// <param name="captcha">Captcha text from the image you got from <see cref="GetRegistrationCaptcha"/>.</param>
        /// <returns>Registration object which contains all information about new user.</returns>
        public async Task<RegistrationResponse> RegisterNewAccount(string captchaCookie, string captcha)
        {
            var guid = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 16);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/ajaxik.php");
            request.Headers.Add("Cookie", $"PHPSESSID={captchaCookie}");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                    { "act", "users" },
                    { "type", "signup" },
                    { "login", guid },
                    { "mail", $"{guid}%40gmail.com" },
                    { "pass", guid },
                    { "pass_check", guid },
                    { "rem", "1" },
                    { "captcha", captcha },
            });

            using (var client = new HttpClient())
            {
                var response = await client.SendAsync(request);
                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<RegistrationResponse>(responseJson);
                var cookie = response.Headers
                    .Where(h => h.Key == "Set-Cookie")
                    .Select(h => h.Value)
                    .FirstOrDefault()
                    ?.Last();
                if (cookie != null)
                {
                    cookie = cookie[(cookie.IndexOf("=") + 1) ..];
                    result.Cookie = cookie.Substring(0, cookie.IndexOf(";"));
                }

                return result;
            }
        }

        /// <summary>
        /// Get's the episode id by episode link.
        /// </summary>
        /// <param name="episodeLink">Link to an episode.</param>
        /// <param name="cookie_lf_session">User's cookie.</param>
        /// <returns>Episode Id. Or Null.</returns>
        public async Task<string> GetEpisodeId(string episodeLink, string cookie_lf_session)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, episodeLink);
            request.Headers.Add("Cookie", $"lf_session={cookie_lf_session}");
            var responseString = await Execute(request);
            var episodeIdMatch = Regex.Match(responseString, "PlayEpisode\\('(\\d+)'\\)");
            if (!episodeIdMatch.Success)
            {
                this.logger.Error($"Cannot find EpisodeId from content:{responseString}");
                return null;
            }

            return episodeIdMatch.Groups[1].Value;
        }

        /// <summary>
        /// GetEpisodeLink.
        /// </summary>
        /// <param name="episodeLink">Link to an episode.</param>
        /// <param name="cookie_lf_session">User's cookie.</param>
        /// <returns>Link.</returns>
        public async Task<string> GetEpisodeLink(string episodeLink, string cookie_lf_session)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, episodeLink);
            request.Headers.Add("Cookie", $"lf_session={cookie_lf_session}");
            var responseString = await Execute(request);
            var linkMatch = Regex.Match(responseString, "url=([^\"]+)");
            if (!linkMatch.Success)
            {
                this.logger.Error($"Cannot find EpisodeLink from content:{responseString}");
                return null;
            }

            return linkMatch.Groups[1].Value;
        }

        /// <summary>
        /// Get user identity.
        /// </summary>
        /// <param name="link">Link.</param>
        /// <returns>User identity.</returns>
        public async Task<string> GetUserIdentity(string link)
        {
            using (var client = new HttpClient())
            {
                var usessResponse = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, link));
                var usessResponseContent = await usessResponse.Content.ReadAsStringAsync();
                var usessMatch = Regex.Match(usessResponseContent, "this.innerHTML = '([^']+)'");
                if (!usessMatch.Success)
                {
                    this.logger.Error($"Cannot get usess from: {Environment.NewLine}{usessResponseContent}");
                    return null;
                }

                return usessMatch.Groups[1].Value;
            }
        }

        /// <summary>
        /// Get series cover image by series name.
        /// </summary>
        /// <param name="seriesName">Series name.</param>
        /// <returns>Cover image stream.</returns>
        public async Task<Stream> DownloadSeriesCoverAsync(string seriesName)
        {
            var seriesSearchPageUri = $"{BaseUrl}/search/?q={Uri.EscapeUriString(Filtered(seriesName))}";
            var seriesSearchPage = await Execute(new HttpRequestMessage(HttpMethod.Get, seriesSearchPageUri));
            var match = Regex.Match(seriesSearchPage, "<a href=\"/series/([^\"]+)\"");
            if (!match.Success)
            {
                this.logger.Error($"Cannot find information on page '{seriesSearchPageUri}'. The response is: '{seriesSearchPage}'");
                return null;
            }

            var seasonPageUri = $"{BaseUrl}/series/{match.Groups[1].Value}/season_1";
            var seasonPageContent = await Execute(new HttpRequestMessage(HttpMethod.Get, seasonPageUri));
            match = Regex.Match(seasonPageContent, "//static.lostfilm.tv/Images/[\\d]+/Posters/t_shmoster_s[\\d]+.jpg");
            if (match == Match.Empty)
            {
                this.logger.Error($"Cannot find information on page '{seasonPageUri}'. The response is: '{seasonPageContent}'");
                return null;
            }

            using (var httpClient = new HttpClient())
            {
                var imageRequest = new HttpRequestMessage(HttpMethod.Get, "https:" + match.Value);
                var imageResponse = await httpClient.SendAsync(imageRequest);
                return await imageResponse.Content.ReadAsStreamAsync();
            }
        }

        /// <summary>
        /// Get torrent file for user.
        /// </summary>
        /// <param name="uid">User Id.</param>
        /// <param name="usess">User ss key.</param>
        /// <param name="torrentFileId">Torrent file Id.</param>
        /// <returns>TorrentFile object which contain file name and content stream.</returns>
        public async Task<TorrentFileResponse> DownloadTorrentFile(string uid, string usess, int torrentFileId)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"http://n.tracktor.site/rssdownloader.php?id={torrentFileId}");
                request.Headers.Add("Cookie", $"uid={uid};usess={usess}");
                HttpResponseMessage response = null;

                try
                {
                    response = await client.SendAsync(request);
                }
                catch (Exception ex)
                {
                    this.logger.Log(ex);
                    return null;
                }

                if (response.Content.Headers.ContentType.MediaType != "application/x-bittorrent")
                {
                    string responseBody = null;
                    if (response.Content.Headers.ContentType.MediaType == "text/html")
                    {
                        responseBody = await response.Content.ReadAsStringAsync();
                    }

                    this.logger.Error($"contentType is not 'application/x-bittorrent' it is '{response.Content.Headers.ContentType.MediaType}'. Response content is: '{responseBody}'. TorrentFileId is: '{torrentFileId}'.");
                    return null;
                }

                response.Content.Headers.TryGetValues("Content-Disposition", out IEnumerable<string> cd);
                var fileName = cd?.FirstOrDefault()?[("attachment;filename=\"".Length + 1) ..];
                if (fileName == null)
                {
                    this.logger.Error($"Something wrong with 'Content-Disposition' header of the response.");
                    return null;
                }

                fileName = fileName[0..^1];
                var stream = await response.Content.ReadAsStreamAsync();
                return new TorrentFileResponse(fileName, stream);
            }
        }

        private static async Task<string> Execute(HttpRequestMessage httpRequestMessage)
        {
            using (var client = new HttpClient())
            {
                var response = await client.SendAsync(httpRequestMessage);
                var responseBytes = await response.Content.ReadAsByteArrayAsync();
                return Encoding.UTF8.GetString(responseBytes);
            }
        }

        private static string Filtered(string series)
        {
            var index = series.IndexOf('(');
            if (index > 0)
            {
                return series.Substring(0, index).Trim();
            }

            return series;
        }
    }
}
