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
        private readonly ILogger logger;
        private readonly IFileSystem fileSystem;
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly LostFilmTV.Client.RssFeed.LostFilmRssFeed lostFilmRssFeed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadCoverImagesCommand"/> class.
        /// </summary>
        /// <param name="logger">Instance of <see cref="ILogger"/>.</param>
        /// <param name="fileSystem">Instance of <see cref="IFileSystem"/>.</param>
        /// <param name="configuration">Instance of <see cref="IConfiguration"/>.</param>
        /// <param name="httpClientFactory">Instance of <see cref="IHttpClientFactory"/>.</param>
        /// <param name="lostFilmRssFeed">Instance of <see cref="LostFilmTV.Client.RssFeed.LostFilmRssFeed"/>.</param>
        public DownloadCoverImagesCommand(
            ILogger logger,
            IFileSystem fileSystem,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            LostFilmTV.Client.RssFeed.LostFilmRssFeed lostFilmRssFeed)
        {
            this.logger = logger?.CreateScope(nameof(DownloadCoverImagesCommand)) ?? throw new ArgumentNullException(nameof(logger));
            this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(logger));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(logger));
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(logger));
            this.lostFilmRssFeed = lostFilmRssFeed ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task ExecuteAsync()
        {
            this.logger.Info($"Call: {nameof(this.ExecuteAsync)}()");
            var items = await this.lostFilmRssFeed.LoadFeedItemsAsync();
            if (items == null)
            {
                return;
            }

            await Task.WhenAll(items.Select(this.CheckItemAsync));
        }

        private static string EscapePath(string path)
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

        private async Task CheckItemAsync(FeedItemResponse item)
        {
            var seriesName = item.Title[.. (item.Title.IndexOf(").") + 2)];
            var fileName = EscapePath(seriesName) + ".jpg";
            if (await this.fileSystem.ExistsAsync(this.configuration.ImagesDirectory, fileName))
            {
                return;
            }

            var match = Regex.Match(item.Description, "Images/(\\d+)/Posters");
            if (!match.Success)
            {
                return;
            }

            var id = match.Groups[1].Value;

            using var httpClient = this.httpClientFactory.CreateClient();
            using var imageStream = await httpClient.GetStreamAsync($"https://static.lostfilm.top/Images/{id}/Posters/t_shmoster_s1.jpg");
            if (imageStream == null)
            {
                return;
            }

            await this.fileSystem.SaveAsync(this.configuration.ImagesDirectory, fileName, imageStream);
        }
    }
}
