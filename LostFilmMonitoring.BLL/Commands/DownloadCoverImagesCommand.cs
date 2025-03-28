// <copyright file="DownloadCoverImagesCommand.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.BLL.Commands;

/// <summary>
/// Checks series cover images.
/// </summary>
public class DownloadCoverImagesCommand : ICommand
{
    private readonly ILogger logger;
    private readonly IFileSystem fileSystem;
    private readonly ISeriesDao seriesDao;
    private readonly ITmdbClient tmdbClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadCoverImagesCommand"/> class.
    /// </summary>
    /// <param name="logger">Instance of <see cref="ILogger"/>.</param>
    /// <param name="fileSystem">Instance of <see cref="IFileSystem"/>.</param>
    /// <param name="seriesDao">Instance of <see cref="ISeriesDao"/>.</param>
    /// <param name="tmdbClient">Instance of <see cref="ITmdbClient"/>.</param>
    public DownloadCoverImagesCommand(
        ILogger logger,
        IFileSystem fileSystem,
        ISeriesDao seriesDao,
        ITmdbClient tmdbClient)
    {
        this.logger = logger?.CreateScope(nameof(DownloadCoverImagesCommand)) ?? throw new ArgumentNullException(nameof(logger));
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        this.seriesDao = seriesDao ?? throw new ArgumentNullException(nameof(seriesDao));
        this.tmdbClient = tmdbClient ?? throw new ArgumentNullException(nameof(tmdbClient));
    }

    /// <inheritdoc/>
    public async Task ExecuteAsync()
    {
        this.logger.Info($"Call: {nameof(this.ExecuteAsync)}()");
        var series = await this.seriesDao.LoadAsync();
        foreach (var seriesItem in series)
        {
            if (!await this.PosterExistsAsync(seriesItem.Id))
            {
                await this.DownloadImageAsync(seriesItem);
            }
        }
    }

    private async Task DownloadImageAsync(Series series)
    {
        var openBraceIndex = series.Name.IndexOf('(');
        var closeBraceIndex = series.Name.IndexOf(')');
        if (openBraceIndex == -1 || closeBraceIndex == -1 || openBraceIndex > closeBraceIndex)
        {
            // cannot parse the series original name
            return;
        }

        var originalName = series.Name.Substring(openBraceIndex + 1, closeBraceIndex - openBraceIndex - 1);
        using var imageStream = await this.tmdbClient.DownloadImageAsync(originalName);
        if (imageStream == null)
        {
            return;
        }

        await this.fileSystem.SaveAsync(Constants.MetadataStorageContainerImages, $"{series.Id}.jpg", "image/jpeg", imageStream);
    }

    private Task<bool> PosterExistsAsync(Guid seriesId) =>
        this.fileSystem.ExistsAsync(Constants.MetadataStorageContainerImages, $"{seriesId}.jpg");
}
