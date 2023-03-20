// <copyright file="AzureBlobStorageClientTests.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.Azure.Tests
{
    [ExcludeFromCodeCoverage]
    public class AzureBlobStorageClientTests
    {
        Mock<BlobClient> blobClient;
        Mock<BlobContainerClient> blobContainerClient;
        Mock<BlobServiceClient> blobServiceClient;
        private Mock<Common.ILogger> logger;

        private readonly string containerName = "TestContatinerName";
        private readonly string blobName = "TestBlobName";
        private readonly string dirName = "DirName";

        [SetUp]
        public void Setup()
        {
            this.blobClient = new Mock<BlobClient>();
            this.blobContainerClient = new Mock<BlobContainerClient>();
            this.blobServiceClient = new Mock<BlobServiceClient>();
            this.logger = new();
            this.logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(this.logger.Object);
            this.blobServiceClient
                .Setup(x => x.GetBlobContainerClient(containerName))
                .Returns(blobContainerClient.Object);
            this.blobContainerClient
                .Setup(x => x.GetBlobClient(blobName))
                .Returns(blobClient.Object);
            this.blobContainerClient
                .Setup(x => x.GetBlobClient($"{dirName}/{blobName}"))
                .Returns(blobClient.Object);
            this.blobClient
                .Setup(x => x.GetPropertiesAsync(null, default))
                .ReturnsAsync(new TestResponse<BlobProperties>(new BlobProperties()));
        }

        [Test]
        public async Task UploadAsync_should_upload()
        {
            var azureBlobStorageClient = GetClient();
            var content = new MemoryStream();
            await azureBlobStorageClient.UploadAsync(containerName, blobName, content, "contentType");
            blobClient.Verify(x => x.UploadAsync(content, It.IsAny<BlobUploadOptions>(), default), Times.Once);
        }

        [Test]
        public async Task UploadAsync_should_upload_2()
        {
            var azureBlobStorageClient = GetClient();
            var stream = new MemoryStream();
            await azureBlobStorageClient.UploadAsync(containerName, blobName, "content", "contentType");
            blobClient.Verify(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), default), Times.Once);
        }

        [Test]
        public async Task UploadAsync_should_upload_3()
        {
            var azureBlobStorageClient = GetClient();
            var content = new MemoryStream();
            await azureBlobStorageClient.UploadAsync(containerName, dirName, blobName, content, "contentType");
            blobClient.Verify(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), default), Times.Once);
        }
        
        [Test]
        public async Task DownloadAsync_should_download_stream()
        {
            var testData = "TEST DATA";
            blobClient.Setup(x => x.DownloadToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Callback<Stream, CancellationToken>(
                (s, ct) =>
                {
                    var bytes = Encoding.UTF8.GetBytes(testData);
                    s.Write(bytes, 0, bytes.Length);
                    s.Seek(0, SeekOrigin.Begin);
                }
            );
            
            var azureBlobStorageClient = GetClient();
            var result = await azureBlobStorageClient.DownloadAsync(containerName, blobName);
            var resultData = Encoding.UTF8.GetString(ReadFully(result));
            Assert.That(string.Equals(testData, resultData));
        }

        [Test]
        public async Task DownloadAsync_should_download_stream_2()
        {
            var testData = "TEST DATA";
            blobClient.Setup(x => x.DownloadToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Callback<Stream, CancellationToken>(
                (s, ct) =>
                {
                    var bytes = Encoding.UTF8.GetBytes(testData);
                    s.Write(bytes, 0, bytes.Length);
                    s.Seek(0, SeekOrigin.Begin);
                }
            );


            var azureBlobStorageClient = GetClient();
            var stream = new MemoryStream();
            var result = await azureBlobStorageClient.DownloadAsync(containerName, dirName, blobName);
            var resultData = Encoding.UTF8.GetString(ReadFully(result));
            Assert.That(string.Equals(testData, resultData));
        }

        [Test]
        public async Task DownloadAsync_should_return_null_when_blob_not_found_2()
        {
            blobClient
                .Setup(x => x.DownloadToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Throws(new RequestFailedException(404, "Not Found", "BlobNotFound", null));

            var azureBlobStorageClient = GetClient();
            var result = await azureBlobStorageClient.DownloadAsync(containerName, dirName, blobName);
            result.Should().BeNull();
        }

        [Test]
        public async Task DownloadAsync_should_return_null_when_blob_not_found()
        {
            blobClient
                .Setup(x => x.DownloadToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException(404, "BlobNotFound", "BlobNotFound", null));
            var azureBlobStorageClient = GetClient();
            var result = await azureBlobStorageClient.DownloadAsync(containerName, blobName);
            result.Should().BeNull();
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
        public async Task DownloadStringAsync_should_return_null_when_blob_not_found()
        {
            blobClient
                .Setup(x => x.DownloadToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException(404, "BlobNotFound", "BlobNotFound", null));
            
            var azureBlobStorageClient = GetClient();
            var result = await azureBlobStorageClient.DownloadStringAsync(containerName, blobName);
            result.Should().BeNull();
        }

        [Test]
        public async Task DownloadStringAsync_should_return_string()
        {
            var testData = "TEST DATA";
            blobClient.Setup(x => x.DownloadToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Callback<Stream, CancellationToken>(
                (s, ct) =>
                {
                    var bytes = Encoding.UTF8.GetBytes(testData);
                    s.Write(bytes, 0, bytes.Length);
                    s.Seek(0, SeekOrigin.Begin);
                }
            );
            
            var azureBlobStorageClient = GetClient();
            var result = await azureBlobStorageClient.DownloadStringAsync(containerName, blobName);
            result.Should().BeEquivalentTo(testData);
        }


        [Test]
        public async Task DeleteAsync_should_delete_file()
        {
            var azureBlobStorageClient = GetClient();
            await azureBlobStorageClient.DeleteAsync(containerName, blobName);
            blobClient.Verify(x => x.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots, null, default), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_should_delete_file_2()
        {
            var azureBlobStorageClient = GetClient();
            await azureBlobStorageClient.DeleteAsync(containerName, dirName, blobName);
            blobClient.Verify(x => x.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots, null, default), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_should_not_fail_when_blob_not_found()
        {
            blobClient
                .Setup(x => x.DeleteAsync(It.IsAny<DeleteSnapshotsOption>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException(404, "BlobNotFound", "BlobNotFound", null));

            var action = () => GetClient().DeleteAsync(containerName, dirName, blobName);
            await action.Should().NotThrowAsync();
        }

        [Test]
        public async Task DeleteAsync_should_throw_ExternalServiceUnavailableException_when_anything_unexpected()
        {
            blobClient
                .Setup(x => x.DeleteAsync(It.IsAny<DeleteSnapshotsOption>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException(400, "InvalidOperation", "InvalidOperation", null));

            var action = () => GetClient().DeleteAsync(containerName, dirName, blobName);
            await action.Should().ThrowAsync<ExternalServiceUnavailableException>();
        }


        [Test]
        public async Task ExistsAsync_should_check_if_file_exists()
        {
            var azureBlobStorageClient = GetClient();
            blobClient
                .Setup(x => x.ExistsAsync(default))
                .ReturnsAsync(new TestResponse<bool>(true));
            await azureBlobStorageClient.ExistsAsync(containerName, blobName);
            blobClient.Verify(x => x.ExistsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task ExistsAsync_should_throw_ExternalServiceUnavailableException_when_anything_unexpected()
        {
            blobClient
                .Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException(400, "InvalidOperation", "InvalidOperation", null));

            var action = () => GetClient().ExistsAsync(containerName, blobName);
            await action.Should().ThrowAsync<ExternalServiceUnavailableException>();
        }


        [Test]
        public async Task SetCacheControlAsync_should_set_cache_control()
        {
            var azureBlobStorageClient = GetClient();
            await azureBlobStorageClient.SetCacheControlAsync(containerName, blobName, "cacheControl");
            blobClient.Verify(x => x.SetHttpHeadersAsync(It.Is<BlobHttpHeaders>(y => y.CacheControl == "cacheControl"), null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task SetCacheControlAsync_should_throw_ExternalServiceUnavailableException_when_anything_unexpected()
        {
            blobClient
                .Setup(x => x.SetHttpHeadersAsync(It.IsAny<BlobHttpHeaders>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException(400, "InvalidOperation", "InvalidOperation", null));

            var action = () => GetClient().SetCacheControlAsync(containerName, blobName, "cacheControl");
            await action.Should().ThrowAsync<ExternalServiceUnavailableException>();
        }


        [Test]
        public async Task UploadAsync_should_throw_exception_when_stream_null()
        {
            var action = () => GetClient().UploadAsync(containerName, blobName, null as Stream, "text/plain");
            var exception = await action.Should().ThrowAsync<ArgumentNullException>();
            exception.Which.ParamName.Should().Be("content");
        }

        private AzureBlobStorageClient GetClient() => new(blobServiceClient.Object, this.logger.Object);

        private static byte[] ReadFully(Stream? input)
        {
            if (input == null) return Array.Empty<byte>();
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
}
