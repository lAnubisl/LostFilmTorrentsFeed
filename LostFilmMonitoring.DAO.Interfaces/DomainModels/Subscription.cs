namespace LostFilmMonitoring.DAO.Interfaces.DomainModels;

/// <summary>
/// Subscription.
/// </summary>
public sealed class Subscription : IEquatable<Subscription>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Subscription"/> class.
    /// </summary>
    /// <param name="seriesName">Name of the series.</param>
    /// <param name="quality">Quality of the series.</param>
    public Subscription(string seriesName, string quality)
    {
        this.SeriesName = seriesName;
        this.Quality = quality;
    }

    /// <summary>
    /// Gets SeriesName.
    /// </summary>
    public string SeriesName { get; }

    /// <summary>
    /// Gets Quantity.
    /// </summary>
    public string Quality { get; }

    /// <summary>
    /// Filter selected subscriptions taking into account existing ones.
    /// </summary>
    /// <param name="selected">User selected subscriptions.</param>
    /// <param name="old">User existing subscriptions.</param>
    /// <returns>New subscriptions that were selected.</returns>
    public static IEnumerable<Subscription> Filter(Subscription[] selected, Subscription[] old)
    {
        var hs = new HashSet<Subscription>(old);
        return selected.Where(s => !hs.Contains(s));
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.SeriesName.GetHashCode(), this.Quality.GetHashCode());
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return this.Equals(obj as Subscription);
    }

    /// <inheritdoc/>
    public bool Equals(Subscription? other)
    {
        if (other == null)
        {
            return false;
        }

        return string.Equals(this.SeriesName, other.SeriesName, StringComparison.OrdinalIgnoreCase)
            && this.Quality == other.Quality;
    }
}
