// <copyright file="AzureBlobStorageModelPersister.cs" company="Alexander Panfilenok">
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
    /// Implements <see cref="IModelPersister"/> for Azure Blob Storage.
    /// </summary>
    public class AzureBlobStorageModelPersister : IModelPersister
    {
        private readonly IAzureBlobStorageClient azureBlobStorageClient;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobStorageModelPersister"/> class.
        /// </summary>
        /// <param name="azureBlobStorageClient">Instance of AzureBlobStorageClient.</param>
        /// <param name="logger">Instance of Logger.</param>
        public AzureBlobStorageModelPersister(IAzureBlobStorageClient azureBlobStorageClient, ILogger logger)
        {
            this.azureBlobStorageClient = azureBlobStorageClient ?? throw new ArgumentNullException(nameof(azureBlobStorageClient));
            this.logger = logger?.CreateScope(nameof(AzureBlobStorageModelPersister)) ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<T?> LoadAsync<T>(string modelName)
            where T : class
        {
            this.logger.Info($"Call: {nameof(this.LoadAsync)}('{modelName}')");
            try
            {
                var json = await this.azureBlobStorageClient.DownloadStringAsync("models", modelName + ".json");
                if (json == null)
                {
                    return null;
                }

                return JsonSerializer.Deserialize<T>(json);
            }
            catch (ExternalServiceUnavailableException ex)
            {
                this.logger.Log($"Cannot load model '{modelName}'.", ex);
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task PersistAsync<T>(string modelName, T model)
        {
            this.logger.Info($"Call: {nameof(this.PersistAsync)}('{modelName}', model)");
            try
            {
                await this.azureBlobStorageClient.UploadAsync("models", $"{modelName}.json", JsonSerializer.Serialize(model, CommonSerializationOptions.Default), "application/json");
            }
            catch (ExternalServiceUnavailableException ex)
            {
                this.logger.Log($"Cannot persist model '{modelName}'.", ex);
            }
        }
    }
}
