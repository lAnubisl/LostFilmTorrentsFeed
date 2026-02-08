namespace LostFilmMonitoring.BLL.Tests;

[ExcludeFromCodeCoverage]
public class ExtensionsTests
{
    [Test]
    public void FeedItemResponse_HasUpdates_should_return_false_for_identical_lists()
    {
        var existingItems = new SortedSet<FeedItemResponse>(
            [
                new FeedItemResponse()
                {
                    Title = "RusSeriesTitle (EngSeriesTitle). RusEposideName (S01E01) [MP4]",
                    Link = "http://n.tracktor.site/rssdownloader.php?id=51230",
                    PublishDate = "2022-05-09T16:00:54Z",
                    PublishDateParsed = new DateTime(2022, 5, 9, 16, 0, 0, DateTimeKind.Utc),
                },
                new FeedItemResponse()
                {
                    Title = "RusSeriesTitle (EngSeriesTitle). RusEposideName (S01E01) [SD]",
                    Link = "http://n.tracktor.site/rssdownloader.php?id=51230",
                    PublishDate = "2022-05-09T16:00:54Z",
                    PublishDateParsed = new DateTime(2022, 5, 9, 16, 0, 0, DateTimeKind.Utc),
                },
                new FeedItemResponse()
                {
                    Title = "RusSeriesTitle (EngSeriesTitle). RusEposideName (S01E01) [1080p]",
                    Link = "http://n.tracktor.site/rssdownloader.php?id=51230",
                    PublishDate = "2022-05-09T16:00:54Z",
                    PublishDateParsed = new DateTime(2022, 5, 9, 16, 0, 0, DateTimeKind.Utc),
                },
            ]);
        var newItems = new SortedSet<FeedItemResponse>(
            [
                new FeedItemResponse()
                {
                    Title = "RusSeriesTitle (EngSeriesTitle). RusEposideName (S01E01) [MP4]",
                    Link = "http://n.tracktor.site/rssdownloader.php?id=51230",
                    PublishDate = "2022-05-09T16:00:54Z",
                    PublishDateParsed = new DateTime(2022, 5, 9, 16, 0, 0, DateTimeKind.Utc),
                },
                new FeedItemResponse()
                {
                    Title = "RusSeriesTitle (EngSeriesTitle). RusEposideName (S01E01) [SD]",
                    Link = "http://n.tracktor.site/rssdownloader.php?id=51230",
                    PublishDate = "2022-05-09T16:00:54Z",
                    PublishDateParsed = new DateTime(2022, 5, 9, 16, 0, 0, DateTimeKind.Utc),
                },
                new FeedItemResponse()
                {
                    Title = "RusSeriesTitle (EngSeriesTitle). RusEposideName (S01E01) [1080p]",
                    Link = "http://n.tracktor.site/rssdownloader.php?id=51230",
                    PublishDate = "2022-05-09T16:00:54Z",
                    PublishDateParsed = new DateTime(2022, 5, 9, 16, 0, 0, DateTimeKind.Utc),
                },
            ]);

        Extensions.HasUpdates(newItems, existingItems).Should().BeFalse();
    }

    [Test]
    public void FeedItemResponse_HasUpdates_should_return_true_for_lists_of_different_length()
    {
        var existingItems = new SortedSet<FeedItemResponse>(
            [
                new FeedItemResponse()
                {
                    Title = "RusSeriesTitle (EngSeriesTitle). RusEposideName (S01E01) [MP4]",
                    Link = "http://n.tracktor.site/rssdownloader.php?id=51230",
                    PublishDate = "2022-05-09T16:00:54Z",
                    PublishDateParsed = new DateTime(2022, 5, 9, 16, 0, 0, DateTimeKind.Utc),
                },
                new FeedItemResponse()
                {
                    Title = "RusSeriesTitle (EngSeriesTitle). RusEposideName (S01E01) [SD]",
                    Link = "http://n.tracktor.site/rssdownloader.php?id=51230",
                    PublishDate = "2022-05-09T16:00:54Z",
                    PublishDateParsed = new DateTime(2022, 5, 9, 16, 0, 0, DateTimeKind.Utc),
                },
            ]);
        var newItems = new SortedSet<FeedItemResponse>(
            [
                new FeedItemResponse()
                {
                    Title = "RusSeriesTitle (EngSeriesTitle). RusEposideName (S01E01) [MP4]",
                    Link = "http://n.tracktor.site/rssdownloader.php?id=51230",
                    PublishDate = "2022-05-09T16:00:54Z",
                    PublishDateParsed = new DateTime(2022, 5, 9, 16, 0, 0, DateTimeKind.Utc),
                },
                new FeedItemResponse()
                {
                    Title = "RusSeriesTitle (EngSeriesTitle). RusEposideName (S01E01) [SD]",
                    Link = "http://n.tracktor.site/rssdownloader.php?id=51230",
                    PublishDate = "2022-05-09T16:00:54Z",
                    PublishDateParsed = new DateTime(2022, 5, 9, 16, 0, 0, DateTimeKind.Utc),
                },
                new FeedItemResponse()
                {
                    Title = "RusSeriesTitle (EngSeriesTitle). RusEposideName (S01E01) [1080p]",
                    Link = "http://n.tracktor.site/rssdownloader.php?id=51230",
                    PublishDate = "2022-05-09T16:00:54Z",
                    PublishDateParsed = new DateTime(2022, 5, 9, 16, 0, 0, DateTimeKind.Utc),
                },
            ]);

        Extensions.HasUpdates(newItems, existingItems).Should().BeTrue();
    }

