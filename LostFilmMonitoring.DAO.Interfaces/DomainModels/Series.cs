// <copyright file="Series.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.Interfaces.DomainModels
{
    using System;

    /// <summary>
    /// Series.
    /// </summary>
    public class Series
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Series"/> class.
        /// </summary>
        /// <param name="name">Series name.</param>
        /// <param name="lastEposide">Last episode date.</param>
        /// <param name="lastEpisodeName">Last episode name.</param>
        /// <param name="linkSD">Link to torrent file SD.</param>
        /// <param name="linkMP4">Link to torrent file MP4.</param>
        /// <param name="link1080">Link to torrent file 1080.</param>
        /// <param name="q1080SeasonNumber">Season number for last episode of quality 1080p.</param>
        /// <param name="qMP4SeasonNumber">Season number for last episode of quality 720p.</param>
        /// <param name="qSDSeasonNumber">Season number for last episode of quality SD.</param>
        /// <param name="q1080EpisodeNumber">Episode number for last episode of quality 1080p.</param>
        /// <param name="qMP4EpisodeNumber">Episode number for last episode of quality 720p.</param>
        /// <param name="qSDEpisodeNumber">Episode number for last episode of quality SD.</param>
        /// <param name="lostFilmId">LostFilm Id.</param>
        public Series(
            string name,
            DateTime lastEposide,
            string lastEpisodeName,
            string? linkSD,
            string? linkMP4,
            string? link1080,
            int? q1080SeasonNumber = null,
            int? qMP4SeasonNumber = null,
            int? qSDSeasonNumber = null,
            int? q1080EpisodeNumber = null,
            int? qMP4EpisodeNumber = null,
            int? qSDEpisodeNumber = null,
            int? lostFilmId = null)
        {
            this.Name = name;
            this.LastEpisode = lastEposide;
            this.LastEpisodeName = lastEpisodeName;
            this.LastEpisodeTorrentLinkSD = linkSD;
            this.LastEpisodeTorrentLinkMP4 = linkMP4;
            this.LastEpisodeTorrentLink1080 = link1080;
            this.Q1080EpisodeNumber = q1080EpisodeNumber;
            this.QMP4EpisodeNumber = qMP4EpisodeNumber;
            this.QSDEpisodeNumber = qSDEpisodeNumber;
            this.Q1080SeasonNumber = q1080SeasonNumber;
            this.QMP4SeasonNumber = qMP4SeasonNumber;
            this.QSDSeasonNumber = qSDSeasonNumber;
            this.LostFilmId = lostFilmId;
        }

        /// <summary>
        /// Gets Name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets LastEpisode Date.
        /// </summary>
        public DateTime LastEpisode { get; private set; }

        /// <summary>
        /// Gets Last episode name.
        /// </summary>
        public string LastEpisodeName { get; private set; }

        /// <summary>
        /// Gets LastEpisodeTorrentLinkSD.
        /// </summary>
        public string? LastEpisodeTorrentLinkSD { get; private set; }

        /// <summary>
        /// Gets LastEpisodeTorrentLinkMP4.
        /// </summary>
        public string? LastEpisodeTorrentLinkMP4 { get; private set; }

        /// <summary>
        /// Gets LastEpisodeTorrentLink1080.
        /// </summary>
        public string? LastEpisodeTorrentLink1080 { get; private set; }

        /// <summary>
        /// Gets season number for last episode of quality 1080p.
        /// </summary>
        public int? Q1080SeasonNumber { get; private set; }

        /// <summary>
        /// Gets season number for last episode of quality 720p.
        /// </summary>
        public int? QMP4SeasonNumber { get; private set; }

        /// <summary>
        /// Gets season number for last episode of quality SD.
        /// </summary>
        public int? QSDSeasonNumber { get; private set; }

        /// <summary>
        /// Gets episode number for last episode of quality 1080p.
        /// </summary>
        public int? Q1080EpisodeNumber { get; private set; }

        /// <summary>
        /// Gets episode number for last episode of quality 720p.
        /// </summary>
        public int? QMP4EpisodeNumber { get; private set; }

        /// <summary>
        /// Gets episode number for last episode of quality SD.
        /// </summary>
        public int? QSDEpisodeNumber { get; private set; }

        /// <summary>
        /// Gets or sets LostFilm Id.
        /// </summary>
        public int? LostFilmId { get; set; }

        /// <summary>
        /// Merge updates from <paramref name="from"/> to current instance.
        /// </summary>
        /// <param name="from">Instance of <see cref="Series"/> to merge changes from.</param>
        public void MergeFrom(Series from)
        {
            this.LastEpisodeName = from.LastEpisodeName;
            this.LastEpisode = from.LastEpisode;
            this.LastEpisodeTorrentLink1080 = from.LastEpisodeTorrentLink1080 ?? this.LastEpisodeTorrentLink1080;
            this.LastEpisodeTorrentLinkMP4 = from.LastEpisodeTorrentLinkMP4 ?? this.LastEpisodeTorrentLinkMP4;
            this.LastEpisodeTorrentLinkSD = from.LastEpisodeTorrentLinkSD ?? this.LastEpisodeTorrentLinkSD;
            this.Q1080SeasonNumber = from.Q1080SeasonNumber ?? this.Q1080SeasonNumber;
            this.QMP4SeasonNumber = from.QMP4SeasonNumber ?? this.QMP4SeasonNumber;
            this.QSDSeasonNumber = from.QSDSeasonNumber ?? this.QSDSeasonNumber;
            this.Q1080EpisodeNumber = from.Q1080EpisodeNumber ?? this.Q1080EpisodeNumber;
            this.QMP4EpisodeNumber = from.QMP4EpisodeNumber ?? this.QMP4EpisodeNumber;
            this.QSDEpisodeNumber = from.QSDEpisodeNumber ?? this.QSDEpisodeNumber;
            this.LostFilmId = from.LostFilmId ?? this.LostFilmId;
        }
    }
}
