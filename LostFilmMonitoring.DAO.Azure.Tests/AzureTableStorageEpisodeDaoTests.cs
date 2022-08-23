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
        public async Task SaveAsync_should_remove_forbidden_characters_from_primarykey()
        {
            var episode = new Episode("SeriesName?", "EpisodeName", 1, 1, "123", "SD");
            await GetDao().SaveAsync(episode);
            tableClient.Verify(x => x.UpsertEntityAsync(
                It.Is<EpisodeTableEntity>(x =>
                    x.SeasonNumber == episode.SeasonNumber
                    && x.EpisodeNumber == episode.EpisodeNumber
                    && x.EpisodeName == episode.EpisodeName
                    && x.PartitionKey == "SeriesName"
                    && x.RowKey == episode.TorrentId
                    && x.Quality == episode.Quality
                ),
                TableUpdateMode.Merge,
                default), Times.Once);
        }

        protected override AzureTableStorageEpisodeDao GetDao()
            => new(serviceClient.Object, new ConsoleLogger("Tests"));
    }
}
