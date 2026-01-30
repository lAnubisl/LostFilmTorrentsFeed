namespace LostFilmMonitoring.Common;

/// <summary>
/// Common serialization options.
/// </summary>
public static class CommonSerializationOptions
{
    /// <summary>
    /// Gets default serialization options.
    /// </summary>
    public static readonly JsonSerializerOptions Default = new ()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
        WriteIndented = true,
    };
}
