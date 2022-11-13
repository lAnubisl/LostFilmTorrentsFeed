// <copyright file="SeriesTableEntity.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.Azure
{
    /// <summary>
    /// Describes Series in Azure Table Storage.
    /// </summary>
    public class SeriesTableEntity : ITableEntity
    {
        /// <inheritdoc/>
        public string PartitionKey { get; set; } = null!;

        /// <inheritdoc/>
        public string RowKey { get; set; } = null!;

        /// <inheritdoc/>
        public DateTimeOffset? Timestamp { get; set; }

        /// <inheritdoc/>
        public ETag ETag { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets LastEpisode Date.
        /// </summary>
        public DateTime LastEpisode { get; set; }

        /// <summary>
        /// Gets or sets Last episode name.
        /// </summary>
        public string LastEpisodeName { get; set; } = null!;

        /// <summary>
        /// Gets or sets LastEpisodeTorrentLinkSD.
        /// </summary>
        public string? LastEpisodeTorrentLinkSD { get; set; }

        /// <summary>
        /// Gets or sets LastEpisodeTorrentLinkMP4.
        /// </summary>
        public string? LastEpisodeTorrentLinkMP4 { get; set; }

        /// <summary>
        /// Gets or sets LastEpisodeTorrentLink1080.
        /// </summary>
        public string? LastEpisodeTorrentLink1080 { get; set; }

        /// <summary>
        /// Gets or sets Season number for 1080 quality.
        /// </summary>
        public int? SeasonNumber1080 { get; set; }

        /// <summary>
        /// Gets or sets Season number for MP4 quality.
        /// </summary>
        public int? SeasonNumberMP4 { get; set; }

        /// <summary>
        /// Gets or sets Season number for SD quality.
        /// </summary>
        public int? SeasonNumberSD { get; set; }

        /// <summary>
        /// Gets or sets Episode number for 1080 quality.
        /// </summary>
        public int? EpisodeNumber1080 { get; set; }

        /// <summary>
        /// Gets or sets Episode number for MP4 quality.
        /// </summary>
        public int? EpisodeNumberMP4 { get; set; }

        /// <summary>
        /// Gets or sets Episode number for SD quality.
        /// </summary>
        public int? EpisodeNumberSD { get; set; }

        /// <summary>
        /// Gets or sets LostFilm Id.
        /// </summary>
        public int? LostFilmId { get; set; }
    }
}
