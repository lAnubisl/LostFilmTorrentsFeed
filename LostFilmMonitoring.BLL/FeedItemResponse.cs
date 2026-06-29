namespace LostFilmMonitoring.BLL;

/// <summary>
/// FeedItemResponse interface.
/// </summary>
public class FeedItemResponse : IComparable<FeedItemResponse>
{
    /// <summary>
    /// Gets or sets Series Name (Russian Part only).
    /// </summary>
    public string? SeriesNameRu { get; set; }

    /// <summary>
    /// Gets or sets Series Name (English Part only).
    /// </summary>
    public string? SeriesNameEn { get; set; }

    /// <summary>
    /// Gets or sets Series Name.
    /// </summary>
    public string? SeriesName { get; set; }

    /// <summary>
    /// Gets or sets Episode Name.
    /// </summary>
    public string? EpisodeName { get; set; }

    /// <summary>
    /// Gets or sets Episode Number.
    /// </summary>
    public int? EpisodeNumber { get; set; }

    /// <summary>
    /// Gets or sets Season Number..
    /// </summary>
    public int? SeasonNumber { get; set; }

    /// <summary>
    /// Gets or sets Quality.
    /// </summary>
    public string? Quality { get; set; }

    /// <summary>
    /// Gets or sets Title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets Link.
    /// </summary>
    public string? Link { get; set; }

    /// <summary>
    /// Gets or sets Description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets PublishDateParsed.
    /// </summary>
    public DateTime PublishDateParsed { get; set; }

    /// <summary>
    /// Gets or sets PublishDate.
    /// </summary>
    public string? PublishDate { get; set; }

    /// <summary>
    /// Gets or sets TorrentId.
    /// </summary>
    public string? TorrentId { get; set; }

    /// <inheritdoc/>
    int IComparable<FeedItemResponse>.CompareTo(FeedItemResponse? that)
    {
        if (that == null)
        {
            return 1;
        }

        if (this.PublishDateParsed < that.PublishDateParsed)
        {
            return 1;
        }

        if (this.PublishDateParsed > that.PublishDateParsed)
        {
            return -1;
        }

        return string.Compare(this.Title, that.Title, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not FeedItemResponse other)
        {
            return false;
        }

        return this.PublishDateParsed == other.PublishDateParsed
            && string.Equals(this.Title, other.Title, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.PublishDateParsed, this.Title?.ToLowerInvariant());
    }

    /// <summary>
    /// Determines whether two specified instances are equal.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>true if left is equal to right; otherwise, false.</returns>
    public static bool operator ==(FeedItemResponse? left, FeedItemResponse? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two specified instances are not equal.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>true if left is not equal to right; otherwise, false.</returns>
    public static bool operator !=(FeedItemResponse? left, FeedItemResponse? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Determines whether one specified instance is less than another specified instance.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>true if left is less than right; otherwise, false.</returns>
    public static bool operator <(FeedItemResponse? left, FeedItemResponse? right)
    {
        if (left is null || right is null)
        {
            return false;
        }

        return left.CompareTo(right) < 0;
    }

    /// <summary>
    /// Determines whether one specified instance is less than or equal to another specified instance.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>true if left is less than or equal to right; otherwise, false.</returns>
    public static bool operator <=(FeedItemResponse? left, FeedItemResponse? right)
    {
        if (left is null || right is null)
        {
            return false;
        }

        return left.CompareTo(right) <= 0;
    }

    /// <summary>
    /// Determines whether one specified instance is greater than another specified instance.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>true if left is greater than right; otherwise, false.</returns>
    public static bool operator >(FeedItemResponse? left, FeedItemResponse? right)
    {
        if (left is null || right is null)
        {
            return false;
        }

        return left.CompareTo(right) > 0;
    }

    /// <summary>
    /// Determines whether one specified instance is greater than or equal to another specified instance.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>true if left is greater than or equal to right; otherwise, false.</returns>
    public static bool operator >=(FeedItemResponse? left, FeedItemResponse? right)
    {
        if (left is null || right is null)
        {
            return false;
        }

        return left.CompareTo(right) >= 0;
    }
}
