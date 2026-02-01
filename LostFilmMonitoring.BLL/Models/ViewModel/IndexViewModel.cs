namespace LostFilmMonitoring.BLL.Models.ViewModel;

/// <summary>
/// Represents information to be shown in home screen.
/// </summary>
public class IndexViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IndexViewModel"/> class.
    /// </summary>
    /// <param name="series">Series.</param>
    public IndexViewModel(ICollection<Series> series)
    {
        this.Items = series.OrderByDescending(s => s.LastEpisode).Select(s => new IndexViewItemModel(s)).ToArray();
        this.Last24HoursItems = Filter(series, s => s.LastEpisode >= DateTime.Now.AddHours(-24));
        this.Last7DaysItems = Filter(series, s => s.LastEpisode < DateTime.Now.AddHours(-24) && s.LastEpisode >= DateTime.Now.AddDays(-7));
        this.OlderItems = Filter(series, s => s.LastEpisode < DateTime.Now.AddDays(-7));
    }

    /// <summary>
    /// Gets or sets episodes updated within last 24 hours.
    /// </summary>
    public string[] Last24HoursItems { get; set; }

    /// <summary>
    /// Gets or sets episodes updated within last 7 days.
    /// </summary>
    public string[] Last7DaysItems { get; set; }

    /// <summary>
    /// Gets or sets episodes older than 7 days..
    /// </summary>
    public string[] OlderItems { get; set; }

    /// <summary>
    /// Gets or sets items to be shown in home screen.
    /// </summary>
    public IndexViewItemModel[] Items { get; set; }

    private static string[] Filter(ICollection<Series> series, Func<Series, bool> predicate)
    {
        return series
            .Where(predicate)
            .OrderByDescending(s => s.LastEpisode)
            .Select(s => s.Name)
            .ToArray();
    }
}
