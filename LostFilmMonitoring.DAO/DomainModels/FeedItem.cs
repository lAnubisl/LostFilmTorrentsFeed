// <copyright file="FeedItem.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.Sql.DomainModels
{
    using System;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// FeedItem.
    /// </summary>
    public class FeedItem : IComparable<FeedItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeedItem"/> class.
        /// </summary>
        public FeedItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedItem"/> class.
        /// </summary>
        /// <param name="xElement">XElement.</param>
        public FeedItem(XElement xElement)
        {
            this.Link = xElement.Elements().First(i => i.Name.LocalName == "link").Value;
            this.PublishDate = xElement.Elements().First(i => i.Name.LocalName == "pubDate").Value;
            this.PublishDateParsed = ParseDate(this.PublishDate);
            this.Title = xElement.Elements().First(i => i.Name.LocalName == "title").Value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedItem"/> class.
        /// </summary>
        /// <param name="title">RSS Feed title.</param>
        /// <param name="link">RSS Feed link.</param>
        /// <param name="publishedDateTime">RSS Feed DateTime.</param>
        public FeedItem(string title, string link, DateTime publishedDateTime)
        {
            this.Link = link;
            this.PublishDateParsed = publishedDateTime;
            this.Title = title;
        }

        /// <summary>
        /// Gets or sets PublishDate.
        /// </summary>
        public string PublishDate { get; set; }

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

        /// <inheritdoc/>
        public int CompareTo(FeedItem other)
        {
            if (this.PublishDateParsed < other.PublishDateParsed)
            {
                return 1;
            }

            if (this.PublishDateParsed > other.PublishDateParsed)
            {
                return -1;
            }

            return this.Title.CompareTo(other.Title);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (!(obj is FeedItem other))
            {
                return false;
            }

            return string.Equals(this.Title, other.Title)
                && string.Equals(this.Link, other.Link);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Title, this.Link);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Title;
        }

        private static DateTime ParseDate(string date)
        {
            var dateTime = DateTime.TryParse(date, out var result) ? result : DateTime.MinValue;
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }
    }
}
