namespace LostFilmMonitoring.BLL.Interfaces;

/// <summary>
/// Responsible for downloading series covers.
/// </summary>
public interface ISeriesCoverService
{
    /// <summary>
    /// Makes sure that the image for series exists in the system.
    /// </summary>
    /// <param name="seriesName">Name of the series to check.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task EnsureCoverDownloadedAsync(string seriesName);
}
