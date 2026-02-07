# Business Logic Exceptions

## ExternalServiceUnavailableException

Thrown when external services (Azure Storage, LostFilm.tv, TMDB) are unavailable.

```csharp
public class ExternalServiceUnavailableException : Exception
{
    public ExternalServiceUnavailableException(string message) 
        : base(message) { }
    
    public ExternalServiceUnavailableException(string message, Exception innerException) 
        : base(message, innerException) { }
}
```

## Usage in DAOs

```csharp
try
{
    var entity = await tableClient.GetEntityAsync<T>(partitionKey, rowKey);
    return entity.Value;
}
catch (RequestFailedException ex) when (ex.Status == 404)
{
    return null; // Not found is expected
}
catch (Exception ex)
{
    throw new ExternalServiceUnavailableException(
        "Azure Table Storage unavailable", ex);
}
```

## Error Handling Strategy

- DAOs throw `ExternalServiceUnavailableException` for service failures
- Commands catch and return error responses (not throw)
- Azure Functions runtime logs unhandled exceptions to Application Insights
- 404 responses are handled as null returns (expected condition)

## Notes

- Used for transient failures (network, timeout, service errors)
- Not used for validation errors (use ValidationResult instead)
- Always include inner exception for diagnostics
