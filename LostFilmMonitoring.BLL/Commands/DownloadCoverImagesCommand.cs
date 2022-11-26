// <copyright file="DownloadCoverImagesCommand.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.BLL.Commands
{
    /// <summary>
    /// Checks series cover images.
    /// </summary>
    public class DownloadCoverImagesCommand : ICommand
    {
        private const string LostFilmIdRegex = "static\\.lostfilm\\.top\\/Images\\/(\\d+)\\/Posters\\/image\\.jpg";
        private readonly ILogger logger;
        private readonly IFileSystem fileSystem;
        private readonly IConfiguration configuration;
        private readonly ISeriesDao seriesDao;
        private readonly IRssFeed rssFeed;
        private readonly ILostFilmClient lostFilmClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadCoverImagesCommand"/> class.
        /// </summary>
        /// <param name="logger">Instance of <see cref="ILogger"/>.</param>
        /// <param name="fileSystem">Instance of <see cref="IFileSystem"/>.</param>
        /// <param name="configuration">Instance of <see cref="IConfiguration"/>.</param>
        /// <param name="lostFilmRssFeed">Instance of <see cref="LostFilmTV.Client.RssFeed.LostFilmRssFeed"/>.</param>
        /// <param name="seriesDao">Instance of <see cref="ISeriesDao"/>.</param>
        /// <param name="lostFilmClient">Instance of <see cref="ILostFilmClient"/>.</param>
        public DownloadCoverImagesCommand(
            ILogger logger,
            IFileSystem fileSystem,
            IConfiguration configuration,
            IRssFeed lostFilmRssFeed,
            ISeriesDao seriesDao,
            ILostFilmClient lostFilmClient)
        {
            this.logger = logger?.CreateScope(nameof(DownloadCoverImagesCommand)) ?? throw new ArgumentNullException(nameof(logger));
            this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.rssFeed = lostFilmRssFeed ?? throw new ArgumentNullException(nameof(lostFilmRssFeed));
            this.seriesDao = seriesDao ?? throw new ArgumentNullException(nameof(seriesDao));
            this.lostFilmClient = lostFilmClient ?? throw new ArgumentNullException(nameof(lostFilmClient));
        }

        /// <inheritdoc/>
        public async Task ExecuteAsync()
        {
            this.logger.Info($"Call: {nameof(this.ExecuteAsync)}()");
            var items = await this.rssFeed.LoadFeedItemsAsync();
            if (items == null)
            {
                return;
            }

            await Task.WhenAll(items.Select(this.CheckItemAsync));
        }

        private async Task CheckItemAsync(FeedItemResponse item)
        {
            var lostFilmId = this.ParseLostFilmId(item);
            if (lostFilmId == null)
            {
                return;
            }

            if (await this.PosterExistsAsync(lostFilmId))
            {
                return;
            }

            await this.DownloadImageAsync(lostFilmId);
            await this.UpdateSeriesAsync(item?.SeriesName, lostFilmId);
        }

        private string? ParseLostFilmId(FeedItemResponse item)
        {
            if (string.IsNullOrEmpty(item?.Description))
            {
                return null;
            }

            try
            {
                var lostFilmIdMatch = Regex.Match(item.Description, LostFilmIdRegex, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));
                if (!lostFilmIdMatch.Success)
                {
                    this.logger.Error($"Regex match failure: {item.Description}");
                    return null;
                }

                return lostFilmIdMatch.Groups[1].Value;
            }
            catch (RegexMatchTimeoutException)
            {
                this.logger.Error($"Regex match timeout: {item.Description}");
                return null;
            }
        }

        private async Task UpdateSeriesAsync(string? seriesName, string lostFilmId)
        {
            if (string.IsNullOrEmpty(seriesName))
            {
                return;
            }

            var series = await this.seriesDao.LoadAsync(seriesName);
            if (series == null)
            {
                return;
            }

            if (series.LostFilmId == null)
            {
                series.LostFilmId = int.Parse(lostFilmId);
                await this.seriesDao.SaveAsync(series);
            }
        }

        private async Task DownloadImageAsync(string lostFilmId)
        {
            using var imageStream = await this.lostFilmClient.DownloadImageAsync(lostFilmId);
            if (imageStream == null)
            {
                return;
            }

            await this.fileSystem.SaveAsync(this.configuration.ImagesDirectory, $"{lostFilmId}.jpg", "image/jpeg", imageStream);
        }

        private Task<bool> PosterExistsAsync(string lostFilmId) =>
            this.fileSystem.ExistsAsync(this.configuration.ImagesDirectory, $"{lostFilmId}.jpg");
    }
}
