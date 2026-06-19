namespace LostFilmTV.Client.Tests;

[TestFixture]
[ExcludeFromCodeCoverage]
public class LostFilmClientTests
{
    private Mock<IHttpClientFactory> httpClientFactory;
    private Mock<ILogger> logger;
    private MockHttpMessageHandler mockHttp;

    [SetUp]
    public void Setup()
    {
        this.httpClientFactory = new();
        this.logger = new();
        this.logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(this.logger.Object);
        this.mockHttp = new();
        this.httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient(mockHttp));
    }

    [Test]
    public void Constructor_should_throw_exception_when_logger_null()
    {
        var action = () => new LostFilmClient(null!, this.httpClientFactory.Object);
        Assert.That(Assert.Throws<ArgumentNullException>(() => action()), Has.Property(nameof(ArgumentNullException.ParamName)).EqualTo("logger"));
    }

    [Test]
    public void Constructor_should_throw_exception_when_logger_createScope_null()
    {
        var action = () => new LostFilmClient(new Mock<ILogger>().Object, this.httpClientFactory.Object);
        Assert.That(Assert.Throws<ArgumentNullException>(() => action()), Has.Property(nameof(ArgumentNullException.ParamName)).EqualTo("logger"));
    }

    [Test]
    public void Constructor_should_throw_exception_when_httpClientFactory_null()
    {
        var action = () => new LostFilmClient(this.logger.Object, null!);
        Assert.That(Assert.Throws<ArgumentNullException>(() => action()), Has.Property(nameof(ArgumentNullException.ParamName)).EqualTo("httpClientFactory"));
    }

    [Test]
    public async Task DownloadTorrentFileAsync_should_return_null_when_contentType_not_set()
    {
        var testString = "Test Data";
        var torrentFileId = "torrentId";
        mockHttp
            .When(HttpMethod.Get, $"https://n.tracktor.site/rssdownloader.php?id={torrentFileId}")
            .Respond(new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(testString))));

        var client = new LostFilmClient(logger.Object, httpClientFactory.Object);
        var result = await client.DownloadTorrentFileAsync("uid", "usess", torrentFileId);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DownloadTorrentFileAsync_should_return_null_when_contentType_not_expected()
    {
        var testString = "Test Data";
        var torrentFileId = "torrentId";
        mockHttp
            .When(HttpMethod.Get, $"https://n.tracktor.site/rssdownloader.php?id={torrentFileId}")
            .Respond("text/html", new MemoryStream(Encoding.UTF8.GetBytes(testString)));

        var client = new LostFilmClient(logger.Object, httpClientFactory.Object);
        var result = await client.DownloadTorrentFileAsync("uid", "usess", torrentFileId);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DownloadTorrentFileAsync_should_return_null_when_content_desposition_not_set()
    {
        var testString = "Test Data";
        var torrentFileId = "torrentId";
        mockHttp
            .When(HttpMethod.Get, $"https://n.tracktor.site/rssdownloader.php?id={torrentFileId}")
            .Respond("application/x-bittorrent", new MemoryStream(Encoding.UTF8.GetBytes(testString)));

        var client = new LostFilmClient(logger.Object, httpClientFactory.Object);
        var result = await client.DownloadTorrentFileAsync("uid", "usess", torrentFileId);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DownloadTorrentFileAsync_should_not_fail()
    {
        var torrentFileId = "torrentId";
        mockHttp
            .When(HttpMethod.Get, $"https://n.tracktor.site/rssdownloader.php?id={torrentFileId}")
            .Throw(new HttpRequestException());

        var client = new LostFilmClient(logger.Object, httpClientFactory.Object);
        var result = await client.DownloadTorrentFileAsync("uid", "usess", torrentFileId);
        Assert.That(result, Is.Null);
    }

    private static byte[] ReadFully(Stream input)
    {
        byte[] buffer = new byte[16 * 1024];
        using MemoryStream ms = new ();
        int read;
        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
        {
            ms.Write(buffer, 0, read);
        }
        return ms.ToArray();
    }
}
