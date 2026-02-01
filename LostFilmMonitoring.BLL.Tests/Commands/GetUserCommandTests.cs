namespace LostFilmMonitoring.BLL.Tests.Commands;

[ExcludeFromCodeCoverage]
internal class GetUserCommandTests
{
    private Mock<IUserDao>? userDao;
    private Mock<Common.ILogger>? logger;

    [SetUp]
    public void Setup()
    {
        this.userDao = new();
        this.logger = new();
        this.logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(this.logger.Object);
    }

    [Test]
    public void Constructor_should_throw_exception_when_userDao_null()
    {
        var action = () => new GetUserCommand(null!, this.logger!.Object);
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("userDao");
    }

    [Test]
    public void Constructor_should_throw_exception_when_logger_null()
    {
        var action = () => new GetUserCommand(this.userDao!.Object, null!);
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("logger");
    }

    [Test]
    public void Constructor_should_throw_exception_when_logger_createScope_null()
    {
        this.logger!.Setup(x => x.CreateScope(It.IsAny<string>())).Returns((null as Common.ILogger)!);
        var action = () => new GetUserCommand(this.userDao!.Object, this.logger.Object);
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("logger");
    }

    [Test]
    public async Task ExecuteAsync_should_return_error_when_request_is_null()
    {
        var command = CreateCommand();
        var response = await command.ExecuteAsync(null);
        response.TrackerId.Should().BeNull();
        response.ValidationResult.Should().NotBeNull();
        response.ValidationResult.IsValid.Should().BeFalse();
        response.ValidationResult.Errors.Count.Should().Be(1);
        response.ValidationResult.Errors.First().Key.Should().Be("model");
        response.ValidationResult.Errors.First().Value.Should().Be(ErrorMessages.RequestNull);
    }

    [Test]
    public async Task ExecuteAsync_should_return_error_when_request_userId_is_null()
    {
        var command = CreateCommand();
        var response = await command.ExecuteAsync(new GetUserRequestModel());
        response.TrackerId.Should().BeNull();
        response.ValidationResult.Should().NotBeNull();
        response.ValidationResult.IsValid.Should().BeFalse();
        response.ValidationResult.Errors.Count.Should().Be(1);
        response.ValidationResult.Errors.First().Key.Should().Be(nameof(GetUserRequestModel.UserId));
        response.ValidationResult.Errors.First().Value.Should().Be(string.Format(ErrorMessages.FieldEmpty, nameof(GetUserRequestModel.UserId)));
    }

    [Test]
    public async Task ExecuteAsync_should_return_error_when_user_not_found()
    {
        var testUserIdValue = "123";
        this.userDao!.Setup(x => x.LoadAsync(It.IsAny<string>())).ReturnsAsync(null as User);
        var command = CreateCommand();
        var response = await command.ExecuteAsync(new GetUserRequestModel { UserId = testUserIdValue });
        response.TrackerId.Should().BeNull();
        response.ValidationResult.Should().NotBeNull();
        response.ValidationResult.IsValid.Should().BeFalse();
        response.ValidationResult.Errors.Count.Should().Be(1);
        response.ValidationResult.Errors.First().Key.Should().Be(nameof(GetUserRequestModel.UserId));
        response.ValidationResult.Errors.First().Value.Should().Be(string.Format(ErrorMessages.UserDoesNotExist, nameof(GetUserRequestModel.UserId), testUserIdValue));
    }

    [Test]
    public async Task ExecuteAsync_should_return_success_when_user_found()
    {
        var testTrackerIdValue = "TrackerId";
        this.userDao!.Setup(x => x.LoadAsync(It.IsAny<string>())).ReturnsAsync(new User(string.Empty, testTrackerIdValue));
        var command = CreateCommand();
        var response = await command.ExecuteAsync(new GetUserRequestModel { UserId = "123" });
        response.TrackerId.Should().Be(testTrackerIdValue);
        response.ValidationResult.Should().NotBeNull();
        response.ValidationResult.IsValid.Should().BeTrue();
        response.ValidationResult.Errors.Count.Should().Be(0);
    }

    private GetUserCommand CreateCommand() => new(this.userDao!.Object, this.logger!.Object);
}
