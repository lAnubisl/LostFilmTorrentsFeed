namespace LostFilmMonitoring.BLL.Tests.Validation;

[ExcludeFromCodeCoverage]
public class EditSubscriptionRequestModelValidatorTests
{
    private Mock<IUserDao>? userDao;
    private Mock<ISeriesDao>? seriesDao;
    private Mock<Common.ILogger>? logger;

    [SetUp]
    public void Setup()
    {
        this.userDao = new();
        this.seriesDao = new();
        this.logger = new();
        this.logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(this.logger.Object);
    }

    [Test]
    public void Constructor_should_throw_exception_when_userDao_null()
    {
        var action = () => new EditSubscriptionRequestModelValidator(
            null!,
            this.seriesDao!.Object
        );
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("userDAO");
    }

    [Test]
    public void Constructor_should_throw_exception_when_seriesDAO_null()
    {
        var action = () => new EditSubscriptionRequestModelValidator(
            this.userDao!.Object,
            null!
        );
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("seriesDAO");
    }

    [Test]
    public async Task ValidateAsync_should_return_fail_when_model_is_null()
    {
        var result = await GetService().ValidateAsync(null!);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().BeEquivalentTo(new { Key = "UserId", Value = "Field 'UserId' is empty." });
    }

    [Test]
    public async Task ValidateAsync_should_return_fail_when_userId_is_null()
    {
        var result = await GetService().ValidateAsync(new EditSubscriptionRequestModel());
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().BeEquivalentTo(new { Key = "UserId", Value = "Field 'UserId' is empty." });
    }

    [Test]
    public async Task ValidateAsync_should_return_fail_when_items_is_null()
    {
        var result = await GetService().ValidateAsync(new EditSubscriptionRequestModel() { UserId = "userId"});
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().BeEquivalentTo(new { Key = "Items", Value = "Field 'Items' is empty." });
    }

    [Test]
    public async Task ValidateAsync_should_return_fail_when_item_seriesName_is_null()
    {
        var result = await GetService().ValidateAsync(new EditSubscriptionRequestModel() { UserId = "userId", Items = [new SubscriptionItem()] });
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().BeEquivalentTo(new { Key = "SeriesId", Value = "Field 'SeriesId' is empty." });
    }

    [Test]
    public async Task ValidateAsync_should_return_fail_when_item_quality_is_null()
    {
        var result = await GetService().ValidateAsync(new EditSubscriptionRequestModel() { UserId = "userId", Items = [new SubscriptionItem() { SeriesId = "Series1" }] });
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().BeEquivalentTo(new { Key = "Quality", Value = "Field 'Quality' should be in [SD, 1080, MP4]." });
    }

    [Test]
    public async Task ValidateAsync_should_return_fail_when_item_seriesName_is_invalid()
    {
        this.seriesDao!.Setup(x => x.LoadAsync("Series1")).ReturnsAsync(null as Series);
        var result = await GetService().ValidateAsync(new EditSubscriptionRequestModel() { UserId = "userId", Items = [new SubscriptionItem() { SeriesId = "Series1", Quality = "SD" }] });
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().BeEquivalentTo(new { Key = "SeriesId", Value = "Series 'Series1' does not exist." });
    }

    [Test]
    public async Task ValidateAsync_should_return_fail_when_item_userId_is_invalid()
    {
        var result = await GetService().ValidateAsync(new EditSubscriptionRequestModel() { UserId = "userId", Items = []});
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().BeEquivalentTo(new { Key = "UserId", Value = "User with Id 'userId' does not exist." });
    }

    [Test]
    public async Task ValidateAsync_should_return_success()
    {
        var testSeries = new Series(Guid.Parse("11111111-1111-1111-1111-111111111111"), string.Empty, DateTime.UtcNow, string.Empty, null, null, null, null, null, null, null, null, null);
        this.seriesDao!.Setup(x => x.LoadAsync()).ReturnsAsync([testSeries]);
        this.userDao!.Setup(x => x.LoadAsync("userId")).ReturnsAsync(new User(string.Empty, string.Empty));
        var result = await GetService().ValidateAsync(new EditSubscriptionRequestModel() { UserId = "userId", Items = [new SubscriptionItem() { SeriesId = "11111111-1111-1111-1111-111111111111", Quality = "SD" }] });
        result.IsValid.Should().BeTrue();
        result.Errors.Should().HaveCount(0);
    }

    private EditSubscriptionRequestModelValidator GetService() => new(
        this.userDao!.Object,
        this.seriesDao!.Object
    );
}
