// <copyright file="AzureTableStorageUserDAO.cs" company="Alexander Panfilenok">
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
    /// Implements <see cref="IUserDAO"/> for Azure Table Storage.
    /// </summary>
    public class AzureTableStorageUserDAO : BaseAzureTableStorageDAO, IUserDAO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTableStorageUserDAO"/> class.
        /// </summary>
        /// <param name="tableServiceClient">Instance of Azure.Data.Tables.TableServiceClient.</param>
        /// <param name="logger">Instance of Logger.</param>
        public AzureTableStorageUserDAO(TableServiceClient tableServiceClient, ILogger logger)
            : base(tableServiceClient, "users", logger?.CreateScope(nameof(AzureTableStorageUserDAO)))
        {
        }

        /// <inheritdoc/>
        public Task<User?> LoadAsync(string userId)
        {
            this.Logger.Info($"Call: {nameof(this.LoadAsync)}('{userId}')");
            return this.TryGetEntityAsync(async (tc) =>
            {
                var response = await tc.GetEntityAsync<UserTableEntity>(userId, userId);
                return Mapper.Map(response.Value);
            });
        }

        /// <inheritdoc/>
        public async Task<User[]> LoadAsync()
        {
            this.Logger.Info($"Call: {nameof(this.LoadAsync)}()");
            return await this.TryGetEntityAsync(async (tc) =>
            {
                var result = new List<User>();
                await foreach (var item in tc.QueryAsync<UserTableEntity>())
                {
                    result.Add(Mapper.Map(item));
                }

                return result.ToArray();
            }) ?? Array.Empty<User>();
        }

        /// <inheritdoc/>
        public Task SaveAsync(User user)
        {
            this.Logger.Info($"Call: {nameof(this.SaveAsync)}(User user)");
            return this.TryExecuteAsync(c => c.UpsertEntityAsync(Mapper.Map(user)));
        }
    }
}
