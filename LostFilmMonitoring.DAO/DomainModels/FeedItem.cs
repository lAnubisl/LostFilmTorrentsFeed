using System;
using System.Linq;
using System.Xml.Linq;

namespace LostFilmMonitoring.DAO.DomainModels
{
    public class FeedItem : IComparable<FeedItem>
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public DateTime PublishDateParsed { get; set; }
        public string PublishDate { get; set; }

        public FeedItem()
        {

        }

        public FeedItem(FeedItem item, string link)
        {
            Link = link;
            PublishDate = item.PublishDate;
            PublishDateParsed = item.PublishDateParsed;
            Title = item.Title;
        }

        public FeedItem(XElement xElement)
        {
            Link = xElement.Elements().First(i => i.Name.LocalName == "link").Value;
            PublishDate = xElement.Elements().First(i => i.Name.LocalName == "pubDate").Value;
            PublishDateParsed = ParseDate(PublishDate);
            Title = xElement.Elements().First(i => i.Name.LocalName == "title").Value;
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime result))
                return result;
            else
                return DateTime.MinValue;
        }

        public int CompareTo(FeedItem other)
        {
            if (PublishDateParsed < other.PublishDateParsed) return 1;
            if (PublishDateParsed > other.PublishDateParsed) return -1;
            return Title.CompareTo(other.Title);
        }

        public override bool Equals(object obj)
        {
            var other = obj as FeedItem;
            if (other == null) return false;
            return string.Equals(Title, other.Title)
                && string.Equals(Link, other.Link)
                && PublishDate.Equals(other.PublishDate);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Title != null ? Title.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (Link != null ? Link.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ PublishDate.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
