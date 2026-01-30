namespace LostFilmMonitoring.BLL;

/// <summary>
/// This comparer is used to detect updates in the feed.
/// </summary>
public class FeedItemResponseEqualityComparer : IEqualityComparer<FeedItemResponse>
{
    /// <summary>
    /// Equals method.
    /// </summary>
    /// <param name="x">First item.</param>
    /// <param name="y">Second item.</param>
    /// <returns>bool.</returns>
    public bool Equals(FeedItemResponse? x, FeedItemResponse? y)
    {
        if (x == null && y == null)
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        if (!string.Equals(x.Title, y.Title, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.Equals(x.Link, y.Link, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.Equals(x.PublishDate, y.PublishDate, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// GetHashCode method.
    /// </summary>
    /// <param name="obj">Item.</param>
    /// <returns>int.</returns>
    public int GetHashCode(FeedItemResponse obj)
    {
        return HashCode.Combine(obj.Title, obj.Link, obj.PublishDate);
    }
}