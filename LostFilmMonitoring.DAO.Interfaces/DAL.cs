namespace LostFilmMonitoring.DAO.Interfaces;

/// <summary>
/// Responsible for providing access to all aspects of Data-Access-Layer.
/// </summary>
public class Dal : IDal
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Dal"/> class.
    /// </summary>
    /// <param name="feedDAO">IFeedDAO.</param>
    /// <param name="seriesDAO">ISeriesDAO.</param>
    /// <param name="subscriptionDAO">ISubscriptionDAO.</param>
    /// <param name="torrentFileDAO">ITorrentFileDAO.</param>
    /// <param name="userDAO">IUserDAO.</param>
    /// <param name="episodeDAO">IEpisodeDAO.</param>
    public Dal(IFeedDao feedDAO, ISeriesDao seriesDAO, ISubscriptionDao subscriptionDAO, ITorrentFileDao torrentFileDAO, IUserDao userDAO, IEpisodeDao episodeDAO)
    {
        this.Feed = feedDAO ?? throw new ArgumentNullException(nameof(feedDAO));
        this.Subscription = subscriptionDAO ?? throw new ArgumentNullException(nameof(subscriptionDAO));
        this.TorrentFile = torrentFileDAO ?? throw new ArgumentNullException(nameof(torrentFileDAO));
        this.Series = seriesDAO ?? throw new ArgumentNullException(nameof(seriesDAO));
        this.User = userDAO ?? throw new ArgumentNullException(nameof(userDAO));
        this.Episode = episodeDAO ?? throw new ArgumentNullException(nameof(episodeDAO));
    }

    /// <inheritdoc/>
    public IFeedDao Feed { get; }

    /// <inheritdoc/>
    public ISeriesDao Series { get; }

    /// <inheritdoc/>
    public ISubscriptionDao Subscription { get; }

    /// <inheritdoc/>
    public ITorrentFileDao TorrentFile { get; }

    /// <inheritdoc/>
    public IUserDao User { get; }

    /// <inheritdoc/>
    public IEpisodeDao Episode { get; }
}
