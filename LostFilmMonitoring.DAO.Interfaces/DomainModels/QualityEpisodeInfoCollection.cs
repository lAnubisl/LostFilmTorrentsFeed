namespace LostFilmMonitoring.DAO.Interfaces.DomainModels;

/// <summary>
/// Collection of quality-specific episode information.
/// </summary>
public class QualityEpisodeInfoCollection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QualityEpisodeInfoCollection"/> class.
    /// </summary>
    /// <param name="q1080SeasonNumber">Season number for last episode of quality 1080p.</param>
    /// <param name="q1080EpisodeNumber">Episode number for last episode of quality 1080p.</param>
    /// <param name="qMP4SeasonNumber">Season number for last episode of quality 720p.</param>
    /// <param name="qMP4EpisodeNumber">Episode number for last episode of quality 720p.</param>
    /// <param name="qSDSeasonNumber">Season number for last episode of quality SD.</param>
    /// <param name="qSDEpisodeNumber">Episode number for last episode of quality SD.</param>
    public QualityEpisodeInfoCollection(
        int? q1080SeasonNumber,
        int? q1080EpisodeNumber,
        int? qMP4SeasonNumber,
        int? qMP4EpisodeNumber,
        int? qSDSeasonNumber,
        int? qSDEpisodeNumber)
    {
        this.Q1080 = new QualityEpisodeInfo(q1080SeasonNumber, q1080EpisodeNumber);
        this.QMP4 = new QualityEpisodeInfo(qMP4SeasonNumber, qMP4EpisodeNumber);
        this.QSD = new QualityEpisodeInfo(qSDSeasonNumber, qSDEpisodeNumber);
    }

    /// <summary>
    /// Gets quality-specific episode information for 1080p quality.
    /// </summary>
    public QualityEpisodeInfo Q1080 { get; }

    /// <summary>
    /// Gets quality-specific episode information for MP4 (720p) quality.
    /// </summary>
    public QualityEpisodeInfo QMP4 { get; }

    /// <summary>
    /// Gets quality-specific episode information for SD quality.
    /// </summary>
    public QualityEpisodeInfo QSD { get; }
}
