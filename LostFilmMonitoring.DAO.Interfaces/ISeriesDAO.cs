namespace LostFilmMonitoring.DAO.Interfaces;

/// <summary>
/// Provides functionality for managing series.
/// </summary>
public interface ISeriesDao
{
    /// <summary>
    /// Load series by name.
    /// </summary>
    /// <param name="name">Series name.</param>
    /// <returns>Series.</returns>
    Task<Series?> LoadAsync(string name);

    /// <summary>
    /// Load series.
    /// </summary>
    /// <returns>All series.</returns>
    Task<Series[]> LoadAsync();

    /// <summary>
    /// Save series.
    /// </summary>
    /// <param name="series">Series to save.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task<Guid> SaveAsync(Series series);

    /// <summary>
    /// Delete series.
    /// </summary>
    /// <param name="series">Series to delete.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task DeleteAsync(Series? series);
}
