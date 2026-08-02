namespace LostFilmMonitoring.DAO.Interfaces.DomainModels;

/// <summary>
/// Series.
/// </summary>
public class Series
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Series"/> class.
    /// </summary>
    /// <param name="id">Series id.</param>
    /// <param name="name">Series name.</param>
    /// <param name="lastEposide">Last episode date.</param>
    /// <param name="lastEpisodeName">Last episode name.</param>
    /// <param name="sd">Information about the last SD episode.</param>
    /// <param name="mp4">Information about the last MP4 episode.</param>
    /// <param name="q1080">Information about the last 1080p episode.</param>
    public Series(
        Guid id,
        string name,
        DateTime lastEposide,
        string lastEpisodeName,
        SeriesQualityInfo sd,
        SeriesQualityInfo mp4,
        SeriesQualityInfo q1080)
    {
        this.Id = id;
        this.Name = name;
        this.LastEpisode = lastEposide;
        this.LastEpisodeName = lastEpisodeName;
        this.LastEpisodeTorrentLinkSD = sd.Link;
        this.LastEpisodeTorrentLinkMP4 = mp4.Link;
        this.LastEpisodeTorrentLink1080 = q1080.Link;
        this.Q1080EpisodeNumber = q1080.EpisodeNumber;
        this.QMP4EpisodeNumber = mp4.EpisodeNumber;
        this.QSDEpisodeNumber = sd.EpisodeNumber;
        this.Q1080SeasonNumber = q1080.SeasonNumber;
        this.QMP4SeasonNumber = mp4.SeasonNumber;
        this.QSDSeasonNumber = sd.SeasonNumber;
    }

    /// <summary>
    /// Gets or sets Id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets Name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets LastEpisode Date.
    /// </summary>
    public DateTime LastEpisode { get; private set; }

    /// <summary>
    /// Gets Last episode name.
    /// </summary>
    public string LastEpisodeName { get; private set; }

    /// <summary>
    /// Gets LastEpisodeTorrentLinkSD.
    /// </summary>
    public string? LastEpisodeTorrentLinkSD { get; private set; }

    /// <summary>
    /// Gets LastEpisodeTorrentLinkMP4.
    /// </summary>
    public string? LastEpisodeTorrentLinkMP4 { get; private set; }

    /// <summary>
    /// Gets LastEpisodeTorrentLink1080.
    /// </summary>
    public string? LastEpisodeTorrentLink1080 { get; private set; }

    /// <summary>
    /// Gets season number for last episode of quality 1080p.
    /// </summary>
    public int? Q1080SeasonNumber { get; private set; }

    /// <summary>
    /// Gets season number for last episode of quality 720p.
    /// </summary>
    public int? QMP4SeasonNumber { get; private set; }

    /// <summary>
    /// Gets season number for last episode of quality SD.
    /// </summary>
    public int? QSDSeasonNumber { get; private set; }

    /// <summary>
    /// Gets episode number for last episode of quality 1080p.
    /// </summary>
    public int? Q1080EpisodeNumber { get; private set; }

    /// <summary>
    /// Gets episode number for last episode of quality 720p.
    /// </summary>
    public int? QMP4EpisodeNumber { get; private set; }

    /// <summary>
    /// Gets episode number for last episode of quality SD.
    /// </summary>
    public int? QSDEpisodeNumber { get; private set; }

    /// <summary>
    /// Merge updates from <paramref name="from"/> to current instance.
    /// </summary>
    /// <param name="from">Instance of <see cref="Series"/> to merge changes from.</param>
    public void MergeFrom(Series from)
    {
        if (from == null)
        {
            return;
        }

        this.LastEpisodeName = from.LastEpisodeName;
        this.LastEpisode = from.LastEpisode;
        this.LastEpisodeTorrentLink1080 = from.LastEpisodeTorrentLink1080 ?? this.LastEpisodeTorrentLink1080;
        this.LastEpisodeTorrentLinkMP4 = from.LastEpisodeTorrentLinkMP4 ?? this.LastEpisodeTorrentLinkMP4;
        this.LastEpisodeTorrentLinkSD = from.LastEpisodeTorrentLinkSD ?? this.LastEpisodeTorrentLinkSD;
        this.Q1080SeasonNumber = from.Q1080SeasonNumber ?? this.Q1080SeasonNumber;
        this.QMP4SeasonNumber = from.QMP4SeasonNumber ?? this.QMP4SeasonNumber;
        this.QSDSeasonNumber = from.QSDSeasonNumber ?? this.QSDSeasonNumber;
        this.Q1080EpisodeNumber = from.Q1080EpisodeNumber ?? this.Q1080EpisodeNumber;
        this.QMP4EpisodeNumber = from.QMP4EpisodeNumber ?? this.QMP4EpisodeNumber;
        this.QSDEpisodeNumber = from.QSDEpisodeNumber ?? this.QSDEpisodeNumber;
    }
}
