﻿// <copyright file="AzureBlobStorageFileSystem.cs" company="Alexander Panfilenok">
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
/// Implements <see cref="IFileSystem"/> for Azure Blob Storage.
/// </summary>
public class AzureBlobStorageFileSystem : IFileSystem
{
    private readonly IAzureBlobStorageClient azureBlobStorageClient;
    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureBlobStorageFileSystem"/> class.
    /// </summary>
    /// <param name="azureBlobStorageClient">Instance of AzureBlobStorageClient.</param>
    /// <param name="logger">Instance of Logger.</param>
    public AzureBlobStorageFileSystem(IAzureBlobStorageClient azureBlobStorageClient, ILogger logger)
    {
        this.azureBlobStorageClient = azureBlobStorageClient ?? throw new ArgumentNullException(nameof(azureBlobStorageClient));
        this.logger = logger?.CreateScope(nameof(AzureBlobStorageFileSystem)) ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public Task<bool> ExistsAsync(string directory, string fileName)
    {
        this.logger.Info($"Call: {nameof(this.ExistsAsync)}({directory}, {fileName})");
        return this.azureBlobStorageClient.ExistsAsync(directory, fileName);
    }

    /// <inheritdoc/>
    public Task SaveAsync(string directory, string fileName, string contentType, Stream contentStream)
    {
        this.logger.Info($"Call: {nameof(this.SaveAsync)}({directory}, {fileName}, {contentStream})");
        return this.azureBlobStorageClient.UploadAsync(directory, fileName, contentStream, contentType);
    }
}