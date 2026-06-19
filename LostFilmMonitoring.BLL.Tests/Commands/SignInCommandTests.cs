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
        Assert.That(response.Success, Is.False);
    }

    [Test]
    public async Task ExecuteAsync_should_return_success_false_when_request_userId_is_null()
    {
        var command = CreateSignInCommand();
        var response = await command.ExecuteAsync(new Models.Request.SignInRequestModel());
        Assert.That(response.Success, Is.False);
    }

    [Test]
    public async Task ExecuteAsync_should_return_success_false_when_user_not_found()
    {
        this.userDao!.Setup(x => x.LoadAsync(It.IsAny<string>())).ReturnsAsync(null as User);
        var command = CreateSignInCommand();
        var response = await command.ExecuteAsync(new Models.Request.SignInRequestModel { UserId = "123" });
        Assert.That(response.Success, Is.False);
    }

    [Test]
    public async Task ExecuteAsync_should_return_success_true_when_user_found()
    {
        this.userDao!.Setup(x => x.LoadAsync(It.IsAny<string>())).ReturnsAsync(new User(string.Empty, string.Empty));
        var command = CreateSignInCommand();
        var response = await command.ExecuteAsync(new Models.Request.SignInRequestModel { UserId = "123" });
        Assert.That(response.Success, Is.True);
    }

    [Test]
    public void Constructor_should_throw_exception_when_userDao_null()
    {
        var action = () => new SignInCommand(null!, this.logger!.Object);
        Assert.That(Assert.Throws<ArgumentNullException>(() => action()), Has.Property(nameof(ArgumentNullException.ParamName)).EqualTo("userDao"));
    }

    [Test]
    public void Constructor_should_throw_exception_when_logger_null()
    {
        var action = () => new SignInCommand(this.userDao!.Object, null!);
        Assert.That(Assert.Throws<ArgumentNullException>(() => action()), Has.Property(nameof(ArgumentNullException.ParamName)).EqualTo("logger"));
    }

    [Test]
    public void Constructor_should_throw_exception_when_logger_createScope_null()
    {
        this.logger!.Setup(x => x.CreateScope(It.IsAny<string>())).Returns((null as Common.ILogger)!);
        var action = () => CreateSignInCommand();
        Assert.That(Assert.Throws<ArgumentNullException>(() => action()), Has.Property(nameof(ArgumentNullException.ParamName)).EqualTo("logger"));
    }

    private SignInCommand CreateSignInCommand() => new(this.userDao!.Object, this.logger!.Object);
}
