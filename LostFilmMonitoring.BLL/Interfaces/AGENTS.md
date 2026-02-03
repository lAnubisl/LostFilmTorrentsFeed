# Business Logic Interfaces

## Core Interfaces

**Command Interfaces:**
```csharp
// Command with return value
public interface ICommand<TRequestModel, TResponseModel>
{
    Task<TResponseModel> ExecuteAsync(TRequestModel? request);
}

// Command without return value (scheduled tasks)
public interface ICommand
{
    Task ExecuteAsync();
}
```

**Client Interfaces:**
```csharp
public interface ILostFilmClient
{
    Task<TorrentFileResponse> DownloadTorrentFileAsync(string url, string cookie);
}

public interface IRssFeed
{
    Task<IReadOnlyCollection<FeedItemResponse>> LoadFeedItemsAsync(string url, string cookie);
}
```

**Validator Interface:**
```csharp
public interface IValidator<T>
{
    Task<ValidationResult> ValidateAsync(T model);
}
```

**File System Interfaces:**
```csharp
public interface IModelPersister
{
    Task<T?> LoadAsync<T>(string key) where T : class;
    Task SaveAsync<T>(string key, T model) where T : class;
}

public interface IFileSystem
{
    Task<bool> FileExistsAsync(string path);
    Task<Stream?> LoadAsync(string path);
    Task SaveAsync(string path, Stream stream);
}
```

## Notes

- All interfaces use async/await patterns
- Nullable reference types indicate optional results
- Commands are the primary execution units
- Validators are used for input validation before command execution
