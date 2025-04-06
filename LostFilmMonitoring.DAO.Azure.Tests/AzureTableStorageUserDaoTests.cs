// <copyright file="AzureTableStorageSeriesDaoTests.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.Azure.Tests;

[ExcludeFromCodeCoverage]
public class AzureTableStorageUserDaoTests : AzureTableStorageDaoTestsBase<AzureTableStorageUserDao>
{
    [Test]
    public async Task LoadAsync_should_return_empty_array_when_no_users()
    {
        tableClient!
            .Setup(x => x.QueryAsync<UserTableEntity>(null as string, null, null, default))
            .Returns(new TestAsyncPageable<UserTableEntity>([]));
        var result = await GetDao().LoadAsync();
        result.Should().Equal([]);
    }

    [Test]
    public async Task LoadAsync_should_return_users()
    {
        var users = new[]
        {
            new UserTableEntity
            {
                PartitionKey = "UserId",
                RowKey = "UserId",
                TrackerId = "TrackerId",
                ETag = new ETag(),
                Timestamp = new DateTimeOffset(DateTime.UtcNow)
            }
        };
        var expected = new[]
        {
            new User("UserId", "TrackerId")
        };
        tableClient!
            .Setup(x => x.QueryAsync<UserTableEntity>(null as string, null, null, default))
            .Returns(new TestAsyncPageable<UserTableEntity>(users));
        var result = await GetDao().LoadAsync();
        Assert.That(result, Is.EquivalentTo(expected));
    }

    [Test]
    public async Task SaveAsync_should_save_user()
    {
        var user = new User("UserId", "TrackerId");
        await GetDao().SaveAsync(user);
        tableClient!.Verify(x => x.UpsertEntityAsync(It.Is<UserTableEntity>(x => x.RowKey == "UserId" && x.TrackerId == "TrackerId"), TableUpdateMode.Merge, default), Times.Once);
    }

    [Test]
    public async Task LoadAsync_should_return_null_when_user_not_found()
    {
        tableClient!
            .Setup(x => x.GetEntityAsync<UserTableEntity>(It.IsAny<string>(), It.IsAny<string>(), null, default))
            .Throws(new RequestFailedException(404, "ResourceNotFound", "ResourceNotFound", null));

        var result = await GetDao().LoadAsync("UserId");
        result.Should().BeNull();
    }

    [Test]
    public async Task LoadAsync_should_return_user()
    {
        var userTableEntity = new UserTableEntity
        {
            RowKey = "UserId",
            TrackerId = "TrackerId",
        };

        tableClient!
            .Setup(x => x.GetEntityAsync<UserTableEntity>(It.IsAny<string>(), It.IsAny<string>(), null, default))
            .ReturnsAsync(new TestResponse<UserTableEntity>(userTableEntity));

        var result = await GetDao().LoadAsync("UserId");
        (result?.Id == userTableEntity.RowKey && result?.TrackerId == userTableEntity.TrackerId).Should().BeTrue();
    }

    protected override AzureTableStorageUserDao GetDao()
        => new(this.serviceClient!.Object, this.logger!.Object);
}
