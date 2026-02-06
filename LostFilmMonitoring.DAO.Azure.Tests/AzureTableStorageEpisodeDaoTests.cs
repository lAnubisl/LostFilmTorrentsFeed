namespace LostFilmMonitoring.DAO.Azure.Tests;

[ExcludeFromCodeCoverage]
public class AzureTableStorageEpisodeDaoTests : AzureTableStorageDaoTestsBase<AzureTableStorageEpisodeDao>
{
    [Test]
    public async Task SaveAsync_should_save_episode()
    {
        var episode = new Episode("SeriesName", "EpisodeName", 1, 1, "123", "SD");
        await GetDao().SaveAsync(episode);
        tableClient!.Verify(x => x.UpsertEntityAsync(
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
        tableClient!.Setup(x => x.UpsertEntityAsync(It.IsAny<EpisodeTableEntity>(), TableUpdateMode.Merge, default))
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

        tableClient!.Setup(x => x.QueryAsync(It.IsAny<Expression<Func<EpisodeTableEntity, bool>>>(), null, null, default)).Returns(new TestAsyncPageable<EpisodeTableEntity>([]));    
        await GetDao().ExistsAsync(seriesName, seasonNumber, episodeNumber, quality);
        tableClient.Verify(x => x.QueryAsync(It.IsAny<Expression<Func<EpisodeTableEntity, bool>>>(), null, null, default), Times.Once);
    }
    
    protected override AzureTableStorageEpisodeDao GetDao()
        => new(this.serviceClient!.Object, this.logger!.Object);
}
