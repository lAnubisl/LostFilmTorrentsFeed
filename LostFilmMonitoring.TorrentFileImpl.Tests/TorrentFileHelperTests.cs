namespace LostFilmMonitoring.TorrentFileImpl.Tests;

[ExcludeFromCodeCoverage]
internal class TorrentFileHelperTests
{
    private TorrentFileHelper helper = null!;

    [SetUp]
    public void Setup()
    {
        this.helper = new TorrentFileHelper();
    }

    [Test]
    public void Parse_should_return_IParsedTorrent()
    {
        using var stream = GetTorrentStream();
        var result = this.helper.Parse(stream);
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IParsedTorrent>();
    }

    [Test]
    public void Parse_should_return_correct_display_name()
    {
        using var stream = GetTorrentStream();
        var result = this.helper.Parse(stream);
        result.DisplayName.Should().Be("The.Flash.S08E13.720p.rus.LostFilm.TV.mp4");
    }

    [Test]
    public void Parse_should_reset_stream_position_to_zero()
    {
        using var stream = GetTorrentStream();
        this.helper.Parse(stream);
        stream.Position.Should().Be(0);
    }

    [Test]
    public void Parse_should_produce_torrent_with_private_flag_cleared()
    {
        using var stream = GetTorrentStream();
        var result = this.helper.Parse(stream);
        var torrentFile = result.ToTorrentFile(["http://tracker.example.com/announce"]);
        torrentFile.Should().NotBeNull();
    }

    [Test]
    public void Parse_ToTorrentFile_should_return_TorrentFile_with_correct_name()
    {
        using var stream = GetTorrentStream();
        var parsedTorrent = this.helper.Parse(stream);
        var torrentFile = parsedTorrent.ToTorrentFile(["http://tracker.example.com/announce"]);
        torrentFile.FileName.Should().Be("The.Flash.S08E13.720p.rus.LostFilm.TV.mp4");
    }

    [Test]
    public void Parse_ToTorrentFile_should_return_non_empty_stream()
    {
        using var stream = GetTorrentStream();
        var parsedTorrent = this.helper.Parse(stream);
        var torrentFile = parsedTorrent.ToTorrentFile(["http://tracker.example.com/announce"]);
        torrentFile.Stream.Length.Should().BeGreaterThan(0);
    }

    [Test]
    public void Parse_ToTorrentFile_should_embed_provided_announces()
    {
        var announce1 = "http://tracker1.example.com/announce";
        var announce2 = "http://tracker2.example.com/announce";
        using var stream = GetTorrentStream();
        var parsedTorrent = this.helper.Parse(stream);

        var torrentFile = parsedTorrent.ToTorrentFile([announce1, announce2]);

        using var resultStream = torrentFile.Stream;
        using var reader = new StreamReader(resultStream);
        var content = reader.ReadToEnd();
        content.Should().Contain(announce1);
        content.Should().Contain(announce2);
    }

    [Test]
    public void Parse_ToTorrentFile_should_be_thread_safe_for_concurrent_calls()
    {
        using var stream = GetTorrentStream();
        var parsedTorrent = this.helper.Parse(stream);

        var results = new System.Collections.Concurrent.ConcurrentBag<TorrentFile>();
        var announces = new[] { "http://tracker1.example.com/announce", "http://tracker2.example.com/announce", "http://tracker3.example.com/announce" };

        Parallel.For(0, 10, i =>
        {
            var torrentFile = parsedTorrent.ToTorrentFile([$"http://tracker{i}.example.com/announce"]);
            results.Add(torrentFile);
        });

        results.Should().HaveCount(10);
        results.Should().AllSatisfy(f => f.FileName.Should().Be("The.Flash.S08E13.720p.rus.LostFilm.TV.mp4"));
    }

    private static Stream GetTorrentStream()
        => typeof(TorrentFileHelperTests).Assembly
            .GetManifestResourceStream("LostFilmMonitoring.TorrentFileImpl.Tests.51439.torrent")!;
}
