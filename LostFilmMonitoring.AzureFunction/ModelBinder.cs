namespace LostFilmMonitoring.AzureFunction;

/// <summary>
/// Responsible for binding <see cref="HttpRequestData"/> to model instance.
/// </summary>
internal static class ModelBinder
{
    /// <summary>
    /// Binds instance of <see cref="HttpRequestData"/> to requested model type.
    /// </summary>
    /// <typeparam name="T">Type of model to bind to.</typeparam>
    /// <param name="req">Instance of <see cref="HttpRequestData"/>.</param>
    /// <returns>Instance of T.</returns>
    internal static T? Bind<T>(HttpRequestData req)
        where T : class
    {
        try
        {
            using var reader = new StreamReader(req.Body);
            var json = reader.ReadToEnd();
            return JsonSerializer.Deserialize<T>(json);
        }
        catch
        {
            return null;
        }
    }
}
