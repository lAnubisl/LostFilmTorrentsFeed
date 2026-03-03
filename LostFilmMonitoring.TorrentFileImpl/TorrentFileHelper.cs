namespace LostFilmMonitoring.TorrentFileImpl;

/// <summary>
/// Helper for working with torrent files using BencodeNET.
/// </summary>
public class TorrentFileHelper : ITorrentFileHelper
{
    /// <inheritdoc/>
    public IParsedTorrent Parse(Stream stream)
    {
        var parser = new BencodeNET.Torrents.TorrentParser(BencodeNET.Torrents.TorrentParserMode.Tolerant);
        var torrent = parser.Parse(new BencodeNET.IO.BencodeReader(stream));
        torrent.IsPrivate = false;
        stream.Position = 0;
        return new ParsedTorrent(torrent);
    }
}
