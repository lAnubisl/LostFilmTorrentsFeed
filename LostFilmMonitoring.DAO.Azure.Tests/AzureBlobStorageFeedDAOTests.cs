namespace LostFilmMonitoring.DAO.Azure.Tests;

[ExcludeFromCodeCoverage]
public class AzureBlobStorageFeedDAOTests
{
    private Mock<IAzureBlobStorageClient>? azureBlobStorageClient;
    private Mock<Common.ILogger>? logger;
    private string? baseFeed;

    [SetUp]
    public void SetUp()
    {
        this.azureBlobStorageClient = new Mock<IAzureBlobStorageClient>();
        this.baseFeed = GetFile("baseFeed.xml");
        azureBlobStorageClient
            .Setup(x => x.DownloadStringAsync("rssfeeds", "baseFeed.xml"))
            .ReturnsAsync(this.baseFeed);
        this.logger = new();
        this.logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(this.logger.Object);
    }

    [Test]
    public async Task DeleteAsync_should_call_azureBlobStorageClient_deleteAsync()
    {
        await GetDao().DeleteAsync("userId");
        azureBlobStorageClient!.Verify(x => x.DeleteAsync("rssfeeds", "userId"));
    }

    [Test]
    public async Task LoadBaseFeedAsync_should_call_azureBlobStorageClient_downloadStringAsync()
    {
        var result = await GetDao().LoadBaseFeedAsync();
        azureBlobStorageClient!.Verify(x => x.DownloadStringAsync("rssfeeds", "baseFeed.xml"));
        var newXml = result.ToArray().GenerateXml();
        newXml.Should().Be(this.baseFeed);
    }

    private AzureBlobStorageFeedDao GetDao()
        => new(azureBlobStorageClient!.Object, logger!.Object);

    private static string GetFile(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"{assembly.GetName().Name}.Resources.{fileName}";
        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream!);
        return reader.ReadToEnd();
    }
}
