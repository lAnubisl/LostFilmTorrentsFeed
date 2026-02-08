namespace LostFilmMonitoring.DAO.Azure.TableStorageEntity;

/// <summary>
/// Describes Series in Azure Table Storage.
/// </summary>
public class SeriesTableEntity : ITableEntity
{
    /// <inheritdoc/>
    public string PartitionKey { get; set; } = null!;

    /// <inheritdoc/>
    public string RowKey { get; set; } = null!;

    /// <inheritdoc/>
    public DateTimeOffset? Timestamp { get; set; }

    /// <inheritdoc/>
    public ETag ETag { get; set; }

    /// <summary>
    /// Gets or sets Name.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets LastEpisode Date.
    /// </summary>
    public DateTime LastEpisode { get; set; }

    /// <summary>
    /// Gets or sets Last episode name.
    /// </summary>
    public string LastEpisodeName { get; set; } = null!;

    /// <summary>
    /// Gets or sets LastEpisodeTorrentLinkSD.
    /// </summary>
    public string? LastEpisodeTorrentLinkSD { get; set; }

    /// <summary>
    /// Gets or sets LastEpisodeTorrentLinkMP4.
    /// </summary>
    public string? LastEpisodeTorrentLinkMP4 { get; set; }

    /// <summary>
    /// Gets or sets LastEpisodeTorrentLink1080.
    /// </summary>
    public string? LastEpisodeTorrentLink1080 { get; set; }

    /// <summary>
    /// Gets or sets Season number for 1080 quality.
    /// </summary>
    public int? SeasonNumber1080 { get; set; }

    /// <summary>
    /// Gets or sets Season number for MP4 quality.
    /// </summary>
    public int? SeasonNumberMP4 { get; set; }

    /// <summary>
    /// Gets or sets Season number for SD quality.
    /// </summary>
    public int? SeasonNumberSD { get; set; }

    /// <summary>
    /// Gets or sets Episode number for 1080 quality.
    /// </summary>
    public int? EpisodeNumber1080 { get; set; }

    /// <summary>
    /// Gets or sets Episode number for MP4 quality.
    /// </summary>
    public int? EpisodeNumberMP4 { get; set; }

    /// <summary>
    /// Gets or sets Episode number for SD quality.
    /// </summary>
    public int? EpisodeNumberSD { get; set; }

    /// <summary>
    /// Gets or sets Id.
    /// </summary>
    public Guid Id { get; set; }
}
