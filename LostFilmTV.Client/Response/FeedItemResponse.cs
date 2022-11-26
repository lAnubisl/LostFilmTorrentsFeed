// <copyright file="FeedItemResponse.cs" company="Alexander Panfilenok">
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

namespace LostFilmTV.Client.Response
{
    /// <summary>
    /// FeedItemResponse.
    /// </summary>
    public sealed class FeedItemResponse : IComparable<FeedItemResponse>, IEquatable<FeedItemResponse>
    {
        // Уокер (Walker). То, чего раньше не было (S03E06) [1080p]
        private const string RegexPattern = @"(?<SeriesNameRu>.+) \((?<SeriesNameEng>.+)\)\. (?<EpisodeNameRu>.+) \(S(?<SeasonNumber>[0-9]+)E(?<EpisodeNumber>[0-9]+)\) \[(?<Quality>MP4|1080p|SD)\]";

        // Периферийные устройства (The Peripheral). А как же Боб?. (S01E05)
        private const string RegexPattern2 = @"(?<SeriesNameRu>.+) \((?<SeriesNameEng>.+)\)\. (?<EpisodeNameRu>.+) \(S(?<SeasonNumber>[0-9]+)E(?<EpisodeNumber>[0-9]+)\)";

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedItemResponse"/> class.
        /// This constructor is required for JSON deserializer.
        /// </summary>
        public FeedItemResponse()
        {
            this.Title = string.Empty;
            this.Link = string.Empty;
            this.Description = string.Empty;
            this.PublishDate = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedItemResponse"/> class.
        /// </summary>
        /// <param name="xElement">element.</param>
        internal FeedItemResponse(XElement xElement)
            : this()
        {
            var elements = xElement.Elements();
            if (elements == null || !elements.Any())
            {
                return;
            }

            this.Link = elements.First(i => i.Name.LocalName == "link").Value;
            this.PublishDate = elements.First(i => i.Name.LocalName == "pubDate").Value;
            this.PublishDateParsed = DateTime.ParseExact(this.PublishDate, "ddd, dd MMM yyyy HH:mm:ss +0000", CultureInfo.InvariantCulture);
            this.PublishDateParsed = DateTime.SpecifyKind(this.PublishDateParsed, DateTimeKind.Utc);
            this.Title = elements.First(i => i.Name.LocalName == "title").Value;
            this.Description = elements.FirstOrDefault(i => i.Name.LocalName == "description")?.Value ?? string.Empty;

            var match = Regex.Match(this.Title, RegexPattern);
            if (!match.Success)
            {
                match = Regex.Match(this.Title, RegexPattern2);
                if (!match.Success)
                {
                    return;
                }
            }

            this.SeriesNameRu = match.Groups["SeriesNameRu"].Value;
            this.SeriesNameEn = match.Groups["SeriesNameEng"].Value;
            this.EpisodeName = match.Groups["EpisodeNameRu"].Value;
            this.SeasonNumber = int.Parse(match.Groups["SeasonNumber"].Value);
            this.EpisodeNumber = int.Parse(match.Groups["EpisodeNumber"].Value);
            this.SeriesName = $"{this.SeriesNameRu} ({this.SeriesNameEn})";
            if (match.Groups.ContainsKey("Quality"))
            {
                this.Quality = match.Groups["Quality"].Value.Replace("1080p", "1080");
            }
        }

        /// <summary>
        /// Gets or sets Series Name (Russian Part only).
        /// </summary>
        public string? SeriesNameRu { get; set; }

        /// <summary>
        /// Gets or sets Series Name (English Part only).
        /// </summary>
        public string? SeriesNameEn { get; set; }

        /// <summary>
        /// Gets or sets Series Name.
        /// </summary>
        public string? SeriesName { get; set; }

        /// <summary>
        /// Gets or sets Episode Name.
        /// </summary>
        public string? EpisodeName { get; set; }

        /// <summary>
        /// Gets or sets Episode Number.
        /// </summary>
        public int? EpisodeNumber { get; set; }

        /// <summary>
        /// Gets or sets Season Number..
        /// </summary>
        public int? SeasonNumber { get; set; }

        /// <summary>
        /// Gets or sets Quality.
        /// </summary>
        public string? Quality { get; set; }

        /// <summary>
        /// Gets or sets Title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets Link.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets PublishDateParsed.
        /// </summary>
        public DateTime PublishDateParsed { get; set; }

        /// <summary>
        /// Gets or sets PublishDate.
        /// </summary>
        public string PublishDate { get; set; }

        /// <summary>
        /// Calculates if there are any updates in <paramref name="newItems"/> in comparison to <paramref name="oldItems"/>.
        /// </summary>
        /// <param name="newItems">Instance of <see cref="SortedSet{FeedItemResponse}"/> representing new items.</param>
        /// <param name="oldItems">Instance of <see cref="SortedSet{FeedItemResponse}"/> representing old items.</param>
        /// <returns>returns true in case when <paramref name="newItems"/> has updates in comparison to <paramref name="oldItems"/>, otherwise false.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="newItems"/> must not be null, <paramref name="oldItems"/> must not be null.</exception>
        public static bool HasUpdates(SortedSet<FeedItemResponse> newItems, SortedSet<FeedItemResponse> oldItems)
        {
            if (newItems == null)
            {
                throw new ArgumentNullException(nameof(newItems));
            }

            if (oldItems == null)
            {
                throw new ArgumentNullException(nameof(oldItems));
            }

            if (newItems.Count != oldItems.Count)
            {
                return true;
            }

            var newEnum = newItems.GetEnumerator();
            var oldEnum = oldItems.GetEnumerator();

            for (int i = 0; i < newItems.Count; i++)
            {
                newEnum.MoveNext();
                oldEnum.MoveNext();

                if (!newEnum.Current.Equals(oldEnum.Current))
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is not FeedItemResponse other)
            {
                return false;
            }

            return string.Equals(this.Title, other.Title)
                && string.Equals(this.Link, other.Link)
                && this.PublishDate.Equals(other.PublishDate);
        }

        /// <inheritdoc/>
        public bool Equals(FeedItemResponse? other)
        {
            if (other == null)
            {
                return false;
            }

            if (!string.Equals(this.Title, other.Title, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!string.Equals(this.Link, other.Link, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!string.Equals(this.PublishDate, other.PublishDate, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
            => HashCode.Combine(this.Title, this.Link, this.PublishDate);

        /// <inheritdoc/>
        public override string ToString()
            => this.Title;

        /// <inheritdoc/>
        int IComparable<FeedItemResponse>.CompareTo(FeedItemResponse? that)
        {
            if (that == null)
            {
                return 1;
            }

            if (this.PublishDateParsed < that.PublishDateParsed)
            {
                return 1;
            }

            if (this.PublishDateParsed > that.PublishDateParsed)
            {
                return -1;
            }

            return this.Title.CompareTo(that.Title);
        }

        /// <summary>
        /// Get torrent file Id.
        /// </summary>
        /// <returns>Torrent file id.</returns>
        public string? GetTorrentId()
            => Extensions.GetTorrentId(this.Link);
    }
}
