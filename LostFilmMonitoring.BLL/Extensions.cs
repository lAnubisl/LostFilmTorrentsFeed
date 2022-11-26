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
    /// <summary>
    /// Usefull extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Generates torrent link for user's feed.
        /// </summary>
        /// <param name="baseUrl">Base website URL.</param>
        /// <param name="userId">User Id.</param>
        /// <param name="torrentFileName">Torrent Id.</param>
        /// <returns>Torrent link.</returns>
        internal static string GenerateTorrentLink(string baseUrl, string userId, string torrentFileName)
        {
            return $"{baseUrl}/{userId}/{torrentFileName}.torrent";
        }

        /// <summary>
        /// Replaces trackers for an instance of <see cref="BencodeNET.Torrents.Torrent"/>.
        /// </summary>
        /// <param name="torrent">Instance of <see cref="BencodeNET.Torrents.Torrent"/>.</param>
        /// <param name="announces">Array of trackers.</param>
        internal static void FixTrackers(this BencodeNET.Torrents.Torrent torrent, string[] announces)
        {
            torrent.Trackers = announces.Select(a => new List<string>() { a } as IList<string>).ToList();
        }

        /// <summary>
        /// Map instance of <see cref="BencodeNET.Torrents.Torrent"/> to <see cref="TorrentFile"/>.
        /// </summary>
        /// <param name="torrent">Instance of <see cref="BencodeNET.Torrents.Torrent"/>.</param>
        /// <returns>Instance of <see cref="TorrentFile"/>.</returns>
        internal static TorrentFile ToTorrentFile(this BencodeNET.Torrents.Torrent torrent)
            => new(torrent.DisplayNameUtf8 ?? torrent.DisplayName, torrent.ToStream());

        /// <summary>
        /// Generate instance of <see cref="BencodeNET.Torrents.Torrent"/> from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">An instance of <see cref="Stream"/> to torrent file.</param>
        /// <returns>Instance of <see cref="BencodeNET.Torrents.Torrent"/>.</returns>
        internal static BencodeNET.Torrents.Torrent ToTorrentDataStructure(this Stream stream)
        {
            var parser = new BencodeNET.Torrents.TorrentParser(BencodeNET.Torrents.TorrentParserMode.Tolerant);
            var torrent = parser.Parse(new BencodeNET.IO.BencodeReader(stream));
            torrent.IsPrivate = false;
            stream.Position = 0;
            return torrent;
        }

        private static Stream ToStream(this BencodeNET.Torrents.Torrent torrent)
        {
            var ms = new MemoryStream();
            torrent.EncodeTo(ms);
            ms.Position = 0;
            return ms;
        }
    }
}
