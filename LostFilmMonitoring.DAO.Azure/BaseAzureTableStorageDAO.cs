// <copyright file="BaseAzureTableStorageDAO.cs" company="Alexander Panfilenok">
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
    /// This class is responsible for managing access to Azure.Data.Tables.TableClient object instance.
    /// </summary>
    public abstract class BaseAzureTableStorageDao
    {
        private readonly TableClient tableClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAzureTableStorageDao"/> class.
        /// </summary>
        /// <param name="tableServiceClient">Instance of Azure.Data.Tables.TableServiceClient.</param>
        /// <param name="tableName">Name of a table to work with.</param>
        /// <param name="logger">Instance of Logger.</param>
        protected BaseAzureTableStorageDao(TableServiceClient tableServiceClient, string tableName, ILogger? logger)
        {
            if (tableServiceClient == null)
            {
                throw new ArgumentNullException(nameof(tableServiceClient));
            }

            tableServiceClient.CreateTableIfNotExists(tableName);
            this.tableClient = tableServiceClient.GetTableClient(tableName);
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Azure table storage PartitionKey is sensitive to "'" sign.
        /// In order to query records it is required to escape this from query.
        /// </summary>
        /// <param name="str">A sting that can contain "'".</param>
        /// <returns>An escaped string.</returns>
        protected static string EscapeKey(string str) => str.Replace("'", "''");

        /// <summary>
        /// Iterates through Azure Response and generates an array of items.
        /// </summary>
        /// <typeparam name="TResult">Type of resulted array.</typeparam>
        /// <typeparam name="TSource">Type of result from Azure.</typeparam>
        /// <param name="items">Items from Azure.</param>
        /// <param name="func">Mapping function from {TSource} to {TResult}.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">items.</exception>
        protected static Task<TResult[]> IterateAsync<TResult, TSource>(AsyncPageable<TSource> items, Func<TSource, TResult> func)
            where TSource : class
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            return IterateInnerAsync(items, func);
        }

        /// <summary>
        /// Iterates through Azure Response and counts items.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="items">The items.</param>
        /// <returns>A <see cref="Task{integer}"/> representing the result of the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">items.</exception>
        protected static Task<int> CountAsync<TSource>(AsyncPageable<TSource> items)
            where TSource : class
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            return CountInnerAsync(items);
        }

        /// <summary>
        /// Use this function to Insert/Update/Delete data in Azure Table Storage.
        /// </summary>
        /// <param name="func">Function to apply to TableClient.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        /// <exception cref="ExternalServiceUnavailableException">Error accessing Azure Table Storage.</exception>
        protected async Task TryExecuteAsync(Func<TableClient, Task> func)
        {
            try
            {
                await func(this.tableClient);
            }
            catch (Exception ex)
            {
                this.Logger.Log(ex);
                throw new ExternalServiceUnavailableException("Azure Table Storage is not accessible", ex);
            }
        }

        /// <summary>
        /// Use this function to get a single entity from Azure Table storage.
        /// </summary>
        /// <typeparam name="T">Type of an instance to get.</typeparam>
        /// <param name="func">Function to apply to TableClient.</param>
        /// <returns>An instance that represents a singe row in Azure Table Storage.</returns>
        /// <exception cref="ExternalServiceUnavailableException">Error accessing Azure Table Storage.</exception>
        protected async Task<T?> TryGetEntityAsync<T>(Func<TableClient, Task<T>> func)
            where T : class?
        {
            try
            {
                return await func(this.tableClient);
            }
            catch (RequestFailedException ex)
            {
                if (ex.ErrorCode == "ResourceNotFound")
                {
                    return null;
                }

                throw new ExternalServiceUnavailableException("Azure Table Storage is not accessible", ex);
            }
            catch (Exception ex)
            {
                this.Logger.Log(ex);
                throw new ExternalServiceUnavailableException("Azure Table Storage is not accessible", ex);
            }
        }

        /// <summary>
        /// Use this function to get a count entities in Azure Table storage.
        /// </summary>
        /// <param name="func">Function to apply to TableClient.</param>
        /// <returns>An instance that represents a singe row in Azure Table Storage.</returns>
        /// <exception cref="ExternalServiceUnavailableException">Error accessing Azure Table Storage.</exception>
        protected async Task<int> TryCountAsync(Func<TableClient, Task<int>> func)
        {
            try
            {
                return await func(this.tableClient);
            }
            catch (Exception ex)
            {
                this.Logger.Log(ex);
                throw new ExternalServiceUnavailableException("Azure Table Storage is not accessible", ex);
            }
        }

        private static async Task<TResult[]> IterateInnerAsync<TResult, TSource>(AsyncPageable<TSource> items, Func<TSource, TResult> func)
            where TSource : class
        {
            var result = new List<TResult>();
            await foreach (var item in items)
            {
                result.Add(func(item));
            }

            return result.ToArray();
        }

        private static async Task<int> CountInnerAsync<TSource>(AsyncPageable<TSource> items)
            where TSource : class
        {
            int result = 0;
            await foreach (var item in items)
            {
                result++;
            }

            return result;
        }
    }
}
