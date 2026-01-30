namespace LostFilmMonitoring.AzureFunction.Functions;

/// <summary>
/// Responsible for signing in the user.
/// </summary>
public class SignInFunction
{
    private readonly Common.ILogger logger;
    private readonly ICommand<SignInRequestModel, SignInResponseModel> command;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignInFunction"/> class.
    /// </summary>
    /// <param name="logger">Instance of <see cref="Common.ILogger"/>.</param>
    /// <param name="command">Instance of <see cref="ICommand"/> to execute.</param>
    public SignInFunction(Common.ILogger logger, ICommand<SignInRequestModel, SignInResponseModel> command)
    {
        this.logger = logger?.CreateScope(nameof(RegisterFunction)) ?? throw new ArgumentNullException(nameof(logger));
        this.command = command ?? throw new ArgumentNullException(nameof(command));
    }

    /// <summary>
    /// Azure Function Entry Point.
    /// </summary>
    /// <param name="req">Instance of <see cref="HttpRequestData"/>.</param>
    /// <returns>A <see cref="Task{HttpResponseData}"/> representing the result of the asynchronous operation.</returns>
    [Function("SignInFunction")]
    [OpenApiOperation(operationId: "SignInFunction", tags: ["user"], Visibility = OpenApiVisibilityType.Important)]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(SignInRequestModel), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(SignInResponseModel))]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        this.logger.Info($"Call: {nameof(this.RunAsync)}(HttpRequestData)");
        var responseModel = await this.command.ExecuteAsync(ModelBinder.Bind<SignInRequestModel>(req));
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");
        await response.WriteStringAsync(JsonSerializer.Serialize(responseModel, CommonSerializationOptions.Default));
        return response;
    }
}
