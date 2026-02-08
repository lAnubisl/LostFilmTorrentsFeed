namespace LostFilmMonitoring.BLL.Interfaces;

/// <summary>
/// TorrentFileResponse interface.
/// </summary>
public interface ITorrentFileResponse
{
    /// <summary>
    /// Gets File Name.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// Gets content stream.
    /// </summary>
    public Stream Content { get; }
}
