namespace LostFilmMonitoring.DAO.Azure;

/// <summary>
/// Describes User Subscrition in Azure Table Storage.
/// </summary>
public class DictionaryTableEntity : ITableEntity
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
    /// Gets or sets Title.
    /// </summary>
    public string Title { get; set; } = null!;
}