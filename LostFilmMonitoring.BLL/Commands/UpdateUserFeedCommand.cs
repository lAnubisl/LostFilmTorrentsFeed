// <copyright file="UpdateUserFeedCommand.cs" company="Alexander Panfilenok">
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
        return new UpdateUserFeedCommandResponseModel
        {
            Success = await this.UpdateSubscribedUserAsync(model.FeedResponseItem!, model.Torrent!, model.UserId!),
        };
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

        TorrentFile? torrentFile = null;
        lock (TorrentFileLocker)
        {
            torrent.FixTrackers(this.configuration.GetTorrentAnnounceList(user.TrackerId));
            torrentFile = torrent.ToTorrentFile();
        }

        await this.dal.TorrentFile.SaveUserFileAsync(userId, torrentFile!);
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