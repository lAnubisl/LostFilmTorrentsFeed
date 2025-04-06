// <copyright file="AzureBlobStorageClient.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.Azure;

/// <summary>
/// Manages access to Azure Blob Storage.
/// </summary>
public class AzureBlobStorageClient : IAzureBlobStorageClient
{
    private static readonly ActivitySource ActivitySource = new (ActivitySourceNames.BlobStorage);
    private readonly BlobServiceClient blobServiceClient;
    private readonly ILogger logger;
    private readonly CancellationToken cancellationToken = CancellationToken.None;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureBlobStorageClient"/> class.
    /// </summary>
    /// <param name="blobServiceClient">Instance of BlobServiceClient.</param>
    /// <param name="logger">Instance of Logger.</param>
    public AzureBlobStorageClient(BlobServiceClient blobServiceClient, ILogger logger)
    {
        this.blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));
        this.logger = logger.CreateScope(nameof(AzureBlobStorageClient)) ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public Task UploadAsync(string containerName, string fileName, Stream? content, string contentType, string cacheControl = "no-cache")
    {
        if (content == null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        this.logger.Info($"Call: {nameof(this.UploadAsync)}('{containerName}', '{fileName}', Stream, '{contentType}', '{cacheControl}')");
        return this.UploadInnerAsync(containerName, fileName, content, contentType, cacheControl);
    }

    /// <inheritdoc/>
    public Task UploadAsync(string containerName, string directoryName, string fileName, Stream? content, string contentType)
        => this.UploadAsync(containerName, $"{directoryName}/{fileName}", content, contentType);

    /// <inheritdoc/>
    public async Task UploadAsync(string containerName, string fileName, string content, string contentType, string cacheControl = "no-cache")
    {
        this.logger.Info($"Call: {nameof(this.UploadAsync)}('{containerName}', '{fileName}', string content)");
        using Stream ms = new MemoryStream();
        using StreamWriter sw = new StreamWriter(ms, Encoding.UTF8);
        await sw.WriteAsync(content);
        await sw.FlushAsync();
        ms.Position = 0;
        using Activity? activity = ActivitySource.StartActivity($"{ActivitySourceNames.BlobStorage}.UploadAsync", ActivityKind.Client);
        await this.UploadAsync(containerName, fileName, ms, contentType, cacheControl);
    }

    /// <inheritdoc/>
    public async Task<Stream?> DownloadAsync(string containerName, string fileName)
    {
        this.logger.Info($"Call: {nameof(this.DownloadAsync)}('{containerName}', '{fileName}')");
        try
        {
            return await this.DownloadAsync(this.GetBlobClient(containerName, fileName));
        }
        catch (Exception ex)
        {
            var message = $"Error downloading file '{fileName}' from container '{containerName}'.";
            this.logger.Log(message, ex);
            throw new ExternalServiceUnavailableException(message, ex);
        }
    }

    /// <inheritdoc/>
    public Task<Stream?> DownloadAsync(string containerName, string directoryName, string fileName)
        => this.DownloadAsync(containerName, $"{directoryName}/{fileName}");

    /// <inheritdoc/>
    public async Task<string?> DownloadStringAsync(string containerName, string fileName)
    {
        this.logger.Info($"Call: {nameof(this.DownloadStringAsync)}('{containerName}', '{fileName}')");
        using var stream = await this.DownloadAsync(containerName, fileName);
        if (stream == null)
        {
            return null;
        }

        using StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
        return await streamReader.ReadToEndAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(string containerName, string fileName)
    {
        this.logger.Info($"Call: {nameof(this.DeleteAsync)}('{containerName}', '{fileName}')");
        try
        {
            BlobClient blobClient = this.GetBlobClient(containerName, fileName);
            using Activity? activity = ActivitySource.StartActivity($"{ActivitySourceNames.BlobStorage}.DeleteAsync", ActivityKind.Client);
            await blobClient.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == "BlobNotFound")
        {
            this.logger.Warning($"File '{fileName}' not found in container '{containerName}'");
            return;
        }
        catch (Exception ex)
        {
            var message = $"Error deleting file '{fileName}' from container '{containerName}'.";
            this.logger.Log(message, ex);
            throw new ExternalServiceUnavailableException(message, ex);
        }

        this.logger.Info($"File '{fileName}' deleted from container '{containerName}'");
    }

    /// <inheritdoc/>
    public Task DeleteAsync(string containerName, string directoryName, string fileName)
        => this.DeleteAsync(containerName, $"{directoryName}/{fileName}");

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(string containerName, string fileName)
    {
        this.logger.Info($"Call: {nameof(this.ExistsAsync)}('{containerName}', '{fileName}')");
        try
        {
            BlobClient blobClient = this.GetBlobClient(containerName, fileName);
            using Activity? activity = ActivitySource.StartActivity($"{ActivitySourceNames.BlobStorage}.ExistsAsync", ActivityKind.Client);
            return await blobClient.ExistsAsync(this.cancellationToken);
        }
        catch (Exception ex)
        {
            var message = $"Error checking if file '{fileName}' exists in container '{containerName}'.";
            this.logger.Log(message, ex);
            throw new ExternalServiceUnavailableException(message, ex);
        }
    }

    /// <inheritdoc/>
    public async Task SetCacheControlAsync(string containerName, string fileName, string cacheControl)
    {
        this.logger.Info($"Call: {nameof(this.SetCacheControlAsync)}('{containerName}', '{fileName}', '{cacheControl}')");
        var blobClient = this.GetBlobClient(containerName, fileName);
        try
        {
            await this.SetCacheControlAsync(blobClient, cacheControl);
        }
        catch (Exception ex)
        {
            var message = $"Error setting properties of file '{fileName}' in container '{containerName}'";
            this.logger.Log(message, ex);
            throw new ExternalServiceUnavailableException(message, ex);
        }
    }

    private async Task SetCacheControlAsync(BlobClient blobClient, string cacheControl)
    {
        Response<BlobProperties>? properties = null;
        using (var activity = ActivitySource.StartActivity($"{ActivitySourceNames.BlobStorage}.CreateContainerAsync", ActivityKind.Client))
        {
            properties = await blobClient.GetPropertiesAsync();
        }

        var httpHeaders = new BlobHttpHeaders
        {
            CacheControl = cacheControl,
            ContentType = properties.Value.ContentType,
            ContentLanguage = properties.Value.ContentLanguage,
            ContentDisposition = properties.Value.ContentDisposition,
            ContentEncoding = properties.Value.ContentEncoding,
            ContentHash = properties.Value.ContentHash,
        };

        using (var activity = ActivitySource.StartActivity($"{ActivitySourceNames.BlobStorage}.SetHttpHeadersAsync", ActivityKind.Client))
        {
            await blobClient.SetHttpHeadersAsync(httpHeaders, cancellationToken: this.cancellationToken);
        }
    }

    private async Task CreateContainerAsync(string containerName)
    {
        try
        {
            using Activity? activity = ActivitySource.StartActivity($"{ActivitySourceNames.BlobStorage}.CreateContainerAsync", ActivityKind.Client);
            await this.blobServiceClient.CreateBlobContainerAsync(containerName);
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == "ContainerAlreadyExists")
        {
            // DO NOTHING
        }
    }

    private async Task UploadInnerAsync(string containerName, string fileName, Stream content, string contentType, string cacheControl = "no-cache")
    {
        var blobClient = this.GetBlobClient(containerName, fileName);

        if (content.CanSeek && content.Position != 0)
        {
            content.Position = 0;
        }

        try
        {
            using (Activity? activity = ActivitySource.StartActivity($"{ActivitySourceNames.BlobStorage}.UploadAsync", ActivityKind.Client))
            {
                await blobClient.UploadAsync(
                    content,
                    new BlobUploadOptions
                    {
                        AccessTier = AccessTier.Hot,
                        HttpHeaders = new BlobHttpHeaders
                        {
                            CacheControl = cacheControl,
                            ContentType = contentType,
                        },
                        Conditions = null,
                    });
            }
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == "ContainerNotFound")
        {
            await this.CreateContainerAsync(containerName);
            await this.UploadInnerAsync(containerName, fileName, content, contentType, cacheControl);
        }
        catch (Exception ex)
        {
            var message = $"Error uploading file '{fileName}' to container '{containerName}'";
            this.logger.Log(message, ex);
            throw new ExternalServiceUnavailableException(message, ex);
        }
    }

    private BlobClient GetBlobClient(string containerName, string fileName)
        => this.blobServiceClient.GetBlobContainerClient(containerName).GetBlobClient(fileName);

    private async Task<Stream?> DownloadAsync(BlobClient blobClient)
    {
        try
        {
            MemoryStream ms = new MemoryStream();
            using Activity? activity = ActivitySource.StartActivity($"{ActivitySourceNames.BlobStorage}.DownloadToAsync", ActivityKind.Client);
            await blobClient.DownloadToAsync(ms, this.cancellationToken);
            ms.Position = 0;
            return ms;
        }
        catch (RequestFailedException ex)
        {
            if (ex.ErrorCode == "BlobNotFound")
            {
                this.logger.Error($"File '{blobClient.Name}' not found in container '{blobClient.BlobContainerName}'.");
                return null;
            }

            throw;
        }
    }
}