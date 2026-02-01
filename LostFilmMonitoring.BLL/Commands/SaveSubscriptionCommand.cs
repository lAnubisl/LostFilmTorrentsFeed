namespace LostFilmMonitoring.BLL.Commands;

/// <summary>
/// Responsible for saving user Subscription.
/// </summary>
public class SaveSubscriptionCommand : ICommand<EditSubscriptionRequestModel, EditSubscriptionResponseModel>
{
    private readonly IValidator<EditSubscriptionRequestModel> validator;
    private readonly IConfiguration configuration;
    private readonly IModelPersister persister;
    private readonly ILogger logger;
    private readonly IDal dal;
    private readonly object locker = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveSubscriptionCommand"/> class.
    /// </summary>
    /// <param name="logger">logger.</param>
    /// <param name="validator">validator.</param>
    /// <param name="dal">dal.</param>
    /// <param name="configuration">configuration.</param>
    /// <param name="persister">Persister.</param>
    public SaveSubscriptionCommand(
        ILogger logger,
        IValidator<EditSubscriptionRequestModel> validator,
        IDal dal,
        IConfiguration configuration,
        IModelPersister persister)
    {
        this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        this.logger = logger?.CreateScope(nameof(SaveSubscriptionCommand)) ?? throw new ArgumentNullException(nameof(logger));
        this.dal = dal ?? throw new ArgumentNullException(nameof(dal));
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        this.persister = persister ?? throw new ArgumentNullException(nameof(persister));
    }

    /// <inheritdoc/>
    public async Task<EditSubscriptionResponseModel> ExecuteAsync(EditSubscriptionRequestModel? model)
    {
        this.logger.Info($"Call: {nameof(this.ExecuteAsync)}(EditSubscriptionRequestModel model)");
        if (model == null)
        {
            return new EditSubscriptionResponseModel(ValidationResult.Fail(ErrorMessages.RequestNull));
        }

        if (string.IsNullOrEmpty(model.UserId))
        {
            return new EditSubscriptionResponseModel(ValidationResult.Fail(nameof(EditSubscriptionRequestModel.UserId), ErrorMessages.FieldEmpty));
        }

        if (model.Items == null || model.Items.Length == 0)
        {
            return new EditSubscriptionResponseModel(ValidationResult.Fail(nameof(EditSubscriptionRequestModel.Items), ErrorMessages.FieldEmpty));
        }

        var validationResult = await this.validator.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            return new EditSubscriptionResponseModel(validationResult);
        }

