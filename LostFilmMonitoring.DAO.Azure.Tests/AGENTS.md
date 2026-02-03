# Azure DAO Tests

## Testing Pattern

Mock Azure SDK clients using test helpers:

```csharp
[ExcludeFromCodeCoverage]
internal class AzureTableStorageUserDaoTests : AzureTableStorageDaoTestsBase
{
    private Mock<TableServiceClient> client;
    private Mock<TableClient> tableClient;
    private AzureTableStorageUserDao dao;
    
    [SetUp]
    public void Setup()
    {
        client = new Mock<TableServiceClient>();
        tableClient = new Mock<TableClient>();
        client.Setup(c => c.GetTableClient(It.IsAny<string>())).Returns(tableClient.Object);
        
        dao = new AzureTableStorageUserDao(client.Object, logger.Object);
    }
}
```

## Test Helpers

- `TestAsyncPageable<T>`: Mock Azure pageable responses
- `TestAsyncEnumerable<T>`: Mock async enumerable collections
- `TestResponse<T>`: Mock Azure Response objects
- `AzureTableStorageDaoTestsBase`: Base class with common setup

## Common Mocks

```csharp
// Mock GetEntityAsync
tableClient.Setup(t => t.GetEntityAsync<UserTableEntity>(...))
    .ReturnsAsync(new TestResponse<UserTableEntity>(entity));

// Mock AddEntityAsync
tableClient.Setup(t => t.AddEntityAsync(...))
    .ReturnsAsync(Mock.Of<Response>());

// Mock QueryAsync
tableClient.Setup(t => t.QueryAsync<T>(...))
    .Returns(new TestAsyncPageable<T>(entities));
```

## Notes

- All Azure SDK interactions must be mocked
- Test resources in `Resources/` subdirectory for sample data
