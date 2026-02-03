# Business Logic Commands

## Command Implementations

Each file implements a specific business operation following command pattern.

## Key Commands

**User Management:**
- `SaveUserCommand`: Register new user or update existing (creates user + empty subscription + empty feed)
- `GetUserCommand`: Retrieve user details for editing
- `SignInCommand`: Authenticate user by checking existence

**Subscription Management:**
- `SaveSubscriptionCommand`: Update user's series subscriptions with quality preferences
- Validates user exists, series exist, and quality values are valid

**Feed Updates:**
- `UpdateFeedsCommand`: Main feed processor - polls LostFilm RSS, processes new episodes, updates all user feeds
- `UpdateUserFeedCommand`: Updates single user's feed with new episode

**Images:**
- `DownloadCoverImageCommand`: Downloads single series cover from TMDB
- `DownloadCoverImagesCommand`: Batch downloads covers for all series

## Command Flow Pattern

```csharp
public async Task<TResponse> ExecuteAsync(TRequest? request)
{
    this.logger.Info($"Call: {nameof(ExecuteAsync)}({request})");
    
    // 1. Validate request
    var validation = await validator.ValidateAsync(request);
    if (!validation.IsValid)
        return new TResponse(validation);
    
    // 2. Execute business logic
    var result = await this.dal.Method(...);
    
    // 3. Return response
    return new TResponse { Success = true, Data = result };
}
```

## Notes

- All commands are stateless
- All dependencies injected via constructor
- Commands don't throw exceptions (return validation errors instead)
- Registered as transient in DI
