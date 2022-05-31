// <copyright file="Episode.cs" company="Alexander Panfilenok">
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
    /// <summary>
    /// Episode.
    /// </summary>
    public class Episode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Episode"/> class.
        /// </summary>
        /// <param name="seriesName">Series name.</param>
        /// <param name="episodeName">Episode name.</param>
        /// <param name="seasonNumber">Season number.</param>
        /// <param name="episodeNumber">Episode number.</param>
        /// <param name="torrentId">Torrent Id.</param>
        /// <param name="quality">Quality.</param>
        public Episode(string seriesName, string episodeName, int? seasonNumber, int? episodeNumber, string torrentId, string quality)
        {
            this.SeriesName = seriesName;
            this.EpisodeName = episodeName;
            this.SeasonNumber = seasonNumber;
            this.EpisodeNumber = episodeNumber;
            this.TorrentId = torrentId;
            this.Quality = quality;
        }

        /// <summary>
        /// Gets Series Name.
        /// </summary>
        public string SeriesName { get; private set; }

        /// <summary>
        /// Gets Episode Name.
        /// </summary>
        public string EpisodeName { get; private set; }

        /// <summary>
        /// Gets Season Number.
        /// </summary>
        public int? SeasonNumber { get; private set; }

        /// <summary>
        /// Gets Episode Number.
        /// </summary>
        public int? EpisodeNumber { get; private set; }

        /// <summary>
        /// Gets Torrent Id.
        /// </summary>
        public string TorrentId { get; private set; }

        /// <summary>
        /// Gets Quality.
        /// </summary>
        public string Quality { get; private set; }
    }
}
