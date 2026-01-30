namespace LostFilmMonitoring.BLL.Models;

/// <summary>
/// Represents user selection.
/// </summary>
public class SubscriptionItem
{
    /// <summary>
    /// Gets or sets Series Name.
    /// </summary>
    public string? SeriesName { get; set; }

    /// <summary>
    /// Gets or sets quality.
    /// </summary>
    public string? Quality { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{this.SeriesName} [{this.Quality}]";
    }

    /// <summary>
    /// Map array of <see cref="SubscriptionItem"/> to an array of <see cref="Subscription"/>.
    /// </summary>
    /// <param name="items">An array of <see cref="SubscriptionItem"/>.</param>
    /// <returns>An array of <see cref="Subscription"/>.</returns>
    internal static Subscription[] ToSubscriptions(SubscriptionItem[] items)
        => items.Select(Map).ToArray();

    private static Subscription Map(SubscriptionItem s)
        => new (
            s.SeriesName ?? throw new InvalidDataException(nameof(Subscription.SeriesName)),
            s.Quality ?? throw new InvalidDataException(nameof(Subscription.Quality)));
}
