namespace LostFilmMonitoring.BLL.Interfaces;

/// <summary>
/// Provides configuration values.
/// </summary>
public interface IConfigurationValuesProvider
{
    /// <summary>
    /// Gets the value by key.
    /// </summary>
    /// <param name="key">Key.</param>
    /// <returns>Value.</returns>
    string? GetValue(string key);
}
