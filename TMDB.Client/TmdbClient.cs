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
        this.logger = logger;
    }

    public async Task<Stream?> DownloadImageAsync(string originalName)
    {
        this.logger.Info($"DownloadImageAsync: {originalName}");
        try
        {
            var searchResult = await this.client.SearchTvShowAsync(originalName);
            var series = searchResult.Results.FirstOrDefault();
            if (series == null)
            {
                this.logger.Error($"Series not found: {originalName}");
                return null;
            }

            var image = await this.client.GetTvShowImagesAsync(series.Id);
            var poster = image.Posters.Where(x => x.Iso_639_1 == "en").OrderByDescending(x => x.VoteCount).FirstOrDefault();
            if (poster == null)
            {
                this.logger.Error($"Poster not found: {originalName}");
                return null;
            }

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
