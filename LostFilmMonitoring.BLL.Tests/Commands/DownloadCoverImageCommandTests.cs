namespace LostFilmMonitoring.BLL.Tests.Commands;

[ExcludeFromCodeCoverage]
public class DownloadCoverImageCommandTests
{
    private Mock<Common.ILogger>? logger;
    private Mock<IFileSystem>? fileSystem;
    private Mock<ITmdbClient>? tmdbClient;

    [SetUp]
    public void Setup()
    {
        this.logger = new();
        this.fileSystem = new();
        this.tmdbClient = new();
        this.logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(this.logger.Object);
    }

    private DownloadCoverImageCommand GetService()
        => new(this.logger!.Object, this.fileSystem!.Object, this.tmdbClient!.Object);

    #region Constructor Tests

    [Test]
    public void Constructor_should_throw_exception_when_logger_null()
    {
        var action = () => new DownloadCoverImageCommand(
            null!,
            this.fileSystem!.Object,
            this.tmdbClient!.Object);
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("logger");
    }

    [Test]
    public void Constructor_should_throw_exception_when_fileSystem_null()
    {
        var action = () => new DownloadCoverImageCommand(
            this.logger!.Object,
            null!,
            this.tmdbClient!.Object);
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("fileSystem");
    }

    [Test]
    public void Constructor_should_throw_exception_when_tmdbClient_null()
    {
        var action = () => new DownloadCoverImageCommand(
            this.logger!.Object,
            this.fileSystem!.Object,
            null!);
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("tmdbClient");
    }

    [Test]
    public void Constructor_should_create_logger_scope()
    {
        GetService();
        this.logger!.Verify(l => l.CreateScope(nameof(DownloadCoverImageCommand)), Times.Once);
    }

    #endregion

    #region ExecuteAsync Tests - Null/Invalid Input

    [Test]
    public async Task ExecuteAsync_should_throw_exception_when_series_null()
    {
        var command = GetService();
        var action = async () => await command.ExecuteAsync(null!);
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    #endregion

    #region ExecuteAsync Tests - Poster Already Exists

    [Test]
    public async Task ExecuteAsync_should_return_early_if_poster_exists()
    {
        var series = CreateSeries(Guid.NewGuid(), "Test Series (Original Name)");
        this.fileSystem!.Setup(x => x.ExistsAsync(Constants.MetadataStorageContainerImages, $"{series.Id}.jpg"))
            .ReturnsAsync(true);

        var command = GetService();
        await command.ExecuteAsync(series);

        this.fileSystem.Verify(x => x.ExistsAsync(Constants.MetadataStorageContainerImages, $"{series.Id}.jpg"), Times.Once);
        this.tmdbClient!.Verify(x => x.DownloadImageAsync(It.IsAny<string>()), Times.Never);
        this.fileSystem.Verify(x => x.SaveAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()), Times.Never);
    }

    #endregion

    #region ExecuteAsync Tests - Series Name Parsing

