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
    using System;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// FeedItemResponse.
    /// </summary>
    public class FeedItemResponse : IComparable<FeedItemResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeedItemResponse"/> class.
        /// </summary>
        /// <param name="xElement">element.</param>
        internal FeedItemResponse(XElement xElement)
        {
            this.Link = xElement.Elements().First(i => i.Name.LocalName == "link").Value;
            this.PublishDate = xElement.Elements().First(i => i.Name.LocalName == "pubDate").Value;
            this.PublishDateParsed = ParseDate(this.PublishDate);
            this.Title = xElement.Elements().First(i => i.Name.LocalName == "title").Value;
        }

        /// <summary>
        /// Gets or sets Title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets Link.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Gets or sets PublishDateParsed.
        /// </summary>
        public DateTime PublishDateParsed { get; set; }

        /// <summary>
        /// Gets or sets PublishDate.
        /// </summary>
        public string PublishDate { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (!(obj is FeedItemResponse other))
            {
                return false;
            }

            return string.Equals(this.Title, other.Title)
                && string.Equals(this.Link, other.Link)
                && this.PublishDate.Equals(other.PublishDate);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Title, this.Link, this.PublishDate);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Title;
        }

        /// <inheritdoc/>
        int IComparable<FeedItemResponse>.CompareTo(FeedItemResponse that)
        {
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
        /// Get Serial Name.
        /// </summary>
        /// <returns>Serial name.</returns>
        public virtual string GetSerialName()
        {
            if (string.IsNullOrEmpty(this.Title))
            {
                return null;
            }

            string marker = ").";
            int index = this.Title.IndexOf(marker);
            if (index < 0)
            {
                return null;
            }

            return this.Title.Substring(0, index + marker.Length);
        }

        /// <summary>
        /// Get Episode Name.
        /// </summary>
        /// <returns>Episode name.</returns>
        public virtual string GetEpisodeName()
        {
            if (string.IsNullOrEmpty(this.Title))
            {
                return null;
            }

            string marker = "[";
            int index = this.Title.IndexOf(marker);
            if (index < 0)
            {
                return null;
            }

            return this.Title.Substring(0, this.Title.IndexOf("["));
        }

        /// <summary>
        /// Get Episode Quality.
        /// </summary>
        /// <returns>Episode Quality.</returns>
        public virtual string GetQuality()
        {
            if (string.IsNullOrEmpty(this.Title))
            {
                return null;
            }

            char startMarker = '[';
            int startIndex = this.Title.LastIndexOf(startMarker);
            if (startIndex < 0)
            {
                return null;
            }

            char endMarker = ']';
            int endIndex = this.Title.LastIndexOf(endMarker);
            if (endIndex < 0)
            {
                return null;
            }

            if (endIndex < startIndex)
            {
                return null;
            }

            var quality = this.Title.Substring(startIndex + 1, endIndex - startIndex - 1);
            if (quality == "1080p")
            {
                quality = "1080";
            }

            return quality;
        }

        /// <summary>
        /// Get torrent file Id.
        /// </summary>
        /// <returns>Torrent file id.</returns>
        public virtual string GetTorrentId()
        {
            return LostFilmTV.Client.Extensions.GetTorrentId(this.Link);
        }

        private static DateTime ParseDate(string date)
        {
            return DateTime.TryParse(date, out DateTime result) ? result : DateTime.MinValue;
        }
    }
}
