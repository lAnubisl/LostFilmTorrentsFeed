// <copyright file="Configuration.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.BLL
{
    using System;
    using System.IO;
    using System.Linq;
    using LostFilmMonitoring.Common;

    /// <inheritdoc/>
    public class Configuration : IConfiguration
    {
        private string[] torrentAnnounceListPatterns;

        /// <inheritdoc/>
        public string BaseFeedCookie { get; private set; }

        /// <inheritdoc/>
        public string ConnectionString { get; private set; }

        /// <inheritdoc/>
        public string ImagesPath { get; private set; }

        /// <inheritdoc/>
        public string TorrentPath { get; private set; }

        /// <inheritdoc/>
        public string BaseUrl { get; private set; }

        /// <inheritdoc/>
        public string BaseLinkUID { get; private set; }

        /// <inheritdoc/>
        public string[] GetTorrentAnnounceList(string link_uid)
        {
            return this.torrentAnnounceListPatterns
                .Select(p => string.Format(p, link_uid ?? this.BaseLinkUID))
                .Select(s => s)
                .ToArray();
        }

        /// <summary>
        /// Initializes the configuration.
        /// </summary>
        /// <param name="contentRootPath">contentRootPath.</param>
        public void Init(string contentRootPath)
        {
            var basePath = Environment.GetEnvironmentVariable("BASEPATH") ?? contentRootPath;
            var baseUrl = Environment.GetEnvironmentVariable("BASEURL") ?? "http://localhost:5000";
            var baseFeedCookie = Environment.GetEnvironmentVariable("BASEFEEDCOOKIE") ?? throw new Exception("Environment variable 'BASEFEEDCOOKIE' is not set.");
            this.BaseLinkUID = Environment.GetEnvironmentVariable("BASELINKUID") ?? throw new Exception("Environment variable 'BASELINKUID' is not set.");
            this.torrentAnnounceListPatterns = Environment.GetEnvironmentVariable("TORRENTTRACKERS")?.Split(',', StringSplitOptions.RemoveEmptyEntries) ??
                new[]
                {
                    "http://bt.tracktor.in/tracker.php/{0}/announce",
                    "http://bt99.tracktor.in/tracker.php/{0}/announce",
                    "http://bt0.tracktor.in/tracker.php/{0}/announce",
                    "http://user5.newtrack.info/tracker.php/{0}/announce",
                    "http://user1.newtrack.info/tracker.php/{0}/announce",
                };

            this.BaseFeedCookie = baseFeedCookie;
            this.ImagesPath = Path.Combine(basePath, "data", "images");
            this.BaseUrl = baseUrl;
            EnsureDirectoryExists(this.ImagesPath);
            this.TorrentPath = Path.Combine(basePath, "data", "torrents");
            EnsureDirectoryExists(this.TorrentPath);
            var dbpath = Path.Combine(basePath, "data", "lostfilmtorrentfeed.db");
            this.ConnectionString = $"Data Source = {dbpath}";
        }

        private static void EnsureDirectoryExists(string path)
        {
            var directory = new DirectoryInfo(path);
            if (!directory.Exists)
            {
                directory.Create();
            }
        }
    }
}
