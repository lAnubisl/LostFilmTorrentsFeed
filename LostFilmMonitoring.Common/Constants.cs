// <copyright file="Constants.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.Common;

/// <summary>
/// Constants.
/// </summary>
public static class Constants
{
    /// <summary>
    /// Table Storage dictionary table name.
    /// </summary>
    public const string MetadataStorageTableNameDictionary = "dictionary";

    /// <summary>
    /// Table Storage episodes table name.
    /// </summary>
    public const string MetadataStorageTableNameEpisodes = "episodes";

    /// <summary>
    /// Table Storage series table name.
    /// </summary>
    public const string MetadataStorageTableNameSeries = "series";

    /// <summary>
    /// Table Storage subscriptions table name.
    /// </summary>
    public const string MetadataStorageTableNameSubscriptions = "subscriptions";

    /// <summary>
    /// Table Storage users table name.
    /// </summary>
    public const string MetadataStorageTableNameUsers = "users";

    /// <summary>
    /// Blob Storage container name for images.
    /// </summary>
    public const string MetadataStorageContainerImages = "images";

    /// <summary>
    /// Blob Storage container name for base torrents.
    /// </summary>
    public const string MetadataStorageContainerBaseTorrents = "basetorrents";

    /// <summary>
    /// Blob Storage container name for models.
    /// </summary>
    public const string MetadataStorageContainerModels = "models";

    /// <summary>
    /// Blob Storage container name for RSS feeds.
    /// </summary>
    public const string MetadataStorageContainerRssFeeds = "rssfeeds";

    /// <summary>
    /// Blob Storage container name for user torrents.
    /// </summary>
    public const string MetadataStorageContainerUserTorrents = "usertorrents";
}