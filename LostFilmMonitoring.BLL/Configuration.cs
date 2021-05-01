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

    /// <summary>
    /// Configuration.
    /// </summary>
    public static class Configuration
    {
        private static string connectionString;
        private static string imagesPath;
        private static string torrentsPath;
        private static string baseFeedCookieValue;
        private static string baseUrlValue;

        /// <summary>
        /// Initializes configuration.
        /// </summary>
        /// <param name="basePath">Base path.</param>
        /// <param name="baseUrl">Base Url.</param>
        /// <param name="baseFeedCookie">Base feed Cookie.</param>
        public static void Init(string basePath, string baseUrl, string baseFeedCookie)
        {
            baseFeedCookieValue = baseFeedCookie;
            imagesPath = Path.Combine(basePath, "data", "images");
            baseUrlValue = baseUrl;
            EnsureDirectoryExists(imagesPath);
            torrentsPath = Path.Combine(basePath, "data", "torrents");
            EnsureDirectoryExists(torrentsPath);
            var dbpath = Path.Combine(basePath, "data", "lostfilmtorrentfeed.db");
            connectionString = $"Data Source = {dbpath}";
        }

        /// <summary>
        /// Get BaseFeedCookie.
        /// </summary>
        /// <returns>BaseFeedCookie.</returns>
        public static string BaseFeedCookie() => baseFeedCookieValue;

        /// <summary>
        /// Get database connection string.
        /// </summary>
        /// <returns>Database connection string.</returns>
        public static string GetConnectionString() => connectionString;

        /// <summary>
        /// Get physical path where series covers are stored.
        /// </summary>
        /// <returns>Physical path where series covers are stored.</returns>
        public static string GetImagesPath() => imagesPath;

        /// <summary>
        /// Get physical path where torrent files are stored.
        /// </summary>
        /// <returns>Physical path where torrent files are stored.</returns>
        public static string GetTorrentPath() => torrentsPath;

        /// <summary>
        /// Get base url where website is hosted.
        /// </summary>
        /// <returns>Base url where website is hosted.</returns>
        public static string GetBaseUrl() => baseUrlValue;

        private static string ReadEnvironmentVariable(string key)
        {
            var value = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException($"Environment variable '{key}' is missing.");
            }

            Environment.SetEnvironmentVariable(key, null);
            return value;
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
