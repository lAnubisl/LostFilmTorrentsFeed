// <copyright file="UpdateFeedsCommand.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2023 Alexander Panfilenok
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the 'Software'), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>

namespace LostFilmMonitoring.BLL.Commands;

/// <summary>
/// Update all feeds.
/// </summary>
public class UpdateFeedsCommand : ICommand
{
    private static readonly object SeriesLocker = new ();
    private readonly ILogger logger;
    private readonly IDal dal;
    private readonly IRssFeed rssFeed;
    private readonly IConfiguration configuration;
    private readonly IModelPersister modelPersister;
    private readonly ILostFilmClient client;
    private readonly ITmdbClient tmdbClient;
    private readonly IFileSystem fileSystem;
    private readonly UpdateUserFeedCommand updateUserFeedCommand;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateFeedsCommand"/> class.
    /// </summary>
    /// <param name="logger">Logger.</param>
    /// <param name="rssFeed">ReteOrgRssFeed.</param>
    /// <param name="dal">dal.</param>
    /// <param name="configuration">IConfiguration.</param>
    /// <param name="modelPersister">modelPersister.</param>
    /// <param name="client">client.</param>
    /// <param name="tmdbClient">tmdbClient.</param>
    /// <param name="fileSystem">fileSystem.</param>
    public UpdateFeedsCommand(
        ILogger logger,
        IRssFeed rssFeed,
        IDal dal,
        IConfiguration configuration,
        IModelPersister modelPersister,
        ILostFilmClient client,
        ITmdbClient tmdbClient,
        IFileSystem fileSystem)
    {
        this.logger = logger != null ? logger.CreateScope(nameof(UpdateFeedsCommand)) : throw new ArgumentNullException(nameof(logger));
        this.dal = dal ?? throw new ArgumentNullException(nameof(dal));
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        this.rssFeed = rssFeed ?? throw new ArgumentNullException(nameof(rssFeed));
        this.modelPersister = modelPersister ?? throw new ArgumentNullException(nameof(modelPersister));
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.tmdbClient = tmdbClient ?? throw new ArgumentNullException(nameof(tmdbClient));
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        this.updateUserFeedCommand = new UpdateUserFeedCommand(logger, dal, configuration);
    }

    /// <inheritdoc/>
    public async Task ExecuteAsync()
    {
        this.logger.Info($"Call: {nameof(this.ExecuteAsync)}()");

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
            var series = feedItem.ToSeries();
            if (series == null)
            {
                return null;
            }

            if (!existingSeries.ContainsKey(series.Name))
            {
                existingSeries.Add(series.Name, series);
                return existingSeries[series.Name];
            }

            var existing = existingSeries[series.Name];
            existing.MergeFrom(series);
            return existing;
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

        BencodeNET.Torrents.Torrent? torrent = await this.GetTorrentAsync(feedItem).ConfigureAwait(false);
        if (torrent == null)
        {
            return false;
        }

        await this.SaveEpisodeAsync(feedItem);
        await this.UpdateAllSubscribedUsersAsync(feedItem, torrent);
        var newSeriesDetected = seriesToUpdate.Id == Guid.Empty;

        seriesToUpdate.Id = await this.dal.Series.SaveAsync(seriesToUpdate);
        if (newSeriesDetected && !await this.PosterExistsAsync(seriesToUpdate.Id))
        {
            await this.DownloadImageAsync(seriesToUpdate);
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

    private async Task<BencodeNET.Torrents.Torrent?> GetTorrentAsync(FeedItemResponse feedResponseItem)
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

        BencodeNET.Torrents.Torrent torrent = torrentFileResponse.Content.ToTorrentDataStructure();
        await this.dal.TorrentFile.SaveBaseFileAsync(torrentId, new TorrentFile(torrentFileResponse.FileName, torrentFileResponse.Content));
        return torrent;
    }

    private async Task UpdateAllSubscribedUsersAsync(FeedItemResponse feedResponseItem, BencodeNET.Torrents.Torrent torrent)
    {
        if (string.IsNullOrEmpty(feedResponseItem.SeriesName) || string.IsNullOrEmpty(feedResponseItem.Quality))
        {
            this.logger.Error($"Cannot update users for feed item {feedResponseItem} because it has no series name or quality.");
            return;
        }

        var userIds = await this.dal.Subscription.LoadUsersIdsAsync(feedResponseItem.SeriesName, feedResponseItem.Quality);
        await this.UpdateAllSubscribedUsersAsync(userIds, feedResponseItem, torrent);
    }

    private Task UpdateAllSubscribedUsersAsync(string[] userIds, FeedItemResponse feedResponseItem, BencodeNET.Torrents.Torrent torrent)
        => Task.WhenAll(userIds.Select(x => this.updateUserFeedCommand.ExecuteAsync(new UpdateUserFeedCommandRequestModel(x, feedResponseItem, torrent))));

    private async Task DownloadImageAsync(Series series)
    {
        var openBraceIndex = series.Name.IndexOf('(');
        var closeBraceIndex = series.Name.IndexOf(')');
        if (openBraceIndex == -1 || closeBraceIndex == -1 || openBraceIndex > closeBraceIndex)
        {
            // cannot parse the series original name
            return;
        }

        var originalName = series.Name.Substring(openBraceIndex + 1, closeBraceIndex - openBraceIndex - 1);
        using var imageStream = await this.tmdbClient.DownloadImageAsync(originalName);
        if (imageStream == null)
        {
            return;
        }

        await this.fileSystem.SaveAsync(Constants.MetadataStorageContainerImages, $"{series.Id}.jpg", "image/jpeg", imageStream);
    }

    private Task<bool> PosterExistsAsync(Guid seriesId) =>
        this.fileSystem.ExistsAsync(Constants.MetadataStorageContainerImages, $"{seriesId}.jpg");
}
