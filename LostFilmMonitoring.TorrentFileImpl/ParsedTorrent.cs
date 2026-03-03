namespace LostFilmMonitoring.TorrentFileImpl;

/// <summary>
/// Represents a parsed torrent file backed by BencodeNET.
/// </summary>
internal sealed class ParsedTorrent : IParsedTorrent
{
    private readonly object locker = new ();
    private readonly BencodeNET.Torrents.Torrent torrent;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParsedTorrent"/> class.
    /// </summary>
    /// <param name="torrent">The parsed BencodeNET torrent instance.</param>
    internal ParsedTorrent(BencodeNET.Torrents.Torrent torrent)
    {
        this.torrent = torrent ?? throw new ArgumentNullException(nameof(torrent));
    }

    /// <inheritdoc/>
    public string DisplayName => this.torrent.DisplayNameUtf8 ?? this.torrent.DisplayName;

    /// <inheritdoc/>
    public TorrentFile ToTorrentFile(string[] announces)
    {
        lock (this.locker)
        {
            this.torrent.Trackers = announces.Select(a => new List<string>() { a } as IList<string>).ToList();
            var ms = new MemoryStream();
            this.torrent.EncodeTo(ms);
            ms.Position = 0;
            return new TorrentFile(this.torrent.DisplayNameUtf8 ?? this.torrent.DisplayName, ms);
        }
    }
}
