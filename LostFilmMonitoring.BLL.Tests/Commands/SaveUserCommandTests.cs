namespace LostFilmMonitoring.BLL.Tests.Commands;

[ExcludeFromCodeCoverage]
internal class SaveUserCommandTests
{
    private Mock<IUserDao>? userDao;
    private Mock<IFeedDao>? feedDao;
    private Mock<IModelPersister>? persister;
    private Mock<Common.ILogger>? logger;

    [SetUp]
    public void Setup()
    {
        this.userDao = new();
        this.feedDao = new();
        this.persister = new();
        this.logger = new();
        this.logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(this.logger.Object);
    }

    [Test]
    public void Constructor_should_throw_exception_when_userDao_null()
    {
        var action = () => new SaveUserCommand(null!, this.logger!.Object, this.persister!.Object, this.feedDao!.Object);
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("userDao");
    }

    [Test]
    public void Constructor_should_throw_exception_when_logger_null()
    {
        var action = () => new SaveUserCommand(this.userDao!.Object, null!, this.persister!.Object, this.feedDao!.Object);
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("logger");
    }

    [Test]
    public void Constructor_should_throw_exception_when_persister_null()
    {
        var action = () => new SaveUserCommand(this.userDao!.Object, this.logger!.Object, null!, this.feedDao!.Object);
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("persister");
    }

    [Test]
    public void Constructor_should_throw_exception_when_feedDao_null()
    {
        var action = () => new SaveUserCommand(this.userDao!.Object, this.logger!.Object, this.persister!.Object, null!);
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("feedDao");
    }

    [Test]
    public void Constructor_should_throw_exception_when_logger_createScope_null()
    {
        this.logger!.Setup(x => x.CreateScope(It.IsAny<string>())).Returns((null as Common.ILogger)!);
        var action = () => new SaveUserCommand(this.userDao!.Object, this.logger.Object, this.persister!.Object, this.feedDao!.Object);
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("logger");
    }

    [Test]
    public async Task ExecuteAsync_should_return_error_when_request_is_null()
    {
        var command = CreateCommand();
        var response = await command.ExecuteAsync(null);
        response.UserId.Should().BeNull();
        response.ValidationResult.Should().NotBeNull();
        response.ValidationResult.IsValid.Should().BeFalse();
        response.ValidationResult.Errors.Should().ContainSingle().Which.Should().BeEquivalentTo(new { Key = "model", Value = ErrorMessages.RequestNull });
    }

    [Test]
    public async Task ExecuteAsync_should_return_error_when_request_trackerId_is_null()
    {
        var command = CreateCommand();
        var response = await command.ExecuteAsync(new EditUserRequestModel());
        response.UserId.Should().BeNull();
        response.ValidationResult.Should().NotBeNull();
        response.ValidationResult.IsValid.Should().BeFalse();
        response.ValidationResult.Errors.Should().ContainSingle().Which.Should().BeEquivalentTo(new { Key = nameof(EditUserRequestModel.TrackerId), Value = string.Format(ErrorMessages.FieldEmpty, nameof(EditUserRequestModel.TrackerId)) });
    }

    [Test]
    public async Task ExecuteAsync_should_generate_userId_for_new_user()
    {
        var trackerIdValue = "TrackerId";
        var command = CreateCommand();
        var response = await command.ExecuteAsync(new EditUserRequestModel() { UserId = null, TrackerId = trackerIdValue });
        response.UserId.Should().NotBeNull();
    }

    [Test]
    public async Task ExecuteAsync_should_update_existing_user()
    {
        var trackerIdValue = "TrackerId";
        var userIdValue = "UserId";
        var command = CreateCommand();
        var response = await command.ExecuteAsync(new EditUserRequestModel() { UserId = userIdValue, TrackerId = trackerIdValue });
        response.UserId.Should().Be(userIdValue);
    }

    [Test]
    public async Task ExecuteAsync_should_trigger_userDao_saveAsync()
    {
        var trackerIdValue = "TrackerId";
        var userIdValue = "UserId";
        var command = CreateCommand();
        await command.ExecuteAsync(new EditUserRequestModel() { UserId = userIdValue, TrackerId = trackerIdValue });
        this.userDao!.Verify(x => x.SaveAsync(It.Is<User>(x => x.Id == userIdValue && x.TrackerId == trackerIdValue)), Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_should_trigger_persister_persistAsync()
    {
        var trackerIdValue = "TrackerId";
        var userIdValue = "UserId";
        var command = CreateCommand();
        await command.ExecuteAsync(new EditUserRequestModel() { UserId = userIdValue, TrackerId = trackerIdValue });
        this.persister!.Verify(x => x.PersistAsync($"subscription_{userIdValue}", Array.Empty<SubscriptionItem>()), Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_should_trigger_feedDAO_saveUserFeedAsync()
    {
        var trackerIdValue = "TrackerId";
        var userIdValue = "UserId";
        var command = CreateCommand();
        var response = await command.ExecuteAsync(new EditUserRequestModel() { UserId = userIdValue, TrackerId = trackerIdValue });
        this.feedDao!.Verify(x => x.SaveUserFeedAsync(userIdValue, Array.Empty<FeedItem>()), Times.Once);
    }

    private SaveUserCommand CreateCommand() => new SaveUserCommand(this.userDao!.Object, this.logger!.Object, this.persister!.Object, this.feedDao!.Object);
}
