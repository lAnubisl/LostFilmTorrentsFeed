namespace LostFilmMonitoring.BLL.Tests.Commands;

[ExcludeFromCodeCoverage]
public class DownloadCoverImagesCommandTests
{
    private Mock<ISeriesDao>? seriesDao;
    private Mock<Common.ILogger>? logger;
    private Mock<IFileSystem>? fileSystem;
    private Mock<ITmdbClient>? tmdbClient;

    [SetUp]
    public void Setup()
    {
        this.fileSystem = new();
        this.tmdbClient = new();
        this.seriesDao = new();
        this.logger = new();
        this.logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(this.logger.Object);
    }

    private DownloadCoverImagesCommand GetService()
            => new(this.logger!.Object, this.fileSystem!.Object, this.seriesDao!.Object, this.tmdbClient!.Object);
}
