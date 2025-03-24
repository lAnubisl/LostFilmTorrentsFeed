namespace LostFilmMonitoring.BLL.Commands;

/// <summary>
/// Update all feeds.
/// </summary>
public class UpdateUserFeedCommand : ICommand<UpdateUserFeedCommandRequestModel, UpdateUserFeedCommandResponseModel>
{
    private static readonly object TorrentFileLocker = new object();
    private readonly ILogger logger;
    private readonly IDal dal;
    private readonly IConfiguration configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUserFeedCommand"/> class.
    /// </summary>
    /// <param name="logger">Logger.</param>
    /// <param name="dal">Data Access Layer.</param>
    /// <param name="configuration">Configuration.</param>
    public UpdateUserFeedCommand(ILogger logger, IDal dal, IConfiguration configuration)
    {
        this.logger = logger.CreateScope(nameof(UpdateUserFeedCommand));
        this.dal = dal;
        this.configuration = configuration;
    }

    /// <inheritdoc/>
    public async Task<UpdateUserFeedCommandResponseModel> ExecuteAsync(UpdateUserFeedCommandRequestModel? model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        if (model.FeedResponseItem == null)
        {
            throw new ArgumentNullException(nameof(model.FeedResponseItem));
        }

        if (model.Torrent == null)
        {
            throw new ArgumentNullException(nameof(model.Torrent));
        }

        if (string.IsNullOrWhiteSpace(model.UserId))
        {
            throw new ArgumentNullException(nameof(model.UserId));
        }

        this.logger.Info($"Call: {nameof(this.ExecuteAsync)}({model.FeedResponseItem}, {model.Torrent}, {model.UserId})");
        var result = new UpdateUserFeedCommandResponseModel();
        result.Success = await this.UpdateSubscribedUserAsync(model.FeedResponseItem!, model.Torrent!, model.UserId!);
        return result;
    }

    private async Task<bool> UpdateSubscribedUserAsync(FeedItemResponse feedResponseItem, BencodeNET.Torrents.Torrent torrent, string userId)
    {
        if (!await this.SaveTorrentFileForUserAsync(userId, torrent))
        {
            this.logger.Error($"Cannot save torrent file for user '{userId}'.");
            return false;
        }

        return await this.UpdateUserFeedAsync(userId, feedResponseItem, torrent.DisplayNameUtf8 ?? torrent.DisplayName);
    }

    private async Task<bool> SaveTorrentFileForUserAsync(string userId, BencodeNET.Torrents.Torrent torrent)
    {
        var user = await this.dal.User.LoadAsync(userId);
        if (user == null)
        {
            this.logger.Error($"User '{userId}' not found.");
            return false;
        }

        TorrentFile torrentFile;
        lock (TorrentFileLocker)
        {
            torrent.FixTrackers(this.configuration.GetTorrentAnnounceList(user.TrackerId));
            torrentFile = torrent.ToTorrentFile();
        }

        await this.dal.TorrentFile.SaveUserFileAsync(userId, torrentFile);
        return true;
    }

    private async Task<bool> UpdateUserFeedAsync(string userId, FeedItemResponse item, string torrentFileName)
    {
        string link = Extensions.GenerateTorrentLink(this.configuration.BaseUrl, userId, torrentFileName);
        var userFeedItem = new FeedItem(item.Title, link, item.PublishDateParsed);
        var userFeed = (await this.dal.Feed.LoadUserFeedAsync(userId)) ?? new SortedSet<FeedItem>();
        userFeed.Add(userFeedItem);
        userFeed.RemoveWhere(x => x == null);
        await this.dal.Feed.SaveUserFeedAsync(userId, userFeed.Take(15).ToArray());
        await this.CleanupOldRssFilesAsync(userId, userFeed.Skip(15).ToArray());
        return true;
    }

    private Task CleanupOldRssFilesAsync(string userId, FeedItem[] oldRssFeedItems)
        => Task.WhenAll(oldRssFeedItems.Select(i => this.CleanupOldRssFileAsync(userId, i)));

    private async Task CleanupOldRssFileAsync(string userId, FeedItem item)
    {
        var fileName = item.GetUserFileName(userId);
        if (fileName == null)
        {
            this.logger.Error($"Cannot get fileName from FeedItem '{item.Link}'.");
            return;
        }

        try
        {
            await this.dal.TorrentFile.DeleteUserFileAsync(userId, fileName);
        }
        catch (Exception ex)
        {
            this.logger.Log($"Error deleting FeedItem '{item.Link}' for user '{userId}'.", ex);
        }
    }
}