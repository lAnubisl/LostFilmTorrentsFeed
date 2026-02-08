namespace LostFilmTV.Client;

/// <summary>
/// Client for LostFilm.TV
/// This class is responsible for all interactions with lostfilm.tv website.
/// </summary>
public class LostFilmClient : ILostFilmClient
{
    private static readonly ActivitySource ActivitySource = new (ActivitySourceNames.DownloadTorrent);
    private readonly ILogger logger;
    private readonly IHttpClientFactory httpClientFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="LostFilmClient"/> class.
    /// </summary>
    /// <param name="logger">Logger.</param>
    /// <param name="httpClientFactory">IHttpClientFactory.</param>
    public LostFilmClient(ILogger logger, IHttpClientFactory httpClientFactory)
    {
        this.logger = logger?.CreateScope(nameof(LostFilmClient)) ?? throw new ArgumentNullException(nameof(logger));
        this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    /// <summary>
    /// Get torrent file for user.
    /// </summary>
    /// <param name="uid">User Id.</param>
    /// <param name="usess">User ss key.</param>
    /// <param name="torrentFileId">Torrent file Id.</param>
    /// <returns>TorrentFile object which contain file name and content stream.</returns>
    public async Task<ITorrentFileResponse?> DownloadTorrentFileAsync(string uid, string usess, string torrentFileId)
    {
        var client = this.httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://n.tracktor.site/rssdownloader.php?id={torrentFileId}");
        using var activity = ActivitySource.StartActivity(nameof(this.DownloadTorrentFileAsync), ActivityKind.Client);
        activity?.SetTag("Type", "Lostfilm");
        request.Headers.Add("Cookie", $"uid={uid};usess={usess};");
        HttpResponseMessage response;

        try
        {
            response = await client.SendAsync(request);
        }
        catch (Exception ex)
        {
            this.logger.Log(ex);
            return null;
        }

        if (response?.Content?.Headers?.ContentType == null)
        {
            this.logger.Error("response?.Content?.Headers?.ContentType == null");
            return null;
        }

        if (response.Content.Headers.ContentType.MediaType != "application/x-bittorrent")
        {
            string? responseBody = null;
            if (response.Content.Headers.ContentType.MediaType == "text/html")
            {
                responseBody = await response.Content.ReadAsStringAsync();
            }

            this.logger.Error($"contentType is not 'application/x-bittorrent' it is '{response.Content.Headers.ContentType.MediaType}'. Response content is: '{responseBody}'. TorrentFileId is: '{torrentFileId}'.");
            return null;
        }

        response.Content.Headers.TryGetValues("Content-Disposition", out IEnumerable<string>? cd);
        var fileName = cd?.FirstOrDefault()?[("attachment;filename=\"".Length + 1) ..];
        if (fileName == null)
        {
            this.logger.Error("Something wrong with 'Content-Disposition' header of the response.");
            return null;
        }

        fileName = fileName[0..^1];
        var stream = await response.Content.ReadAsStreamAsync();
        return new TorrentFileResponse(fileName, stream);
    }
}
