// <copyright file="SeriesCoverService.cs" company="Alexander Panfilenok">
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
    using System.IO;
    using System.Threading.Tasks;
    using LostFilmMonitoring.Common;
    using LostFilmTV.Client;

    /// <summary>
    /// Manages series cover images.
    /// </summary>
    public class SeriesCoverService
    {
        private readonly string seriesCoverDirectoryPath;
        private readonly Client lostFileClient;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesCoverService"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="lostFileClient">Client.</param>
        public SeriesCoverService(ILogger logger, Client lostFileClient)
        {
            this.seriesCoverDirectoryPath = Configuration.GetImagesPath();
            this.logger = logger == null ? throw new ArgumentNullException(nameof(logger)) : logger.CreateScope(nameof(SeriesCoverService));
            this.lostFileClient = lostFileClient ?? throw new ArgumentNullException(nameof(lostFileClient));
        }

        /// <summary>
        /// Downloads series cover image if it was not previously downloaded.
        /// </summary>
        /// <param name="seriesName">Series name.</param>
        /// <returns>Awaitable task.</returns>
        public async Task EnsureCoverDownloadedAsync(string seriesName)
        {
            this.logger.Info($"Call: {nameof(this.EnsureCoverDownloadedAsync)}('{seriesName}')");
            var coverPath = Path.Combine(this.seriesCoverDirectoryPath, seriesName.EscapePath() + ".jpg");
            if (File.Exists(coverPath))
            {
                return;
            }

            using (var imageStream = await this.lostFileClient.DownloadSeriesCoverAsync(seriesName))
            {
                if (imageStream == null)
                {
                    return;
                }

                using (var fileStream = File.Create(coverPath))
                {
                    imageStream.Seek(0, SeekOrigin.Begin);
                    imageStream.CopyTo(fileStream);
                }
            }
        }
    }
}
