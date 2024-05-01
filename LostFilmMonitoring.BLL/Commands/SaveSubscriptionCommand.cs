// <copyright file="SaveSubscriptionCommand.cs" company="Alexander Panfilenok">
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
    public async Task<EditSubscriptionResponseModel> ExecuteAsync(EditSubscriptionRequestModel? request)
    {
        this.logger.Info($"Call: {nameof(this.ExecuteAsync)}(EditSubscriptionRequestModel model)");
        if (request == null)
        {
            return new EditSubscriptionResponseModel(ValidationResult.Fail(ErrorMessages.RequestNull));
        }

        if (request.UserId == null)
        {
            return new EditSubscriptionResponseModel(ValidationResult.Fail(nameof(EditSubscriptionRequestModel.UserId), ErrorMessages.FieldEmpty));
        }

        if (request.Items == null)
        {
            return new EditSubscriptionResponseModel(ValidationResult.Fail(nameof(EditSubscriptionRequestModel.Items), ErrorMessages.FieldEmpty));
        }

        var validationResult = await this.validator.ValidateAsync(request).ConfigureAwait(false);
        if (!validationResult.IsValid)
        {
            return new EditSubscriptionResponseModel(validationResult);
        }

        var newUserSubscriptions = SubscriptionItem.ToSubscriptions(request.Items);
        await this.UpdateUserFeedItemsAsync(request.UserId, newUserSubscriptions).ConfigureAwait(false);
        await this.dal.Subscription.SaveAsync(request.UserId, newUserSubscriptions).ConfigureAwait(false);
        await this.UpdatePresentationModelAsync(request.UserId, request.Items).ConfigureAwait(false);
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
            Quality.SD => LostFilmTV.Client.LostFilmTvExtensions.GetTorrentId(series.LastEpisodeTorrentLinkSD),
            Quality.H1080 => LostFilmTV.Client.LostFilmTvExtensions.GetTorrentId(series.LastEpisodeTorrentLink1080),
            Quality.H720 => LostFilmTV.Client.LostFilmTvExtensions.GetTorrentId(series.LastEpisodeTorrentLinkMP4),
            _ => throw new InvalidOperationException("Quality not supported"),
        };
    }

    private Task UpdatePresentationModelAsync(string userId, SubscriptionItem[] selectedSubscriptions)
        => this.persister.PersistAsync($"subscription_{userId}", selectedSubscriptions);

    private async Task UpdateUserFeedItemsAsync(string userId, Subscription[] selectedSubscriptions)
    {
        var userSubscriptionsTask = this.dal.Subscription.LoadAsync(userId);
        var userFeedItemsTask = this.dal.Feed.LoadUserFeedAsync(userId);
        var userTask = this.dal.User.LoadAsync(userId);
        await Task.WhenAll(userSubscriptionsTask, userFeedItemsTask, userTask).ConfigureAwait(false);
        var userFeedItems = await userFeedItemsTask.ConfigureAwait(false) ?? new SortedSet<FeedItem>();
        var user = await userTask.ConfigureAwait(false);
        if (user == null)
        {
            this.logger.LogError($"User for id '{userId}' not found.");
            return;
        }

        var newSubscriptions = Subscription.Filter(selectedSubscriptions, await userSubscriptionsTask.ConfigureAwait(false));
        await Task.WhenAll(newSubscriptions.Select(async s => await this.UpdateUserFeedItemsAsync(user, userFeedItems, s).ConfigureAwait(false))).ConfigureAwait(false);
        await this.dal.Feed.SaveUserFeedAsync(userId, userFeedItems.Take(15).ToArray()).ConfigureAwait(false);
        await this.CleanupOldRssFilesAsync(userId, userFeedItems.Skip(15).ToArray()).ConfigureAwait(false);
    }

    private async Task UpdateUserFeedItemsAsync(User user, SortedSet<FeedItem> userFeedItems, Subscription subscription)
    {
        var series = await this.dal.Series.LoadAsync(subscription.SeriesName).ConfigureAwait(false);
        if (series == null)
        {
            this.logger.LogError($"Tried to update users feed with series '{subscription.SeriesName}' but haven't found this series in the storage. ");
            return;
        }

        var torrentId = GetTorrentId(series, subscription.Quality);
        if (torrentId == null)
        {
            return;
        }

        var torrentFile = await this.dal.TorrentFile.LoadBaseFileAsync(torrentId).ConfigureAwait(false);
        if (torrentFile?.Stream == null)
        {
            return;
        }

        var torrent = ToTorrentDataStructure(torrentFile);
        torrent.FixTrackers(this.configuration.GetTorrentAnnounceList(user.TrackerId));
        var userTorrentFile = torrent.ToTorrentFile();
        if (userTorrentFile.FileName == null)
        {
            this.logger.LogError($"File name for torrent id '{torrent}' is null.");
            return;
        }

        await this.dal.TorrentFile.SaveUserFileAsync(user.Id, userTorrentFile).ConfigureAwait(false);
        FeedItem? itemToRemove;
        lock (this.locker)
        {
            itemToRemove = AddFeedItem(
                userFeedItems,
                series.LastEpisodeName,
                LostFilmMonitoringBllExtensions.GenerateTorrentLink(this.configuration.BaseUrl, user.Id, userTorrentFile.FileName));
        }

        if (itemToRemove != null)
        {
            await this.CleanupOldRssFileAsync(user.Id, itemToRemove).ConfigureAwait(false);
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

            await this.dal.TorrentFile.DeleteUserFileAsync(userId, fileName).ConfigureAwait(false);
        }
        catch (ExternalServiceUnavailableException ex)
        {
            this.logger.Log(ex);
        }
    }

    private Task CleanupOldRssFilesAsync(string userId, FeedItem[] oldRssFeedItems)
        => Task.WhenAll(oldRssFeedItems.Select(i => this.CleanupOldRssFileAsync(userId, i)));
}
