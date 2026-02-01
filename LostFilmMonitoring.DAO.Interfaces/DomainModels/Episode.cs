namespace LostFilmMonitoring.DAO.Interfaces.DomainModels;

/// <summary>
/// Episode.
/// </summary>
public class Episode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Episode"/> class.
    /// </summary>
    /// <param name="seriesName">Series name.</param>
    /// <param name="episodeName">Episode name.</param>
    /// <param name="seasonNumber">Season number.</param>
    /// <param name="episodeNumber">Episode number.</param>
    /// <param name="torrentId">Torrent Id.</param>
    /// <param name="quality">Quality.</param>
    public Episode(string seriesName, string episodeName, int seasonNumber, int episodeNumber, string torrentId, string quality)
    {
        this.SeriesName = seriesName;
        this.EpisodeName = episodeName;
        this.SeasonNumber = seasonNumber;
        this.EpisodeNumber = episodeNumber;
        this.TorrentId = torrentId;
        this.Quality = quality;
    }

    /// <summary>
    /// Gets Series Name.
    /// </summary>
    public string SeriesName { get; private set; }

    /// <summary>
    /// Gets Episode Name.
    /// </summary>
    public string EpisodeName { get; private set; }

    /// <summary>
    /// Gets Season Number.
    /// </summary>
    public int SeasonNumber { get; private set; }

    /// <summary>
    /// Gets Episode Number.
    /// </summary>
    public int EpisodeNumber { get; private set; }

    /// <summary>
    /// Gets Torrent Id.
    /// </summary>
    public string TorrentId { get; private set; }

    /// <summary>
    /// Gets Quality.
    /// </summary>
    public string Quality { get; private set; }
}
