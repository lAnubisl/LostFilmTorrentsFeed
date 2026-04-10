namespace LostFilmMonitoring.BLL.Models.CommandModels;

/// <summary>
/// UpdateUserFeedCommandRequestModel.
/// </summary>
public class UpdateUserFeedCommandRequestModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUserFeedCommandRequestModel"/> class.
    /// </summary>
    /// <param name="userId">User id.</param>
    /// <param name="feedResponseItem">Feed response item.</param>
    /// <param name="torrent">Parsed torrent.</param>
    public UpdateUserFeedCommandRequestModel(string? userId, FeedItemResponse? feedResponseItem, IParsedTorrent? torrent)
    {
        this.UserId = userId;
        this.FeedResponseItem = feedResponseItem;
        this.Torrent = torrent;
    }

    /// <summary>
    /// Gets user id.
    /// </summary>
    public string? UserId { get; }

    /// <summary>
    /// Gets feed response item.
    /// </summary>
    public FeedItemResponse? FeedResponseItem { get; }

    /// <summary>
    /// Gets parsed torrent.
    /// </summary>
    public IParsedTorrent? Torrent { get; }
}
