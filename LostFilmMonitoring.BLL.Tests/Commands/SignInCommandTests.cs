namespace LostFilmMonitoring.BLL.Tests.Commands;

[ExcludeFromCodeCoverage]
internal class SignInCommandTests
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
    public async Task ExecuteAsync_should_return_success_false_when_request_is_null()
    {
        var command = CreateSignInCommand();
        var response = await command.ExecuteAsync(null);
        response.Success.Should().BeFalse();
    }

    [Test]
    public async Task ExecuteAsync_should_return_success_false_when_request_userId_is_null()
    {
        var command = CreateSignInCommand();
        var response = await command.ExecuteAsync(new Models.Request.SignInRequestModel());
        response.Success.Should().BeFalse();
    }

    [Test]
    public async Task ExecuteAsync_should_return_success_false_when_user_not_found()
    {
        this.userDao!.Setup(x => x.LoadAsync(It.IsAny<string>())).ReturnsAsync(null as User);
        var command = CreateSignInCommand();
        var response = await command.ExecuteAsync(new Models.Request.SignInRequestModel { UserId = "123" });
        response.Success.Should().BeFalse();
    }

    [Test]
    public async Task ExecuteAsync_should_return_success_true_when_user_found()
    {
        this.userDao!.Setup(x => x.LoadAsync(It.IsAny<string>())).ReturnsAsync(new User(string.Empty, string.Empty));
        var command = CreateSignInCommand();
        var response = await command.ExecuteAsync(new Models.Request.SignInRequestModel { UserId = "123" });
        response.Success.Should().BeTrue();
    }

    [Test]
    public void Constructor_should_throw_exception_when_userDao_null()
    {
        var action = () => new SignInCommand(null!, this.logger!.Object);
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("userDao");
    }

    [Test]
    public void Constructor_should_throw_exception_when_logger_null()
    {
        var action = () => new SignInCommand(this.userDao!.Object, null!);
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("logger");
    }

    [Test]
    public void Constructor_should_throw_exception_when_logger_createScope_null()
    {
        this.logger!.Setup(x => x.CreateScope(It.IsAny<string>())).Returns((null as Common.ILogger)!);
        var action = () => CreateSignInCommand();
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("logger");
    }

    private SignInCommand CreateSignInCommand() => new(this.userDao!.Object, this.logger!.Object);
}
