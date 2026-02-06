namespace LostFilmMonitoring.BLL.Tests.Commands;

[ExcludeFromCodeCoverage]
public class DownloadCoverImagesCommandTests
{
    private Mock<ISeriesDao>? seriesDao;
    private Mock<Common.ILogger>? logger;
    private Mock<ICommand<Series>>? downloadCoverImageCommand;

    [SetUp]
    public void Setup()
    {
        this.downloadCoverImageCommand = new();
        this.seriesDao = new();
        this.logger = new();
        this.logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(this.logger.Object);
    }

    private DownloadCoverImagesCommand GetService()
            => new(this.logger!.Object, this.seriesDao!.Object, this.downloadCoverImageCommand!.Object);

    #region Constructor Tests

    [Test]
    public void Constructor_should_throw_exception_when_logger_null()
    {
        var action = () => new DownloadCoverImagesCommand(
            null!,
            this.seriesDao!.Object,
            this.downloadCoverImageCommand!.Object);
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("logger");
    }

    [Test]
    public void Constructor_should_throw_exception_when_seriesDao_null()
    {
        var action = () => new DownloadCoverImagesCommand(
            this.logger!.Object,
            null!,
            this.downloadCoverImageCommand!.Object);
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("seriesDao");
    }

    [Test]
    public void Constructor_should_throw_exception_when_downloadCoverImageCommand_null()
    {
        var action = () => new DownloadCoverImagesCommand(
            this.logger!.Object,
            this.seriesDao!.Object,
            null!);
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("downloadCoverImageCommand");
    }

    [Test]
    public void Constructor_should_create_logger_scope()
    {
        GetService();
        this.logger!.Verify(l => l.CreateScope(nameof(DownloadCoverImagesCommand)), Times.Once);
    }

    #endregion

    #region ExecuteAsync Tests

    [Test]
    public async Task ExecuteAsync_should_call_LoadAsync_on_seriesDao()
    {
        this.seriesDao!.Setup(x => x.LoadAsync()).ReturnsAsync(Array.Empty<Series>());
        var command = GetService();

        await command.ExecuteAsync();

        this.seriesDao.Verify(x => x.LoadAsync(), Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_should_execute_downloadCoverImageCommand_for_each_series()
    {
        var series1 = CreateSeries(Guid.NewGuid(), "Series 1");
        var series2 = CreateSeries(Guid.NewGuid(), "Series 2");
        var series3 = CreateSeries(Guid.NewGuid(), "Series 3");
        var seriesList = new[] { series1, series2, series3 };

        this.seriesDao!.Setup(x => x.LoadAsync()).ReturnsAsync(seriesList);
        var command = GetService();

        await command.ExecuteAsync();

        this.downloadCoverImageCommand!.Verify(x => x.ExecuteAsync(series1), Times.Once);
        this.downloadCoverImageCommand!.Verify(x => x.ExecuteAsync(series2), Times.Once);
        this.downloadCoverImageCommand!.Verify(x => x.ExecuteAsync(series3), Times.Once);
        this.downloadCoverImageCommand!.Verify(x => x.ExecuteAsync(It.IsAny<Series>()), Times.Exactly(3));
    }

    [Test]
    public async Task ExecuteAsync_should_handle_empty_series_list()
    {
        this.seriesDao!.Setup(x => x.LoadAsync()).ReturnsAsync(Array.Empty<Series>());
        var command = GetService();

        await command.ExecuteAsync();

        this.downloadCoverImageCommand!.Verify(x => x.ExecuteAsync(It.IsAny<Series>()), Times.Never);
        this.seriesDao!.Verify(x => x.LoadAsync(), Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_should_execute_downloadCoverImageCommand_in_correct_order()
    {
        var series1 = CreateSeries(Guid.NewGuid(), "Series 1");
        var series2 = CreateSeries(Guid.NewGuid(), "Series 2");
        var seriesList = new[] { series1, series2 };

        this.seriesDao!.Setup(x => x.LoadAsync()).ReturnsAsync(seriesList);
        var command = GetService();

        var callOrder = new List<Series>();
        this.downloadCoverImageCommand!
            .Setup(x => x.ExecuteAsync(It.IsAny<Series>()))
            .Callback<Series>(s => callOrder.Add(s))
            .Returns(Task.CompletedTask);

        await command.ExecuteAsync();

        callOrder.Should().HaveCount(2);
        callOrder[0].Should().Be(series1);
        callOrder[1].Should().Be(series2);
    }

    [Test]
    public async Task ExecuteAsync_should_log_entry_point()
    {
        this.seriesDao!.Setup(x => x.LoadAsync()).ReturnsAsync(Array.Empty<Series>());
        var command = GetService();

        await command.ExecuteAsync();

        this.logger!.Verify(
            l => l.Info(It.Is<string>(msg => msg.Contains("ExecuteAsync"))),
            Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_should_handle_single_series()
    {
        var series = CreateSeries(Guid.NewGuid(), "Single Series");
        this.seriesDao!.Setup(x => x.LoadAsync()).ReturnsAsync(new[] { series });
        var command = GetService();

        await command.ExecuteAsync();

        this.downloadCoverImageCommand!.Verify(x => x.ExecuteAsync(series), Times.Once);
        this.downloadCoverImageCommand!.Verify(x => x.ExecuteAsync(It.IsAny<Series>()), Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_should_propagate_exception_from_downloadCoverImageCommand()
    {
        var series1 = CreateSeries(Guid.NewGuid(), "Series 1");
        var series2 = CreateSeries(Guid.NewGuid(), "Series 2");
        var seriesList = new[] { series1, series2 };

        this.seriesDao!.Setup(x => x.LoadAsync()).ReturnsAsync(seriesList);
        var command = GetService();

        this.downloadCoverImageCommand!
            .Setup(x => x.ExecuteAsync(It.IsAny<Series>()))
            .ThrowsAsync(new InvalidOperationException("Download failed"));

        var action = async () => await command.ExecuteAsync();
        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    [Test]
    public async Task ExecuteAsync_should_not_call_LoadAsync_multiple_times()
    {
        this.seriesDao!.Setup(x => x.LoadAsync()).ReturnsAsync(Array.Empty<Series>());
        var command = GetService();

        await command.ExecuteAsync();

        this.seriesDao.Verify(x => x.LoadAsync(), Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_with_large_series_list_should_execute_all()
    {
        var seriesList = Enumerable.Range(1, 100)
            .Select(i => CreateSeries(Guid.NewGuid(), $"Series {i}"))
            .ToArray();

        this.seriesDao!.Setup(x => x.LoadAsync()).ReturnsAsync(seriesList);
        var command = GetService();

        await command.ExecuteAsync();

        this.downloadCoverImageCommand!.Verify(
            x => x.ExecuteAsync(It.IsAny<Series>()),
            Times.Exactly(100));
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
