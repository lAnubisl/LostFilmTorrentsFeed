namespace LostFilmMonitoring.BLL.Models.CommandModels;

/// <summary>
/// UpdateUserFeedCommandRequestModel.
/// </summary>
public class UpdateUserFeedCommandRequestModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUserFeedCommandRequestModel"/> class.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="feedResponseItem"></param>
    /// <param name="torrent"></param>
    public UpdateUserFeedCommandRequestModel(string? userId, FeedItemResponse? feedResponseItem, BencodeNET.Torrents.Torrent? torrent)
    {
        this.UserId = userId;
        this.FeedResponseItem = feedResponseItem;
        this.Torrent = torrent;
    }

    /// <summary>
    /// Gets UserId.
    /// </summary>
    public string? UserId { get; }

    /// <summary>
    /// Gets FeedItemResponse.
    /// </summary>
    public FeedItemResponse? FeedResponseItem { get; }

    /// <summary>
    /// Gets Torrent.
    /// </summary>
    public BencodeNET.Torrents.Torrent? Torrent { get; }
}
