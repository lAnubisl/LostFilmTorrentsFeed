namespace LostFilmMonitoring.AzureFunction.Functions;

/// <summary>
/// Responsible for updating user subscriptions.
/// </summary>
public class SubscriptionUpdateFunction
{
    private readonly Common.ILogger logger;
    private readonly ICommand<EditSubscriptionRequestModel, EditSubscriptionResponseModel> command;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionUpdateFunction"/> class.
    /// </summary>
    /// <param name="logger">Instance of <see cref="Common.ILogger"/>.</param>
    /// <param name="command">Instance of command to execute.</param>
    public SubscriptionUpdateFunction(Common.ILogger logger, ICommand<EditSubscriptionRequestModel, EditSubscriptionResponseModel> command)
    {
        this.logger = logger?.CreateScope(nameof(SubscriptionUpdateFunction)) ?? throw new ArgumentNullException(nameof(logger));
        this.command = command ?? throw new ArgumentNullException(nameof(command));
    }

    /// <summary>
    /// Azure Function Entry Point.
    /// </summary>
    /// <param name="req">Instance of <see cref="HttpRequestData"/>.</param>
    /// <returns>A <see cref="Task{HttpResponseData}"/> representing the result of the asynchronous operation.</returns>
    [Function("SubscriptionUpdateFunction")]
    [OpenApiOperation(operationId: "SubscriptionUpdateFunction", tags: ["Subscription"], Visibility = OpenApiVisibilityType.Important)]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(EditSubscriptionRequestModel), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(EditSubscriptionResponseModel))]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        this.logger.Info($"Call: {nameof(this.RunAsync)}(HttpRequestData)");
        var responseModel = await this.command.ExecuteAsync(ModelBinder.Bind<EditSubscriptionRequestModel>(req));
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");
        await response.WriteStringAsync(JsonSerializer.Serialize(responseModel, CommonSerializationOptions.Default));
        return response;
    }
}
