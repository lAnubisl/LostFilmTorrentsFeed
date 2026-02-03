# Azure Data Access Implementations

## Storage Strategy

- **Azure Table Storage**: Structured data (users, series, episodes, subscriptions)
- **Azure Blob Storage**: Files (RSS feeds, torrent files, JSON models, images)

## Table Storage Pattern

```csharp
public class AzureTableStorageUserDao : BaseAzureTableStorageDao, IUserDao
{
    public AzureTableStorageUserDao(TableServiceClient client, ILogger logger)
        : base(client, logger, Constants.MetadataStorageTableUsers)
    {
    }
    
    public async Task<User?> LoadAsync(string userId)
    {
        var entity = await GetEntityAsync<UserTableEntity>(userId, userId);
        return entity != null ? Mapper.ToDomainModel(entity) : null;
    }
}
```

## Blob Storage Pattern

```csharp
public class AzureBlobStorageFeedDao : IFeedDao
{
    private readonly IAzureBlobStorageClient client;
    
    public async Task SaveUserFeedAsync(string userId, FeedItem[] items)
    {
        var xml = GenerateRssXml(items);
        await client.UploadAsync(
            Constants.MetadataStorageContainerRssFeeds,
            $"{userId}.xml",
            xml);
    }
}
```

## Entity Mapping

- `*TableEntity`: Azure Table Storage entities (PartitionKey, RowKey, properties)
- `Mapper.cs`: Converts between domain models and table entities
- Use `DictionaryTableEntry` for flexible key-value storage

## Error Handling

- Throw `ExternalServiceUnavailableException` for Azure service failures
- Log all operations with scoped logger

## Authentication

- **Table Storage**: Uses `TableSharedKeyCredential` (fast, low latency)
- **Blob Storage**: Uses `DefaultAzureCredential` (managed identity, RBAC)
