namespace LostFilmTV.Client.Tests;

[ExcludeFromCodeCoverage]
internal class Helper
{
    protected Helper()
    {
    }

    internal static string GetEmbeddedResource(string name)
    {
        using var resource = typeof(Helper).GetTypeInfo().Assembly.GetManifestResourceStream(name);
        using var reader = new StreamReader(resource);
        return reader.ReadToEnd();
    }
}
