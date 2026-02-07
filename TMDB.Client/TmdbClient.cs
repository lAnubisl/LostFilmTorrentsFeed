using TMDbLib.Client;
using LostFilmMonitoring.BLL.Interfaces;
using LostFilmMonitoring.Common;

namespace Tmdb.Client;

public class TmdbClient : ITmdbClient
{
    private readonly TMDbClient client;
    private readonly ILogger logger;

    public TmdbClient(string apiKey, ILogger logger)
    {
        this.client = new TMDbClient(apiKey);
        this.logger = logger.CreateScope(nameof(TmdbClient)) ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Stream?> DownloadImageAsync(string originalName)
    {
        this.logger.Info($"DownloadImageAsync: {originalName}");
        try
        {
            var searchResult = await this.client.SearchTvShowAsync(originalName);
            if (searchResult.Results.Count == 0)
            {
                this.logger.Error($"Series not found: {originalName}");
                return null;
            }

            var ordered = searchResult.Results.OrderByDescending(x => x.FirstAirDate);

            // Try to find series with the same name ignoring case
            var series = ordered.FirstOrDefault(x => string.Equals(x.Name, originalName, StringComparison.OrdinalIgnoreCase));
            if (series == null)
            {
                // Try to find any matching series
                series = ordered.FirstOrDefault();
                if (series == null)
                {
                    this.logger.Error($"Series not found: {originalName}");
                    return null;
                }
            }

            var image = await this.client.GetTvShowImagesAsync(series.Id);
            var poster = image.Posters.Where(x => x.Iso_639_1 == "en").OrderByDescending(x => x.VoteCount).FirstOrDefault();
            if (poster == null)
            {
                this.logger.Error($"Poster not found: {originalName}");
                return null;
            }

            // We need to get config before requesting image bytes, otherwise it won't work.
            var config = await this.client.GetConfigAsync();
            var imageBytes = await this.client.GetImageBytesAsync("w185", poster.FilePath);
            this.logger.Info($"Image downloaded: {originalName}");
            return new MemoryStream(imageBytes);
        }
        catch (Exception ex)
        {
            this.logger.Error($"Error downloading image: {originalName}");
            this.logger.Log(ex);
            return null;
        }
    }
}
