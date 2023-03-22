// <copyright file="LoggerTests.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2023 Alexander Panfilenok
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the 'Software'), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>

namespace LostFilmMonitoring.BLL.Tests;

[ExcludeFromCodeCoverage]
public class LoggerTests
{
    private Mock<ILoggerFactory> loggerFactoryMock;
    private Mock<Microsoft.Extensions.Logging.ILogger> loggerMock;

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

        loggerMock.Verify(
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

        loggerMock.Verify(
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

        loggerMock.Verify(
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

        loggerMock.Verify(
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

        loggerMock.Verify(
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

        loggerMock.Verify(
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

        loggerMock.Verify(
           x => x.Log(
               LogLevel.Critical,
               It.IsAny<EventId>(),
               It.Is<It.IsAnyType>((o, t) => o.ToString() == "testingScope: message"),
               ex,
               It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
           Times.Once);
    }

    private Common.ILogger GetLogger() => new Logger(loggerFactoryMock.Object).CreateScope("testingScope");
}
