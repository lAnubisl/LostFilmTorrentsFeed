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
    private static readonly HashSet<char> ForbiddenPrimaryKeyCharacters = new () { '/', '\\', '?', '#', '\t', '\r', '\n', '+' };
    private static readonly object SeriesLocker = new ();
    private static readonly object TorrentFileLocker = new ();
    private readonly ILogger logger;
    private readonly IDal dal;
    private readonly IRssFeed rssFeed;
    private readonly IConfiguration configuration;
    private readonly IModelPersister modelPersister;
    private readonly ILostFilmClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateFeedsCommand"/> class.
    /// </summary>
    /// <param name="logger">Logger.</param>
    /// <param name="rssFeed">ReteOrgRssFeed.</param>
    /// <param name="dal">dal.</param>
    /// <param name="configuration">IConfiguration.</param>
    /// <param name="modelPersister">modelPersister.</param>
    /// <param name="client">client.</param>
    public UpdateFeedsCommand(
        ILogger logger,
        IRssFeed rssFeed,
        IDal dal,
        IConfiguration configuration,
        IModelPersister modelPersister,
        ILostFilmClient client)
    {
        this.logger = logger != null ? logger.CreateScope(nameof(UpdateFeedsCommand)) : throw new ArgumentNullException(nameof(logger));
        this.dal = dal ?? throw new ArgumentNullException(nameof(dal));
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        this.rssFeed = rssFeed ?? throw new ArgumentNullException(nameof(rssFeed));
        this.modelPersister = modelPersister ?? throw new ArgumentNullException(nameof(modelPersister));
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    /// <inheritdoc/>
    public async Task ExecuteAsync()
    {
        this.logger.Info($"Call: {nameof(this.ExecuteAsync)}()");
        var feedItemsResponse = await this.LoadFeedUpdatesAsync().ConfigureAwait(false);
        CleanForbiddenCharacters(feedItemsResponse);
        var persistedItemsRespone = await this.LoadLastFeedUpdatesAsync().ConfigureAwait(false);
        if (!FeedItemResponse.HasUpdates(feedItemsResponse, persistedItemsRespone))
        {
            this.logger.Info("No updates.");
            return;
        }

        this.logger.Info("Found an Update.");
        var success = await this.ProcessFeedItemsAsync(feedItemsResponse).ConfigureAwait(false);
        if (success)
        {
            await this.SaveFeedUpdatesAsync(feedItemsResponse).ConfigureAwait(false);
        }
    }

    private static string? ReplaceForbiddenCharacters(string? str)
        => str == null
            ? null
            : new (str.ToCharArray().Where(c => !ForbiddenPrimaryKeyCharacters.Contains(c)).ToArray());

    private static void CleanForbiddenCharacters(IEnumerable<FeedItemResponse> items)
    {
        foreach (var item in items)
        {
            item.Title = ReplaceForbiddenCharacters(item.Title) !;
            item.EpisodeName = ReplaceForbiddenCharacters(item.EpisodeName);
            item.SeriesName = ReplaceForbiddenCharacters(item.SeriesName);
            item.SeriesNameEn = ReplaceForbiddenCharacters(item.SeriesNameEn);
            item.SeriesNameRu = ReplaceForbiddenCharacters(item.SeriesNameRu);
        }
    }

    private static Episode? ToEpisode(FeedItemResponse feedItem)
    {
        var torrentId = feedItem.GetTorrentId();
        if (string.IsNullOrEmpty(feedItem.SeriesName)
            || string.IsNullOrEmpty(feedItem.EpisodeName)
            || feedItem.EpisodeNumber == null
            || feedItem.SeasonNumber == null
            || string.IsNullOrEmpty(feedItem.Quality)
            || string.IsNullOrEmpty(torrentId))
        {
            return null;
        }

        return new (
            feedItem.SeriesName,
            feedItem.EpisodeName,
            feedItem.SeasonNumber.Value,
            feedItem.EpisodeNumber.Value,
            torrentId,
            feedItem.Quality);
    }

    private static Series? ToSeries(FeedItemResponse feedItem)
    {
        if (feedItem == null || string.IsNullOrEmpty(feedItem.SeriesName))
        {
            return null;
        }

        return new (
            feedItem.SeriesName,
            feedItem.PublishDateParsed,
            $"{feedItem.SeriesName}. {feedItem.EpisodeName} (S{feedItem.SeasonNumber:D2}E{feedItem.EpisodeNumber:D2}) ",
            ParseLink(feedItem, Quality.SD),
            ParseLink(feedItem, Quality.H720),
            ParseLink(feedItem, Quality.H1080),
            ParseSeasonNumber(feedItem, Quality.H1080),
            ParseSeasonNumber(feedItem, Quality.H720),
            ParseSeasonNumber(feedItem, Quality.SD),
            ParseEpisodeNumber(feedItem, Quality.H1080),
            ParseEpisodeNumber(feedItem, Quality.H720),
            ParseEpisodeNumber(feedItem, Quality.SD));
    }

    private static BencodeNET.Torrents.Torrent ToTorrentDataStructure(TorrentFileResponse torrentFileResponse)
        => torrentFileResponse.Content.ToTorrentDataStructure();

    private static int? ParseEpisodeNumber(FeedItemResponse feedItem, string quality)
        => feedItem.Quality == quality ? feedItem.EpisodeNumber : null;

    private static int? ParseSeasonNumber(FeedItemResponse feedItem, string quality)
        => feedItem.Quality == quality ? feedItem.SeasonNumber : null;

    private static string? ParseLink(FeedItemResponse feedItem, string quality)
        => feedItem.Quality == quality ? feedItem.Link : null;

    private static bool EpisodeIsCorrect(Episode? episode)
        => episode != null && episode.EpisodeNumber != 999;

    private static TorrentFile ToTorrentFile(TorrentFileResponse x)
        => new (x.FileName, x.Content);

    private static FeedItem ToFeedItem(FeedItemResponse x, string link)
        => new (x.Title, link, x.PublishDateParsed);

    private static Series? GetSeriesToUpdate(Dictionary<string, Series> existingSeries, FeedItemResponse feedItem)
    {
        lock (SeriesLocker)
        {
            var series = ToSeries(feedItem);
            if (series == null)
            {
                return null;
            }

            if (!existingSeries.TryGetValue(series.Name, out var existing))
            {
                existingSeries.Add(series.Name, series);
                return existingSeries[series.Name];
            }

            existing.MergeFrom(series);
            return existing;
        }
    }

    private async Task<bool> ProcessFeedItemsAsync(SortedSet<FeedItemResponse> feedItems)
    {
        var allSeries = await this.LoadSeriesAsync().ConfigureAwait(false);
        var success = true;
        foreach (var feedItem in feedItems)
        {
            success &= await this.ProcessFeedItemAsync(feedItem, allSeries).ConfigureAwait(false);
        }

        await this.UpdateIndexViewModelAsync(allSeries.Values).ConfigureAwait(false);
        return success;
    }

    private Task<bool> EpisodeAlreadyExistAsync(Episode episode)
        => this.dal.Episode.ExistsAsync(episode!.SeriesName, episode.SeasonNumber, episode.EpisodeNumber, episode.Quality);

    private async Task<bool> ProcessFeedItemAsync(FeedItemResponse feedItem, Dictionary<string, Series> series)
    {
        var episode = ToEpisode(feedItem);
        if (!EpisodeIsCorrect(episode))
        {
            return true;
        }

        if (await this.EpisodeAlreadyExistAsync(episode!).ConfigureAwait(false))
        {
            return true;
        }

        var seriesToUpdate = GetSeriesToUpdate(series, feedItem);
        if (seriesToUpdate == null)
        {
            return true;
        }

        var torrent = await this.GetTorrentAsync(feedItem).ConfigureAwait(false);
        if (torrent == null)
        {
            return false;
        }

        await this.SaveEpisodeAsync(feedItem).ConfigureAwait(false);
        await this.UpdateAllSubscribedUsersAsync(feedItem, torrent).ConfigureAwait(false);
        await this.dal.Series.SaveAsync(seriesToUpdate).ConfigureAwait(false);
        return true;
    }

    private async Task SaveEpisodeAsync(FeedItemResponse feedItem)
    {
        var episode = ToEpisode(feedItem);
        if (episode != null)
        {
            await this.dal.Episode.SaveAsync(episode).ConfigureAwait(false);
        }
    }

    private async Task<SortedSet<FeedItemResponse>> LoadFeedUpdatesAsync()
        => (await this.rssFeed.LoadFeedItemsAsync().ConfigureAwait(false)) ?? new SortedSet<FeedItemResponse>();

    private async Task<SortedSet<FeedItemResponse>> LoadLastFeedUpdatesAsync()
        => (await this.modelPersister.LoadAsync<SortedSet<FeedItemResponse>>("ReteOrgItems").ConfigureAwait(false)) ?? new SortedSet<FeedItemResponse>();

    private Task SaveFeedUpdatesAsync(SortedSet<FeedItemResponse> feedItemsResponse)
        => this.modelPersister.PersistAsync("ReteOrgItems", feedItemsResponse);

    private async Task<Dictionary<string, Series>> LoadSeriesAsync()
        => (await this.dal.Series.LoadAsync().ConfigureAwait(false)).ToDictionary(x => x.Name, x => x);

    private Task UpdateIndexViewModelAsync(ICollection<Series> existingSeries)
        => this.modelPersister.PersistAsync("index", new IndexViewModel(existingSeries));

    private async Task<BencodeNET.Torrents.Torrent?> GetTorrentAsync(FeedItemResponse feedResponseItem)
    {
        var torrentId = feedResponseItem.GetTorrentId();
        if (torrentId == null)
        {
            this.logger.LogError($"Could not get torrent id for {feedResponseItem.Title}");
            return null;
        }

        var torrentFileResponse = await this.client.DownloadTorrentFileAsync(this.configuration.BaseUID, this.configuration.BaseUSESS, torrentId).ConfigureAwait(false);
        if (torrentFileResponse == null)
        {
            return null;
        }

        var torrent = ToTorrentDataStructure(torrentFileResponse);
        await this.dal.TorrentFile.SaveBaseFileAsync(torrentId, ToTorrentFile(torrentFileResponse)).ConfigureAwait(false);
        return torrent;
    }

    private async Task UpdateAllSubscribedUsersAsync(FeedItemResponse feedResponseItem, BencodeNET.Torrents.Torrent torrent)
    {
        if (string.IsNullOrEmpty(feedResponseItem.SeriesName) || string.IsNullOrEmpty(feedResponseItem.Quality))
        {
            this.logger.LogError($"Cannot update users for feed item {feedResponseItem} because it has no series name or quality.");
            return;
        }

        var userIds = await this.dal.Subscription.LoadUsersIdsAsync(feedResponseItem.SeriesName, feedResponseItem.Quality).ConfigureAwait(false);
        await this.UpdateAllSubscribedUsersAsync(userIds, feedResponseItem, torrent).ConfigureAwait(false);
    }

    private Task UpdateAllSubscribedUsersAsync(string[] userIds, FeedItemResponse feedResponseItem, BencodeNET.Torrents.Torrent torrent)
        => Task.WhenAll(userIds.Select(x => this.UpdateSubscribedUserAsync(feedResponseItem, torrent, x)));

    private async Task UpdateSubscribedUserAsync(FeedItemResponse feedResponseItem, BencodeNET.Torrents.Torrent torrent, string userId)
    {
        if (await this.SaveTorrentFileForUserAsync(userId, torrent).ConfigureAwait(false))
        {
            await this.UpdateUserFeedAsync(userId, feedResponseItem, torrent.DisplayNameUtf8 ?? torrent.DisplayName).ConfigureAwait(false);
        }
    }

    private async Task<bool> SaveTorrentFileForUserAsync(string userId, BencodeNET.Torrents.Torrent torrent)
    {
        var user = await this.dal.User.LoadAsync(userId).ConfigureAwait(false);
        if (user == null)
        {
            this.logger.LogError($"User '{userId}' not found.");
            return false;
        }

        TorrentFile torrentFile;
        lock (TorrentFileLocker)
        {
            torrent.FixTrackers(this.configuration.GetTorrentAnnounceList(user.TrackerId));
            torrentFile = torrent.ToTorrentFile();
        }

        await this.dal.TorrentFile.SaveUserFileAsync(userId, torrentFile).ConfigureAwait(false);
        return true;
    }

    private async Task UpdateUserFeedAsync(string userId, FeedItemResponse item, string torrentFileName)
    {
        string link = LostFilmMonitoringBllExtensions.GenerateTorrentLink(this.configuration.BaseUrl, userId, torrentFileName);
        var userFeedItem = ToFeedItem(item, link);
        var userFeed = (await this.dal.Feed.LoadUserFeedAsync(userId).ConfigureAwait(false)) ?? new SortedSet<FeedItem>();
        userFeed.Add(userFeedItem);
        userFeed.RemoveWhere(x => x == null);
        await this.dal.Feed.SaveUserFeedAsync(userId, userFeed.Take(15).ToArray()).ConfigureAwait(false);
        await this.CleanupOldRssFilesAsync(userId, userFeed.Skip(15).ToArray()).ConfigureAwait(false);
    }

    private async Task CleanupOldRssFileAsync(string userId, FeedItem item)
    {
        this.logger.Info($"Call: {nameof(this.CleanupOldRssFileAsync)}('{userId}', '{item.Link}')");
        var fileName = item.GetUserFileName(userId);
        if (fileName == null)
        {
            this.logger.LogError($"Cannot get fileName from FeedItem '{item.Link}'.");
            return;
        }

        try
        {
            await this.dal.TorrentFile.DeleteUserFileAsync(userId, fileName).ConfigureAwait(false);
        }
        catch (ExternalServiceUnavailableException ex)
        {
            this.logger.Log($"Error deleting FeedItem '{item.Link}' for user '{userId}'.", ex);
        }
    }

    private Task CleanupOldRssFilesAsync(string userId, FeedItem[] oldRssFeedItems)
        => Task.WhenAll(oldRssFeedItems.Select(i => this.CleanupOldRssFileAsync(userId, i)));
}
