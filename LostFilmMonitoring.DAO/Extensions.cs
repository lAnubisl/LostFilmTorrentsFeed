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
                          select new FeedItem(item);
            return new SortedSet<FeedItem>(entries);
        }
    }
}