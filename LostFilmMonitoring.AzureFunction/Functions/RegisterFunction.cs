namespace LostFilmMonitoring.AzureFunction.Functions;

/// <summary>
/// Responsible for registering user.
/// </summary>
public class RegisterFunction
{
    private readonly Common.ILogger logger;
    private readonly ICommand<EditUserRequestModel, EditUserResponseModel> command;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterFunction"/> class.
    /// </summary>
    /// <param name="logger">Instance of <see cref="Common.ILogger"/>.</param>
    /// <param name="command">Instance of <see cref="ICommand{EditUserRequestModel, EditUserResponseModel}"/>.</param>
    public RegisterFunction(Common.ILogger logger, ICommand<EditUserRequestModel, EditUserResponseModel> command)
    {
        this.logger = logger?.CreateScope(nameof(RegisterFunction)) ?? throw new ArgumentNullException(nameof(logger));
        this.command = command ?? throw new ArgumentNullException(nameof(command));
    }

    /// <summary>
    /// Azure Function Entry Point.
    /// </summary>
    /// <param name="req">Instance of <see cref="HttpRequestData"/>.</param>
    /// <returns>A <see cref="Task{HttpResponseData}"/> representing the result of the asynchronous operation.</returns>
    [Function("RegisterFunction")]
    [OpenApiOperation(operationId: "RegisterFunction", tags: ["user"], Visibility = OpenApiVisibilityType.Important)]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(EditUserRequestModel), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(EditUserResponseModel))]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        this.logger.Info($"Call: {nameof(this.RunAsync)}(HttpRequestData)");
        var responseModel = await this.command.ExecuteAsync(ModelBinder.Bind<EditUserRequestModel>(req));
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");
        await response.WriteStringAsync(JsonSerializer.Serialize(responseModel, CommonSerializationOptions.Default));
        return response;
    }
}
