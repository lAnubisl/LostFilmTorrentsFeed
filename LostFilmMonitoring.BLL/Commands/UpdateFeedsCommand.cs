namespace LostFilmMonitoring.BLL.Commands;

/// <summary>
/// Update all feeds.
/// </summary>
public class UpdateFeedsCommand : ICommand
{
    private static readonly object SeriesLocker = new ();
    private static readonly ActivitySource ActivitySource = new (ActivitySourceNames.UpdateFeedsCommand);
    private readonly ILogger logger;
    private readonly IDal dal;
    private readonly IRssFeed rssFeed;
    private readonly IConfiguration configuration;
    private readonly IModelPersister modelPersister;
    private readonly ILostFilmClient client;
    private readonly ITorrentFileHelper torrentFileHelper;
    private readonly UpdateUserFeedCommand updateUserFeedCommand;
    private readonly ICommand<Series> downloadCoverImagesCommand;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateFeedsCommand"/> class.
    /// </summary>
    /// <param name="dependencies">The dependencies container.</param>
    public UpdateFeedsCommand(UpdateFeedsCommandDependencies dependencies)
    {
        ArgumentNullException.ThrowIfNull(dependencies);
        this.logger = dependencies.Logger != null ? dependencies.Logger.CreateScope(nameof(UpdateFeedsCommand)) : throw new ArgumentNullException(nameof(dependencies.Logger));
        this.dal = dependencies.Dal ?? throw new ArgumentNullException(nameof(dependencies.Dal));
        this.configuration = dependencies.Configuration ?? throw new ArgumentNullException(nameof(dependencies.Configuration));
        this.rssFeed = dependencies.RssFeed ?? throw new ArgumentNullException(nameof(dependencies.RssFeed));
        this.modelPersister = dependencies.ModelPersister ?? throw new ArgumentNullException(nameof(dependencies.ModelPersister));
        this.client = dependencies.Client ?? throw new ArgumentNullException(nameof(dependencies.Client));
        this.torrentFileHelper = dependencies.TorrentFileHelper ?? throw new ArgumentNullException(nameof(dependencies.TorrentFileHelper));
        this.downloadCoverImagesCommand = dependencies.DownloadCoverImagesCommand ?? throw new ArgumentNullException(nameof(dependencies));
        this.updateUserFeedCommand = new UpdateUserFeedCommand(dependencies.Logger, dependencies.Dal, dependencies.Configuration);
    }

    /// <inheritdoc/>
    public async Task ExecuteAsync()
    {
        using var activity = ActivitySource.StartActivity(nameof(this.ExecuteAsync), ActivityKind.Internal);
        var loadFeedUpdatesTask = this.LoadFeedUpdatesAsync();
        var loadLastFeedUpdatesTask = this.LoadLastFeedUpdatesAsync();
        await Task.WhenAll(loadFeedUpdatesTask, loadLastFeedUpdatesTask);
        var feedItemsResponse = await loadFeedUpdatesTask;
        var persistedItemsRespone = await loadLastFeedUpdatesTask;

        feedItemsResponse.CleanForbiddenCharacters();
        if (!Extensions.HasUpdates(feedItemsResponse, persistedItemsRespone))
        {
            this.logger.Info("No updates.");
            return;
        }

        this.logger.Info("Found an Update.");
        var success = await this.ProcessFeedItemsAsync(feedItemsResponse);
        if (success)
        {
            await this.SaveFeedUpdatesAsync(feedItemsResponse);
        }
    }

    private static bool IsEpisodeCorrect(Episode? episode)
        => episode != null && episode.EpisodeNumber != 999;

    private static Series? GetSeriesToUpdate(Dictionary<string, Series> existingSeries, FeedItemResponse feedItem)
    {
        lock (SeriesLocker)
        {
            if (feedItem.ToSeries() is not { } series)
            {
                return null;
            }

            if (existingSeries.TryGetValue(series.Name, out var existing))
            {
                existing.MergeFrom(series);
                return existing;
            }

            existingSeries.Add(series.Name, series);
            return existingSeries[series.Name];
        }
    }

    private async Task<bool> ProcessFeedItemsAsync(SortedSet<FeedItemResponse> feedItems)
    {
        var allSeries = await this.LoadSeriesAsync();
        var success = true;
        foreach (var feedItem in feedItems)
        {
            success &= await this.ProcessFeedItemAsync(feedItem, allSeries);
        }

        await this.UpdateIndexViewModelAsync(allSeries.Values);
        return success;
    }

