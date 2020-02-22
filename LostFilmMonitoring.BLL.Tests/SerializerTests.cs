using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace LostFilmMonitoring.BLL.Tests
{
    public class SerializerTests
    {
        [Fact]
        public void SerializerShouldSerialize()
        {
			var rss = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<rss version=""0.91"">
<channel>
<title>LostFilm.TV</title>
<description>Новинки от LostFilm.TV</description>
<link>https://www.lostfilm.tv/</link>
<lastBuildDate>Thu, 20 Feb 2020 20:07:12 +0000</lastBuildDate>
<language>ru</language><item>
	<title>Блудный сын (Prodigal Son). На пороге смерти(S01E15) [MP4]</title>
	<category>[MP4]</category>
	<pubDate>Thu, 20 Feb 2020 20:07:08 +0000</pubDate>
	<link>http://n.tracktor.site/rssdownloader.php?id=38034</link>
</item>
<item>
	<title>Блудный сын(Prodigal Son). На пороге смерти(S01E15) [1080p]</title>
	<category>[1080p]</category>
	<pubDate>Thu, 20 Feb 2020 20:07:08 +0000</pubDate>
	<link>http://n.tracktor.site/rssdownloader.php?id=38033</link>
</item>
<item>
	<title>Блудный сын(Prodigal Son). На пороге смерти(S01E15) [SD]</title>
	<category>[SD]</category>
	<pubDate>Thu, 20 Feb 2020 20:07:08 +0000</pubDate>
	<link>http://n.tracktor.site/rssdownloader.php?id=38032</link>
</item>
<item>
	<title>Ключи Локков(Locke & Key). Игры разума(S01E03) [MP4]</title>
	<category>[MP4]</category>
	<pubDate>Thu, 20 Feb 2020 18:51:35 +0000</pubDate>
	<link>http://n.tracktor.site/rssdownloader.php?id=38031</link>
</item>
<item>
	<title>Ключи Локков(Locke & Key). Игры разума(S01E03) [1080p]</title>
	<category>[1080p]</category>
	<pubDate>Thu, 20 Feb 2020 18:51:35 +0000</pubDate>
	<link>http://n.tracktor.site/rssdownloader.php?id=38030</link>
</item>
<item>
	<title>Ключи Локков(Locke & Key). Игры разума(S01E03) [SD]</title>
	<category>[SD]</category>
	<pubDate>Thu, 20 Feb 2020 18:51:35 +0000</pubDate>
	<link>http://n.tracktor.site/rssdownloader.php?id=38029</link>
</item>
<item>
	<title>Легенды завтрашнего дня(Legends of Tomorrow). Всему голова(S05E05) [MP4]</title>
	<category>[MP4]</category>
	<pubDate>Thu, 20 Feb 2020 17:37:02 +0000</pubDate>
	<link>http://n.tracktor.site/rssdownloader.php?id=38028</link>
</item>
<item>
	<title>Легенды завтрашнего дня(Legends of Tomorrow). Всему голова(S05E05) [1080p]</title>
	<category>[1080p]</category>
	<pubDate>Thu, 20 Feb 2020 17:37:02 +0000</pubDate>
	<link>http://n.tracktor.site/rssdownloader.php?id=38027</link>
</item>
<item>
	<title>Легенды завтрашнего дня(Legends of Tomorrow). Всему голова(S05E05) [SD]</title>
	<category>[SD]</category>
	<pubDate>Thu, 20 Feb 2020 17:37:02 +0000</pubDate>
	<link>http://n.tracktor.site/rssdownloader.php?id=38026</link>
</item>
<item>
	<title>Флэш(The Flash). Девушка по имени Сью(S06E12) [MP4]</title>
	<category>[MP4]</category>
	<pubDate>Thu, 20 Feb 2020 17:07:46 +0000</pubDate>
	<link>http://n.tracktor.site/rssdownloader.php?id=38025</link>
</item>
<item>
	<title>Флэш(The Flash). Девушка по имени Сью(S06E12) [1080p]</title>
	<category>[1080p]</category>
	<pubDate>Thu, 20 Feb 2020 17:07:46 +0000</pubDate>
	<link>http://n.tracktor.site/rssdownloader.php?id=38024</link>
</item>
<item>
	<title>Флэш(The Flash). Девушка по имени Сью(S06E12) [SD]</title>
	<category>[SD]</category>
	<pubDate>Thu, 20 Feb 2020 17:07:46 +0000</pubDate>
	<link>http://n.tracktor.site/rssdownloader.php?id=38023</link>
</item>
<item>
	<title>Хороший доктор(The Good Doctor). Невысказанное(S03E15) [MP4]</title>
	<category>[MP4]</category>
	<pubDate>Wed, 19 Feb 2020 20:11:00 +0000</pubDate>
	<link>http://n.tracktor.site/rssdownloader.php?id=38022</link>
</item>
<item>
	<title>Хороший доктор(The Good Doctor). Невысказанное(S03E15) [1080p]</title>
	<category>[1080p]</category>
	<pubDate>Wed, 19 Feb 2020 20:11:00 +0000</pubDate>
	<link>http://n.tracktor.site/rssdownloader.php?id=38021</link>
</item>
<item>
	<title>Хороший доктор(The Good Doctor). Невысказанное(S03E15) [SD]</title>
	<category>[SD]</category>
	<pubDate>Wed, 19 Feb 2020 20:11:00 +0000</pubDate>
	<link>http://n.tracktor.site/rssdownloader.php?id=38020</link>
</item>
</channel>
</rss>";
			Exception exThrown = null;
			try
			{
				var doc = RSSParser.Parse(rss);
			}
			catch(Exception ex)
			{
				exThrown = ex;
			}

			Assert.Null(exThrown);
		}
    }
}
