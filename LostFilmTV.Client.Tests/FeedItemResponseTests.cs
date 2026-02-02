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
                    <title>Title</title>
                    <category>[MP4]</category>
                    <pubDate>Sat, 21 May 2022 20:58:00 +0000</pubDate>
                    <link>{link}</link>
                </item>");
        ReteOrgFeedItemResponse feedItemResponse = new ReteOrgFeedItemResponse(el);
        Assert.That(feedItemResponse.TorrentId, Is.EqualTo(expected));
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
        ReteOrgFeedItemResponse feedItemResponse = new ReteOrgFeedItemResponse(el);
        Assert.That(feedItemResponse.SeriesName, Is.EqualTo(expected));
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
        ReteOrgFeedItemResponse feedItemResponse = new ReteOrgFeedItemResponse(el);
        Assert.That(feedItemResponse.EpisodeName, Is.EqualTo(expected));
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
        ReteOrgFeedItemResponse feedItemResponse = new ReteOrgFeedItemResponse(el);
        Assert.That(feedItemResponse.Quality, Is.EqualTo(expected));
    }

    [Test]
    public void FeedItemResponse_ToString_should_return_string()
    {
        var title ="Внешние сферы (Outer Range). Неведомое (S01E07) (MP4)";
        var el = XElement.Parse(
           @$"<item>
                    <title>{title}</title>
                    <category>[MP4]</category>
                    <pubDate>Sat, 21 May 2022 20:58:00 +0000</pubDate>
                    <link>http://n.tracktor.site/rssdownloader.php?id=51439</link>
                </item>");
        ReteOrgFeedItemResponse feedItemResponse = new ReteOrgFeedItemResponse(el);
        Assert.That(feedItemResponse.ToString(), Is.EqualTo(title));
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
        var item = new ReteOrgFeedItemResponse(element);
        using (Assert.EnterMultipleScope())
        {
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
}
