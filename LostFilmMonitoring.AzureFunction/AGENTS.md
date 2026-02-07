# Azure Functions HTTP Triggers

## Key Patterns

- All functions are `[Function]` decorated classes with `RunAsync` methods
- Use `[HttpTrigger(AuthorizationLevel.Anonymous, "post")]` for HTTP endpoints
- Inject commands via constructor: `ICommand<TRequest, TResponse>`
- Use `ModelBinder.Bind<T>(req)` to deserialize request body
- Serialize responses with `JsonSerializer.Serialize(responseModel, CommonSerializationOptions.Default)`
- Add OpenAPI attributes for documentation: `[OpenApiOperation]`, `[OpenApiRequestBody]`, `[OpenApiResponseWithBody]`

## Constructor Pattern

```csharp
public MyFunction(ICommand<TReq, TRes> command, ILogger logger)
{
    this.command = command ?? throw new ArgumentNullException(nameof(command));
    this.logger = logger?.CreateScope(nameof(MyFunction)) ?? throw new ArgumentNullException(nameof(logger));
}
```

## Response Pattern

```csharp
var responseModel = await command.ExecuteAsync(ModelBinder.Bind<TRequest>(req));
var response = req.CreateResponse(HttpStatusCode.OK);
await response.WriteStringAsync(JsonSerializer.Serialize(responseModel, CommonSerializationOptions.Default));
return response;
```

## Notes

- Functions run in Isolated Worker mode (.NET 8)
- Authentication is handled at command level, not function level
- All dependencies registered in `Program.cs`
