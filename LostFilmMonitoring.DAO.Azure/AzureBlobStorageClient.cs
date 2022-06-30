﻿// <copyright file="AzureBlobStorageClient.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.Azure
{
    /// <summary>
    /// Manages access to Azure Blob Storage.
    /// </summary>
    public class AzureBlobStorageClient
    {
        private readonly BlobServiceClient blobServiceClient;
        private readonly ILogger logger;

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

        /// <summary>
        /// Upload file content to Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="content">Stream with content of the file.</param>
        /// <param name="cacheControl">Cache-Control property of the file.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        /// <exception cref="ExternalServiceUnavailableException">Error accessing Azure Table Storage.</exception>
        public async Task UploadAsync(string containerName, string fileName, Stream? content, string cacheControl = "no-cache")
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            this.logger.Info($"Call: {nameof(this.UploadAsync)}('{containerName}', '{fileName}', Stream)");
            var blobClient = this.GetBlobClient(containerName, fileName);

            try
            {
                content.Position = 0;
                await blobClient.UploadAsync(content, overwrite: true);
            }
            catch (RequestFailedException ex) when (ex.ErrorCode == "ContainerNotFound")
            {
                await this.CreateContainerAsync(containerName);
                await this.UploadAsync(containerName, fileName, content, cacheControl);
            }
            catch (Exception ex)
            {
                var message = $"Error uploading file '{fileName}' to container '{containerName}'";
                this.logger.Log(message, ex);
                throw new ExternalServiceUnavailableException(message, ex);
            }

            try
            {
                await SetCacheControlAsync(blobClient, cacheControl);
            }
            catch (Exception ex)
            {
                var message = $"Error setting properties of file '{fileName}' in container '{containerName}'";
                this.logger.Log(message, ex);
                throw new ExternalServiceUnavailableException(message, ex);
            }
        }

        /// <summary>
        /// Upload file content to Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="directoryName">Name of the directory in a container.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="content">Stream with content of the file.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        /// <exception cref="ExternalServiceUnavailableException">Error accessing Azure Table Storage.</exception>
        public Task UploadAsync(string containerName, string directoryName, string fileName, Stream? content)
            => this.UploadAsync(containerName, $"{directoryName}/{fileName}", content);

        /// <summary>
        /// Upload file content to Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="content">Text content of the file.</param>
        /// <param name="cacheControl">Cache-Control property of the file.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        /// <exception cref="ExternalServiceUnavailableException">Error accessing Azure Table Storage.</exception>
        public async Task UploadAsync(string containerName, string fileName, string content, string cacheControl = "no-cache")
        {
            this.logger.Info($"Call: {nameof(this.UploadAsync)}('{containerName}', '{fileName}', string content)");
            using Stream ms = new MemoryStream();
            using StreamWriter sw = new StreamWriter(ms, Encoding.UTF8);
            await sw.WriteAsync(content);
            await sw.FlushAsync();
            ms.Position = 0;
            await this.UploadAsync(containerName, fileName, ms, cacheControl);
        }

        /// <summary>
        /// Download file stream from Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        /// <exception cref="ExternalServiceUnavailableException">Error accessing Azure Table Storage.</exception>
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

        /// <summary>
        /// Download text from Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="directoryName">Name of the directory in container.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>String content of the file.</returns>
        /// <exception cref="ExternalServiceUnavailableException">Error accessing Azure Table Storage.</exception>
        public Task<Stream?> DownloadAsync(string containerName, string directoryName, string fileName)
            => this.DownloadAsync(containerName, $"{directoryName}/{fileName}");

        /// <summary>
        /// Download text from Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        /// <exception cref="ExternalServiceUnavailableException">Error accessing Azure Table Storage.</exception>
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

        /// <summary>
        /// Delete file from Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        /// <exception cref="ExternalServiceUnavailableException">Error accessing Azure Table Storage.</exception>
        public async Task DeleteAsync(string containerName, string fileName)
        {
            this.logger.Info($"Call: {nameof(this.DeleteAsync)}('{containerName}', '{fileName}')");
            try
            {
                await this.GetBlobClient(containerName, fileName).DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots);
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

        /// <summary>
        /// Delete file from Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="directoryName">Name of the directory in the container.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        /// <exception cref="ExternalServiceUnavailableException">Error accessing Azure Table Storage.</exception>
        public Task DeleteAsync(string containerName, string directoryName, string fileName)
            => this.DeleteAsync(containerName, $"{directoryName}/{fileName}");

        /// <summary>
        /// Checks if file exists in Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>True - file exists. False - file does not exist.</returns>
        /// <exception cref="ExternalServiceUnavailableException">Error accessing Azure Table Storage.</exception>
        public async Task<bool> ExistsAsync(string containerName, string fileName)
        {
            this.logger.Info($"Call: {nameof(this.ExistsAsync)}('{containerName}', '{fileName}')");
            try
            {
                return await this.GetBlobClient(containerName, fileName).ExistsAsync();
            }
            catch (Exception ex)
            {
                var message = $"Error checking if file '{fileName}' exists in container '{containerName}'.";
                this.logger.Log(message, ex);
                throw new ExternalServiceUnavailableException(message, ex);
            }
        }

        public async Task SetCacheControlAsync(string containerName, string fileName, string cacheControl)
        {
            this.logger.Info($"Call: {nameof(this.SetCacheControlAsync)}('{containerName}', '{fileName}', '{cacheControl}')");
            var blobClient = this.GetBlobClient(containerName, fileName);
            try
            {
                await SetCacheControlAsync(blobClient, cacheControl);
            }
            catch (Exception ex)
            {
                var message = $"Error setting properties of file '{fileName}' in container '{containerName}'";
                this.logger.Log(message, ex);
                throw new ExternalServiceUnavailableException(message, ex);
            }
        }

        private async Task CreateContainerAsync(string containerName)
        {
            try
            {
                await this.blobServiceClient.CreateBlobContainerAsync(containerName);
            }
            catch (RequestFailedException ex) when (ex.ErrorCode == "ContainerAlreadyExists")
            {
                return;
            }
        }

        private static async Task SetCacheControlAsync(BlobClient blobClient, string cacheControl)
        {
            var properties = await blobClient.GetPropertiesAsync();
            var httpHeaders = new BlobHttpHeaders
            {
                CacheControl = cacheControl,
                ContentType = properties.Value.ContentType,
                ContentLanguage = properties.Value.ContentLanguage,
                ContentDisposition = properties.Value.ContentDisposition,
                ContentEncoding = properties.Value.ContentEncoding,
                ContentHash = properties.Value.ContentHash,
            };
            await blobClient.SetHttpHeadersAsync(httpHeaders);
        }

        private BlobClient GetBlobClient(string containerName, string fileName)
            => this.blobServiceClient.GetBlobContainerClient(containerName).GetBlobClient(fileName);

        private async Task<Stream?> DownloadAsync(BlobClient blobClient)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                await blobClient.DownloadToAsync(ms);
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
}