namespace LostFilmMonitoring.DAO.Azure.Tests;

[ExcludeFromCodeCoverage]
public class AzureBlobStorageTorrentFileDaoTests
{
    Mock<IAzureBlobStorageClient>? azureBlobStorageClient;
    private Mock<Common.ILogger>? logger;

    [SetUp]
    public void Setup()
    {
        this.azureBlobStorageClient = new Mock<IAzureBlobStorageClient>();
        this.logger = new();
        this.logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(this.logger.Object);
    }

    [Test]
    public async Task DeleteUserFileAsync_should_call_DeleteAsync()
    {
        var userId = "userId";
        var torrentFileName = "torrentFileName";
        var dao = new AzureBlobStorageTorrentFileDao(this.azureBlobStorageClient!.Object, this.logger!.Object);
        await dao.DeleteUserFileAsync(userId, torrentFileName);
        this.azureBlobStorageClient.Verify(x => x.DeleteAsync("usertorrents", userId, torrentFileName), Times.Once);
    }

    [Test]
    public async Task LoadBaseFileAsync_should_return_null()
    {
        var torrentId = "torrentId";
        var dao = new AzureBlobStorageTorrentFileDao(this.azureBlobStorageClient!.Object, this.logger!.Object);
        this.azureBlobStorageClient.Setup(x => x.DownloadAsync("basetorrents", $"{torrentId}.torrent")).ReturnsAsync(null as Stream);
        var result = await dao.LoadBaseFileAsync(torrentId);
        this.azureBlobStorageClient.Verify(x => x.DownloadAsync("basetorrents", $"{torrentId}.torrent"), Times.Once);
        result.Should().BeNull();
    }

    [Test]
    public async Task LoadBaseFileAsync_should_return_TorrentFile()
    {
        var torrentId = "torrentId";
        var dao = new AzureBlobStorageTorrentFileDao(this.azureBlobStorageClient!.Object, this.logger!.Object);
        this.azureBlobStorageClient.Setup(x => x.DownloadAsync("basetorrents", $"{torrentId}.torrent")).ReturnsAsync(new MemoryStream());
        var result = await dao.LoadBaseFileAsync(torrentId);
        this.azureBlobStorageClient.Verify(x => x.DownloadAsync("basetorrents", $"{torrentId}.torrent"), Times.Once);
        result.Should().NotBeNull();
        string.Equals(result!.FileName, $"{torrentId}.torrent").Should().BeTrue();
    }
    
    [Test]
    public async Task SaveBaseFileAsync_should_call_UploadAsync()
    {
        var torrentId = "torrentId";
        var dao = new AzureBlobStorageTorrentFileDao(this.azureBlobStorageClient!.Object, this.logger!.Object);
        await dao.SaveBaseFileAsync(torrentId, new TorrentFile(torrentId, new MemoryStream()));
        this.azureBlobStorageClient.Verify(x => x.UploadAsync("basetorrents", $"{torrentId}.torrent", It.IsAny<Stream>(), "applications/x-bittorrent"), Times.Once);
    }

    [Test]
    public async Task SaveUserFileAsync_should_call_UploadAsync()
    {
        var userId = "userId";
        var dao = new AzureBlobStorageTorrentFileDao(this.azureBlobStorageClient!.Object, this.logger!.Object);
        await dao.SaveUserFileAsync(userId, new TorrentFile("fileName", new MemoryStream()));
        this.azureBlobStorageClient.Verify(x => x.UploadAsync("usertorrents", userId, "fileName.torrent", It.IsAny<Stream>(), "applications/x-bittorrent"), Times.Once);
    }
}
