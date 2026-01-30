namespace LostFilmMonitoring.DAO.Interfaces;

/// <summary>
/// Provides functionality for managing episode.
/// </summary>
public interface IEpisodeDao
{
    /// <summary>
    /// Save series.
    /// </summary>
    /// <param name="episode">Episode to save.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task SaveAsync(Episode episode);

    /// <summary>
    /// Check if suche episode already exist.
    /// </summary>
    /// <param name="seriesName">Name of the series.</param>
    /// <param name="seasonNumber">The season number.</param>
    /// <param name="episideNumber">The eposide number.</param>
    /// <param name="quality">The quality.</param>
    /// <returns>True is such episode exists. Otherwise false.</returns>
    Task<bool> ExistsAsync(string seriesName, int seasonNumber, int episideNumber, string quality);

    /// <summary>
    /// Load all episodes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task<Episode[]> LoadAsync();
}
