namespace LostFilmTV.Client.Response;

/// <summary>
/// Represents torrent file with content.
/// </summary>
public class TorrentFileResponse : ITorrentFileResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TorrentFileResponse"/> class.
    /// </summary>
    /// <param name="fileName">File Name.</param>
    /// <param name="content">Content stream.</param>
    internal TorrentFileResponse(string fileName, Stream content)
    {
        this.FileName = fileName;
        this.Content = content;
    }

    /// <summary>
    /// Gets File Name.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// Gets content stream.
    /// </summary>
    public Stream Content { get; }
}
