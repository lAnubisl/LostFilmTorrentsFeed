using LostFilmMonitoring.DAO.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LostFilmMonitoring.DAO
{
    public static class Extensions
    {
        public static string GenerateXml(this FeedItem[] items)
        {
            XDocument rss = new XDocument();
            rss.Add(
                new XElement("rss",
                    new XAttribute("version", "2.0"),
                    new XElement("channel")
                )
            );

            foreach (var item in items)
            {
                rss.Element("rss").Element("channel").Add(
                    new XElement("item",
                        new XElement("title", item.Title),
                        new XElement("link", item.Link),
                        new XElement("pubDate", item.PublishDate)
                    )
                );
            }

            return rss.ToString();
        }

        public static SortedSet<FeedItem> GetItems(this XDocument doc)
        {
            var entries = from item in doc.Root.Descendants()
                          .First(i => i.Name.LocalName == "channel")
                          .Elements()
                          .Where(i => i.Name.LocalName == "item")
                          select new FeedItem
                          {
                              Link = item
                                    .Elements()
                                    .First(i => i.Name.LocalName == "link")
                                    .Value,
                              PublishDate = item
                                        .Elements()
                                        .First(i => i.Name.LocalName == "pubDate")
                                        .Value,
                              PublishDateParsed = ParseDate(
                                    item
                                        .Elements()
                                        .First(i => i.Name.LocalName == "pubDate")
                                        .Value
                                    ),
                              Title = item.Elements()
                                    .First(i => i.Name.LocalName == "title")
                                    .Value
                          };
            return new SortedSet<FeedItem>(entries);
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime result))
                return result;
            else
                return DateTime.MinValue;
        }
    }
}