﻿// <copyright file="UpdateFeedsCommandTests.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.BLL.Tests.Commands
{
    public class UpdateFeedsCommandTests
    {
        private const string BaseUid = "baseUid";
        private const string BaseUsess = "baseUsess";
        private Mock<IDAL> dal;
        private Mock<IConfiguration> configuration;
        private Mock<IModelPersister> persister;
        private Mock<ILogger> logger;
        private Mock<IRssFeed> rssFeed;
        private Mock<ILostFilmClient> lostFilmClient;
        private Mock<ISeriesDAO> seriesDAO;
        private Mock<IFeedDAO> feedDAO;
        private Mock<ITorrentFileDAO> torrentFileDAO;
        private Mock<ISubscriptionDAO> subscriptionDAO;
        private Mock<IUserDAO> userDao;
        private Mock<IEpisodeDAO> episodeDao;

        private UpdateFeedsCommand CreateCommand()
        {
            return new UpdateFeedsCommand(
                this.logger.Object,
                this.rssFeed.Object,
                this.dal.Object,
                this.configuration.Object,
                this.persister.Object,
                this.lostFilmClient.Object);
        }

        [SetUp]
        public void Setup()
        {
            this.dal = new();
            this.seriesDAO = new();
            this.feedDAO = new();
            this.episodeDao = new();
            this.userDao = new();
            this.torrentFileDAO = new();
            this.subscriptionDAO = new();
            this.dal.Setup(x => x.Series).Returns(this.seriesDAO.Object);
            this.dal.Setup(x => x.Episode).Returns(this.episodeDao.Object);
            this.dal.Setup(x => x.Feed).Returns(this.feedDAO.Object);
            this.dal.Setup(x => x.TorrentFile).Returns(this.torrentFileDAO.Object);
            this.dal.Setup(x => x.Subscription).Returns(this.subscriptionDAO.Object);
            this.dal.Setup(x => x.User).Returns(this.userDao.Object);
            this.configuration = new();
            this.lostFilmClient = new();
            this.persister = new();
            this.logger = new();
            this.rssFeed = new();
            this.logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(this.logger.Object);
            this.configuration.Setup(x => x.BaseUID).Returns(BaseUid);
            this.configuration.Setup(x => x.BaseUSESS).Returns(BaseUsess);
        }

        [Test]
        public void Constructor_should_throw_exception_when_logger_null()
        {
            var action = () => new UpdateFeedsCommand(
                null!,
                this.rssFeed.Object,
                this.dal.Object,
                this.configuration.Object,
                this.persister.Object,
                this.lostFilmClient.Object);
            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("logger");
        }

        [Test]
        public void Constructor_should_throw_exception_when_rssFeed_null()
        {
            var action = () => new UpdateFeedsCommand(
                this.logger.Object,
                null!,
                this.dal.Object,
                this.configuration.Object,
                this.persister.Object,
                this.lostFilmClient.Object);
            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("rssFeed");
        }

        [Test]
        public void Constructor_should_throw_exception_when_dal_null()
        {
            var action = () => new UpdateFeedsCommand(
                this.logger.Object,
                this.rssFeed.Object,
                null!,
                this.configuration.Object,
                this.persister.Object,
                this.lostFilmClient.Object);
            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("dal");
        }

        [Test]
        public void Constructor_should_throw_exception_when_configuration_null()
        {
            var action = () => new UpdateFeedsCommand(
                this.logger.Object,
                this.rssFeed.Object,
                this.dal.Object,
                null!,
                this.persister.Object,
                this.lostFilmClient.Object);
            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("configuration");
        }

        [Test]
        public void Constructor_should_throw_exception_when_persister_null()
        {
            var action = () => new UpdateFeedsCommand(
                this.logger.Object,
                this.rssFeed.Object,
                this.dal.Object,
                this.configuration.Object,
                null!,
                this.lostFilmClient.Object);
            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("modelPersister");
        }

        [Test]
        public void Constructor_should_throw_exception_when_lostfilmclient_null()
        {
            var action = () => new UpdateFeedsCommand(
                this.logger.Object,
                this.rssFeed.Object,
                this.dal.Object,
                this.configuration.Object,
                this.persister.Object,
                null!);
            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("client");
        }

        [Test]
        public async Task ExecuteAsync_should_not_do_anything_if_no_updates()
        {
            var command = CreateCommand();
            var storedItems = new SortedSet<FeedItemResponse>() {
                new FeedItemResponse()
                {
                    Title = "item 1"
                },
                new FeedItemResponse()
                {
                    Title = "item 2"
                }
            };

            // rssFeed and persister responds with identical set of items.
            this.rssFeed.Setup(x => x.LoadFeedItemsAsync()).ReturnsAsync(storedItems);
            this.persister.Setup(x => x.LoadAsync<SortedSet<FeedItemResponse>>("ReteOrgItems")).ReturnsAsync(storedItems);

            await command.ExecuteAsync();

            this.logger.Verify(x => x.Info("No updates."), Times.Once);
            this.seriesDAO.Verify(x => x.LoadAsync(), Times.Never);
        }

        [Test]
        public async Task ExecuteAsync_should_not_do_anything_if_no_items()
        {
            var command = CreateCommand();
            var storedItems = new SortedSet<FeedItemResponse>() { new FeedItemResponse() { Title = "item 1" } };
            var rssItems = new SortedSet<FeedItemResponse>() { new FeedItemResponse() { Title = "Флэш (The Flash). (S08E999) [MP4]" } };
            this.persister.Setup(x => x.LoadAsync<SortedSet<FeedItemResponse>>("ReteOrgItems")).ReturnsAsync(storedItems);

            // rssFeed does have only season item
            this.rssFeed.Setup(x => x.LoadFeedItemsAsync()).ReturnsAsync(rssItems);

            await command.ExecuteAsync();

            this.lostFilmClient.Verify(x => x.DownloadTorrentFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ExecuteAsync_should_save_new_series()
        {
            var command = CreateCommand();
            this.persister.Setup(x => x.LoadAsync<SortedSet<FeedItemResponse>>("ReteOrgItems")).ReturnsAsync(new SortedSet<FeedItemResponse>());

            var rssItems = new SortedSet<FeedItemResponse>() 
            { 
                new FeedItemResponse(XElement.Parse(
                @"<item>
                    <title>Флэш (The Flash). Падение смерти (S08E13) [MP4]</title>
                    <category>[MP4]</category>
                    <pubDate>Sat, 21 May 2022 20:58:00 +0000</pubDate>
                    <link>http://n.tracktor.site/rssdownloader.php?id=51439</link>
                </item>")),
                new FeedItemResponse(XElement.Parse(
                @"<item>
                    <title>Флэш (The Flash). Падение смерти (S08E13) [1080p]</title>
                    <category>[MP4]</category>
                    <pubDate>Sat, 21 May 2022 20:58:00 +0000</pubDate>
                    <link>http://n.tracktor.site/rssdownloader.php?id=51438</link>
                </item>")),
                new FeedItemResponse(XElement.Parse(
                @"<item>
                    <title>Флэш (The Flash). Падение смерти (S08E13) [SD]</title>
                    <category>[MP4]</category>
                    <pubDate>Sat, 21 May 2022 20:58:00 +0000</pubDate>
                    <link>http://n.tracktor.site/rssdownloader.php?id=51437</link>
                </item>")),
            };

            // RSS feed returns new series
            this.rssFeed.Setup(x => x.LoadFeedItemsAsync()).ReturnsAsync(rssItems);

            // There is no such series in the system
            this.seriesDAO.Setup(x => x.LoadAsync()).ReturnsAsync(Array.Empty<Series>());

            var torrentFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LostFilmMonitoring.BLL.Tests.51439.torrent");
            this.lostFilmClient.Setup(x => x.DownloadTorrentFileAsync(BaseUid, BaseUsess, "51439")).ReturnsAsync(new TorrentFileResponse("Флэш (The Flash). Падение смерти (S08E13) [MP4].torrent", torrentFileStream));
            this.lostFilmClient.Setup(x => x.DownloadTorrentFileAsync(BaseUid, BaseUsess, "51438")).ReturnsAsync(new TorrentFileResponse("Флэш (The Flash). Падение смерти (S08E13) [1080].torrent", torrentFileStream));
            this.lostFilmClient.Setup(x => x.DownloadTorrentFileAsync(BaseUid, BaseUsess, "51437")).ReturnsAsync(new TorrentFileResponse("Флэш (The Flash). Падение смерти (S08E13) [SD].torrent", torrentFileStream));

            await command.ExecuteAsync();

            // Verify series is saved
            this.seriesDAO.Verify(x => x.SaveAsync(It.Is<Series>(x => 
                x.Name == "Флэш (The Flash)"
             && x.LastEpisodeName == "Флэш (The Flash). Падение смерти (S08E13) "
             && x.LastEpisode == new DateTime(2022, 5, 21, 20, 58, 00, DateTimeKind.Utc)
             && x.LastEpisodeTorrentLink1080 == "http://n.tracktor.site/rssdownloader.php?id=51438"
             && x.LastEpisodeTorrentLinkMP4 == "http://n.tracktor.site/rssdownloader.php?id=51439"
             && x.LastEpisodeTorrentLinkSD == "http://n.tracktor.site/rssdownloader.php?id=51437")));
        }

        [Test]
        public async Task ExecuteAsync_should_update_existing_series()
        {
            var command = CreateCommand();
            this.persister.Setup(x => x.LoadAsync<SortedSet<FeedItemResponse>>("ReteOrgItems")).ReturnsAsync(new SortedSet<FeedItemResponse>());

            var rssItems = new SortedSet<FeedItemResponse>()
            {
                new FeedItemResponse(XElement.Parse(
                @"<item>
                    <title>Флэш (The Flash). Падение смерти (S08E13) [MP4]</title>
                    <category>[MP4]</category>
                    <pubDate>Sat, 21 May 2022 20:58:00 +0000</pubDate>
                    <link>http://n.tracktor.site/rssdownloader.php?id=51439</link>
                </item>")),
                new FeedItemResponse(XElement.Parse(
                @"<item>
                    <title>Флэш (The Flash). Падение смерти (S08E13) [1080p]</title>
                    <category>[MP4]</category>
                    <pubDate>Sat, 21 May 2022 20:58:00 +0000</pubDate>
                    <link>http://n.tracktor.site/rssdownloader.php?id=51438</link>
                </item>")),
                new FeedItemResponse(XElement.Parse(
                @"<item>
                    <title>Флэш (The Flash). Падение смерти (S08E13) [SD]</title>
                    <category>[MP4]</category>
                    <pubDate>Sat, 21 May 2022 20:58:00 +0000</pubDate>
                    <link>http://n.tracktor.site/rssdownloader.php?id=51437</link>
                </item>")),
            };

            // RSS feed returns new series
            this.rssFeed.Setup(x => x.LoadFeedItemsAsync()).ReturnsAsync(rssItems);

            // There is an old series in the system
            this.seriesDAO.Setup(x => x.LoadAsync()).ReturnsAsync(new Series[] {
                new Series(
                    "Флэш (The Flash)",
                    new DateTime(2022, 1, 1, 1, 1, 1, DateTimeKind.Utc),
                    "Флэш (The Flash). Предыдущая серия (S08E12) ",
                    "http://n.tracktor.site/rssdownloader.php?id=51436",
                    "http://n.tracktor.site/rssdownloader.php?id=51435",
                    "http://n.tracktor.site/rssdownloader.php?id=51434")
            });

            var torrentFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LostFilmMonitoring.BLL.Tests.51439.torrent");
            this.lostFilmClient.Setup(x => x.DownloadTorrentFileAsync(BaseUid, BaseUsess, "51439")).ReturnsAsync(new TorrentFileResponse("Флэш (The Flash). Падение смерти (S08E13) [MP4].torrent", torrentFileStream));
            this.lostFilmClient.Setup(x => x.DownloadTorrentFileAsync(BaseUid, BaseUsess, "51438")).ReturnsAsync(new TorrentFileResponse("Флэш (The Flash). Падение смерти (S08E13) [1080].torrent", torrentFileStream));
            this.lostFilmClient.Setup(x => x.DownloadTorrentFileAsync(BaseUid, BaseUsess, "51437")).ReturnsAsync(new TorrentFileResponse("Флэш (The Flash). Падение смерти (S08E13) [SD].torrent", torrentFileStream));

            await command.ExecuteAsync();

            // Verify series is saved
            this.seriesDAO.Verify(x => x.SaveAsync(It.Is<Series>(x =>
                x.Name == "Флэш (The Flash)"
             && x.LastEpisodeName == "Флэш (The Flash). Падение смерти (S08E13) "
             && x.LastEpisode == new DateTime(2022, 5, 21, 20, 58, 00, DateTimeKind.Utc)
             && x.LastEpisodeTorrentLink1080 == "http://n.tracktor.site/rssdownloader.php?id=51438"
             && x.LastEpisodeTorrentLinkMP4 == "http://n.tracktor.site/rssdownloader.php?id=51439"
             && x.LastEpisodeTorrentLinkSD == "http://n.tracktor.site/rssdownloader.php?id=51437")));
        }

        [Test]
        public async Task ExecuteAsync_should_not_add_series_when_torrent_download_failed()
        {
            var command = CreateCommand();
            this.persister.Setup(x => x.LoadAsync<SortedSet<FeedItemResponse>>("ReteOrgItems")).ReturnsAsync(new SortedSet<FeedItemResponse>());

            var rssItems = new SortedSet<FeedItemResponse>()
            {
                new FeedItemResponse(XElement.Parse(
                @"<item>
                    <title>Флэш (The Flash). Падение смерти (S08E13) [MP4]</title>
                    <category>[MP4]</category>
                    <pubDate>Sat, 21 May 2022 20:58:00 +0000</pubDate>
                    <link>http://n.tracktor.site/rssdownloader.php?id=51439</link>
                </item>"))
            };

            // RSS feed returns new series
            this.rssFeed.Setup(x => x.LoadFeedItemsAsync()).ReturnsAsync(rssItems);

            // There is no such series in the system
            this.seriesDAO.Setup(x => x.LoadAsync()).ReturnsAsync(Array.Empty<Series>);

            this.lostFilmClient.Setup(x => x.DownloadTorrentFileAsync(BaseUid, BaseUsess, "51439")).ReturnsAsync(null as TorrentFileResponse);

            await command.ExecuteAsync();

            // Verify series is saved
            this.seriesDAO.Verify(x => x.SaveAsync(It.IsAny<Series>()), Times.Never);
        }

        [Test]
        public async Task ExecuteAsync_should_update_user_feed()
        {
            var command = CreateCommand();
            this.persister.Setup(x => x.LoadAsync<SortedSet<FeedItemResponse>>("ReteOrgItems")).ReturnsAsync(new SortedSet<FeedItemResponse>());

            var rssItems = new SortedSet<FeedItemResponse>()
            {
                new FeedItemResponse(XElement.Parse(
                @"<item>
                    <title>Флэш (The Flash). Падение смерти (S08E13) [MP4]</title>
                    <category>[MP4]</category>
                    <pubDate>Sat, 21 May 2022 20:58:00 +0000</pubDate>
                    <link>http://n.tracktor.site/rssdownloader.php?id=51439</link>
                </item>")),
            };

            // RSS feed returns new series
            this.rssFeed.Setup(x => x.LoadFeedItemsAsync()).ReturnsAsync(rssItems);

            // There is no such series in the system
            this.seriesDAO.Setup(x => x.LoadAsync()).ReturnsAsync(Array.Empty<Series>());

            var torrentFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LostFilmMonitoring.BLL.Tests.51439.torrent");
            this.lostFilmClient.Setup(x => x.DownloadTorrentFileAsync(BaseUid, BaseUsess, "51439")).ReturnsAsync(new TorrentFileResponse("Флэш (The Flash). Падение смерти (S08E13) [MP4].torrent", torrentFileStream));

            this.subscriptionDAO.Setup(x => x.LoadUsersIdsAsync("Флэш (The Flash)", Quality.H720)).ReturnsAsync(new[] { "User#1" });
            this.userDao.Setup(x => x.LoadAsync("User#1")).ReturnsAsync(new User("User#1", "Tracker#1"));
            this.configuration.Setup(x => x.GetTorrentAnnounceList("Tracker#1")).Returns(new string[] { "Announce#1" });

            await command.ExecuteAsync();

            this.torrentFileDAO.Verify(x => x.SaveUserFileAsync("User#1", It.IsAny<TorrentFile>()));
        }

        [Test]
        public async Task ExecuteAsync_should_not_update_feed_when_user_not_found()
        {
            var command = CreateCommand();
            this.persister.Setup(x => x.LoadAsync<SortedSet<FeedItemResponse>>("ReteOrgItems")).ReturnsAsync(new SortedSet<FeedItemResponse>());

            var rssItems = new SortedSet<FeedItemResponse>()
            {
                new FeedItemResponse(XElement.Parse(
                @"<item>
                    <title>Флэш (The Flash). Падение смерти (S08E13) [MP4]</title>
                    <category>[MP4]</category>
                    <pubDate>Sat, 21 May 2022 20:58:00 +0000</pubDate>
                    <link>http://n.tracktor.site/rssdownloader.php?id=51439</link>
                </item>")),
            };

            // RSS feed returns new series
            this.rssFeed.Setup(x => x.LoadFeedItemsAsync()).ReturnsAsync(rssItems);

            // There is no such series in the system
            this.seriesDAO.Setup(x => x.LoadAsync()).ReturnsAsync(Array.Empty<Series>());

            var torrentFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LostFilmMonitoring.BLL.Tests.51439.torrent");
            this.lostFilmClient.Setup(x => x.DownloadTorrentFileAsync(BaseUid, BaseUsess, "51439")).ReturnsAsync(new TorrentFileResponse("Флэш (The Flash). Падение смерти (S08E13) [MP4].torrent", torrentFileStream));
            this.subscriptionDAO.Setup(x => x.LoadUsersIdsAsync("Флэш (The Flash)", Quality.H720)).ReturnsAsync(new[] { "User#1" });
            this.userDao.Setup(x => x.LoadAsync("User#1")).ReturnsAsync(null as User);

            await command.ExecuteAsync();

            this.logger.Verify(x => x.Error("User 'User#1' not found."));
            this.torrentFileDAO.Verify(x => x.SaveUserFileAsync(It.IsAny<string>(), It.IsAny<TorrentFile>()), Times.Never);
            this.feedDAO.Verify(x => x.SaveUserFeedAsync(It.IsAny<string>(), It.IsAny<FeedItem[]>()), Times.Never);
        }

        [Test]
        public async Task ExecuteAsync_should_not_update_feed_when_series_not_updated()
        {
            var command = CreateCommand();
            this.persister.Setup(x => x.LoadAsync<SortedSet<FeedItemResponse>>("ReteOrgItems")).ReturnsAsync(new SortedSet<FeedItemResponse>());

            var rssItems = new SortedSet<FeedItemResponse>()
            {
                new FeedItemResponse(XElement.Parse(
                @"<item>
                    <title>Флэш (The Flash). Падение смерти (S08E13) [MP4]</title>
                    <category>[MP4]</category>
                    <pubDate>Sat, 21 May 2022 20:58:00 +0000</pubDate>
                    <link>http://n.tracktor.site/rssdownloader.php?id=51439</link>
                </item>")),
            };

            // RSS feed returns new series
            this.rssFeed.Setup(x => x.LoadFeedItemsAsync()).ReturnsAsync(rssItems);

            // There is such series in the system already
            this.seriesDAO.Setup(x => x.LoadAsync()).ReturnsAsync(new[]
            {
                new Series(
                    "Флэш (The Flash)",
                    new DateTime(2022, 5, 21, 22, 58, 0, DateTimeKind.Utc),
                    "Флэш (The Flash). Падение смерти (S08E13) ",
                    null,
                    "http://n.tracktor.site/rssdownloader.php?id=51439",
                    null,
                    8, 8, 8,
                    13, 13, 13)
            });

            await command.ExecuteAsync();
            this.lostFilmClient.Verify(x => x.DownloadTorrentFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            this.seriesDAO.Verify(x => x.SaveAsync(It.IsAny<Series>()), Times.Never);
        }
    }
}