﻿// <copyright file="Series.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.Sql.DomainModels
{
    using System;

    /// <summary>
    /// Series.
    /// </summary>
    public class Series
    {
        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets LastEpisode Date.
        /// </summary>
        public DateTime LastEpisode { get; set; }

        /// <summary>
        /// Gets or sets Last episode name.
        /// </summary>
        public string LastEpisodeName { get; set; }

        /// <summary>
        /// Gets or sets LastEpisodeTorrentLinkSD.
        /// </summary>
        public string LastEpisodeTorrentLinkSD { get; set; }

        /// <summary>
        /// Gets or sets LastEpisodeTorrentLinkMP4.
        /// </summary>
        public string LastEpisodeTorrentLinkMP4 { get; set; }

        /// <summary>
        /// Gets or sets LastEpisodeTorrentLink1080.
        /// </summary>
        public string LastEpisodeTorrentLink1080 { get; set; }
    }
}
