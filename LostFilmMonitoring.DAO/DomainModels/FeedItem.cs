using System;
using System.Linq;
using System.Xml.Linq;
using LostFilmTV.Client.Response;

namespace LostFilmMonitoring.DAO.DomainModels
{
    public class FeedItem : IComparable<FeedItem>
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public DateTime PublishDateParsed { get; set; }

        public FeedItem()
        {

        }

        public FeedItem(XElement xElement)
        {
            this.Link = xElement.Elements().First(i => i.Name.LocalName == "link").Value;
            this.PublishDate = xElement.Elements().First(i => i.Name.LocalName == "pubDate").Value;
            this.PublishDateParsed = ParseDate(this.PublishDate);
            this.Title = xElement.Elements().First(i => i.Name.LocalName == "title").Value;
        }

        /// <summary>
        /// Gets or sets PublishDate.
        /// </summary>
        public string PublishDate { get; set; }


        private static DateTime ParseDate(string date)
        {
            return DateTime.TryParse(date, out DateTime result) ? result : DateTime.MinValue;
        }

        public FeedItem(FeedItemResponse item)
        {
            Link = item.Link;
            PublishDateParsed = item.PublishDateParsed;
            Title = item.Title;
        }

        public FeedItem(FeedItemResponse item, string link) : this(item)
        {
            Link = link;
        }

        public int CompareTo(FeedItem other)
        {
            if (PublishDateParsed < other.PublishDateParsed) return 1;
            if (PublishDateParsed > other.PublishDateParsed) return -1;
            return Title.CompareTo(other.Title);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FeedItem other)) return false;
            return string.Equals(Title, other.Title)
                && string.Equals(Link, other.Link);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Title, Link);
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
