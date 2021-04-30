// <copyright file="Extensions.cs" company="Alexander Panfileok">
// MIT License
// Copyright (c) 2021 Alexander Panfilenok
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using LostFilmMonitoring.DAO.DomainModels;

namespace LostFilmMonitoring.DAO
{
    internal static class Extensions
    {
        public static string GenerateXml(this FeedItem[] items)
        {
            XDocument rss = new XDocument();
            rss.Add(
                new XElement(
                    "rss",
                    new XAttribute("version", "2.0"),
                    new XElement("channel")));

            foreach (var item in items)
            {
                rss.Element("rss").Element("channel").Add(
                    new XElement(
                        "item",
                        new XElement("title", item.Title),
                        new XElement("link", item.Link),
                        new XElement("pubDate", item.PublishDateParsed)));
            }

            return rss.ToString();
        }

        public static SortedSet<FeedItem> GetItems(this XDocument doc)
        {
            if (doc == null)
            {
                return null;
            }

            var entries = from item in doc.Root.Descendants()
                          .First(i => i.Name.LocalName == "channel")
                          .Elements()
                          .Where(i => i.Name.LocalName == "item")
                          select new FeedItem(item);
            return new SortedSet<FeedItem>(entries);
        }
    }
}
