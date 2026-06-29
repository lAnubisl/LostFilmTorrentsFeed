namespace LostFilmMonitoring.DAO.Azure.Tests;

[ExcludeFromCodeCoverage]
public class AzureBlobStorageClientTests
{
    Mock<BlobClient>? blobClient;
    Mock<BlobServiceClient>? blobServiceClient;
    private Mock<Common.ILogger>? logger;

    private readonly string containerName = "TestContatinerName";
    private readonly string blobName = "TestBlobName";
    private readonly string dirName = "DirName";

    [SetUp]
    public void Setup()
    {
        this.blobClient = new Mock<BlobClient>();
        var blobContainerClient = new Mock<BlobContainerClient>();
        this.blobServiceClient = new Mock<BlobServiceClient>();
        this.logger = new();
        this.logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(this.logger.Object);
        this.blobServiceClient
            .Setup(x => x.GetBlobContainerClient(containerName))
            .Returns(blobContainerClient.Object);
        blobContainerClient
            .Setup(x => x.GetBlobClient(blobName))
            .Returns(blobClient.Object);
        blobContainerClient
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
        blobClient!.Verify(x => x.UploadAsync(content, It.IsAny<BlobUploadOptions>(), default), Times.Once);
    }

    [Test]
    public async Task UploadAsync_should_upload_2()
    {
        var azureBlobStorageClient = GetClient();
        await azureBlobStorageClient.UploadAsync(containerName, blobName, "content", "contentType");
        blobClient!.Verify(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), default), Times.Once);
    }

    [Test]
    public async Task UploadAsync_string_should_not_write_bom()
    {
        Stream captured = null!;
        blobClient!
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, BlobUploadOptions, CancellationToken>((s, _, _) =>
            {
                // copy stream content for inspection
                var ms = new MemoryStream();
                s.CopyTo(ms);
                ms.Position = 0;
                captured = ms;
            })
            .ReturnsAsync(Mock.Of<global::Azure.Response<global::Azure.Storage.Blobs.Models.BlobContentInfo>>());

        var azureBlobStorageClient = GetClient();
        var content = "<?xml version=\"1.0\" encoding=\"utf-8\"?><rss></rss>";
        await azureBlobStorageClient.UploadAsync(containerName, blobName, content, "contentType");

        Assert.That(captured, Is.Not.Null);
        var bytes = ReadFully(captured);
        // Ensure there is no UTF-8 BOM at the start
        if (bytes.Length >= 3)
        {
            Assert.Multiple(() =>
            {
                Assert.That(bytes[0], Is.Not.EqualTo((byte)0xEF));
                Assert.That(bytes[1], Is.Not.EqualTo((byte)0xBB));
                Assert.That(bytes[2], Is.Not.EqualTo((byte)0xBF));
            });
        }
    }

    [Test]
    public async Task UploadAsync_should_upload_3()
    {
        var azureBlobStorageClient = GetClient();
        var content = new MemoryStream();
        await azureBlobStorageClient.UploadAsync(containerName, dirName, blobName, content, "contentType");
        blobClient!.Verify(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), default), Times.Once);
    }
    
    [Test]
    public async Task DownloadAsync_should_download_stream()
    {
        var testData = "TEST DATA";
        blobClient!.Setup(x => x.DownloadToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Callback<Stream, CancellationToken>(
            (s, _) =>
            {
                var bytes = Encoding.UTF8.GetBytes(testData);
                s.Write(bytes, 0, bytes.Length);
                s.Seek(0, SeekOrigin.Begin);
            }
        );
        
        var azureBlobStorageClient = GetClient();
        var result = await azureBlobStorageClient.DownloadAsync(containerName, blobName);
        var resultData = Encoding.UTF8.GetString(ReadFully(result));
        Assert.That(string.Equals(testData, resultData), Is.True);
    }

    [Test]
    public async Task DownloadAsync_should_download_stream_2()
    {
        var testData = "TEST DATA";
        blobClient!.Setup(x => x.DownloadToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Callback<Stream, CancellationToken>(
            (s, _) =>
            {
                var bytes = Encoding.UTF8.GetBytes(testData);
                s.Write(bytes, 0, bytes.Length);
                s.Seek(0, SeekOrigin.Begin);
            }
        );

        var azureBlobStorageClient = GetClient();
        var result = await azureBlobStorageClient.DownloadAsync(containerName, dirName, blobName);
        var resultData = Encoding.UTF8.GetString(ReadFully(result));
        Assert.That(string.Equals(testData, resultData), Is.True);
    }

    [Test]
    public async Task DownloadAsync_should_return_null_when_blob_not_found_2()
    {
        blobClient!
            .Setup(x => x.DownloadToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Throws(new RequestFailedException(404, "Not Found", "BlobNotFound", null));

        var azureBlobStorageClient = GetClient();
        var result = await azureBlobStorageClient.DownloadAsync(containerName, dirName, blobName);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DownloadAsync_should_return_null_when_blob_not_found()
    {
        blobClient!
            .Setup(x => x.DownloadToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException(404, "BlobNotFound", "BlobNotFound", null));
        var azureBlobStorageClient = GetClient();
        var result = await azureBlobStorageClient.DownloadAsync(containerName, blobName);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DownloadAsync_should_return_throw_when_unexpected_error()
    {
        var azureBlobStorageClient = GetClient();
        blobClient!
            .Setup(x => x.DownloadToAsync(It.IsAny<Stream>()))
            .ThrowsAsync(new Exception());
        var func = async () => { await azureBlobStorageClient.DownloadAsync("containerName", "fileName"); };
        Assert.CatchAsync<Exception>(async () => await func());
    }

    [Test]
    public async Task DownloadStringAsync_should_return_null_when_blob_not_found()
    {
        blobClient!
            .Setup(x => x.DownloadToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException(404, "BlobNotFound", "BlobNotFound", null));
        
        var azureBlobStorageClient = GetClient();
        var result = await azureBlobStorageClient.DownloadStringAsync(containerName, blobName);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DownloadStringAsync_should_return_string()
    {
        var testData = "TEST DATA";
        blobClient!.Setup(x => x.DownloadToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Callback<Stream, CancellationToken>(
            (s, _) =>
            {
                var bytes = Encoding.UTF8.GetBytes(testData);
                s.Write(bytes, 0, bytes.Length);
                s.Seek(0, SeekOrigin.Begin);
            }
        );
        
        var azureBlobStorageClient = GetClient();
        var result = await azureBlobStorageClient.DownloadStringAsync(containerName, blobName);
        Assert.That(result, Is.EqualTo(testData));
    }

    [Test]
    public async Task DeleteAsync_should_delete_file()
    {
        var azureBlobStorageClient = GetClient();
        await azureBlobStorageClient.DeleteAsync(containerName, blobName);
        blobClient!.Verify(x => x.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots, null, default), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_should_delete_file_2()
    {
        var azureBlobStorageClient = GetClient();
        await azureBlobStorageClient.DeleteAsync(containerName, dirName, blobName);
        blobClient!.Verify(x => x.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots, null, default), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_should_not_fail_when_blob_not_found()
    {
        blobClient!
            .Setup(x => x.DeleteAsync(It.IsAny<DeleteSnapshotsOption>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException(404, "BlobNotFound", "BlobNotFound", null));

        var action = () => GetClient().DeleteAsync(containerName, dirName, blobName);
        Assert.DoesNotThrowAsync(async () => await action());
    }

    [Test]
    public async Task DeleteAsync_should_throw_ExternalServiceUnavailableException_when_anything_unexpected()
    {
        blobClient!
            .Setup(x => x.DeleteAsync(It.IsAny<DeleteSnapshotsOption>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException(400, "InvalidOperation", "InvalidOperation", null));

        var action = () => GetClient().DeleteAsync(containerName, dirName, blobName);
        Assert.ThrowsAsync<ExternalServiceUnavailableException>(async () => await action());
    }

    [Test]
    public async Task ExistsAsync_should_check_if_file_exists()
    {
        var azureBlobStorageClient = GetClient();
        blobClient!
            .Setup(x => x.ExistsAsync(default))
            .ReturnsAsync(new TestResponse<bool>(true));
        await azureBlobStorageClient.ExistsAsync(containerName, blobName);
        blobClient.Verify(x => x.ExistsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ExistsAsync_should_throw_ExternalServiceUnavailableException_when_anything_unexpected()
    {
        blobClient!
            .Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException(400, "InvalidOperation", "InvalidOperation", null));

        var action = () => GetClient().ExistsAsync(containerName, blobName);
        Assert.ThrowsAsync<ExternalServiceUnavailableException>(async () => await action());
    }

    [Test]
    public async Task SetCacheControlAsync_should_set_cache_control()
    {
        var azureBlobStorageClient = GetClient();
        await azureBlobStorageClient.SetCacheControlAsync(containerName, blobName, "cacheControl");
        blobClient!.Verify(x => x.SetHttpHeadersAsync(It.Is<BlobHttpHeaders>(y => y.CacheControl == "cacheControl"), null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task SetCacheControlAsync_should_throw_ExternalServiceUnavailableException_when_anything_unexpected()
    {
        blobClient!
            .Setup(x => x.SetHttpHeadersAsync(It.IsAny<BlobHttpHeaders>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException(400, "InvalidOperation", "InvalidOperation", null));

        var action = () => GetClient().SetCacheControlAsync(containerName, blobName, "cacheControl");
        Assert.ThrowsAsync<ExternalServiceUnavailableException>(async () => await action());
    }

    [Test]
    public async Task UploadAsync_should_throw_exception_when_stream_null()
    {
        var action = () => GetClient().UploadAsync(containerName, blobName, null as Stream, "text/plain");
        var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await action());
        Assert.That(exception!.ParamName, Is.EqualTo("content"));
    }

    private AzureBlobStorageClient GetClient() => new(blobServiceClient!.Object, this.logger!.Object);

    private static byte[] ReadFully(Stream? input)
    {
        if (input == null)
        {
            return [];
        }

        byte[] buffer = new byte[16 * 1024];
        using MemoryStream ms = new ();
        int read = 0;
        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
        {
            ms.Write(buffer, 0, read);
        }
        return ms.ToArray();
    }
}
