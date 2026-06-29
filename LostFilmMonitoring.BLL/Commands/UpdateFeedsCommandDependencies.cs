namespace LostFilmMonitoring.BLL.Commands;

/// <summary>
/// Encapsulates all dependencies required by UpdateFeedsCommand to reduce constructor parameter count.
/// </summary>
public class UpdateFeedsCommandDependencies
{
    /// <summary>
    /// Gets or sets the logger instance.
    /// </summary>
    public required ILogger Logger { get; set; }

    /// <summary>
    /// Gets or sets the RSS feed loader.
    /// </summary>
    public required IRssFeed RssFeed { get; set; }

    /// <summary>
    /// Gets or sets the data access layer.
    /// </summary>
    public required IDal Dal { get; set; }

    /// <summary>
    /// Gets or sets the configuration provider.
    /// </summary>
    public required IConfiguration Configuration { get; set; }

    /// <summary>
    /// Gets or sets the model persister.
    /// </summary>
    public required IModelPersister ModelPersister { get; set; }

    /// <summary>
    /// Gets or sets the LostFilm client.
    /// </summary>
    public required ILostFilmClient Client { get; set; }

    /// <summary>
    /// Gets or sets the torrent file helper.
    /// </summary>
    public required ITorrentFileHelper TorrentFileHelper { get; set; }

    /// <summary>
    /// Gets or sets the command for downloading cover images.
    /// </summary>
    public required ICommand<Series> DownloadCoverImagesCommand { get; set; }
}
