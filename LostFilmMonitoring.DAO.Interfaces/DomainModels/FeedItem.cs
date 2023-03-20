// <copyright file="FeedItem.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.Interfaces.DomainModels
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// FeedItem.
    /// </summary>
    public class FeedItem : IComparable<FeedItem>
    {
        private const string XmlDateFormat = "yyyy-MM-dd'T'HH:mm:ss'Z'";

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
            this.PublishDateParsed = DateTime.ParseExact(this.PublishDate, XmlDateFormat, CultureInfo.InvariantCulture);
            this.PublishDateParsed = DateTime.SpecifyKind(this.PublishDateParsed, DateTimeKind.Utc);
            this.Title = xElement.Elements().First(i => i.Name.LocalName == "title").Value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedItem"/> class.
        /// </summary>
        /// <param name="title">Feed Item Title.</param>
        /// <param name="link">Feed Item Link.</param>
        /// <param name="publishedDateTime">Feed Item Date.</param>
        public FeedItem(string title, string link, DateTime publishedDateTime)
        {
            this.Link = link;
            this.PublishDateParsed = publishedDateTime;
            this.PublishDate = publishedDateTime.ToString(XmlDateFormat);
            this.Title = title;
        }

        /// <summary>
        /// Gets or sets PublishDate.
        /// </summary>
        public string? PublishDate { get; set; }

        /// <summary>
        /// Gets or sets Title.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets Link.
        /// </summary>
        public string? Link { get; set; }

        /// <summary>
        /// Gets or sets PublishDateParsed.
        /// </summary>
        public DateTime PublishDateParsed { get; set; }

        /// <summary>
        /// For user torrent feed item returns file name.
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <returns>File name.</returns>
        public string? GetUserFileName(string userId)
        {
            if (string.IsNullOrEmpty(this?.Link))
            {
                return null;
            }

            var index = this.Link.IndexOf(userId);
            if (index < 0)
            {
                return null;
            }

            var subStr = this.Link[(index + userId.Length) ..];
            if (subStr.Length == 0)
            {
                return null;
            }

            var fileName = subStr[1..];
            if (!fileName.EndsWith(".torrent"))
            {
                return null;
            }

            return fileName;
        }

        /// <inheritdoc/>
        public int CompareTo(FeedItem? other)
        {
            if (other == null)
            {
                return -1;
            }

            if (this.PublishDateParsed < other.PublishDateParsed)
            {
                return 1;
            }

            if (this.PublishDateParsed > other.PublishDateParsed)
            {
                return -1;
            }

            if (this.Title == null)
            {
                return 1;
            }

            return this.Title.CompareTo(other.Title);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is not FeedItem other)
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
            return this.Title ?? string.Empty;
        }

        /// <summary>
        /// Converts an instance to a XElement.
        /// </summary>
        /// <returns>XElement.</returns>
        public XElement ToXElement()
            => new (
                "item",
                new XElement("title", this.Title),
                new XElement("link", this.Link),
                new XElement("pubDate", this.PublishDateParsed.ToString(XmlDateFormat)));
    }
}
