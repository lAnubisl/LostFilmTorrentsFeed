// <copyright file="Extensions.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2021 Alexander Panfilenok
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

namespace LostFilmMonitoring.BLL
{
    using System;

    /// <summary>
    /// Usefull extensions.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Escapes danger characters.
        /// </summary>
        /// <param name="path">Original path.</param>
        /// <returns>Escaped path.</returns>
        public static string EscapePath(this string path)
        {
            return path
                .Replace(":", "_")
                .Replace("*", "_")
                .Replace("\"", "_")
                .Replace("/", "_")
                .Replace("?", "_")
                .Replace(">", "_")
                .Replace("<", "_")
                .Replace("|", "_");
        }

        /// <summary>
        /// Generates torrent link for user's feed.
        /// </summary>
        /// <param name="baseUrl">Base website URL.</param>
        /// <param name="userId">User Id.</param>
        /// <param name="torrentId">Torrent Id.</param>
        /// <returns>Torrent link.</returns>
        internal static string GenerateTorrentLink(string baseUrl, Guid userId, string torrentId)
        {
            return $"{baseUrl}/Rss/{userId}/{torrentId}";
        }
    }
}
