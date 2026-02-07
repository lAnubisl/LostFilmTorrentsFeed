# Azure Function Endpoints

## Function Implementations

Each file is a single Azure Function with HTTP trigger.

## Standard Function Template

```csharp
public class MyFunction
{
    private readonly ICommand<TRequest, TResponse> command;
    private readonly ILogger logger;

    public MyFunction(ICommand<TRequest, TResponse> command, ILogger logger)
    {
        this.command = command ?? throw new ArgumentNullException(nameof(command));
        this.logger = logger?.CreateScope(nameof(MyFunction)) ?? throw new ArgumentNullException(nameof(logger));
    }

    [Function("MyFunction")]
    [OpenApiOperation(operationId: "MyFunction", tags: ["category"])]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(TRequest))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, bodyType: typeof(TResponse))]
    public async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        this.logger.Info($"Call: {nameof(RunAsync)}");
        
        var responseModel = await this.command.ExecuteAsync(ModelBinder.Bind<TRequest>(req));
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync(
            JsonSerializer.Serialize(responseModel, CommonSerializationOptions.Default));
        
        return response;
    }
}
```

## Available Functions

- `CheckImagesFunction`: Timer trigger for downloading cover images
- `GetUserFunction`: Retrieve user details
- `RegisterFunction`: Register new user
- `SignInFunction`: User authentication
- `SubscriptionUpdateFunction`: Update user subscriptions
- `UpdateRssFeedFunction`: Timer trigger for RSS feed updates

## Notes

- Timer triggers use cron expressions: `[TimerTrigger("0 */10 * * * *")]`
- All POST endpoints accept JSON and return JSON
- No authorization at function level (handled in commands)