    private Task<bool> EpisodeAlreadyExistAsync(Episode episode)
        => this.dal.Episode.ExistsAsync(episode!.SeriesName, episode.SeasonNumber, episode.EpisodeNumber, episode.Quality);

    private async Task<bool> ProcessFeedItemAsync(FeedItemResponse feedItem, Dictionary<string, Series> series)
    {
        var episode = feedItem.ToEpisode();
        if (!IsEpisodeCorrect(episode))
        {
            return true;
        }

        if (await this.EpisodeAlreadyExistAsync(episode!))
        {
            return true;
        }

        Series? seriesToUpdate = GetSeriesToUpdate(series, feedItem);
        if (seriesToUpdate == null)
        {
            return true;
        }

        IParsedTorrent? torrent = await this.GetTorrentAsync(feedItem).ConfigureAwait(false);
        if (torrent == null)
        {
            return false;
        }

        await this.SaveEpisodeAsync(feedItem);
        await this.UpdateAllSubscribedUsersAsync(feedItem, torrent);
        var newSeriesDetected = seriesToUpdate.Id == Guid.Empty;

        seriesToUpdate.Id = await this.dal.Series.SaveAsync(seriesToUpdate);
        if (newSeriesDetected)
        {
            await this.downloadCoverImagesCommand.ExecuteAsync(seriesToUpdate);
        }

        return true;
    }

    private async Task SaveEpisodeAsync(FeedItemResponse feedItem)
    {
        var episode = feedItem.ToEpisode();
        if (episode == null)
        {
            return;
        }

        await this.dal.Episode.SaveAsync(episode);
    }

    private async Task<SortedSet<FeedItemResponse>> LoadFeedUpdatesAsync()
        => (await this.rssFeed.LoadFeedItemsAsync()) ?? new SortedSet<FeedItemResponse>();

    private async Task<SortedSet<FeedItemResponse>> LoadLastFeedUpdatesAsync()
        => (await this.modelPersister.LoadAsync<SortedSet<FeedItemResponse>>("ReteOrgItems")) ?? new SortedSet<FeedItemResponse>();

    private Task SaveFeedUpdatesAsync(SortedSet<FeedItemResponse> feedItemsResponse)
        => this.modelPersister.PersistAsync("ReteOrgItems", feedItemsResponse);

    private async Task<Dictionary<string, Series>> LoadSeriesAsync()
        => (await this.dal.Series.LoadAsync()).ToDictionary(x => x.Name, x => x);

    private Task UpdateIndexViewModelAsync(ICollection<Series> existingSeries)
        => this.modelPersister.PersistAsync("index", new IndexViewModel(existingSeries));

    private async Task<IParsedTorrent?> GetTorrentAsync(FeedItemResponse feedResponseItem)
    {
        string? torrentId = feedResponseItem.TorrentId;
        if (torrentId == null)
        {
            this.logger.Error($"Could not get torrent id for {feedResponseItem.Title}");
            return null;
        }

        ITorrentFileResponse? torrentFileResponse = await this.client.DownloadTorrentFileAsync(this.configuration.BaseUID, this.configuration.BaseUSESS, torrentId).ConfigureAwait(false);
        if (torrentFileResponse == null)
        {
            return null;
        }

        IParsedTorrent parsedTorrent = this.torrentFileHelper.Parse(torrentFileResponse.Content);
        await this.dal.TorrentFile.SaveBaseFileAsync(torrentId, new TorrentFile(torrentFileResponse.FileName, torrentFileResponse.Content));
        return parsedTorrent;
    }

    private async Task UpdateAllSubscribedUsersAsync(FeedItemResponse feedResponseItem, IParsedTorrent torrent)
    {
        if (string.IsNullOrEmpty(feedResponseItem.SeriesName) || string.IsNullOrEmpty(feedResponseItem.Quality))
        {
            this.logger.Error($"Cannot update users for feed item {feedResponseItem} because it has no series name or quality.");
            return;
        }

        var userIds = await this.dal.Subscription.LoadUsersIdsAsync(feedResponseItem.SeriesName, feedResponseItem.Quality);
        await this.UpdateAllSubscribedUsersAsync(userIds, feedResponseItem, torrent);
    }

    private Task UpdateAllSubscribedUsersAsync(string[] userIds, FeedItemResponse feedResponseItem, IParsedTorrent torrent)
        => Task.WhenAll(userIds.Select(x => this.updateUserFeedCommand.ExecuteAsync(new UpdateUserFeedCommandRequestModel(x, feedResponseItem, torrent))));
}
