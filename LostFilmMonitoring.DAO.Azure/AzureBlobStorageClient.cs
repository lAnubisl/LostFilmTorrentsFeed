// <copyright file="AzureBlobStorageClient.cs" company="Alexander Panfilenok">
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
using Azure.Storage.Blobs;
using LostFilmMonitoring.Common;

namespace LostFilmMonitoring.DAO.Azure
{
    public class AzureBlobStorageClient
    {
        private readonly BlobServiceClient blobServiceClient;
        private readonly ILogger logger;

        public AzureBlobStorageClient(string storageAccountConnectionString, ILogger logger)
        {
            this.blobServiceClient = new BlobServiceClient(storageAccountConnectionString);
            this.logger = logger.CreateScope(nameof(AzureBlobStorageClient));
        }

        public async Task UploadAsync(string containerName, string fileName, Stream content)
        {
            this.logger.Info($"Call: {nameof(UploadAsync)}('{containerName}', '{fileName}', Stream)");
            BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(content);
            this.logger.Info($"File '{fileName}' uploaded to blob storage.");
        }

        public async Task UploadAsync(string containerName, string fileName, string content)
        {
            this.logger.Info($"Call: {nameof(UploadAsync)}('{containerName}', '{fileName}', string)");
            using Stream ms = new MemoryStream();
            using StreamWriter sw = new StreamWriter(ms, Encoding.UTF8);
            sw.Write(content);
            await this.UploadAsync(containerName, fileName, ms);
        }

        public async Task<Stream> DownloadAsync(string containerName, string fileName)
        {
            this.logger.Info($"Call: {nameof(DownloadAsync)}('{containerName}', '{fileName}')");
            BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            MemoryStream ms = new MemoryStream();
            await blobClient.DownloadToAsync(ms);
            return ms;
        }

        public async Task<string> DownloadStringAsync(string containerName, string fileName)
        {
            this.logger.Info($"Call: {nameof(DownloadStringAsync)}('{containerName}', '{fileName}')");
            using Stream stream = await DownloadAsync(containerName, fileName);
            using StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
            return streamReader.ReadToEnd();
        }

        public async Task DeleteAsync(string containerName, string fileName)
        {
            this.logger.Info($"Call: {nameof(DeleteAsync)}('{containerName}', '{fileName}')");
            BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.DeleteAsync();
            this.logger.Info($"File '{fileName}' deleted from blob storage.");
        }
    }
}
