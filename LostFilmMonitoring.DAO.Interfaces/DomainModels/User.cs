namespace LostFilmMonitoring.DAO.Interfaces.DomainModels;

/// <summary>
/// User.
/// </summary>
public class User
{
    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class.
    /// </summary>
    /// <param name="id">User Id.</param>
    /// <param name="trackerId">Tracker Id.</param>
    public User(string id, string trackerId)
    {
        this.Id = id;
        this.TrackerId = trackerId;
        this.CreatedAt = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class.
    /// </summary>
    /// <param name="id">User Id.</param>
    /// <param name="trackerId">Tracker Id.</param>
    /// <param name="createdAt">Created At.</param>
    public User(string id, string trackerId, DateTime createdAt)
    {
        this.Id = id;
        this.TrackerId = trackerId;
        this.CreatedAt = createdAt;
    }

    /// <summary>
    /// Gets Id.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets user identifier which should be in link of torrent file announces.
    /// <![CDATA[http://bt.tracktor.in/tracker.php/1b07a52cb12a12945e15cca756f83789/announce
    ///          http://bt99.tracktor.in/tracker.php/1b07a52cb12a12945e15cca756f83789/announce
    ///          http://bt0.tracktor.in/tracker.php/1b07a52cb12a12945e15cca756f83789/announce
    ///          http://user5.newtrack.info/tracker.php/1b07a52cb12a12945e15cca756f83789/announce
    ///          http://user1.newtrack.info/tracker.php/1b07a52cb12a12945e15cca756f83789/announce]]>
    /// Here '1b07a52cb12a12945e15cca756f83789' is the user id.
    /// </summary>
    public string TrackerId { get; }

    /// <summary>
    /// Gets CreatedAt.
    /// </summary>
    public DateTime? CreatedAt { get; }

    /// <inheritdoc/>
    public override string ToString() => $"{this.Id}, {this.TrackerId}";

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is User user)
        {
            return this.Id == user.Id && this.TrackerId == user.TrackerId;
        }

        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(this.Id, this.TrackerId);
}
