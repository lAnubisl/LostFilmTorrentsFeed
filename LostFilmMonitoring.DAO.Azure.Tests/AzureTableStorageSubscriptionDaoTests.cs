// <copyright file="AzureTableStorageSeriesDaoTests.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.Azure.Tests
{
    [ExcludeFromCodeCoverage]
    public class AzureTableStorageSubscriptionDaoTests : AzureTableStorageDaoTestsBase<AzureTableStorageSubscriptionDao>
    {
        [Test]
        public async Task LoadAsync_should_return_empty_array_when_userId_empty()
        {
            var result = await GetDao().LoadAsync(string.Empty);
            tableClient.Verify(x => x.QueryAsync<SubscriptionTableEntity>(It.IsAny<Expression<Func<SubscriptionTableEntity, bool>>>(), null, null, default), Times.Never);
            Assert.That(result, Is.EqualTo(Array.Empty<Subscription>()));
        }

        [Test]
        public async Task LoadAsync_should_return_empty_array_when_userId_null()
        {
            var result = await GetDao().LoadAsync(null!);
            tableClient.Verify(x => x.QueryAsync<SubscriptionTableEntity>(It.IsAny<Expression<Func<SubscriptionTableEntity, bool>>>(), null, null, default), Times.Never);
            Assert.That(result, Is.EqualTo(Array.Empty<Subscription>()));
        }

        [Test]
        public async Task LoadAsync_should_return_empty_array_when_userId_not_found()
        {
            tableClient
                .Setup(x => x.QueryAsync<SubscriptionTableEntity>(It.IsAny<Expression<Func<SubscriptionTableEntity, bool>>>(), null, null, default))
                .Returns(new TestAsyncPageable<SubscriptionTableEntity>(Array.Empty<SubscriptionTableEntity>()));
            var result = await GetDao().LoadAsync(Guid.NewGuid().ToString());
            tableClient.Verify(x => x.QueryAsync<SubscriptionTableEntity>(It.IsAny<Expression<Func<SubscriptionTableEntity, bool>>>(), null, null, default), Times.Once);
            Assert.That(result, Is.EqualTo(Array.Empty<Subscription>()));
        }

        [Test]
        public async Task LoadAsync_should_return_array_when_userId_found()
        {
            var userId = Guid.NewGuid().ToString();
            var entity = new SubscriptionTableEntity
            {
                PartitionKey = "SeriesName",
                RowKey = userId,
                Quality = "SD",
            };
            tableClient
                .Setup(x => x.QueryAsync<SubscriptionTableEntity>(It.IsAny<Expression<Func<SubscriptionTableEntity, bool>>>(), null, null, default))
                .Returns(new TestAsyncPageable<SubscriptionTableEntity>(new[] { entity }));
            var result = await GetDao().LoadAsync(userId);
            tableClient.Verify(x => x.QueryAsync<SubscriptionTableEntity>(It.IsAny<Expression<Func<SubscriptionTableEntity, bool>>>(), null, null, default), Times.Once);
            Assert.That(result, Is.EquivalentTo(new[] { new Subscription(entity.PartitionKey, entity.Quality) }));
        }

        [Test]
        public async Task LoadAsync_should_return_empty_array_when_user_not_found()
        {
            tableClient
                .Setup(x => x.QueryAsync<SubscriptionTableEntity>(It.IsAny<Expression<Func<SubscriptionTableEntity, bool>>>(), null, null, default))
                .Throws(new RequestFailedException(404, "ResourceNotFound", "ResourceNotFound", null));
            var result = await GetDao().LoadAsync(Guid.NewGuid().ToString());
            tableClient.Verify(x => x.QueryAsync<SubscriptionTableEntity>(It.IsAny<Expression<Func<SubscriptionTableEntity, bool>>>(), null, null, default), Times.Once);
            Assert.That(result, Is.EqualTo(Array.Empty<Subscription>()));
        }

        [Test]
        public async Task LoadUsersIdsAsync_should_return_empty_array_when_userId_empty()
        {
            var result = await GetDao().LoadUsersIdsAsync("A", string.Empty);
            tableClient.Verify(x => x.QueryAsync<SubscriptionTableEntity>(It.IsAny<Expression<Func<SubscriptionTableEntity, bool>>>(), null, null, default), Times.Never);
            Assert.That(result, Is.EqualTo(Array.Empty<string>()));
        }

        [Test]
        public async Task LoadUsersIdsAsync_should_return_empty_array_when_seriesName_empty()
        {
            var result = await GetDao().LoadUsersIdsAsync(string.Empty, "User123");
            tableClient.Verify(x => x.QueryAsync<SubscriptionTableEntity>(It.IsAny<Expression<Func<SubscriptionTableEntity, bool>>>(), null, null, default), Times.Never);
            Assert.That(result, Is.EqualTo(Array.Empty<string>()));
        }

        [Test]
        public async Task LoadUsersIdsAsync_should_return_userIds()
        {
            var seriesName = "SeriesName";
            var userId = "User123";
            var entity = new SubscriptionTableEntity
            {
                PartitionKey = seriesName,
                RowKey = userId,
                Quality = "SD",
            };
            tableClient
                .Setup(x => x.QueryAsync<SubscriptionTableEntity>(It.IsAny<Expression<Func<SubscriptionTableEntity, bool>>>(), null, null, default))
                .Returns(new TestAsyncPageable<SubscriptionTableEntity>(new[] { entity }));
            var result = await GetDao().LoadUsersIdsAsync(seriesName, userId);
            tableClient.Verify(x => x.QueryAsync<SubscriptionTableEntity>(It.IsAny<Expression<Func<SubscriptionTableEntity, bool>>>(), null, null, default), Times.Once);
            Assert.That(result, Is.EquivalentTo(new[] { userId }));
        }

        [Test]
        public async Task LoadUsersIdsAsync_should_return_empty_array_when_subscription_not_found()
        {
            tableClient
                .Setup(x => x.QueryAsync<SubscriptionTableEntity>(It.IsAny<Expression<Func<SubscriptionTableEntity, bool>>>(), null, null, default))
                .Throws(new RequestFailedException(404, "ResourceNotFound", "ResourceNotFound", null));
            var result = await GetDao().LoadUsersIdsAsync("SeriesName", "User123");
            tableClient.Verify(x => x.QueryAsync<SubscriptionTableEntity>(It.IsAny<Expression<Func<SubscriptionTableEntity, bool>>>(), null, null, default), Times.Once);
            Assert.That(result, Is.EqualTo(Array.Empty<string>()));
        }

        [Test]
        public async Task SaveAsync_should_delete_old_subscriptions_and_save_new_subscriptions()
        {
            var existingEntity = new SubscriptionTableEntity
            {
                PartitionKey = "SeriesName 1",
                RowKey = "User123",
                Quality = "SD",
            };
            tableClient
                .Setup(x => x.QueryAsync<SubscriptionTableEntity>(It.IsAny<Expression<Func<SubscriptionTableEntity, bool>>>(), null, null, default))
                .Returns(new TestAsyncPageable<SubscriptionTableEntity>(new[] { existingEntity }));

            await GetDao().SaveAsync("User123", new[] { new Subscription("seriesName 2", "HD") });

            tableClient.Verify(x => x.QueryAsync<SubscriptionTableEntity>(It.IsAny<Expression<Func<SubscriptionTableEntity, bool>>>(), null, null, default), Times.Once);
            tableClient.Verify(x => x.DeleteEntityAsync(existingEntity.PartitionKey, existingEntity.RowKey, default, default), Times.Once);
            tableClient.Verify(x => x.UpsertEntityAsync(It.IsAny<SubscriptionTableEntity>(), TableUpdateMode.Merge, default), Times.Once);
        }

        protected override AzureTableStorageSubscriptionDao GetDao()
            => new(serviceClient.Object, new ConsoleLogger("Tests"));
    }
}
