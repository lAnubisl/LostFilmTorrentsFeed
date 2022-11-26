// <copyright file="Extensions.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.Interfaces
{
    using System.Xml.Linq;
    using LostFilmMonitoring.DAO.Interfaces.DomainModels;

    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Generate XML document from array or FeedItems.
        /// </summary>
        /// <param name="items">FeedItem[].</param>
        /// <returns>XML.</returns>
        public static string GenerateXml(this FeedItem[] items)
        {
            var rss = new XDocument();
            var channel = new XElement("channel");
            rss.Add(new XElement("rss", new XAttribute("version", "2.0"), channel));
            foreach (var item in items)
            {
                channel.Add(item.ToXElement());
            }

            return rss.ToString();
        }

        /// <summary>
        /// Parse <see cref="XDocument"/>.
        /// </summary>
        /// <param name="doc">XDocument to parse.</param>
        /// <returns>SortedSet.</returns>
        public static SortedSet<FeedItem> GetItems(this XDocument doc)
        {
            if (doc.Root == null)
            {
                return new SortedSet<FeedItem>();
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
