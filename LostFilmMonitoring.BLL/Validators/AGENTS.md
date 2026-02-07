# Request Model Validators

## Validator Pattern

```csharp
public interface IValidator<T>
{
    Task<ValidationResult> ValidateAsync(T model);
}
```

## Implementation Pattern

```csharp
public class EditUserRequestModelValidator : IValidator<EditUserRequestModel>
{
    private readonly IDal dal;
    
    public EditUserRequestModelValidator(IDal dal)
    {
        this.dal = dal ?? throw new ArgumentNullException(nameof(dal));
    }
    
    public async Task<ValidationResult> ValidateAsync(EditUserRequestModel? model)
    {
        if (model == null)
            return ValidationResult.Fail("model", "Model is required");
            
        if (string.IsNullOrWhiteSpace(model.TrackerId))
            return ValidationResult.Fail(nameof(model.TrackerId), "TrackerId is required");
        
        // Database validation
        var existingUser = await this.dal.User.LoadAsync(model.UserId);
        if (existingUser == null)
            return ValidationResult.Fail("UserId", "User not found");
        
        return ValidationResult.Success();
    }
}
```

## Available Validators

- `EditUserRequestModelValidator`: Validates user registration/update (TrackerId required)
- `EditSubscriptionRequestModelValidator`: Validates subscriptions (user exists, series exist, valid quality: SD/1080/MP4)

## Validation Rules

- Check null/empty required fields
- Verify referenced entities exist in database
- Validate enum/constant values (e.g., quality must be SD, 1080, or MP4)
- Return first validation error encountered

## Usage in Commands

```csharp
var validationResult = await validator.ValidateAsync(request);
if (!validationResult.IsValid)
{
    return new ResponseModel(validationResult);
}
```
