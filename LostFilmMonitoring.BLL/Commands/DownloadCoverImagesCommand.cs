namespace LostFilmMonitoring.BLL.Commands;

/// <summary>
/// Checks series cover images.
/// </summary>
public class DownloadCoverImagesCommand : ICommand
{
    private readonly ILogger logger;
    private readonly ISeriesDao seriesDao;
    private readonly ICommand<Series> downloadCoverImageCommand;

    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadCoverImagesCommand"/> class.
    /// </summary>
    /// <param name="logger">Instance of <see cref="ILogger"/>.</param>
    /// <param name="seriesDao">Instance of <see cref="ISeriesDao"/>.</param>
    /// <param name="downloadCoverImageCommand">Instance of <see cref="ICommand{Series}"/>.</param>
    public DownloadCoverImagesCommand(
        ILogger logger,
        ISeriesDao seriesDao,
        ICommand<Series> downloadCoverImageCommand)
    {
        this.logger = logger?.CreateScope(nameof(DownloadCoverImagesCommand)) ?? throw new ArgumentNullException(nameof(logger));
        this.seriesDao = seriesDao ?? throw new ArgumentNullException(nameof(seriesDao));
        this.downloadCoverImageCommand = downloadCoverImageCommand ?? throw new ArgumentNullException(nameof(downloadCoverImageCommand));
    }

    /// <inheritdoc/>
    public async Task ExecuteAsync()
    {
        this.logger.Info($"Call: {nameof(this.ExecuteAsync)}()");
        var series = await this.seriesDao.LoadAsync();
        foreach (var seriesItem in series)
        {
            await this.downloadCoverImageCommand.ExecuteAsync(seriesItem);
        }
    }
}