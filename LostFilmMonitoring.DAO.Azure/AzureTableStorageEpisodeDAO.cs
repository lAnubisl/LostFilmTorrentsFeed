// <copyright file="AzureTableStorageEpisodeDAO.cs" company="Alexander Panfilenok">
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
/// Implements <see cref="IEpisodeDao"/> for Azure Table Storage.
/// </summary>
public class AzureTableStorageEpisodeDao : BaseAzureTableStorageDao, IEpisodeDao
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureTableStorageEpisodeDao"/> class.
    /// </summary>
    /// <param name="tableServiceClient">Instance of Azure.Data.Tables.TableServiceClient.</param>
    /// <param name="logger">Instance of Logger.</param>
    public AzureTableStorageEpisodeDao(TableServiceClient tableServiceClient, ILogger? logger)
        : base(tableServiceClient, Constants.MetadataStorageTableNameEpisodes, logger)
    {
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(string seriesName, int seasonNumber, int episideNumber, string quality)
    {
        this.Logger.Info($"Call: {nameof(this.ExistsAsync)}('{seriesName}', {seasonNumber}, {episideNumber}, '{quality}')");
        return (await this.TryCountAsync(tc =>
        {
            var query = tc.QueryAsync<EpisodeTableEntity>(entity =>
                entity.PartitionKey == EscapeKey(seriesName) &&
                entity.Quality == quality &&
                entity.SeasonNumber == seasonNumber &&
                entity.EpisodeNumber == episideNumber);
            return CountAsync(query);
        })) > 0;
    }

    /// <inheritdoc/>
    public async Task<Episode[]> LoadAsync()
    {
        this.Logger.Info($"Call: {nameof(this.LoadAsync)}()");
        return await this.TryGetEntityAsync(tc =>
        {
            var query = tc.QueryAsync<EpisodeTableEntity>();
            return IterateAsync(query, Mapper.Map);
        }) ?? Array.Empty<Episode>();
    }

    /// <inheritdoc/>
    public async Task SaveAsync(Episode episode)
    {
        this.Logger.Info($"Call: {nameof(this.SaveAsync)}(Episode episode)");
        try
        {
            await this.TryExecuteAsync(c => c.UpsertEntityAsync(Mapper.Map(episode)));
        }
        catch (ExternalServiceUnavailableException)
        {
            this.Logger.Fatal($"Error while saving episode: '{JsonSerializer.Serialize(episode, CommonSerializationOptions.Default)}'");
            throw;
        }
    }
}