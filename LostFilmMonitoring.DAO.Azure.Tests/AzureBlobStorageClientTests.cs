// <copyright file="AzureBlobStorageClientTests.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2021 Alexander Panfilenok
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

using System.Text;

namespace LostFilmMonitoring.DAO.Azure.Tests
{
    [ExcludeFromCodeCoverage]
    public class AzureBlobStorageClientTests
    {
        Mock<BlobClient> blobClient;
        Mock<BlobContainerClient> blobContainerClient;
        Mock<BlobServiceClient> blobServiceClient;

        [SetUp]
        public void Setup()
        {
            this.blobClient = new Mock<BlobClient>();
            this.blobContainerClient = new Mock<BlobContainerClient>();
            this.blobServiceClient = new Mock<BlobServiceClient>();
            blobServiceClient
                .Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
                .Returns(blobContainerClient.Object);
            blobContainerClient
                .Setup(x => x.GetBlobClient(It.IsAny<string>()))
                .Returns(blobClient.Object);
            blobClient
                .Setup(x => x.GetPropertiesAsync(null, default))
                .ReturnsAsync(new TestResponse<BlobProperties>(new BlobProperties()));
        }

        [Test]
        public async Task UploadAsync_should_upload()
        {
            var azureBlobStorageClient = GetClient();
            var content = new MemoryStream();
            await azureBlobStorageClient.UploadAsync("containerName", "fileName", content, "contentType");
            blobClient.Verify(x => x.UploadAsync(content, It.IsAny<BlobUploadOptions>(), default), Times.Once);
        }

        [Test]
        public async Task UploadAsync_should_upload_2()
        {
            var azureBlobStorageClient = GetClient();
            var stream = new MemoryStream();
            await azureBlobStorageClient.UploadAsync("containerName", "fileName", "content", "contentType");
            blobClient.Verify(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), default), Times.Once);
        }

        [Test]
        public async Task UploadAsync_should_upload_3()
        {
            var azureBlobStorageClient = GetClient();
            var content = new MemoryStream();
            await azureBlobStorageClient.UploadAsync("containerName", "directoryName", "fileName", content, "contentType");
            blobClient.Verify(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), default), Times.Once);
        }
        
        [Test]
        public async Task DownloadAsync_should_download_stream()
        {
            var azureBlobStorageClient = GetClient();
            var stream = new MemoryStream();
            var result = await azureBlobStorageClient.DownloadAsync("containerName", "fileName");
            blobClient.Verify(x => x.DownloadToAsync(result), Times.Once);
        }

        [Test]
        public async Task DownloadAsync_should_return_null_when_blob_not_found()
        {
            var azureBlobStorageClient = GetClient();
            blobClient
                .Setup(x => x.DownloadToAsync(It.IsAny<Stream>()))
                .ThrowsAsync(new RequestFailedException(404, "BlobNotFound", "BlobNotFound", null));
            var result = await azureBlobStorageClient.DownloadAsync("containerName", "fileName");
            Assert.IsNull(result);
        }

        [Test]
        public async Task DownloadAsync_should_return_throw_when_unexpected_error()
        {
            var azureBlobStorageClient = GetClient();
            blobClient
                .Setup(x => x.DownloadToAsync(It.IsAny<Stream>()))
                .ThrowsAsync(new Exception());
            var func = async () => { await azureBlobStorageClient.DownloadAsync("containerName", "fileName"); };
            await func.Should().ThrowAsync<Exception>();
        }

        [Test]
        public async Task DownloadAsync_should_download_stream_2()
        {
            var azureBlobStorageClient = GetClient();
            var stream = new MemoryStream();
            var result = await azureBlobStorageClient.DownloadAsync("containerName", "directoryName", "fileName");
            blobClient.Verify(x => x.DownloadToAsync(result), Times.Once);
            blobContainerClient.Verify(x => x.GetBlobClient("directoryName/fileName"), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_should_delete_file()
        {
            var azureBlobStorageClient = GetClient();
            await azureBlobStorageClient.DeleteAsync("containerName", "fileName");
            blobClient.Verify(x => x.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots, null, default), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_should_delete_file_2()
        {
            var azureBlobStorageClient = GetClient();
            await azureBlobStorageClient.DeleteAsync("containerName", "directoryName", "fileName");
            blobClient.Verify(x => x.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots, null, default), Times.Once);
            blobContainerClient.Verify(x => x.GetBlobClient("directoryName/fileName"), Times.Once);
        }

        [Test]
        public async Task ExistsAsync_should_check_if_file_exists()
        {
            var azureBlobStorageClient = GetClient();
            blobClient
                .Setup(x => x.ExistsAsync(default))
                .ReturnsAsync(new TestResponse<bool>(true));
            await azureBlobStorageClient.ExistsAsync("containerName", "fileName");
            blobClient.Verify(x => x.ExistsAsync(default), Times.Once);
        }

        [Test]
        public async Task SetCacheControlAsync_should_set_cache_control()
        {
            var azureBlobStorageClient = GetClient();
            await azureBlobStorageClient.SetCacheControlAsync("containerName", "fileName", "cacheControl");
            blobClient.Verify(x => x.SetHttpHeadersAsync(It.Is<BlobHttpHeaders>(y => y.CacheControl == "cacheControl"), null, default), Times.Once);
        }
        
        [Test]
        public void UploadAsync_should_throw_exception_when_stream_null()
        {
            var action = async () => await GetClient().UploadAsync("test", "test", null as Stream, "text/plain");
            action.Should().ThrowAsync<ArgumentNullException>().Result.Which.ParamName.Should().Be("content");
        }

        private AzureBlobStorageClient GetClient() => new(blobServiceClient.Object, new ConsoleLogger("tests"));
    }
}
