namespace LostFilmMonitoring.BLL.Commands;

/// <summary>
/// Encapsulates all dependencies required by UpdateFeedsCommand to reduce constructor parameter count.
/// </summary>
public class UpdateFeedsCommandDependencies
{
    /// <summary>
    /// Gets or sets the logger instance.
    /// </summary>
    required public ILogger Logger { get; set; }

    /// <summary>
    /// Gets or sets the RSS feed loader.
    /// </summary>
    required public IRssFeed RssFeed { get; set; }

    /// <summary>
    /// Gets or sets the data access layer.
    /// </summary>
    required public IDal Dal { get; set; }

    /// <summary>
    /// Gets or sets the configuration provider.
    /// </summary>
    required public IConfiguration Configuration { get; set; }

    /// <summary>
    /// Gets or sets the model persister.
    /// </summary>
    required public IModelPersister ModelPersister { get; set; }

    /// <summary>
    /// Gets or sets the LostFilm client.
    /// </summary>
    required public ILostFilmClient Client { get; set; }

    /// <summary>
    /// Gets or sets the torrent file helper.
    /// </summary>
    required public ITorrentFileHelper TorrentFileHelper { get; set; }

    /// <summary>
    /// Gets or sets the command for downloading cover images.
    /// </summary>
    required public ICommand<Series> DownloadCoverImagesCommand { get; set; }
}
