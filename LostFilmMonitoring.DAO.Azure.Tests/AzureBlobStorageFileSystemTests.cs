namespace LostFilmMonitoring.DAO.Azure.Tests;

[ExcludeFromCodeCoverage]
public class AzureBlobStorageFileSystemTests
{
    private Mock<IAzureBlobStorageClient>? azureBlobStorageClientMock;
    private Mock<Common.ILogger>? logger;

    [SetUp]
    public void Setup()
    {
        this.azureBlobStorageClientMock = new();
        this.logger = new();
        this.logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(this.logger.Object);
    }

    [Test]
    public async Task ExistsAsync_should_call_IAzureBlobStorageClient()
    {
        await GetServide().ExistsAsync("directory", "fileName");
        azureBlobStorageClientMock!.Verify(x => x.ExistsAsync("directory", "fileName"), Times.Once);
    }

    [Test]
    public async Task SaveAsync_should_call_IAzureBlobStorageClient()
    {
        var ms = new MemoryStream();
        await GetServide().SaveAsync("directory", "fileName", "contentType", ms);
        azureBlobStorageClientMock!.Verify(x => x.UploadAsync("directory", "fileName", ms, "contentType"), Times.Once);
    }

    protected AzureBlobStorageFileSystem GetServide()
        => new(this.azureBlobStorageClientMock!.Object, this.logger!.Object);
}
