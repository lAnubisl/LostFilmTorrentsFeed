// <copyright file="EnvironmentVariables.cs" company="Alexander Panfilenok">
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
public static class EnvironmentVariables
{
    /// <summary>
    /// Metadata Storage Account Name.
    /// </summary>
    public static readonly string MetadataStorageAccountName = "MetadataStorageAccountName";

    /// <summary>
    /// Metadata Storage Account Key.
    /// </summary>
    public static readonly string MetadataStorageAccountKey = "MetadataStorageAccountKey";

    /// <summary>
    /// TMDB Api Key.
    /// </summary>
    public static readonly string TmdbApiKey = "TMDB_API_KEY";

    /// <summary>
    /// Base URI for website.
    /// </summary>
    public static readonly string BaseUrl = "BASEURL";

    /// <summary>
    /// LostFilm Base Cookie.
    /// </summary>
    public static readonly string BaseFeedCookie = "BASEFEEDCOOKIE";

    /// <summary>
    /// LostFilm Base User Identifier.
    /// </summary>
    public static readonly string BaseLinkUID = "BASELINKUID";

    /// <summary>
    /// List of torrent trackers.
    /// </summary>
    public static readonly string TorrentTrackers = "TORRENTTRACKERS";
}
