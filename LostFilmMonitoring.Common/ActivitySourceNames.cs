// <copyright file="ActivitySourceNames.cs" company="Alexander Panfilenok">
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
/// Contains all activity source names for an application.
/// </summary>
public static class ActivitySourceNames
{
    /// <summary>
    /// Name of the activity source for Azure Blob Storage.
    /// </summary>
    public const string BlobStorage = "BlobStorage";

    /// <summary>
    /// Name of the activity source for Azure Table Storage.
    /// </summary>
    public const string TableStorage = "TableStorage";

    /// <summary>
    /// Command for updating feeds.
    /// </summary>
    public const string UpdateFeedsCommand = "UpdateFeedsCommand";

    /// <summary>
    /// Name of the activity source for RSS feeds.
    /// </summary>
    public const string RssFeed = "RssFeed";

    /// <summary>
    /// Gets an array of activity source names used for monitoring and logging purposes.
    /// </summary>
    /// <value>
    /// An array of strings representing the names of activity sources, such as BlobStorage, TableStorage, and UpdateFeedsCommand.
    /// </value>
    public static string[] ActivitySources =>
    [
        BlobStorage, TableStorage, UpdateFeedsCommand, RssFeed
    ];
}