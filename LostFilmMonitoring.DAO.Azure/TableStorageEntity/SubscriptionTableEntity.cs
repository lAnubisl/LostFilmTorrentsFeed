namespace LostFilmMonitoring.DAO.Azure.TableStorageEntity;

/// <summary>
/// Describes User Subscrition in Azure Table Storage.
/// </summary>
public class SubscriptionTableEntity : ITableEntity
{
    /// <summary>
    /// Gets or sets the series name.
    /// example: "The Walking Dead".
    /// </summary>
    public string PartitionKey { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user id.
    /// example: "df1410d2-d23a-4217-a326-c8877e0555c1".
    /// </summary>
    public string RowKey { get; set; } = null!;

    /// <inheritdoc/>
    public DateTimeOffset? Timestamp { get; set; }

    /// <inheritdoc/>
    public ETag ETag { get; set; }

    /// <summary>
    /// Gets or sets Series Quality.
    /// </summary>
    public string Quality { get; set; } = null!;
}
