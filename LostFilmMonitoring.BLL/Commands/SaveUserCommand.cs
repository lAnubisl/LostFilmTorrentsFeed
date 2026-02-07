namespace LostFilmMonitoring.BLL.Commands;

/// <summary>
/// Saves user.
/// </summary>
public class SaveUserCommand : ICommand<EditUserRequestModel, EditUserResponseModel>
{
    private static readonly ActivitySource ActivitySource = new (ActivitySourceNames.SaveUserCommand);
    private readonly IUserDao userDAO;
    private readonly IFeedDao feedDAO;
    private readonly ILogger logger;
    private readonly IModelPersister persister;

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveUserCommand"/> class.
    /// </summary>
    /// <param name="userDao">Instance of <see cref="IUserDao"/>.</param>
    /// <param name="logger">Instance of <see cref="ILogger"/>.</param>
    /// <param name="persister">Instance of <see cref="IModelPersister"/>.</param>
    /// <param name="feedDao">Instance of <see cref="IFeedDao"/>.</param>
    public SaveUserCommand(IUserDao userDao, ILogger logger, IModelPersister persister, IFeedDao feedDao)
    {
        this.userDAO = userDao ?? throw new ArgumentNullException(nameof(userDao));
        this.logger = logger?.CreateScope(nameof(SaveUserCommand)) ?? throw new ArgumentNullException(nameof(logger));
        this.persister = persister ?? throw new ArgumentNullException(nameof(persister));
        this.feedDAO = feedDao ?? throw new ArgumentNullException(nameof(feedDao));
    }

    /// <summary>
    /// Registers new user.
    /// </summary>
    /// <param name="request">Registration model.</param>
    /// <returns>Awaitable task.</returns>
    public async Task<EditUserResponseModel> ExecuteAsync(EditUserRequestModel? request)
    {
        using var activity = ActivitySource.StartActivity(nameof(this.ExecuteAsync), ActivityKind.Internal);
        this.logger.Info($"Call: {nameof(this.ExecuteAsync)}(EditUserRequestModel model)");
        if (request == null)
        {
            return new EditUserResponseModel(ValidationResult.Fail(ErrorMessages.RequestNull));
        }

        if (request.TrackerId == null)
        {
            return new EditUserResponseModel(ValidationResult.Fail(nameof(EditUserRequestModel.TrackerId), ErrorMessages.FieldEmpty));
        }

        var userId = string.IsNullOrWhiteSpace(request.UserId) ? Guid.NewGuid().ToString() : request.UserId;
        var user = new User(userId, request.TrackerId);
        await this.userDAO.SaveAsync(user);
        await this.persister.PersistAsync($"subscription_{user.Id}", Array.Empty<SubscriptionItem>());
        await this.feedDAO.SaveUserFeedAsync(user.Id, []);
        return new EditUserResponseModel(user.Id);
    }
}
