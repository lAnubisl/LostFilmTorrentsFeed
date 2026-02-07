# Business Logic Commands

## Command Implementations

Each file implements a specific business operation following command pattern.

## Key Commands

### User Management:
- `SaveUserCommand`: Register new user or update existing (creates user + empty subscription + empty feed)
- `GetUserCommand`: Retrieve user details for editing
- `SignInCommand`: Authenticate user by checking existence

### Subscription Management:
- `SaveSubscriptionCommand`: Update user's series subscriptions with quality preferences
- Validates user exists, series exist, and quality values are valid

### Feed Updates:
- `UpdateFeedsCommand`: Main feed processor - polls LostFilm RSS, processes new episodes, updates all user feeds
- `UpdateUserFeedCommand`: Updates single user's feed with new episode

### Images:
- `DownloadCoverImageCommand`: Downloads single series cover from TMDB
- `DownloadCoverImagesCommand`: Batch downloads covers for all series

## Command Flow Pattern

The project uses three command interface patterns:

### 1. ICommand<TRequestModel, TResponseModel> - Command with request and response

Example: `SaveUserCommand`, `GetUserCommand`, `SignInCommand`

```csharp
public class SaveUserCommand : ICommand<EditUserRequestModel, EditUserResponseModel>
{
    public async Task<EditUserResponseModel> ExecuteAsync(EditUserRequestModel? request)
    {
        this.logger.Info($"Call: {nameof(ExecuteAsync)}(EditUserRequestModel)");
        
        // 1. Validate request
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return new EditUserResponseModel(validation);
        
        // 2. Execute business logic
        var result = await this.dal.User.SaveAsync(user);
        
        // 3. Return response
        return new EditUserResponseModel { UserId = user.Id };
    }
}
```

### 2. ICommand<in TRequestModel> - Command with request, no response

Example: `DownloadCoverImageCommand` (processes single Series)

```csharp
public class DownloadCoverImageCommand : ICommand<Series>
{
    public async Task ExecuteAsync(Series? series)
    {
        ArgumentNullException.ThrowIfNull(series);
        this.logger.Info($"Call: {nameof(ExecuteAsync)}()");
        
        // Check if already exists
        if (await this.PosterExistsAsync(series.Id))
            return;
        
        // Download and save image
        using var imageStream = await this.tmdbClient.DownloadImageAsync(originalName);
        await this.fileSystem.SaveAsync(containerName, fileName, contentType, imageStream);
    }
}
```

### 3. ICommand - Command with no parameters or response

Example: `DownloadCoverImagesCommand` (batch operation), `UpdateFeedsCommand`

```csharp
public class DownloadCoverImagesCommand : ICommand
{
    public async Task ExecuteAsync()
    {
        this.logger.Info($"Call: {nameof(ExecuteAsync)}()");
        
        // Load all series
        var series = await this.seriesDao.LoadAsync();
        
        // Process each series using single-item command
        foreach (var seriesItem in series)
        {
            await this.downloadCoverImageCommand.ExecuteAsync(seriesItem);
        }
    }
}
```

## Notes

- All commands are stateless
- All dependencies injected via constructor
- Commands don't throw exceptions (return validation errors instead)
- Registered as transient in DI
