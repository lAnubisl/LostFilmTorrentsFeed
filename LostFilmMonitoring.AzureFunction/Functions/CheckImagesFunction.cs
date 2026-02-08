namespace LostFilmMonitoring.AzureFunction.Functions;

/// <summary>
/// Responsible for updating RSS feeds.
/// </summary>
public class CheckImagesFunction
{
    private readonly DownloadCoverImagesCommand command;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckImagesFunction"/> class.
    /// </summary>
    /// <param name="command">Instance of <see cref="UpdateFeedsCommand"/>.</param>
    public CheckImagesFunction(DownloadCoverImagesCommand command)
    {
        this.command = command ?? throw new ArgumentNullException(nameof(command));
    }

    /// <summary>
    /// Azure Function Entry Point.
    /// </summary>
    /// <param name="myTimer">Timer object.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [Function("CheckImagesFunction")]
    public Task RunAsync([TimerTrigger("0 0 0 * * *")] object myTimer)
    {
        return this.command.ExecuteAsync();
    }
}
