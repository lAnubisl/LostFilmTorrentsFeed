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
}