namespace LostFilmMonitoring.BLL.Commands;

/// <summary>
/// Sign in user.
/// </summary>
public class SignInCommand : ICommand<SignInRequestModel, SignInResponseModel>
{
    private static readonly ActivitySource ActivitySource = new (ActivitySourceNames.SignInCommand);
    private readonly ILogger logger;
    private readonly IUserDao userDao;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignInCommand"/> class.
    /// </summary>
    /// <param name="userDao">Instance of <see cref="IUserDao"/>.</param>
    /// <param name="logger">Instance of <see cref="ILogger"/>.</param>
    public SignInCommand(IUserDao userDao, ILogger logger)
    {
        this.userDao = userDao ?? throw new ArgumentNullException(nameof(userDao));
        this.logger = logger?.CreateScope(nameof(SignInCommand)) ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<SignInResponseModel> ExecuteAsync(SignInRequestModel? request)
    {
        using var activity = ActivitySource.StartActivity(nameof(this.ExecuteAsync), ActivityKind.Internal);
        this.logger.Info($"Call: {nameof(this.ExecuteAsync)}(SingInRequestModel request);");
        if (request == null)
        {
            return new SignInResponseModel { Success = false };
        }

        if (request.UserId == null)
        {
            return new SignInResponseModel { Success = false };
        }

        var user = await this.userDao.LoadAsync(request.UserId);
        return new SignInResponseModel { Success = user != null };
    }
}
