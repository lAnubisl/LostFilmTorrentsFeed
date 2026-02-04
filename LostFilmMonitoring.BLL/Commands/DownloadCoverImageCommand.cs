namespace LostFilmMonitoring.BLL.Commands;

/// <summary>
/// Checks series cover image.
/// </summary>
public class DownloadCoverImageCommand : ICommand<Series>
{
    private readonly ILogger logger;
    private readonly IFileSystem fileSystem;
    private readonly ITmdbClient tmdbClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadCoverImageCommand"/> class.
    /// </summary>
    /// <param name="logger">Instance of <see cref="ILogger"/>.</param>
    /// <param name="fileSystem">Instance of <see cref="IFileSystem"/>.</param>
    /// <param name="tmdbClient">Instance of <see cref="ITmdbClient"/>.</param>
    public DownloadCoverImageCommand(
        ILogger logger,
        IFileSystem fileSystem,
        ITmdbClient tmdbClient)
    {
        this.logger = logger?.CreateScope(nameof(DownloadCoverImageCommand)) ?? throw new ArgumentNullException(nameof(logger));
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        this.tmdbClient = tmdbClient ?? throw new ArgumentNullException(nameof(tmdbClient));
    }

    /// <inheritdoc/>
    public async Task ExecuteAsync(Series? series)
    {
        ArgumentNullException.ThrowIfNull(series);
        this.logger.Info($"Call: {nameof(this.ExecuteAsync)}()");
        if (await this.PosterExistsAsync(series.Id))
        {
            return;
        }

        var openBraceIndex = series.Name.IndexOf('(');
        var closeBraceIndex = series.Name.IndexOf(')');
        if (openBraceIndex == -1 || closeBraceIndex == -1 || openBraceIndex > closeBraceIndex)
        {
            // cannot parse the series original name
            this.logger.Error($"Cannot parse original name for series '{series.Name}'");
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
