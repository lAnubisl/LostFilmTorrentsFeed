namespace LostFilmMonitoring.AzureFunction.Functions;

/// <summary>
/// Responsible for updating RSS feeds.
/// </summary>
public class UpdateRssFeedFunction
{
    private readonly UpdateFeedsCommand updateFeedCommand;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateRssFeedFunction"/> class.
    /// </summary>
    /// <param name="updateFeedCommand">Instance of <see cref="UpdateFeedsCommand"/>.</param>
    public UpdateRssFeedFunction(UpdateFeedsCommand updateFeedCommand)
    {
        this.updateFeedCommand = updateFeedCommand ?? throw new ArgumentNullException(nameof(updateFeedCommand));
    }

    /// <summary>
    /// Azure Function Entry Point.
    /// </summary>
    /// <param name="myTimer">Timer object.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [Function("UpdateRssFeedFunction")]
    public Task RunAsync([TimerTrigger("0 */5 7-23 * * *")] object myTimer)
    {
        return this.updateFeedCommand.ExecuteAsync();
    }
}
