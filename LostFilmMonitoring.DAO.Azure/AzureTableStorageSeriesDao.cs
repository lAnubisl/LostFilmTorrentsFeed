// <copyright file="AzureTableStorageSeriesDao.cs" company="Alexander Panfilenok">
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
    /// Implements <see cref="ISeriesDao"/> for Azure Table Storage.
    /// </summary>
    public class AzureTableStorageSeriesDao : BaseAzureTableStorageDao, ISeriesDao
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTableStorageSeriesDao"/> class.
        /// </summary>
        /// <param name="tableServiceClient">Instance of Azure.Data.Tables.TableServiceClient.</param>
        /// <param name="logger">Instance of Logger.</param>
        public AzureTableStorageSeriesDao(TableServiceClient tableServiceClient, ILogger logger)
            : base(tableServiceClient, "series", logger?.CreateScope(nameof(AzureTableStorageUserDao)))
        {
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(Series? series)
        {
            this.Logger.Info($"Call: {nameof(this.DeleteAsync)}(series)");

            if (series == null)
            {
                return;
            }

            try
            {
                await this.TryExecuteAsync(c => c.DeleteEntityAsync(EscapeKey(series.Name), EscapeKey(series.Name)));
            }
            catch (ExternalServiceUnavailableException ex)
            {
                this.Logger.Log($"Error deleting series (Name='{series.Name}')", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<Series?> LoadAsync(string name)
        {
            this.Logger.Info($"Call: {nameof(this.LoadAsync)}('{name}')");
            return await this.TryGetEntityAsync(async (tc) =>
            {
                var response = await tc.GetEntityAsync<SeriesTableEntity>(name, name);
                return Mapper.Map(response.Value);
            });
        }

        /// <inheritdoc/>
        public async Task<Series[]> LoadAsync()
        {
            this.Logger.Info($"Call: {nameof(this.LoadAsync)}()");
            return await this.TryGetEntityAsync(async (tc) =>
            {
                var result = new List<Series>();
                await foreach (var item in tc.QueryAsync<SeriesTableEntity>())
                {
                    result.Add(Mapper.Map(item));
                }

                return result.ToArray();
            }) ?? Array.Empty<Series>();
        }

        /// <inheritdoc/>
        public Task SaveAsync(Series series)
        {
            this.Logger.Info($"Call: {nameof(this.SaveAsync)}(Series series)");
            return this.TryExecuteAsync(c => c.UpsertEntityAsync(Mapper.Map(series)));
        }
    }
}
