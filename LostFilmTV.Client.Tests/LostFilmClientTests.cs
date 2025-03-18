// <copyright file="LostFilmClientTests.cs" company="Alexander Panfilenok">
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
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("logger");
    }

    [Test]
    public void Constructor_should_throw_exception_when_logger_createScope_null()
    {
        var action = () => new LostFilmClient(new Mock<ILogger>().Object, this.httpClientFactory.Object);
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("logger");
    }

    [Test]
    public void Constructor_should_throw_exception_when_httpClientFactory_null()
    {
        var action = () => new LostFilmClient(this.logger.Object, null!);
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("httpClientFactory");
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
        result.Should().BeNull();
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
        result.Should().BeNull();
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
        result.Should().BeNull();
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
        result.Should().BeNull();
    }

    private static byte[] ReadFully(Stream input)
    {
        byte[] buffer = new byte[16 * 1024];
        using MemoryStream ms = new MemoryStream();
        int read;
        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
        {
            ms.Write(buffer, 0, read);
        }
        return ms.ToArray();
    }
}
