namespace LostFilmMonitoring.DAO.Azure;

/// <summary>
/// Mapper.
/// </summary>
internal static class Mapper
{
    /// <summary>
    /// Map <see cref="SubscriptionTableEntity"/> to <see cref="Subscription"/>.
    /// </summary>
    /// <param name="e">Instance of <see cref="SubscriptionTableEntity"/>.</param>
    /// <returns>Instance of <see cref="Subscription"/>.</returns>
    internal static Subscription Map(SubscriptionTableEntity e)
        => new (e.PartitionKey, e.Quality);

    /// <summary>
    /// Map <see cref="Subscription"/> to <see cref="SubscriptionTableEntity"/>.
    /// </summary>
    /// <param name="subscription">Instance of <see cref="Subscription"/>.</param>
    /// <param name="userId">User Id.</param>
    /// <returns>Instance of <see cref="SubscriptionTableEntity"/>.</returns>
    internal static SubscriptionTableEntity Map(Subscription subscription, string userId)
    {
        return new SubscriptionTableEntity
        {
            PartitionKey = subscription.SeriesName,
            RowKey = userId,
            Timestamp = DateTime.UtcNow,
            Quality = subscription.Quality,
        };
    }

    /// <summary>
    /// Map <see cref="Series"/> to <see cref="SeriesTableEntity"/>.
    /// </summary>
    /// <param name="series">Instance of <see cref="Series"/>.</param>
    /// <returns>Instance of <see cref="SeriesTableEntity"/>.</returns>
    internal static SeriesTableEntity Map(Series series)
    {
        var result = new SeriesTableEntity
        {
            PartitionKey = series.Name,
            RowKey = series.Name,
            Id = series.Id == Guid.Empty ? Guid.NewGuid() : series.Id,
            Timestamp = DateTime.UtcNow,
            LastEpisode = series.LastEpisode,
            LastEpisodeName = series.LastEpisodeName,
            LastEpisodeTorrentLink1080 = series.LastEpisodeTorrentLink1080,
            LastEpisodeTorrentLinkMP4 = series.LastEpisodeTorrentLinkMP4,
            LastEpisodeTorrentLinkSD = series.LastEpisodeTorrentLinkSD,
            SeasonNumber1080 = series.Q1080SeasonNumber,
            SeasonNumberMP4 = series.QMP4SeasonNumber,
            SeasonNumberSD = series.QSDSeasonNumber,
            EpisodeNumber1080 = series.Q1080EpisodeNumber,
            EpisodeNumberMP4 = series.QMP4EpisodeNumber,
            EpisodeNumberSD = series.QSDEpisodeNumber,
            Name = series.Name,
        };

        return result;
    }

    /// <summary>
    /// Map <see cref="Episode"/> to <see cref="EpisodeTableEntity"/>.
    /// </summary>
    /// <param name="episode">Instance of <see cref="Episode"/>.</param>
    /// <returns>Instance of <see cref="EpisodeTableEntity"/>.</returns>
    internal static EpisodeTableEntity Map(Episode episode)
    {
        return new ()
        {
            PartitionKey = episode.SeriesName,
            RowKey = episode.TorrentId,
            Timestamp = DateTime.UtcNow,
            EpisodeName = episode.EpisodeName,
            SeasonNumber = episode.SeasonNumber,
            EpisodeNumber = episode.EpisodeNumber,
            Quality = episode.Quality,
        };
    }

    /// <summary>
    /// Map <see cref="EpisodeTableEntity"/> to <see cref="Episode"/>.
    /// </summary>
    /// <param name="entity">Instance of <see cref="EpisodeTableEntity"/>.</param>
    /// <returns>Instance of <see cref="Episode"/>.</returns>
    internal static Episode Map(EpisodeTableEntity entity)
    {
        return new (
            seriesName: entity.PartitionKey,
            episodeName: entity.EpisodeName,
            seasonNumber: entity.SeasonNumber,
            episodeNumber: entity.EpisodeNumber,
            torrentId: entity.RowKey,
            quality: entity.Quality
        );
    }

    /// <summary>
    /// Map <see cref="SeriesTableEntity"/> to <see cref="Series"/>.
    /// </summary>
    /// <param name="entity">Instance of <see cref="SeriesTableEntity"/>.</param>
    /// <returns>Instance of <see cref="Series"/>.</returns>
    internal static Series Map(SeriesTableEntity entity)
        => new (
            entity.Id,
            entity.Name,
            entity.LastEpisode,
            entity.LastEpisodeName,
            entity.LastEpisodeTorrentLinkSD,
            entity.LastEpisodeTorrentLinkMP4,
            entity.LastEpisodeTorrentLink1080,
            entity.SeasonNumber1080,
            entity.SeasonNumberMP4,
            entity.SeasonNumberSD,
            entity.EpisodeNumber1080,
            entity.EpisodeNumberMP4,
            entity.EpisodeNumberSD);

    /// <summary>
    /// Map <see cref="UserTableEntity"/> to <see cref="User"/>.
    /// </summary>
    /// <param name="entity">Instance of <see cref="UserTableEntity"/>.</param>
    /// <returns>Instance of <see cref="User"/>.</returns>
    internal static User Map(UserTableEntity entity)
        => new (
            id: entity.RowKey,
            trackerId: entity.TrackerId,
            createdAt: entity.Timestamp.HasValue ? entity.Timestamp.Value.DateTime : DateTime.MinValue);

    /// <summary>
    /// Map <see cref="User"/> to <see cref="UserTableEntity"/>.
    /// </summary>
    /// <param name="user">Instance of <see cref="User"/>.</param>
    /// <returns>Instance of <see cref="UserTableEntity"/>.</returns>
    internal static UserTableEntity Map(User user)
    {
        return new UserTableEntity
        {
            PartitionKey = user.Id,
            RowKey = user.Id,
            Timestamp = user.CreatedAt ?? DateTime.UtcNow,
            TrackerId = user.TrackerId,
        };
    }
}
