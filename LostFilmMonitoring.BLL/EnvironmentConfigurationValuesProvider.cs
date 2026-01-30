namespace LostFilmMonitoring.BLL;

/// <summary>
/// Provides configuration values from environment variables.
/// </summary>
public class EnvironmentConfigurationValuesProvider : IConfigurationValuesProvider
{
    /// <inheritdoc/>
    public string? GetValue(string key)
        => Environment.GetEnvironmentVariable(key);
}
