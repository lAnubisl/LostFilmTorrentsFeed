﻿// <copyright file="IConfiguration.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2023 Alexander Panfilenok
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

namespace LostFilmMonitoring.Common
{
    /// <summary>
    /// Configuration.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Gets Base torrent tracker user identifier.
        /// </summary>
        /// <returns>Base torrent tracker user identifier.</returns>
        string BaseUID { get; }

        /// <summary>
        /// Gets BaseFeedCookie.
        /// </summary>
        /// <returns>BaseFeedCookie.</returns>
        string BaseUSESS { get; }

        /// <summary>
        /// Gets physical path where series covers are stored.
        /// </summary>
        /// <returns>Physical path where series covers are stored.</returns>
        string ImagesDirectory { get; }

        /// <summary>
        /// Gets physical path where torrent files are stored.
        /// </summary>
        /// <returns>Physical path where torrent files are stored.</returns>
        string TorrentsDirectory { get; }

        /// <summary>
        /// Gets base url where website is hosted.
        /// </summary>
        /// <returns>Base url where website is hosted.</returns>
        string BaseUrl { get; }

        /// <summary>
        /// Get list of torrent trackers for torrent file.
        /// </summary>
        /// <param name="link_uid">Torrent tracker user identifier.</param>
        /// <returns>List of torrent trackers for torrent file.</returns>
        string[] GetTorrentAnnounceList(string link_uid);
    }
}
