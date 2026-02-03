# Client Exceptions

## ExternalServiceUnavailableException

Thrown when LostFilm.tv or rete.org services are unavailable or return errors.

```csharp
public class ExternalServiceUnavailableException : Exception
{
    public ExternalServiceUnavailableException(string message) 
        : base(message) { }
    
    public ExternalServiceUnavailableException(string message, Exception innerException) 
        : base(message, innerException) { }
}
```

## Usage

```csharp
try
{
    var response = await httpClient.GetAsync(url);
    response.EnsureSuccessStatusCode();
}
catch (HttpRequestException ex)
{
    throw new ExternalServiceUnavailableException(
        "Failed to connect to LostFilm.tv", ex);
}
```

## Common Scenarios

- HTTP request timeout
- HTTP 500+ server errors
- Network connectivity issues
- Invalid authentication cookies
- Malformed RSS feed XML

## Notes

- Exception propagates to Azure Functions runtime
- Logged to Application Insights automatically
- Commands should catch and return error response instead of throwing
