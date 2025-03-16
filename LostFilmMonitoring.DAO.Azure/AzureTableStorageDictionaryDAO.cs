// <copyright file="AzureTableStorageDictionaryDAO.cs" company="Alexander Panfilenok">
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
/// Implementation of <see cref="IDictionaryDao"/> for Azure Table Storage.
/// </summary>
public class AzureTableStorageDictionaryDao : BaseAzureTableStorageDao, IDictionaryDao
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureTableStorageDictionaryDao"/> class.
    /// </summary>
    /// <param name="tableServiceClient">Instance of Azure.Data.Tables.TableServiceClient.</param>
    /// <param name="logger">Instance of Logger.</param>
    public AzureTableStorageDictionaryDao(TableServiceClient tableServiceClient, ILogger? logger)
        : base(tableServiceClient, Constants.MetadataStorageTableNameDictionary, logger)
    {
    }

    /// <inheritdoc/>
    public async Task<IDictionary<string, int>> LoadAsync()
    {
        this.Logger.Info($"Call: {nameof(this.LoadAsync)}()");
        return await this.TryGetEntityAsync(async (tc) =>
        {
            var result = new Dictionary<string, int>();
            await foreach (var item in tc.QueryAsync<DictionaryTableEntity>())
            {
                if (!result.ContainsKey(item.Title))
                {
                    result.Add(item.Title, int.Parse(item.RowKey));
                }
            }

            return result;
        }) ?? new Dictionary<string, int>();
    }
}