        var newUserSubscriptions = SubscriptionItem.ToSubscriptions(model.Items);
        await this.UpdateUserFeedItemsAsync(model.UserId, newUserSubscriptions);
        await this.dal.Subscription.SaveAsync(model.UserId, newUserSubscriptions);
        await this.UpdatePresentationModelAsync(model.UserId, model.Items);
        return new EditSubscriptionResponseModel(ValidationResult.Ok);
    }

    private static BencodeNET.Torrents.Torrent ToTorrentDataStructure(TorrentFile torrentFile)
        => torrentFile.Stream.ToTorrentDataStructure();

    private static FeedItem? AddFeedItem(SortedSet<FeedItem> set, string title, string link)
    {
        var itemToRemove = set.FirstOrDefault(i => string.Equals(i.Title, title, StringComparison.OrdinalIgnoreCase));
        if (itemToRemove != null)
        {
            set.Remove(itemToRemove);
        }

        set.Add(new FeedItem(title, link, DateTime.UtcNow));
        return itemToRemove;
    }

    private static string? GetTorrentId(Series series, string quality)
    {
        return quality switch
        {
            Quality.SD => GetTorrentId(series.LastEpisodeTorrentLinkSD),
            Quality.H1080 => GetTorrentId(series.LastEpisodeTorrentLink1080),
            Quality.H720 => GetTorrentId(series.LastEpisodeTorrentLinkMP4),
            _ => throw new InvalidOperationException("Quality not supported"),
        };
    }

    private static string? GetTorrentId(string? reteOrgUrl)
    {
        // http://tracktor.in/rssdownloader.php?id=33572
        if (string.IsNullOrEmpty(reteOrgUrl))
        {
            return null;
        }

        string marker = "rssdownloader.php?id=";
        int index = reteOrgUrl.IndexOf(marker);
        if (index < 0)
        {
            return null;
        }

        return reteOrgUrl[(index + marker.Length) ..];
    }

    private Task UpdatePresentationModelAsync(string userId, SubscriptionItem[] selectedSubscriptions)
        => this.persister.PersistAsync($"subscription_{userId}", selectedSubscriptions);

    private async Task UpdateUserFeedItemsAsync(string userId, Subscription[] selectedSubscriptions)
    {
        var userSubscriptionsTask = this.dal.Subscription.LoadAsync(userId);
        var userFeedItemsTask = this.dal.Feed.LoadUserFeedAsync(userId);
        var userTask = this.dal.User.LoadAsync(userId);
        await Task.WhenAll(userSubscriptionsTask, userFeedItemsTask, userTask);
        var userFeedItems = await userFeedItemsTask ?? new SortedSet<FeedItem>();
        var user = await userTask;
        if (user == null)
        {
            this.logger.Error($"User for id '{userId}' not found.");
            return;
        }

        var newSubscriptions = Subscription.Filter(selectedSubscriptions, await userSubscriptionsTask);
        await Task.WhenAll(newSubscriptions.Select(async s => await this.UpdateUserFeedItemsAsync(user, userFeedItems, s)));
        await this.dal.Feed.SaveUserFeedAsync(userId, userFeedItems.Take(15).ToArray());
        await this.CleanupOldRssFilesAsync(userId, userFeedItems.Skip(15).ToArray());
    }

    private async Task UpdateUserFeedItemsAsync(User user, SortedSet<FeedItem> userFeedItems, Subscription subscription)
    {
        var series = await this.dal.Series.LoadAsync(subscription.SeriesName);
        if (series == null)
        {
            this.logger.Error($"Tried to update users feed with series '{subscription.SeriesName}' but haven't found this series in the storage. ");
            return;
        }

        var torrentId = GetTorrentId(series, subscription.Quality);
        if (torrentId == null)
        {
            return;
        }

        var torrentFile = await this.dal.TorrentFile.LoadBaseFileAsync(torrentId);
        if (torrentFile?.Stream == null)
        {
            return;
        }

        var torrent = ToTorrentDataStructure(torrentFile);
        torrent.FixTrackers(this.configuration.GetTorrentAnnounceList(user.TrackerId));
        var userTorrentFile = torrent.ToTorrentFile();
        if (userTorrentFile.FileName == null)
        {
            this.logger.Error($"File name for torrent id '{torrent}' is null.");
            return;
        }

        await this.dal.TorrentFile.SaveUserFileAsync(user.Id, userTorrentFile);
        FeedItem? itemToRemove;
        lock (this.locker)
        {
            itemToRemove = AddFeedItem(
                userFeedItems,
                series.LastEpisodeName,
                Extensions.GenerateTorrentLink(this.configuration.BaseUrl, user.Id, userTorrentFile.FileName));
        }

        if (itemToRemove != null)
        {
            await this.CleanupOldRssFileAsync(user.Id, itemToRemove);
        }
    }

    private async Task CleanupOldRssFileAsync(string userId, FeedItem item)
    {
        this.logger.Info($"Call: {nameof(this.CleanupOldRssFileAsync)}('{userId}', '{item.Link}')");
        try
        {
            var fileName = item.GetUserFileName(userId);
            if (fileName == null)
            {
                return;
            }

            await this.dal.TorrentFile.DeleteUserFileAsync(userId, fileName);
        }
        catch (Exception ex)
        {
            this.logger.Log(ex);
        }
    }

    private Task CleanupOldRssFilesAsync(string userId, FeedItem[] oldRssFeedItems)
        => Task.WhenAll(oldRssFeedItems.Select(i => this.CleanupOldRssFileAsync(userId, i)));
}
