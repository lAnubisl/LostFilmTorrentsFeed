namespace LostFilmMonitoring.DAO.Interfaces.DomainModels;

/// <summary>
/// Quality-specific episode information.
/// </summary>
public class QualityEpisodeInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QualityEpisodeInfo"/> class.
    /// </summary>
    /// <param name="seasonNumber">Season number for last episode of this quality.</param>
    /// <param name="episodeNumber">Episode number for last episode of this quality.</param>
    public QualityEpisodeInfo(int? seasonNumber, int? episodeNumber)
    {
        this.SeasonNumber = seasonNumber;
        this.EpisodeNumber = episodeNumber;
    }

    /// <summary>
    /// Gets season number for last episode of this quality.
    /// </summary>
    public int? SeasonNumber { get; }

    /// <summary>
    /// Gets episode number for last episode of this quality.
    /// </summary>
    public int? EpisodeNumber { get; }
}
