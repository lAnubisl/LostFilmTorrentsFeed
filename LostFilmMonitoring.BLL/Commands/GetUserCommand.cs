namespace LostFilmMonitoring.BLL.Commands;

/// <summary>
/// Get user information.
/// </summary>
public class GetUserCommand : ICommand<GetUserRequestModel, GetUserResponseModel>
{
    private readonly IUserDao userDao;
    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserCommand"/> class.
    /// </summary>
    /// <param name="userDao">Instance of <see cref="IUserDao"/>.</param>
    /// <param name="logger">Instance of <see cref="ILogger"/>.</param>
    public GetUserCommand(IUserDao userDao, ILogger logger)
    {
        this.logger = logger?.CreateScope(nameof(GetUserCommand)) ?? throw new ArgumentNullException(nameof(logger));
        this.userDao = userDao ?? throw new ArgumentNullException(nameof(userDao));
    }

    /// <inheritdoc/>
    public async Task<GetUserResponseModel> ExecuteAsync(GetUserRequestModel? request)
    {
        this.logger.Info($"Call: {nameof(this.ExecuteAsync)}(GetUserRequestModel request)");
        if (request == null)
        {
            return new GetUserResponseModel(ValidationResult.Fail(ErrorMessages.RequestNull));
        }

        if (request.UserId == null)
        {
            return new GetUserResponseModel(ValidationResult.Fail(nameof(GetUserRequestModel.UserId), ErrorMessages.FieldEmpty));
        }

        var user = await this.userDao.LoadAsync(request.UserId);
        if (user == null)
        {
            return new GetUserResponseModel(ValidationResult.Fail(nameof(GetUserRequestModel.UserId), ErrorMessages.UserDoesNotExist, request.UserId));
        }

        return new GetUserResponseModel(user);
    }
}
