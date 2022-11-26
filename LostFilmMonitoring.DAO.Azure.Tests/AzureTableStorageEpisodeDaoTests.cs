// <copyright file="AzureTableStorageSeriesDaoTests.cs" company="Alexander Panfilenok">
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

using Azure.Data.Tables;

namespace LostFilmMonitoring.DAO.Azure.Tests
{
    [ExcludeFromCodeCoverage]
    public class AzureTableStorageEpisodeDaoTests : AzureTableStorageDaoTestsBase<AzureTableStorageEpisodeDao>
    {
        [Test]
        public async Task SaveAsync_should_save_episode()
        {
            var episode = new Episode("SeriesName", "EpisodeName", 1, 1, "123", "SD");
            await GetDao().SaveAsync(episode);
            tableClient.Verify(x => x.UpsertEntityAsync(
                It.Is<EpisodeTableEntity>(x =>
                    x.SeasonNumber == episode.SeasonNumber
                    && x.EpisodeNumber == episode.EpisodeNumber
                    && x.EpisodeName == episode.EpisodeName
                    && x.PartitionKey == episode.SeriesName
                    && x.RowKey == episode.TorrentId
                    && x.Quality == episode.Quality
                ),
                TableUpdateMode.Merge,
                default), Times.Once);
        }

        [Test]
        public async Task SaveAsync_throw_ExternalServiceUnavailableException()
        {
            var episode = new Episode("SeriesName", "EpisodeName", 1, 1, "123", "SD");
            tableClient.Setup(x => x.UpsertEntityAsync(It.IsAny<EpisodeTableEntity>(), TableUpdateMode.Merge, default))
                .ThrowsAsync(new RequestFailedException(500, "Internal Server Error"));
            var action = async () => await GetDao().SaveAsync(episode);
            await action.Should().ThrowAsync<ExternalServiceUnavailableException>().WithMessage("Azure Table Storage is not accessible");
        }

        [Test]
        public async Task ExistsAsync_should_check_if_episode_exists()
        {
            var seriesName = "Series#1";
            var seasonNumber = 1;
            var episodeNumber = 1;
            var quality = "SD";

            Expression<Func<EpisodeTableEntity, bool>> expression = entity =>
                    entity.PartitionKey == seriesName &&
                    entity.Quality == quality &&
                    entity.SeasonNumber == seasonNumber &&
                    entity.EpisodeNumber == episodeNumber;

            tableClient.Setup(x => x.QueryAsync(It.IsAny<Expression<Func<EpisodeTableEntity, bool>>>(), null, null, default)).Returns(new TestAsyncPageable<EpisodeTableEntity>(Array.Empty<EpisodeTableEntity>()));
            await GetDao().ExistsAsync(seriesName, seasonNumber, episodeNumber, quality);
            tableClient.Verify(x => x.QueryAsync(It.IsAny<Expression<Func<EpisodeTableEntity, bool>>>(), null, null, default), Times.Once);
        }

        protected override AzureTableStorageEpisodeDao GetDao()
            => new(this.serviceClient.Object, this.logger.Object);
    }
}
