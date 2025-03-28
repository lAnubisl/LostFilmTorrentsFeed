// <copyright file="UpdateUserFeedCommandRequestModel.cs" company="Alexander Panfilenok">
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
    /// <param name="torrent">Torrent.</param>
    public UpdateUserFeedCommandRequestModel(string? userId, FeedItemResponse? feedResponseItem, BencodeNET.Torrents.Torrent? torrent)
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
    /// Gets torrent.
    /// </summary>
    public BencodeNET.Torrents.Torrent? Torrent { get; }
}
