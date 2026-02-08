namespace LostFilmMonitoring.BLL.Models;

/// <summary>
/// Represents user selection.
/// </summary>
public class SubscriptionItem
{
    /// <summary>
    /// Gets or sets quality.
    /// </summary>
    public string? Quality { get; set; }

    /// <summary>
    /// Gets or sets Series Id.
    /// </summary>
    public string? SeriesId { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{this.SeriesId} [{this.Quality}]";
    }

    /// <summary>
    /// Map array of <see cref="SubscriptionItem"/> to an array of <see cref="Subscription"/>.
    /// </summary>
    /// <param name="items">An array of <see cref="SubscriptionItem"/>.</param>
    /// <param name="seriesIdToName">A dictionary of series IDs to names.</param>
    /// <returns>An array of <see cref="Subscription"/>.</returns>
    internal static Subscription[] ToSubscriptions(SubscriptionItem[] items, IDictionary<Guid, string> seriesIdToName)
    {
        List<Subscription> subscriptions = new List<Subscription>(items.Length);
        foreach (var item in items)
        {
            if (item.SeriesId == null)
            {
                continue;
            }

            if (!Guid.TryParse(item.SeriesId, out var seriesId))
            {
                continue;
            }

            if (item.Quality == null)
            {
                continue;
            }

            if (!seriesIdToName.TryGetValue(seriesId, out var seriesName))
            {
                continue;
            }

            subscriptions.Add(new Subscription(seriesName, item.Quality));
        }

        return subscriptions.ToArray();
    }
}
