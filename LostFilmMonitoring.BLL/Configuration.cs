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
    /// <inheritdoc/>
    public class Configuration : IConfiguration
    {
        private readonly string[] torrentAnnounceListPatterns;

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        public Configuration()
        {
            this.BaseUrl = Environment.GetEnvironmentVariable("BASEURL") ?? "https://lostfilmfeedstorage.blob.core.windows.net/usertorrents";
            this.BaseUSESS = Environment.GetEnvironmentVariable("BASEFEEDCOOKIE") ?? "8c460d627c46325ae1ad3b7e82ddc468";
            this.BaseUID = Environment.GetEnvironmentVariable("BASELINKUID") ?? "1874597";
            this.SqlServerConnectionString = Environment.GetEnvironmentVariable("SqlServerConnectionString") ?? string.Empty;
            this.torrentAnnounceListPatterns = Environment.GetEnvironmentVariable("TORRENTTRACKERS")?.Split(',', StringSplitOptions.RemoveEmptyEntries) ??
                new[]
                {
                    "http://bt.tracktor.in/tracker.php/{0}/announce",
                    "http://bt99.tracktor.in/tracker.php/{0}/announce",
                    "http://bt0.tracktor.in/tracker.php/{0}/announce",
                    "http://user5.newtrack.info/tracker.php/{0}/announce",
                    "http://user1.newtrack.info/tracker.php/{0}/announce",
                };

            this.ImagesDirectory = Environment.GetEnvironmentVariable("IMAGESDIRECTORY") ?? "images";
            this.TorrentsDirectory = Environment.GetEnvironmentVariable("TORRENTSDIRECTORY") ?? "torrentfiles";
        }

        /// <inheritdoc/>
        public string BaseUSESS { get; private set; }

        /// <inheritdoc/>
        public string ImagesDirectory { get; private set; }

        /// <inheritdoc/>
        public string TorrentsDirectory { get; private set; }

        /// <inheritdoc/>
        public string BaseUrl { get; private set; }

        /// <inheritdoc/>
        public string BaseUID { get; private set; }

        /// <inheritdoc/>
        public string SqlServerConnectionString { get; private set; }

        /// <inheritdoc/>
        public string[] GetTorrentAnnounceList(string link_uid)
        {
            return this.torrentAnnounceListPatterns
                .Select(p => string.Format(p, link_uid ?? this.BaseUID))
                .Select(s => s)
                .ToArray();
        }
    }
}
