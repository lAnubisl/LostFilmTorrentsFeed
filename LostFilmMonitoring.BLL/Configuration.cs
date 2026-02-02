namespace LostFilmMonitoring.BLL;

/// <inheritdoc/>
public class Configuration : IConfiguration
{
    private readonly string[] torrentAnnounceListPatterns;

    /// <summary>
    /// Initializes a new instance of the <see cref="Configuration"/> class.
    /// </summary>
    /// <param name="provider">Instance of <see cref="IConfigurationValuesProvider"/>.</param>
    public Configuration(IConfigurationValuesProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider, nameof(provider));
        this.BaseUrl = provider.GetValue(EnvironmentVariables.BaseUrl) ?? throw new InvalidOperationException($"Environment variable '{EnvironmentVariables.BaseUrl}' is not defined.");
        this.BaseUSESS = provider.GetValue(EnvironmentVariables.BaseFeedCookie) ?? throw new InvalidOperationException($"Environment variable '{EnvironmentVariables.BaseFeedCookie}' is not defined.");
        this.BaseUID = provider.GetValue(EnvironmentVariables.BaseLinkUID) ?? throw new InvalidOperationException($"Environment variable '{EnvironmentVariables.BaseLinkUID}' is not defined.");
        this.torrentAnnounceListPatterns = provider.GetValue(EnvironmentVariables.TorrentTrackers)?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? throw new Exception($"Environment variable '{EnvironmentVariables.TorrentTrackers}' is not defined.");
    }

    /// <inheritdoc/>
    public string BaseUSESS { get; private set; }

    /// <inheritdoc/>
    public string BaseUrl { get; private set; }

    /// <inheritdoc/>
    public string BaseUID { get; private set; }

    /// <inheritdoc/>
    public string[] GetTorrentAnnounceList(string link_uid)
    {
        return this.torrentAnnounceListPatterns
            .Select(p => string.Format(p, link_uid ?? this.BaseUID))
            .Select(s => s)
            .ToArray();
    }
}
