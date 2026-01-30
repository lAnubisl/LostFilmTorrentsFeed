namespace LostFilmMonitoring.DAO.Interfaces;

/// <summary>
/// Responsible for providing access to all aspects of Data-Access-Layer.
/// </summary>
public interface IDal
{
    /// <summary>
    /// Gets an instance of IFeedDAO.
    /// </summary>
    IFeedDao Feed { get; }

    /// <summary>
    /// Gets an instance of ISeriesDAO.
    /// </summary>
    ISeriesDao Series { get; }

    /// <summary>
    /// Gets an instance of ISubscriptionDAO.
    /// </summary>
    ISubscriptionDao Subscription { get; }

    /// <summary>
    /// Gets an instance of ITorrentFileDAO.
    /// </summary>
    ITorrentFileDao TorrentFile { get; }

    /// <summary>
    /// Gets an instance of IUserDAO.
    /// </summary>
    IUserDao User { get; }

    /// <summary>
    /// Gets an instance of IEpisodeDAO.
    /// </summary>
    IEpisodeDao Episode { get; }
}
