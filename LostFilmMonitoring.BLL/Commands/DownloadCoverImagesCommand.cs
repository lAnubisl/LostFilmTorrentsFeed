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
    private readonly IConfiguration configuration;
    private readonly ISeriesDao seriesDao;
    private readonly ILostFilmClient lostFilmClient;
    private readonly IDictionaryDao dictionaryDao;

    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadCoverImagesCommand"/> class.
    /// </summary>
    /// <param name="logger">Instance of <see cref="ILogger"/>.</param>
    /// <param name="fileSystem">Instance of <see cref="IFileSystem"/>.</param>
    /// <param name="configuration">Instance of <see cref="IConfiguration"/>.</param>
    /// <param name="seriesDao">Instance of <see cref="ISeriesDao"/>.</param>
    /// <param name="lostFilmClient">Instance of <see cref="ILostFilmClient"/>.</param>
    /// <param name="dictionaryDao">Instance of <see cref="IDictionaryDao"/>.</param>
    public DownloadCoverImagesCommand(
        ILogger logger,
        IFileSystem fileSystem,
        IConfiguration configuration,
        ISeriesDao seriesDao,
        ILostFilmClient lostFilmClient,
        IDictionaryDao dictionaryDao)
    {
        this.logger = logger?.CreateScope(nameof(DownloadCoverImagesCommand)) ?? throw new ArgumentNullException(nameof(logger));
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        this.seriesDao = seriesDao ?? throw new ArgumentNullException(nameof(seriesDao));
        this.lostFilmClient = lostFilmClient ?? throw new ArgumentNullException(nameof(lostFilmClient));
        this.dictionaryDao = dictionaryDao ?? throw new ArgumentNullException(nameof(dictionaryDao));
    }

    /// <inheritdoc/>
    public async Task ExecuteAsync()
    {
        this.logger.Info($"Call: {nameof(this.ExecuteAsync)}()");
        var series = await this.seriesDao.LoadAsync();
        var dictionary = await this.dictionaryDao.LoadAsync();
        foreach (var seriesItem in series)
        {
            if (seriesItem.LostFilmId == null)
            {
                var openBraceIndex = seriesItem.Name.IndexOf('(');
                var closeBraceIndex = seriesItem.Name.IndexOf(')');
                if (openBraceIndex == -1 || closeBraceIndex == -1 || openBraceIndex > closeBraceIndex)
                {
                    // cannot parse the series original name
                    continue;
                }

                var originalName = seriesItem.Name.Substring(openBraceIndex + 1, closeBraceIndex - openBraceIndex - 1);
                if (!dictionary.ContainsKey(originalName))
                {
                    // the series is not found in the dictionary
                    continue;
                }

                seriesItem.LostFilmId = dictionary[originalName];
                await this.seriesDao.SaveAsync(seriesItem);
            }

            if (seriesItem.LostFilmId == null)
            {
                // cannot continue without LostFilmId
                continue;
            }

            var lostFilmId = seriesItem.LostFilmId.ToString()!;
            if (await this.PosterExistsAsync(lostFilmId))
            {
                await this.DownloadImageAsync(lostFilmId);
            }
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
