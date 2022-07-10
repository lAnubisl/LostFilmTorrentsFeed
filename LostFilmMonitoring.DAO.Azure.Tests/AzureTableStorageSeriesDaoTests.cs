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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using LostFilmMonitoring.DAO.Interfaces.DomainModels;

namespace LostFilmMonitoring.DAO.Azure.Tests
{
    [ExcludeFromCodeCoverage]
    public class AzureTableStorageSeriesDaoTests : AzureTableStorageDaoTestsBase<AzureTableStorageSeriesDao>
    {
        [Test]
        public async Task DeleteAsync_should_do_nothing_when_series_null()
        {
            await GetDao().DeleteAsync(null!);
            tableClient.Verify(x => x.DeleteEntityAsync(It.IsAny<string>(), It.IsAny<string>(), default, default), Times.Never);
        }

        [Test]
        public async Task DeleteAsync_should_delete_series()
        {
            var series = new Series(
                "Name",
                new DateTime(2022, 07, 08, 11, 06, 00, DateTimeKind.Utc),
                "LastEpisodeName",
                "linkSD",
                "linkMP4",
                "link1080p");

            await GetDao().DeleteAsync(series);
            tableClient.Verify(x => x.DeleteEntityAsync(series.Name, series.Name, default, default), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_should_escape_key_when_delete_series()
        {
            var originalName = "Test'o-bug";
            var escapedName = "Test''o-bug";
            var series = new Series(
                originalName,
                new DateTime(2022, 07, 08, 11, 06, 00, DateTimeKind.Utc),
                "LastEpisodeName",
                "linkSD",
                "linkMP4",
                "link1080p");

            await GetDao().DeleteAsync(series);
            tableClient.Verify(x => x.DeleteEntityAsync(escapedName, escapedName, default, default), Times.Once);
        }

        [Test]
        public async Task LoadAsync_name_should_return_null_when_series_not_found()
        {
            tableClient
                .Setup(x => x.GetEntityAsync<SeriesTableEntity>("Name", "Name", null, default))
                .Throws(new RequestFailedException(404, "ResourceNotFound", "ResourceNotFound", null));
            var series = await GetDao().LoadAsync("Name");
            Assert.That(series, Is.Null);
        }

        [Test]
        public async Task LoadAsync_name_should_return_series()
        {
            var entity = new SeriesTableEntity
            {
                Name = "Name"
            };

            tableClient
                .Setup(x => x.GetEntityAsync<SeriesTableEntity>(entity.Name, entity.Name, null, default))
                .ReturnsAsync(new TestResponse<SeriesTableEntity>(entity));
            var loadedSeries = await GetDao().LoadAsync(entity.Name);
            Assert.That(loadedSeries?.Name, Is.EqualTo(entity.Name));
        }

        [Test]
        public async Task TaskAsync_should_return_empty_array()
        {
            tableClient
                .Setup(x => x.QueryAsync<SeriesTableEntity>(
                    null as string,
                    null as int?,
                    null as IEnumerable<string>,
                    default))
                .Throws(new RequestFailedException(404, "ResourceNotFound", "ResourceNotFound", null));
            var result = await GetDao().LoadAsync();
            Assert.That(result, Is.EqualTo(Array.Empty<Series>()));
        }

        [Test]
        public async Task TaskAsync_should_return_records()
        {
            var values = new[]
            { 
                new SeriesTableEntity() { Name = "A" }, 
                new SeriesTableEntity() { Name = "B" } 
            };
            var expected = new TestAsyncPageable<SeriesTableEntity>(values);

            tableClient
                .Setup(x => x.QueryAsync<SeriesTableEntity>(
                    null as string,
                    null as int?,
                    null as IEnumerable<string>,
                    default))
                .Returns(expected);
            
            var result = await GetDao().LoadAsync();
            Assert.That(
                result != null &&
                result.Length == 2 &&
                result.Any(x => x.Name == "A") && 
                result.Any(x => x.Name == "B")
            );
        }

        [Test]
        public async Task SaveAsync_should_save_series()
        {
            var series = new Series(
                "Name",
                new DateTime(2022, 07, 08, 11, 06, 00, DateTimeKind.Utc),
                "LastEpisodeName",
                "linkSD",
                "linkMP4",
                "link1080p");
            await GetDao().SaveAsync(series);
            tableClient.Verify(x => x.UpsertEntityAsync(
                It.Is<SeriesTableEntity>(x => 
                    x.Name == series.Name
                    && x.LastEpisode == series.LastEpisode
                    && x.LastEpisodeName == series.LastEpisodeName
                    && x.LastEpisodeTorrentLink1080 == series.LastEpisodeTorrentLink1080
                    && x.LastEpisodeTorrentLinkMP4 == series.LastEpisodeTorrentLinkMP4
                    && x.LastEpisodeTorrentLinkSD == series.LastEpisodeTorrentLinkSD
                ),
                TableUpdateMode.Merge,
                default), Times.Once);
        }

        protected override AzureTableStorageSeriesDao GetDao()
            => new(serviceClient.Object, new ConsoleLogger("Tests"));
    }
}
