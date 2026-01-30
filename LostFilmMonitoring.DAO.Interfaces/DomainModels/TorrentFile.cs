namespace LostFilmMonitoring.DAO.Interfaces.DomainModels;

/// <summary>
/// TorrentFile.
/// </summary>
public class TorrentFile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TorrentFile"/> class.
    /// </summary>
    /// <param name="fileName">File name.</param>
    /// <param name="stream">File content stream.</param>
    public TorrentFile(string fileName, Stream stream)
    {
        this.FileName = fileName;
        this.Stream = stream;
    }

    /// <summary>
    /// Gets FileName.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// Gets stream.
    /// </summary>
    public Stream Stream { get; }
}
