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

namespace LostFilmTV.Client
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using LostFilmTV.Client.Response;

    /// <summary>
    /// Useful extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Generages XDocument from input string.
        /// </summary>
        /// <param name="rssString">Input string.</param>
        /// <returns>XDocument.</returns>
        public static XDocument Parse(string rssString)
        {
            // string pattern = "(?<start>>)(?<content>.+?(?<!>))(?<end><)|(?<start>\")(?<content>.+?)(?<end>\")";
            // string result = Regex.Replace(rssString, pattern, m =>
            //             m.Groups["start"].Value +
            //             HttpUtility.HtmlEncode(HttpUtility.HtmlDecode(m.Groups["content"].Value)) +
            //             m.Groups["end"].Value);
            return XDocument.Parse(rssString);
        }

        /// <summary>
        /// Read feed items from XDocument.
        /// </summary>
        /// <param name="doc">XDocument.</param>
        /// <returns>Set of FeedItemResponse.</returns>
        public static SortedSet<FeedItemResponse> GetItems(this XDocument doc)
        {
            if (doc == null)
            {
                return null;
            }

            var entries = from item in doc.Root.Descendants()
                          .First(i => i.Name.LocalName == "channel")
                          .Elements()
                          .Where(i => i.Name.LocalName == "item")
                          select new FeedItemResponse(item);
            return new SortedSet<FeedItemResponse>(entries);
        }

        /// <summary>
        /// Extracts torrent id from ReteOrg url.
        /// </summary>
        /// <param name="reteOrgUrl">reteOrgUrl.</param>
        /// <returns>Torrent Id.</returns>
        public static string GetTorrentId(string reteOrgUrl)
        {
            // http://tracktor.in/rssdownloader.php?id=33572
            if (string.IsNullOrEmpty(reteOrgUrl))
            {
                return null;
            }

            string marker = "rssdownloader.php?id=";
            int index = reteOrgUrl.IndexOf(marker);
            if (index < 0)
            {
                return null;
            }

            return reteOrgUrl[(index + marker.Length) ..];
        }
    }
}
