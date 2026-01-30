namespace LostFilmMonitoring.AzureFunction.Functions;

/// <summary>
/// Geys user details for user edit page.
/// </summary>
public class GetUserFunction
{
    private readonly Common.ILogger logger;
    private readonly ICommand<GetUserRequestModel, GetUserResponseModel> command;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserFunction"/> class.
    /// </summary>
    /// <param name="logger">Instance of <see cref="Common.ILogger"/>.</param>
    /// <param name="command">Instance of <see cref="ICommand{GetUserRequestModel, GetUserResponseModel}"/>.</param>
    public GetUserFunction(Common.ILogger logger, ICommand<GetUserRequestModel, GetUserResponseModel> command)
    {
        this.logger = logger?.CreateScope(nameof(GetUserFunction)) ?? throw new ArgumentNullException(nameof(logger));
        this.command = command ?? throw new ArgumentNullException(nameof(command));
    }

    /// <summary>
    /// Entry point for Http function execution.
    /// </summary>
    /// <param name="req">Instance of <see cref="HttpRequestData"/>.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    [Function("GetUserFunction")]
    [OpenApiOperation(operationId: "GetUserFunction", tags: ["user"], Visibility = OpenApiVisibilityType.Important)]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(GetUserRequestModel), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GetUserResponseModel))]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        this.logger.Info($"Call: {nameof(this.RunAsync)}(HttpRequestData)");
        var responseModel = await this.command.ExecuteAsync(ModelBinder.Bind<GetUserRequestModel>(req));
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");
        await response.WriteStringAsync(JsonSerializer.Serialize(responseModel, CommonSerializationOptions.Default));
        return response;
    }
}
