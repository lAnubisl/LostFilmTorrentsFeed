# Business Logic Layer

## Command Pattern

All business logic implemented as commands injected via DI.

```csharp
// Command with return value
public interface ICommand<TRequestModel, TResponseModel>
{
    Task<TResponseModel> ExecuteAsync(TRequestModel? request);
}

// Command without return value
public interface ICommand
{
    Task ExecuteAsync();
}
```

## Implementation Template

```csharp
public class MyCommand : ICommand<TRequest, TResponse>
{
    private readonly IDal dal;
    private readonly ILogger logger;

    public MyCommand(IDal dal, ILogger logger)
    {
        this.dal = dal ?? throw new ArgumentNullException(nameof(dal));
        this.logger = logger?.CreateScope(nameof(MyCommand)) ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> ExecuteAsync(TRequest? request)
    {
        this.logger.Info($"Call: {nameof(ExecuteAsync)}({request})");
        
        // Validation if validator exists
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new TResponse(validationResult);
        
        // Business logic
        // ...
        
        return new TResponse();
    }
}
```

## Key Commands

- `SaveUserCommand`: Register/update users (creates user, empty subscription, empty feed)
- `SaveSubscriptionCommand`: Update user subscriptions with quality preferences
- `SignInCommand`: Authenticate users
- `UpdateFeedsCommand`: Poll LostFilm RSS, process new episodes, update subscriptions
- `UpdateUserFeedCommand`: Update individual user's RSS feed

## Validators

- Implement `IValidator<T>` interface
- Return `ValidationResult` with errors dictionary
- Use DAOs to check entity existence
- Register in `Program.cs`

## Notes

- Always use scoped logger: `logger.CreateScope(nameof(ClassName))`
- All commands registered as transient in DI