    [Test]
    public async Task ExecuteAsync_should_log_entry_point()
    {
        var series = CreateSeries(Guid.NewGuid(), "Test Series (Original Name)");
        this.fileSystem!.Setup(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
        this.tmdbClient!.Setup(x => x.DownloadImageAsync(It.IsAny<string>())).ReturnsAsync((Stream?)null);

        var command = GetService();
        await command.ExecuteAsync(series);

        this.logger!.Verify(
            l => l.Info(It.Is<string>(msg => msg.Contains("ExecuteAsync"))),
            Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_should_extract_original_name_from_series_name()
    {
        var series = CreateSeries(Guid.NewGuid(), "Breaking Bad (Breaking Bad)");
        this.fileSystem!.Setup(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
        this.tmdbClient!.Setup(x => x.DownloadImageAsync(It.IsAny<string>())).ReturnsAsync((Stream?)null);

        var command = GetService();
        await command.ExecuteAsync(series);

        this.tmdbClient.Verify(x => x.DownloadImageAsync("Breaking Bad"), Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_should_extract_original_name_with_different_name()
    {
        var series = CreateSeries(Guid.NewGuid(), "American Horror Story (American Horror Story)");
        this.fileSystem!.Setup(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
        this.tmdbClient!.Setup(x => x.DownloadImageAsync(It.IsAny<string>())).ReturnsAsync((Stream?)null);

        var command = GetService();
        await command.ExecuteAsync(series);

        this.tmdbClient.Verify(x => x.DownloadImageAsync("American Horror Story"), Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_should_handle_series_name_with_spaces_in_original_name()
    {
        var series = CreateSeries(Guid.NewGuid(), "TV Show (Original Name With Spaces)");
        this.fileSystem!.Setup(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
        this.tmdbClient!.Setup(x => x.DownloadImageAsync(It.IsAny<string>())).ReturnsAsync((Stream?)null);

        var command = GetService();
        await command.ExecuteAsync(series);

        this.tmdbClient.Verify(x => x.DownloadImageAsync("Original Name With Spaces"), Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_should_handle_series_name_with_single_char_original_name()
    {
        var series = CreateSeries(Guid.NewGuid(), "TV Show (X)");
        this.fileSystem!.Setup(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
        this.tmdbClient!.Setup(x => x.DownloadImageAsync(It.IsAny<string>())).ReturnsAsync((Stream?)null);

        var command = GetService();
        await command.ExecuteAsync(series);

        this.tmdbClient.Verify(x => x.DownloadImageAsync("X"), Times.Once);
    }

    #endregion

    #region ExecuteAsync Tests - Invalid Name Format

    [Test]
    public async Task ExecuteAsync_should_return_early_if_series_name_has_no_opening_parenthesis()
    {
        var series = CreateSeries(Guid.NewGuid(), "Test Series Without Parenthesis)");
        this.fileSystem!.Setup(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

        var command = GetService();
        await command.ExecuteAsync(series);

        this.logger!.Verify(
            l => l.Error(It.Is<string>(msg => msg.Contains("Cannot parse original name"))),
            Times.Once);
        this.tmdbClient!.Verify(x => x.DownloadImageAsync(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task ExecuteAsync_should_return_early_if_series_name_has_no_closing_parenthesis()
    {
        var series = CreateSeries(Guid.NewGuid(), "Test Series (Without Closing");
        this.fileSystem!.Setup(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

        var command = GetService();
        await command.ExecuteAsync(series);

        this.logger!.Verify(
            l => l.Error(It.Is<string>(msg => msg.Contains("Cannot parse original name"))),
            Times.Once);
        this.tmdbClient!.Verify(x => x.DownloadImageAsync(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task ExecuteAsync_should_return_early_if_closing_parenthesis_before_opening()
    {
        var series = CreateSeries(Guid.NewGuid(), "Test Series )Before (Opening");
        this.fileSystem!.Setup(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

        var command = GetService();
        await command.ExecuteAsync(series);

        this.logger!.Verify(
            l => l.Error(It.Is<string>(msg => msg.Contains("Cannot parse original name"))),
            Times.Once);
        this.tmdbClient!.Verify(x => x.DownloadImageAsync(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task ExecuteAsync_should_log_error_with_series_name()
    {
        var seriesName = "My Test Series Without Parenthesis";
        var series = CreateSeries(Guid.NewGuid(), seriesName);
        this.fileSystem!.Setup(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

        var command = GetService();
        await command.ExecuteAsync(series);

        this.logger!.Verify(
            l => l.Error(It.Is<string>(msg => msg.Contains(seriesName))),
            Times.Once);
    }

    #endregion

    #region ExecuteAsync Tests - TMDB Download

    [Test]
    public async Task ExecuteAsync_should_return_early_if_tmdbClient_returns_null()
    {
        var series = CreateSeries(Guid.NewGuid(), "Test Series (Original Name)");
        this.fileSystem!.Setup(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
        this.tmdbClient!.Setup(x => x.DownloadImageAsync(It.IsAny<string>())).ReturnsAsync((Stream?)null);

        var command = GetService();
        await command.ExecuteAsync(series);

        this.fileSystem.Verify(x => x.SaveAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()), Times.Never);
    }

    #endregion

    #region ExecuteAsync Tests - File Save

    [Test]
    public async Task ExecuteAsync_should_save_image_to_file_system()
    {
        var seriesId = Guid.NewGuid();
        var series = CreateSeries(seriesId, "Test Series (Original Name)");
        var imageStream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5 });

        this.fileSystem!.Setup(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
        this.tmdbClient!.Setup(x => x.DownloadImageAsync(It.IsAny<string>())).ReturnsAsync(imageStream);

        var command = GetService();
        await command.ExecuteAsync(series);

        this.fileSystem.Verify(
            x => x.SaveAsync(
                Constants.MetadataStorageContainerImages,
                $"{seriesId}.jpg",
                "image/jpeg",
                imageStream),
            Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_should_use_correct_container_name_for_save()
    {
        var series = CreateSeries(Guid.NewGuid(), "Test Series (Original Name)");
        var imageStream = new MemoryStream();

        this.fileSystem!.Setup(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
        this.tmdbClient!.Setup(x => x.DownloadImageAsync(It.IsAny<string>())).ReturnsAsync(imageStream);

        var command = GetService();
        await command.ExecuteAsync(series);

        this.fileSystem.Verify(
            x => x.SaveAsync(
                Constants.MetadataStorageContainerImages,
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Stream>()),
            Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_should_use_correct_mime_type_for_save()
    {
        var series = CreateSeries(Guid.NewGuid(), "Test Series (Original Name)");
        var imageStream = new MemoryStream();

        this.fileSystem!.Setup(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
        this.tmdbClient!.Setup(x => x.DownloadImageAsync(It.IsAny<string>())).ReturnsAsync(imageStream);

        var command = GetService();
        await command.ExecuteAsync(series);

        this.fileSystem.Verify(
            x => x.SaveAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                "image/jpeg",
                It.IsAny<Stream>()),
            Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_should_use_series_id_as_filename()
    {
        var seriesId = Guid.NewGuid();
        var series = CreateSeries(seriesId, "Test Series (Original Name)");
        var imageStream = new MemoryStream();

        this.fileSystem!.Setup(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
        this.tmdbClient!.Setup(x => x.DownloadImageAsync(It.IsAny<string>())).ReturnsAsync(imageStream);

        var command = GetService();
        await command.ExecuteAsync(series);

        this.fileSystem.Verify(
            x => x.SaveAsync(
                It.IsAny<string>(),
                $"{seriesId}.jpg",
                It.IsAny<string>(),
                It.IsAny<Stream>()),
            Times.Once);
    }

    #endregion

    #region ExecuteAsync Tests - Integration Scenarios

    [Test]
    public async Task ExecuteAsync_should_complete_successfully_with_valid_series_and_image()
    {
        var series = CreateSeries(Guid.NewGuid(), "Breaking Bad (Breaking Bad)");
        var imageStream = new MemoryStream(new byte[] { 1, 2, 3 });

        this.fileSystem!.Setup(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
        this.tmdbClient!.Setup(x => x.DownloadImageAsync("Breaking Bad")).ReturnsAsync(imageStream);

        var command = GetService();
        await command.ExecuteAsync(series);

        this.fileSystem.Verify(
            x => x.SaveAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()),
            Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_should_check_existence_with_correct_series_id()
    {
        var seriesId = Guid.NewGuid();
        var series = CreateSeries(seriesId, "Test Series (Original Name)");
        this.fileSystem!.Setup(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
        this.tmdbClient!.Setup(x => x.DownloadImageAsync(It.IsAny<string>())).ReturnsAsync((Stream?)null);

        var command = GetService();
        await command.ExecuteAsync(series);

        this.fileSystem.Verify(
            x => x.ExistsAsync(Constants.MetadataStorageContainerImages, $"{seriesId}.jpg"),
            Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_should_handle_stream_disposal()
    {
        var series = CreateSeries(Guid.NewGuid(), "Test Series (Original Name)");
        var imageStream = new MemoryStream(new byte[] { 1, 2, 3 });

        this.fileSystem!.Setup(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
        this.tmdbClient!.Setup(x => x.DownloadImageAsync(It.IsAny<string>())).ReturnsAsync(imageStream);

        var command = GetService();
        await command.ExecuteAsync(series);

        // This should not throw
        this.fileSystem.Verify(
            x => x.SaveAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), imageStream),
            Times.Once);
    }

    #endregion

    #region Helper Methods

    private static Series CreateSeries(Guid id, string name)
    {
        return new Series(
            id,
            name,
            DateTime.UtcNow,
            "Test Episode",
            "http://link.sd",
            "http://link.mp4",
            "http://link.1080");
    }

    #endregion
}
