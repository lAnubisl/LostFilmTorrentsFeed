namespace LostFilmMonitoring.BLL.Interfaces;

/// <summary>
/// Responsible for downloading series covers.
/// </summary>
public interface ITmdbClient
{
    /// <summary>
    /// Downloads the image for the series.
    /// </summary>
    /// <param name="originalName">Name of the series to download the image for.</param>
    /// <returns>A <see cref="Stream"/> representing the image.</returns>
    Task<Stream?> DownloadImageAsync(string originalName);
}
