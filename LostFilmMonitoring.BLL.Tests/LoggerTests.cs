namespace LostFilmMonitoring.BLL.Tests;

[ExcludeFromCodeCoverage]
public class LoggerTests
{
    private Mock<ILoggerFactory>? loggerFactoryMock;
    private Mock<Microsoft.Extensions.Logging.ILogger>? loggerMock;

    [SetUp]
    public void Setup()
    {
        loggerMock = new();
        loggerFactoryMock = new();
        loggerFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(loggerMock.Object);
    }

    [Test]
    public void Constructor_should_throw_exception_when_logger_null()
    {
        var action = () => new Logger(null!);
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("loggerFactory");
    }

    [Test]
    public void Debug_should_call_logger()
    {
        GetLogger().Debug("message");

        loggerMock!.Verify(
           x => x.Log(
               LogLevel.Debug,
               It.IsAny<EventId>(),
               It.Is<It.IsAnyType>((o, t) => o.ToString() == "testingScope: message"),
               It.IsAny<Exception>(),
               It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
           Times.Once);
    }

    [Test]
    public void Error_should_call_logger()
    {
        GetLogger().Error("message");

        loggerMock!.Verify(
           x => x.Log(
               LogLevel.Error,
               It.IsAny<EventId>(),
               It.Is<It.IsAnyType>((o, t) => o.ToString() == "testingScope: message"),
               It.IsAny<Exception>(),
               It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
           Times.Once);
    }

    [Test]
    public void Fatal_should_call_logger()
    {
        GetLogger().Fatal("message");

        loggerMock!.Verify(
           x => x.Log(
               LogLevel.Critical,
               It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString() == "testingScope: message"),
               It.IsAny<Exception>(),
               It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
           Times.Once);
    }


    [Test]
    public void Info_should_call_logger()
    {
        GetLogger().Info("message");

        loggerMock!.Verify(
           x => x.Log(
               LogLevel.Information,
               It.IsAny<EventId>(),
               It.Is<It.IsAnyType>((o, t) => o.ToString() == "testingScope: message"),
               It.IsAny<Exception>(),
               It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
           Times.Once);
    }

    [Test]
    public void Warning_should_call_logger()
    {
        GetLogger().Warning("message");

        loggerMock!.Verify(
           x => x.Log(
               LogLevel.Warning,
               It.IsAny<EventId>(),
               It.Is<It.IsAnyType>((o, t) => o.ToString() == "testingScope: message"),
               It.IsAny<Exception>(),
               It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
           Times.Once);
    }

    [Test]
    public void Log_should_call_logger_with_exception_details()
    {
        var ex = new Exception();
        GetLogger().Log(ex);

        loggerMock!.Verify(
           x => x.Log(
               LogLevel.Critical,
               It.IsAny<EventId>(),
               It.Is<It.IsAnyType>((o, t) => o.ToString() == "testingScope: Exception occurred."),
               ex,
               It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
           Times.Once);
    }

    [Test]
    public void Log_should_call_logger_with_message_and_exception_details()
    {
        var ex = new Exception();
        var message = "message";
        GetLogger().Log(message, ex);

        loggerMock!.Verify(
           x => x.Log(
               LogLevel.Critical,
               It.IsAny<EventId>(),
               It.Is<It.IsAnyType>((o, t) => o.ToString() == "testingScope: message"),
               ex,
               It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
           Times.Once);
    }

    private Common.ILogger GetLogger() => new Logger(loggerFactoryMock!.Object).CreateScope("testingScope");
}
