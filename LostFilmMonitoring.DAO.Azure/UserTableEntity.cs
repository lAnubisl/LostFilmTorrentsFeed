namespace LostFilmMonitoring.DAO.Azure;

/// <summary>
/// Describes user in Azure Table Storage.
/// </summary>
public class UserTableEntity : ITableEntity
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
    /// Gets or sets user identifier which should be in link of torrent file announces.
    /// <![CDATA[http://bt.tracktor.in/tracker.php/1b07a52cb12a12945e15cca756f83789/announce
    ///          http://bt99.tracktor.in/tracker.php/1b07a52cb12a12945e15cca756f83789/announce
    ///          http://bt0.tracktor.in/tracker.php/1b07a52cb12a12945e15cca756f83789/announce
    ///          http://user5.newtrack.info/tracker.php/1b07a52cb12a12945e15cca756f83789/announce
    ///          http://user1.newtrack.info/tracker.php/1b07a52cb12a12945e15cca756f83789/announce]]>
    /// Here '1b07a52cb12a12945e15cca756f83789' is the user id.
    /// </summary>
    public string TrackerId { get; set; } = null!;
}