// <copyright file="AzureBlobStorageTorrentFileDaoTests.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.Azure.Tests;

[ExcludeFromCodeCoverage]
public class AzureBlobStorageTorrentFileDaoTests
{
    Mock<IAzureBlobStorageClient> azureBlobStorageClient;
    private Mock<Common.ILogger> logger;

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
        var dao = new AzureBlobStorageTorrentFileDao(this.azureBlobStorageClient.Object, this.logger.Object);
        await dao.DeleteUserFileAsync(userId, torrentFileName);
        this.azureBlobStorageClient.Verify(x => x.DeleteAsync("usertorrents", userId, torrentFileName), Times.Once);
    }

    [Test]
    public async Task LoadBaseFileAsync_should_return_null()
    {
        var torrentId = "torrentId";
        var dao = new AzureBlobStorageTorrentFileDao(this.azureBlobStorageClient.Object, this.logger.Object);
        this.azureBlobStorageClient.Setup(x => x.DownloadAsync("basetorrents", $"{torrentId}.torrent")).ReturnsAsync(null as Stream);
        var result = await dao.LoadBaseFileAsync(torrentId);
        this.azureBlobStorageClient.Verify(x => x.DownloadAsync("basetorrents", $"{torrentId}.torrent"), Times.Once);
        Assert.Null(result);
    }

    [Test]
    public async Task LoadBaseFileAsync_should_return_TorrentFile()
    {
        var torrentId = "torrentId";
        var dao = new AzureBlobStorageTorrentFileDao(this.azureBlobStorageClient.Object, this.logger.Object);
        this.azureBlobStorageClient.Setup(x => x.DownloadAsync("basetorrents", $"{torrentId}.torrent")).ReturnsAsync(new MemoryStream());
        var result = await dao.LoadBaseFileAsync(torrentId);
        this.azureBlobStorageClient.Verify(x => x.DownloadAsync("basetorrents", $"{torrentId}.torrent"), Times.Once);
        Assert.NotNull(result);
        Assert.That(string.Equals(result.FileName, $"{torrentId}.torrent"));
    }
    
    [Test]
    public async Task SaveBaseFileAsync_should_call_UploadAsync()
    {
        var torrentId = "torrentId";
        var dao = new AzureBlobStorageTorrentFileDao(this.azureBlobStorageClient.Object, this.logger.Object);
        await dao.SaveBaseFileAsync(torrentId, new TorrentFile(torrentId, new MemoryStream()));
        this.azureBlobStorageClient.Verify(x => x.UploadAsync("basetorrents", $"{torrentId}.torrent", It.IsAny<Stream>(), "applications/x-bittorrent", "no-cache"), Times.Once);
    }

    [Test]
    public async Task SaveUserFileAsync_should_call_UploadAsync()
    {
        var userId = "userId";
        var dao = new AzureBlobStorageTorrentFileDao(this.azureBlobStorageClient.Object, this.logger.Object);
        await dao.SaveUserFileAsync(userId, new TorrentFile("fileName", new MemoryStream()));
        this.azureBlobStorageClient.Verify(x => x.UploadAsync("usertorrents", userId, "fileName.torrent", It.IsAny<Stream>(), "applications/x-bittorrent"), Times.Once);
    }
}
