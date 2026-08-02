namespace LostFilmMonitoring.DAO.Interfaces.DomainModels;

/// <summary>
/// Quality-specific information about the last episode of a series.
/// </summary>
public sealed record SeriesQualityInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeriesQualityInfo"/> class.
    /// </summary>
    /// <param name="link">Link to the torrent file.</param>
    /// <param name="seasonNumber">Season number.</param>
    /// <param name="episodeNumber">Episode number.</param>
    public SeriesQualityInfo(string? link, int? seasonNumber, int? episodeNumber)
    {
        this.Link = link;
        this.SeasonNumber = seasonNumber;
        this.EpisodeNumber = episodeNumber;
    }

    /// <summary>
    /// Gets the link to the torrent file.
    /// </summary>
    public string? Link { get; }

    /// <summary>
    /// Gets the season number.
    /// </summary>
    public int? SeasonNumber { get; }

    /// <summary>
    /// Gets the episode number.
    /// </summary>
    public int? EpisodeNumber { get; }
}
