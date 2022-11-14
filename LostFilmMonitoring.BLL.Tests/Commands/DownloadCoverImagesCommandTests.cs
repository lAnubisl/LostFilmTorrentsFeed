// <copyright file="DownloadCoverImagesCommandTests.cs" company="Alexander Panfilenok">
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

using LostFilmMonitoring.DAO.Interfaces;

namespace LostFilmMonitoring.BLL.Tests.Commands
{
    [ExcludeFromCodeCoverage]
    public class DownloadCoverImagesCommandTests
    {
        private Mock<ISeriesDao> seriesDao;
        private Mock<ILogger> logger;
        private Mock<IFileSystem> fileSystem;
        private Mock<IConfiguration> configuration;
        private Mock<ILostFilmClient> lostFilmClient;
        private Mock<IRssFeed> feed;

        [SetUp]
        public void Setup()
        {
            this.fileSystem = new();
            this.feed = new();
            this.configuration = new();
            this.lostFilmClient = new();
            this.seriesDao = new();
            this.logger = new();
            this.logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(this.logger.Object);
        }

        [Test]
        public async Task ExecuteAsync_should_do_nothing_when_no_items_in_feed()
        {
            feed.Setup(x => x.LoadFeedItemsAsync()).ReturnsAsync(new SortedSet<FeedItemResponse>());
            await GetService().ExecuteAsync();
            lostFilmClient.Verify(x => x.DownloadImageAsync(It.IsAny<string>()), Times.Never);
            seriesDao.Verify(x => x.LoadAsync(It.IsAny<string>()), Times.Never);
            seriesDao.Verify(x => x.SaveAsync(It.IsAny<Series>()), Times.Never);
            fileSystem.Verify(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            configuration.Verify(x => x.ImagesDirectory, Times.Never);
        }

        [Test]
        public async Task ExecuteAsync_should_do_nothing_when_feed_returned_null()
        {
            feed.Setup(x => x.LoadFeedItemsAsync()).ReturnsAsync(null as SortedSet<FeedItemResponse>);
            await GetService().ExecuteAsync();
            lostFilmClient.Verify(x => x.DownloadImageAsync(It.IsAny<string>()), Times.Never);
            seriesDao.Verify(x => x.LoadAsync(It.IsAny<string>()), Times.Never);
            seriesDao.Verify(x => x.SaveAsync(It.IsAny<Series>()), Times.Never);
            fileSystem.Verify(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            configuration.Verify(x => x.ImagesDirectory, Times.Never);
        }

        [Test]
        public async Task ExecuteAsync_should_do_nothing_when_rss_item_description_is_mailformed()
        {
            feed.Setup(x => x.LoadFeedItemsAsync()).ReturnsAsync(new SortedSet<FeedItemResponse>() { new FeedItemResponse { Description = "malformed" } });
            await GetService().ExecuteAsync();
            lostFilmClient.Verify(x => x.DownloadImageAsync(It.IsAny<string>()), Times.Never);
            seriesDao.Verify(x => x.LoadAsync(It.IsAny<string>()), Times.Never);
            seriesDao.Verify(x => x.SaveAsync(It.IsAny<Series>()), Times.Never);
            fileSystem.Verify(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            configuration.Verify(x => x.ImagesDirectory, Times.Never);
        }

        [Test]
        public async Task ExecuteAsync_should_check_if_image_exists()
        {
            var id = "568";
            var link = $"static.lostfilm.top/Images/{id}/Posters/image.jpg";
            var fileName = $"{id}.jpg";
            var dir = "images";
            feed.Setup(x => x.LoadFeedItemsAsync()).ReturnsAsync(new SortedSet<FeedItemResponse>() { new FeedItemResponse { Description = link } });
            configuration.Setup(x => x.ImagesDirectory).Returns(dir);
            fileSystem.Setup(x => x.ExistsAsync(dir, fileName)).ReturnsAsync(true);
            await GetService().ExecuteAsync();
            fileSystem.Verify(x => x.ExistsAsync(dir, fileName), Times.Once);
            lostFilmClient.Verify(x => x.DownloadImageAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ExecuteAsync_should_download_image_if_it_does_not_exist()
        {
            var id = "568";
            var link = $"static.lostfilm.top/Images/{id}/Posters/image.jpg";
            var fileName = $"{id}.jpg";
            var dir = "images";
            feed.Setup(x => x.LoadFeedItemsAsync()).ReturnsAsync(new SortedSet<FeedItemResponse>() { new FeedItemResponse { Description = link } });
            configuration.Setup(x => x.ImagesDirectory).Returns(dir);
            fileSystem.Setup(x => x.ExistsAsync(dir, fileName)).ReturnsAsync(false);
            await GetService().ExecuteAsync();
            lostFilmClient.Verify(x => x.DownloadImageAsync(id), Times.Once);
        }

        [Test]
        public async Task ExecuteAsync_should_save_downloaded_image()
        {
            var id = "568";
            var link = $"static.lostfilm.top/Images/{id}/Posters/image.jpg";
            var fileName = $"{id}.jpg";
            var dir = "images";
            var stream = new MemoryStream();
            feed.Setup(x => x.LoadFeedItemsAsync()).ReturnsAsync(new SortedSet<FeedItemResponse>() { new FeedItemResponse { Description = link } });
            configuration.Setup(x => x.ImagesDirectory).Returns(dir);
            fileSystem.Setup(x => x.ExistsAsync(dir, fileName)).ReturnsAsync(false);
            lostFilmClient.Setup(x => x.DownloadImageAsync(id)).ReturnsAsync(stream);
            await GetService().ExecuteAsync();
            fileSystem.Verify(x => x.SaveAsync(dir, fileName, "image/jpeg", stream), Times.Once);
        }

        [Test]
        public async Task ExecuteAsync_should_update_series()
        {
            int id = 568;
            var link = $"static.lostfilm.top/Images/{id}/Posters/image.jpg";
            var fileName = $"{id}.jpg";
            var dir = "images";
            var stream = new MemoryStream();
            var seriesName = "seriesName";
            var series = new Series(seriesName, DateTime.UtcNow, null!, null, null, null, null);
            feed.Setup(x => x.LoadFeedItemsAsync()).ReturnsAsync(new SortedSet<FeedItemResponse>() { new FeedItemResponse { Description = link, SeriesName = seriesName } });
            configuration.Setup(x => x.ImagesDirectory).Returns(dir);
            fileSystem.Setup(x => x.ExistsAsync(dir, fileName)).ReturnsAsync(false);
            lostFilmClient.Setup(x => x.DownloadImageAsync(id.ToString())).ReturnsAsync(stream);
            seriesDao.Setup(x => x.LoadAsync(seriesName)).ReturnsAsync(series);
            await GetService().ExecuteAsync();

            seriesDao.Verify(x => x.LoadAsync(seriesName), Times.Once);
            seriesDao.Verify(x => x.SaveAsync(It.Is<Series>(s => s.LostFilmId == id)));
        }

        private DownloadCoverImagesCommand GetService()
            => new(this.logger.Object, this.fileSystem.Object, this.configuration.Object, this.feed.Object, this.seriesDao.Object, this.lostFilmClient.Object);
    }
}
