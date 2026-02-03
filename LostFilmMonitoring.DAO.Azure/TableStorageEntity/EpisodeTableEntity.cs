namespace LostFilmMonitoring.DAO.Azure.TableStorageEntity;

/// <summary>
/// Describes Episode in Azure Table Storage.
/// </summary>
public class EpisodeTableEntity : ITableEntity
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
    /// Gets or sets Episode Name.
    /// </summary>
    public string EpisodeName { get; set; } = null!;

    /// <summary>
    /// Gets or sets Quality.
    /// </summary>
    public string Quality { get; set; } = null!;

    /// <summary>
    /// Gets or sets Season Number.
    /// </summary>
    public int SeasonNumber { get; set; }

    /// <summary>
    /// Gets or sets Episode Number.
    /// </summary>
    public int EpisodeNumber { get; set; }
}