// <copyright file="ExtensionsTests.cs" company="Alexander Panfilenok">
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

namespace LostFilmTV.Client.Tests
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// Contains tests for LostFilmTV.Client.Extenstions.
    /// </summary>
    public class ExtensionsTests
    {
        /// <summary>
        /// It is possible that incoming RSS is broken and contains invalid characters.
        /// In this case we have to fix RSS before parsing.
        /// </summary>
        [Test]
        public void BrokenXmlShouldBeFixedBeforeParsing()
        {
            var brokenXML = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
    <rss version=""0.91"">
        <channel>
            <title>LostFilm.TV</title>
            <description>Новинки от LostFilm.TV</description>
            <link>https://www.lostfilm.tv/</link>
            <lastBuildDate>Fri, 21 May 2021 13:07:17 + 0000</lastBuildDate>
            <language>ru</language>
            <item>
                <title>Любовь, смерть и роботы(Love, Death & Robots).Лед(S02E02)[MP4]</title>
                <category>[MP4]</category>
                <pubDate>Fri, 21 May 2021 13:07:14 + 0000</pubDate>
                <link>http://n.tracktor.site/rssdownloader.php?id=44795</link>
            </item>
        </channel>
    </rss>
";
            try
            {
                var items = Extensions.Parse(brokenXML);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
    }
}
