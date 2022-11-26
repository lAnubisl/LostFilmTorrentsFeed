// <copyright file="IAzureBlobStorageClient.cs" company="Alexander Panfilenok">
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
    public interface IAzureBlobStorageClient
    {
        /// <summary>
        /// Delete file from Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        /// <exception cref="ExternalServiceUnavailableException">Error accessing Azure Table Storage.</exception>
        Task DeleteAsync(string containerName, string fileName);

        /// <summary>
        /// Delete file from Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="directoryName">Name of the directory in the container.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        /// <exception cref="ExternalServiceUnavailableException">Error accessing Azure Table Storage.</exception>
        Task DeleteAsync(string containerName, string directoryName, string fileName);

        /// <summary>
        /// Download file stream from Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        /// <exception cref="ExternalServiceUnavailableException">Error accessing Azure Table Storage.</exception>
        Task<Stream?> DownloadAsync(string containerName, string fileName);

        /// <summary>
        /// Download text from Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="directoryName">Name of the directory in container.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>String content of the file.</returns>
        /// <exception cref="ExternalServiceUnavailableException">Error accessing Azure Table Storage.</exception>
        Task<Stream?> DownloadAsync(string containerName, string directoryName, string fileName);

        /// <summary>
        /// Download text from Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        /// <exception cref="ExternalServiceUnavailableException">Error accessing Azure Table Storage.</exception>
        Task<string?> DownloadStringAsync(string containerName, string fileName);

        /// <summary>
        /// Checks if file exists in Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>True - file exists. False - file does not exist.</returns>
        /// <exception cref="ExternalServiceUnavailableException">Error accessing Azure Table Storage.</exception>
        Task<bool> ExistsAsync(string containerName, string fileName);

        /// <summary>Sets the cache control property for a file.</summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="cacheControl">The cache control value.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task SetCacheControlAsync(string containerName, string fileName, string cacheControl);

        /// <summary>
        /// Upload file content to Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="content">Stream with content of the file.</param>
        /// <param name="contentType">Content-Type property of the file.</param>
        /// <param name="cacheControl">Cache-Control property of the file.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Content is null.</exception>
        /// <exception cref="ExternalServiceUnavailableException">Error accessing Azure Table Storage.</exception>
        Task UploadAsync(string containerName, string fileName, Stream? content, string contentType, string cacheControl = "no-cache");

        /// <summary>
        /// Upload file content to Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="content">Text content of the file.</param>
        /// <param name="contentType">Content-Type property of the file.</param>
        /// <param name="cacheControl">Cache-Control property of the file.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        /// <exception cref="ExternalServiceUnavailableException">Error accessing Azure Table Storage.</exception>
        Task UploadAsync(string containerName, string fileName, string content, string contentType, string cacheControl = "no-cache");

        /// <summary>
        /// Upload file content to Azure Blob Storage.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="directoryName">Name of the directory in a container.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="content">Stream with content of the file.</param>
        /// <param name="contentType">Content-Type property of the file.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        /// <exception cref="ExternalServiceUnavailableException">Error accessing Azure Table Storage.</exception>
        Task UploadAsync(string containerName, string directoryName, string fileName, Stream? content, string contentType);
    }
}
