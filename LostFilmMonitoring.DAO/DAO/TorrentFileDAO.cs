// <copyright file="TorrentFileDAO.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.DAO
{
    using System.IO;
    using System.Threading.Tasks;
    using LostFilmMonitoring.Common;
    using LostFilmMonitoring.DAO.DomainModels;

    /// <summary>
    /// Provides functionality for managing torrent files on disk.
    /// </summary>
    public class TorrentFileDAO
    {
        private readonly string torrentFilesDirectoryPath;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TorrentFileDAO"/> class.
        /// </summary>
        /// <param name="configuration">IConfiguration.</param>
        /// <param name="logger">Logger.</param>
        public TorrentFileDAO(IConfiguration configuration, ILogger logger)
        {
            this.torrentFilesDirectoryPath = configuration.TorrentPath;
            this.logger = logger.CreateScope(nameof(TorrentFileDAO));
        }

        /// <summary>
        /// Save torrent file on disk.
        /// </summary>
        /// <param name="fileName">FileName.</param>
        /// <param name="fileContentStream">FileContentStream.</param>
        /// <param name="torrentId">TorrentId.</param>
        /// <returns>Awaitable task.</returns>
        public async Task SaveAsync(string fileName, Stream fileContentStream, int torrentId)
        {
            fileName = $"{torrentId}_{fileName}";
            using (var fs = new FileStream(
                Path.Combine(this.torrentFilesDirectoryPath, fileName),
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                useAsync: true))
            {
                await fileContentStream.CopyToAsync(fs);
            }

            this.logger.Info($"File '{fileName}' downloaded.");
        }

        /// <summary>
        /// Tries to find torrent file by torrent id.
        /// </summary>
        /// <param name="torrentId">Torrent id.</param>
        /// <returns>TorrentFile if it is found. Otherwise Null.</returns>
        public TorrentFile TryFind(int torrentId)
        {
            var directory = new DirectoryInfo(this.torrentFilesDirectoryPath);
            var files = directory.GetFiles($"{torrentId}_*.torrent");
            if (files.Length == 0)
            {
                return null;
            }

            return new TorrentFile
            {
                Stream = files[0].OpenRead(),
                FileName = files[0].Name,
            };
        }
    }
}
