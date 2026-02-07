# Request/Response Models

## Model Types

**Request Models:** Data transferred from frontend/API to commands
- `EditUserRequestModel`: User registration/update data
- `EditSubscriptionRequestModel`: Subscription update data
- `SignInRequestModel`: User sign-in credentials
- `GetUserRequestModel`: User retrieval request

**Response Models:** Data returned from commands to frontend/API
- `EditUserResponseModel`: User operation results (with UserId)
- `EditSubscriptionResponseModel`: Subscription update confirmation
- `SignInResponseModel`: Authentication result (with UserId)
- `GetUserResponseModel`: User details with subscriptions

**View Models:** JSON models for frontend consumption
- `IndexViewModel`: Series list for subscription page
- `SubscriptionViewModel`: User's current subscriptions

## BaseResponseModel Pattern

```csharp
public class EditUserResponseModel : BaseResponseModel
{
    public string? UserId { get; set; }
    
    public EditUserResponseModel() { }
    
    public EditUserResponseModel(ValidationResult validationResult) 
        : base(validationResult) { }
}

public abstract class BaseResponseModel
{
    public bool IsSuccess => !Errors.Any();
    public Dictionary<string, string> Errors { get; set; } = new();
    
    protected BaseResponseModel() { }
    protected BaseResponseModel(ValidationResult result)
    {
        Errors = result.Errors;
    }
}
```

## Notes

- All models are serializable with System.Text.Json
- Request models should have nullable reference types for validation
- Response models include success/error information
- View models are persisted as JSON in blob storage
