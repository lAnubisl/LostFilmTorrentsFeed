// <copyright file="ConfigurationTests.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2023 Alexander Panfilenok
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