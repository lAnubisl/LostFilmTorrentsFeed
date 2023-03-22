// <copyright file="AzureBlobStorageFeedDAOTests.cs" company="Alexander Panfilenok">
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
public class AzureBlobStorageFeedDAOTests
{
    private Mock<IAzureBlobStorageClient> azureBlobStorageClient;
    private Mock<Common.ILogger> logger;
    private string baseFeed;

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
        azureBlobStorageClient.Verify(x => x.DeleteAsync("rssfeeds", "userId"));
    }

    [Test]
    public async Task LoadBaseFeedAsync_should_call_azureBlobStorageClient_downloadStringAsync()
    {
        var result = await GetDao().LoadBaseFeedAsync();
        azureBlobStorageClient.Verify(x => x.DownloadStringAsync("rssfeeds", "baseFeed.xml"));
        var newXml = result.ToArray().GenerateXml();
        Assert.That(newXml, Is.EqualTo(this.baseFeed));
    }

    private AzureBlobStorageFeedDao GetDao()
        => new(azureBlobStorageClient.Object, logger.Object);

    private static string GetFile(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"{assembly.GetName().Name}.Resources.{fileName}";
        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream!);
        return reader.ReadToEnd();
    }
}
