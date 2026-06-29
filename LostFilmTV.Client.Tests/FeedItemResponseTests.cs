using System.Linq;

namespace LostFilmTV.Client.Tests;

[TestFixture]
[ExcludeFromCodeCoverage]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "Test class.")]
public class FeedItemResponseTests
{
    [Test]
    [TestCase("http://tracktor.in/rssdownloader.php?id=33572", "33572")]
    [TestCase(null, null)]
    [TestCase("", null)]
    [TestCase("http://tracktor.in/test.php?id=33572", null)]
    public void FeedItemResponse_GetTorrentId_should_return_id(string link, string expected)
    {
        var el = XElement.Parse(
           @$"<item>
                <title>Братья Харди (The Hardy Boys). Неожиданное возвращение (S02E10) [1080p]</title>
                <category>[1080p]</category>
                <pubDate>Sat, 21 May 2022 20:58:00 +0000</pubDate>
                <link>{link}</link>
            </item>");
        ReteOrgFeedItemResponse feedItemResponse;
        var ok = ReteOrgFeedItemResponse.TryParseFromXElement(el, out feedItemResponse);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ok, Is.True);
            Assert.That(feedItemResponse!.TorrentId, Is.EqualTo(expected));
        }
    }

    [Test]
    [TestCase("Внешние сферы (Outer Range). Неведомое (S01E07) [MP4]", "Внешние сферы (Outer Range)", TestName = "FeedItemResponse_GetSeriesName_should_return_name(valid)")]
    [TestCase("Внешние сферы (Outer Range) Неведомое (S01E07) [MP4]", null, TestName = "FeedItemResponse_GetSeriesName_should_return_name(invalid)")]
    [TestCase(null, null, TestName = "FeedItemResponse_GetSeriesName_should_return_name(null)")]
    [TestCase("", null, TestName = "FeedItemResponse_GetSeriesName_should_return_name(empty string)")]
    public void FeedItemResponse_GetSeriesName_should_return_name(string title, string expected)
    {
        var el = XElement.Parse(
           @$"<item>
                    <title>{title}</title>
                    <category>[MP4]</category>
                    <pubDate>Sat, 21 May 2022 20:58:00 +0000</pubDate>
                    <link>http://n.tracktor.site/rssdownloader.php?id=51439</link>
                </item>");
        ReteOrgFeedItemResponse feedItemResponse;
        var ok = ReteOrgFeedItemResponse.TryParseFromXElement(el, out feedItemResponse);
        if (expected == null)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(ok, Is.False);
                Assert.That(feedItemResponse, Is.Null);
            }
        }
        else
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(ok, Is.True);
                Assert.That(feedItemResponse!.SeriesName, Is.EqualTo(expected));
            }
        }
    }

    [Test]
    [TestCase("Внешние сферы (Outer Range). Неведомое (S01E07) [MP4]", "Неведомое", TestName = "FeedItemResponse_GetEpisodeName_should_return_name(valid)")]
    [TestCase("Внешние сферы (Outer Range) Неведомое (S01E07) (MP4)", null)]
    [TestCase(null, null)]
    [TestCase("", null)]
    public void FeedItemResponse_GetEpisodeName_should_return_name(string title, string expected)
    {
        var el = XElement.Parse(
           @$"<item>
                    <title>{title}</title>
                    <category>[MP4]</category>
                    <pubDate>Sat, 21 May 2022 20:58:00 +0000</pubDate>
                    <link>http://n.tracktor.site/rssdownloader.php?id=51439</link>
                </item>");
        ReteOrgFeedItemResponse feedItemResponse;
        var ok = ReteOrgFeedItemResponse.TryParseFromXElement(el, out feedItemResponse);
        if (expected == null)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(ok, Is.False);
                Assert.That(feedItemResponse, Is.Null);
            }
        }
        else
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(ok, Is.True);
                Assert.That(feedItemResponse!.EpisodeName, Is.EqualTo(expected));
            }
        }
    }

    [Test]
    [TestCase("Внешние сферы (Outer Range). Неведомое (S01E07) [MP4]", "MP4", TestName = "FeedItemResponse_GetQuality_should_return_quality(valid MP4)")]
    [TestCase("Внешние сферы (Outer Range). Неведомое (S01E07) [SD]", "SD", TestName = "FeedItemResponse_GetQuality_should_return_quality(valid SD)")]
    [TestCase("Внешние сферы (Outer Range). Неведомое (S01E07) [1080p]", "1080", TestName = "FeedItemResponse_GetQuality_should_return_quality(valid 1080)")]
    [TestCase("Внешние сферы (Outer Range). Неведомое (S01E07) [MP4)", null, TestName = "FeedItemResponse_GetQuality_should_return_quality(invalid [MP4))")]
    [TestCase("Внешние сферы (Outer Range). Неведомое (S01E07) (MP4]", null, TestName = "FeedItemResponse_GetQuality_should_return_quality(invalid (MP4]")]
    [TestCase("Внешние сферы (Outer Range). Неведомое (S01E07) ]MP4[", null, TestName = "FeedItemResponse_GetQuality_should_return_quality(invalid ]MP4[)")]
    [TestCase("Внешние сферы (Outer Range). Неведомое (S01E07) (MP4)", null, TestName = "FeedItemResponse_GetQuality_should_return_quality(invalid (MP4))")]
    [TestCase(null, null)]
    [TestCase("", null)]
    public void FeedItemResponse_GetQuality_should_return_quality(string title, string expected)
    {
        var el = XElement.Parse(
           @$"<item>
                    <title>{title}</title>
                    <category>[MP4]</category>
                    <pubDate>Sat, 21 May 2022 20:58:00 +0000</pubDate>
                    <link>http://n.tracktor.site/rssdownloader.php?id=51439</link>
                </item>");
        ReteOrgFeedItemResponse feedItemResponse;
        var ok = ReteOrgFeedItemResponse.TryParseFromXElement(el, out feedItemResponse);
        if (expected == null)
        {
           using (Assert.EnterMultipleScope())
           {
               Assert.That(ok, Is.False);
               Assert.That(feedItemResponse, Is.Null);
           }
        }
        else
        {
           using (Assert.EnterMultipleScope())
           {
               Assert.That(ok, Is.True);
               Assert.That(feedItemResponse!.Quality, Is.EqualTo(expected));
           }
        }
    }

    [Test]
    public void FeedItemResponse_ToString_should_return_string()
    {
        var title ="Внешние сферы (Outer Range). Неведомое (S01E07) [MP4]";
        var el = XElement.Parse(
          @$"<item>
                    <title>{title}</title>
                    <category>[MP4]</category>
                    <pubDate>Sat, 21 May 2022 20:58:00 +0000</pubDate>
                    <link>http://n.tracktor.site/rssdownloader.php?id=51439</link>
                </item>");
        ReteOrgFeedItemResponse feedItemResponse;
        var ok = ReteOrgFeedItemResponse.TryParseFromXElement(el, out feedItemResponse);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ok, Is.True);
            Assert.That(feedItemResponse!.ToString(), Is.EqualTo(title));
        }
    }

    [Test]
    public void FeedItemResponse_constructor_from_XElement()
    {
        var element = XElement.Parse(@"
                <item>
                    <title>Братья Харди (The Hardy Boys). Неожиданное возвращение (S02E10) [1080p]</title>
                    <category>[1080p]</category>
                    <pubDate>Mon, 09 May 2022 20:27:53 +0000</pubDate>
                    <link>http://n.tracktor.site/rssdownloader.php?id=51236</link>
                </item>");
        ReteOrgFeedItemResponse item;
        var ok = ReteOrgFeedItemResponse.TryParseFromXElement(element, out item);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ok, Is.True);
            Assert.That(item.Title, Is.EqualTo("Братья Харди (The Hardy Boys). Неожиданное возвращение (S02E10) [1080p]"));
            Assert.That(item.Link, Is.EqualTo("http://n.tracktor.site/rssdownloader.php?id=51236"));
            Assert.That(item.PublishDate, Is.EqualTo("Mon, 09 May 2022 20:27:53 +0000"));
            Assert.That(item.PublishDateParsed, Is.EqualTo(new DateTime(2022, 05, 09, 20, 27, 53, DateTimeKind.Utc)));
        }
    }

    [Test]
    public void FeedItemResponse_should_be_sortable()
    {
        var items = new SortedSet<ReteOrgFeedItemResponse>(
            [
                new ReteOrgFeedItemResponse()
                {
                    Title = "Title1",
                    Link = "http://any.com",
                    PublishDate = "2022-05-09T00:00:00Z",
                    PublishDateParsed = new DateTime(2022, 5, 9, 00, 0, 0, DateTimeKind.Utc),
                },
                new ReteOrgFeedItemResponse()
                {
                    Title = "Title2",
                    Link = "http://any.com",
                    PublishDate = "2022-05-08T00:00:00Z",
                    PublishDateParsed = new DateTime(2022, 5, 8, 00, 0, 0, DateTimeKind.Utc),
                },
                new ReteOrgFeedItemResponse()
                {
                    Title = "Title3",
                    Link = "http://any.com",
                    PublishDate = "2022-05-10T00:00:00Z",
                    PublishDateParsed = new DateTime(2022, 5, 10, 00, 0, 0, DateTimeKind.Utc),
                },
            ]);
        var arr = items.ToArray();  
        using (Assert.EnterMultipleScope())
        {
            Assert.That(arr[0].Title, Is.EqualTo("Title3"));
            Assert.That(arr[1].Title, Is.EqualTo("Title1"));
            Assert.That(arr[2].Title, Is.EqualTo("Title2"));
        }
    }

    [Test]
    [TestCase("Бороуз (The Boroughs).  Запретный плод (S01E04) [1080p]", "Бороуз", "The Boroughs", "Запретный плод", "1080", TestName = "FeedItemResponse_GetQuality_should_handle_double_space")]
    [TestCase("Маршалы (Marshals).   Волки на пороге (S01E13) [MP4]", "Маршалы", "Marshals", "Волки на пороге", "MP4", TestName = "FeedItemResponse_GetQuality_should_handle_triple_space")]
    public void FeedItemResponse_regex_should_handle_variable_whitespace(string title, string expectedSeriesNameRu, string expectedSeriesNameEng, string expectedEpisodeName, string expectedQuality)
    {
        var el = XElement.Parse(
           @$"<item>
                    <title>{title}</title>
                    <category>[MP4]</category>
                    <pubDate>Sat, 21 May 2022 20:58:00 +0000</pubDate>
                    <link>http://n.tracktor.site/rssdownloader.php?id=51439</link>
                </item>");
        ReteOrgFeedItemResponse feedItemResponse;
        var ok = ReteOrgFeedItemResponse.TryParseFromXElement(el, out feedItemResponse);
        Assert.That(ok, Is.True);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(feedItemResponse.SeriesNameRu, Is.EqualTo(expectedSeriesNameRu));
            Assert.That(feedItemResponse.SeriesNameEn, Is.EqualTo(expectedSeriesNameEng));
            Assert.That(feedItemResponse.EpisodeName, Is.EqualTo(expectedEpisodeName));
            Assert.That(feedItemResponse.Quality, Is.EqualTo(expectedQuality));
        }
    }

    [Test]
    [TestCase("Бороуз (The Boroughs). Запретный плод (S01E04) [1080p]", "Бороуз", "The Boroughs", "Запретный плод", 1, 4, "1080", TestName = "FeedItemResponse_optimized_regex_should_parse_valid_title")]
    [TestCase("Дерзость (The Audacity). Гранфаллон (S01E08) [MP4]", "Дерзость", "The Audacity", "Гранфаллон", 1, 8, "MP4", TestName = "FeedItemResponse_optimized_regex_should_handle_MP4_quality")]
    [TestCase("Эйфория (Euphoria). И в дождь, и в зной (S03E07) [SD]", "Эйфория", "Euphoria", "И в дождь, и в зной", 3, 7, "SD", TestName = "FeedItemResponse_optimized_regex_should_handle_SD_quality")]
    public void FeedItemResponse_optimized_regex_should_extract_all_fields(string title, string expectedSeriesNameRu, string expectedSeriesNameEng, string expectedEpisodeName, int expectedSeason, int expectedEpisode, string expectedQuality)
    {
        var el = XElement.Parse(
           @$"<item>
                    <title>{title}</title>
                    <category>{expectedQuality}</category>
                    <pubDate>Sat, 21 May 2022 20:58:00 +0000</pubDate>
                    <link>http://n.tracktor.site/rssdownloader.php?id=51439</link>
                </item>");
        ReteOrgFeedItemResponse feedItemResponse;
        var ok = ReteOrgFeedItemResponse.TryParseFromXElement(el, out feedItemResponse);
        Assert.That(ok, Is.True);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(feedItemResponse.SeriesNameRu, Is.EqualTo(expectedSeriesNameRu));
            Assert.That(feedItemResponse.SeriesNameEn, Is.EqualTo(expectedSeriesNameEng));
            Assert.That(feedItemResponse.EpisodeName, Is.EqualTo(expectedEpisodeName));
            Assert.That(feedItemResponse.SeasonNumber, Is.EqualTo(expectedSeason));
            Assert.That(feedItemResponse.EpisodeNumber, Is.EqualTo(expectedEpisode));
            Assert.That(feedItemResponse.Quality, Is.EqualTo(expectedQuality));
            Assert.That(feedItemResponse.SeriesName, Is.EqualTo($"{expectedSeriesNameRu} ({expectedSeriesNameEng})"));
        }
    }

    [Test]
    [TestCase("Периферийные устройства (The Peripheral). А как же Боб?. (S01E05)", "Периферийные устройства", "The Peripheral", "А как же Боб?.", 1, 5, null, TestName = "FeedItemResponse_optimized_regex_pattern2_should_parse_without_quality")]
    [TestCase("Внешние сферы (Outer Range). Неведомое (S01E07)", "Внешние сферы", "Outer Range", "Неведомое", 1, 7, null, TestName = "FeedItemResponse_optimized_regex_pattern2_should_work_for_various_titles")]
    public void FeedItemResponse_optimized_regex_pattern2_should_extract_fields_without_quality(string title, string expectedSeriesNameRu, string expectedSeriesNameEng, string expectedEpisodeName, int expectedSeason, int expectedEpisode, string expectedQuality)
    {
        var el = XElement.Parse(
           @$"<item>
                    <title>{title}</title>
                    <category>[MP4]</category>
                    <pubDate>Sat, 21 May 2022 20:58:00 +0000</pubDate>
                    <link>http://n.tracktor.site/rssdownloader.php?id=51439</link>
                </item>");
        ReteOrgFeedItemResponse feedItemResponse;
        var ok = ReteOrgFeedItemResponse.TryParseFromXElement(el, out feedItemResponse);
        Assert.That(ok, Is.True);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(feedItemResponse.SeriesNameRu, Is.EqualTo(expectedSeriesNameRu));
            Assert.That(feedItemResponse.SeriesNameEn, Is.EqualTo(expectedSeriesNameEng));
            Assert.That(feedItemResponse.EpisodeName, Is.EqualTo(expectedEpisodeName));
            Assert.That(feedItemResponse.SeasonNumber, Is.EqualTo(expectedSeason));
            Assert.That(feedItemResponse.EpisodeNumber, Is.EqualTo(expectedEpisode));
            Assert.That(feedItemResponse.Quality, Is.EqualTo(expectedQuality));
        }
    }

    [Test]
    [TestCase("Бороуз The Boroughs. Запретный плод (S01E04) [1080p]", null, TestName = "FeedItemResponse_malformed_should_fail_gracefully_missing_parens_1")]
    [TestCase("Бороуз (The Boroughs) Запретный плод (S01E04) [1080p]", null, TestName = "FeedItemResponse_malformed_should_fail_gracefully_missing_dot")]
    [TestCase("Бороуз (The Boroughs). (S01E04) [1080p]", null, TestName = "FeedItemResponse_malformed_should_fail_gracefully_missing_episode_name")]
    [TestCase("Бороуз (The Boroughs). Запретный плод S01E04 [1080p]", null, TestName = "FeedItemResponse_malformed_should_fail_gracefully_missing_season_format")]
    public void FeedItemResponse_malformed_input_should_fail_gracefully(string title, string expected)
    {
        var el = XElement.Parse(
           @$"<item>
                    <title>{title}</title>
                    <category>[MP4]</category>
                    <pubDate>Sat, 21 May 2022 20:58:00 +0000</pubDate>
                    <link>http://n.tracktor.site/rssdownloader.php?id=51439</link>
                </item>");
        ReteOrgFeedItemResponse feedItemResponse;
        var ok = ReteOrgFeedItemResponse.TryParseFromXElement(el, out feedItemResponse);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ok, Is.False);
            Assert.That(feedItemResponse, Is.Null);
        }
    }

    [Test]
    public void FeedItemResponse_should_handle_very_long_malformed_input_without_timeout()
    {
        // Test that very long malformed input completes quickly without timeout
        var longTitle = "Very long title that might cause backtracking " + new string('A', 100);
        var el = XElement.Parse(
           @$"<item>
                    <title>{longTitle}</title>
                    <category>[MP4]</category>
                    <pubDate>Sat, 21 May 2022 20:58:00 +0000</pubDate>
                    <link>http://n.tracktor.site/rssdownloader.php?id=51439</link>
                </item>");
        
        var watch = System.Diagnostics.Stopwatch.StartNew();
        ReteOrgFeedItemResponse feedItemResponse;
        var ok = ReteOrgFeedItemResponse.TryParseFromXElement(el, out feedItemResponse);
        watch.Stop();

        // Should complete quickly (well under the 100ms timeout)
        Assert.That(watch.ElapsedMilliseconds, Is.LessThan(50), "Regex should complete quickly on malformed input");
        Assert.That(ok, Is.False);
        Assert.That(feedItemResponse, Is.Null, "Malformed input should result in null feedItemResponse");
    }

    [Test]
    public void FeedItemResponse_should_parse_valid_title_without_regex()
    {
        var el = XElement.Parse(@"
                <item>
                    <title>Братья Харди (The Hardy Boys). Неожиданное возвращение (S02E10) [1080p]</title>
                    <category>[1080p]</category>
                    <pubDate>Mon, 09 May 2022 20:27:53 +0000</pubDate>
                    <link>http://n.tracktor.site/rssdownloader.php?id=51236</link>
                </item>");

        ReteOrgFeedItemResponse feedItemResponse = null;
        Assert.DoesNotThrow(() => ReteOrgFeedItemResponse.TryParseFromXElement(el, out feedItemResponse));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(feedItemResponse.SeriesNameRu, Is.EqualTo("Братья Харди"));
            Assert.That(feedItemResponse.SeriesNameEn, Is.EqualTo("The Hardy Boys"));
            Assert.That(feedItemResponse.EpisodeName, Is.EqualTo("Неожиданное возвращение"));
            Assert.That(feedItemResponse.Quality, Is.EqualTo("1080"));
        }
    }

    [Test]
    public void FeedItemResponse_should_not_throw_on_long_pathological_title()
    {
        var longTitle = "Проблемный сериал (Problem Series). " + new string('A', 5000) + " (S01E01) [BAD]";
        var el = XElement.Parse(
           @$"<item>
                    <title>{longTitle}</title>
                    <category>[MP4]</category>
                    <pubDate>Sat, 21 May 2022 20:58:00 +0000</pubDate>
                    <link>http://n.tracktor.site/rssdownloader.php?id=51439</link>
                </item>");

        Assert.DoesNotThrow(() => ReteOrgFeedItemResponse.TryParseFromXElement(el, out _));
    }

    [Test]
    public void FeedItemResponse_should_parse_real_rss_feed_items_without_timeout()
    {
        var feedXml = Helper.GetEmbeddedResource("LostFilmTV.Client.Tests.TestData.ReteOrgFeed.xml");
        var document = XDocument.Parse(feedXml);
        var itemElements = document.Descendants("item").ToArray();

        Assert.That(itemElements, Is.Not.Empty, "Test data feed should contain items");
        foreach (var element in itemElements)
        {
            Assert.DoesNotThrow(
                () => ReteOrgFeedItemResponse.TryParseFromXElement(element, out _),
                $"Item title caused a timeout: {element.Element("title")?.Value}");
        }
    }
}