    [Test]
    public void FeedItemResponse_HasUpdates_should_return_true_for_different_lists()
    {
        var existingItems = new SortedSet<FeedItemResponse>(
            [
                new FeedItemResponse()
                {
                    Title = "RusSeriesTitle (EngSeriesTitle). RusEposideName (S01E01) [MP4]",
                    Link = "http://n.tracktor.site/rssdownloader.php?id=51230",
                    PublishDate = "2022-05-09T16:00:54Z",
                    PublishDateParsed = new DateTime(2022, 5, 9, 16, 0, 0, DateTimeKind.Utc),
                },
                new FeedItemResponse()
                {
                    Title = "RusSeriesTitle (EngSeriesTitle). RusEposideName (S01E01) [SD]",
                    Link = "http://n.tracktor.site/rssdownloader.php?id=51230",
                    PublishDate = "2022-05-09T16:00:54Z",
                    PublishDateParsed = new DateTime(2022, 5, 9, 16, 0, 0, DateTimeKind.Utc),
                },
                new FeedItemResponse()
                {
                    Title = "RusSeriesTitle (EngSeriesTitle). RusEposideName (S01E01) [1080p]",
                    Link = "http://n.tracktor.site/rssdownloader.php?id=51230",
                    PublishDate = "2022-05-09T16:00:54Z",
                    PublishDateParsed = new DateTime(2022, 5, 9, 16, 0, 0, DateTimeKind.Utc),
                },
            ]);
        var newItems = new SortedSet<FeedItemResponse>(
            [
                new FeedItemResponse()
                {
                    Title = "RusSeriesTitle (EngSeriesTitle). RusEposideName (S01E02) [1080p]",
                    Link = "http://n.tracktor.site/rssdownloader.php?id=51230",
                    PublishDate = "2022-05-09T16:00:54Z",
                    PublishDateParsed = new DateTime(2022, 5, 9, 16, 0, 0, DateTimeKind.Utc),
                },
                new FeedItemResponse()
                {
                    Title = "RusSeriesTitle (EngSeriesTitle). RusEposideName (S01E01) [SD]",
                    Link = "http://n.tracktor.site/rssdownloader.php?id=51230",
                    PublishDate = "2022-05-09T16:00:54Z",
                    PublishDateParsed = new DateTime(2022, 5, 9, 16, 0, 0, DateTimeKind.Utc),
                },
                new FeedItemResponse()
                {
                    Title = "RusSeriesTitle (EngSeriesTitle). RusEposideName (S01E01) [1080p]",
                    Link = "http://n.tracktor.site/rssdownloader.php?id=51230",
                    PublishDate = "2022-05-09T16:00:54Z",
                    PublishDateParsed = new DateTime(2022, 5, 9, 16, 0, 0, DateTimeKind.Utc),
                },
            ]);

        Extensions.HasUpdates(newItems, existingItems).Should().BeTrue();
    }

    [Test]
    public void FeedItemResponse_HasUpdates_should_throw_exception_if_newItems_null()
    {
        SortedSet<FeedItemResponse> oldItems = [];
        SortedSet<FeedItemResponse> newItems = null!;
        Action act = () => Extensions.HasUpdates(oldItems, newItems);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void FeedItemResponse_HasUpdates_should_throw_exception_if_oldItems_null()
    {
        SortedSet<FeedItemResponse> oldItems = null!;
        SortedSet<FeedItemResponse> newItems = [];
        Action act = () => Extensions.HasUpdates(oldItems, newItems);
        act.Should().Throw<ArgumentNullException>();
    }
}   
