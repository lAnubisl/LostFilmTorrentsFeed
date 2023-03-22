// <copyright file="AzureBlobStorageFileSystemTests.cs" company="Alexander Panfilenok">
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
        azureBlobStorageClientMock!.Verify(x => x.UploadAsync("directory", "fileName", ms, "contentType", "no-cache"), Times.Once);
    }

    protected AzureBlobStorageFileSystem GetServide()
        => new(this.azureBlobStorageClientMock!.Object, this.logger!.Object);
}
