namespace LostFilmMonitoring.BLL.Tests.Validation;

[ExcludeFromCodeCoverage]
public class EditSubscriptionRequestModelValidatorTests
{
    private Mock<IUserDao>? userDao;
    private Mock<ISeriesDao>? seriesDao;

    [SetUp]
    public void Setup()
    {
        this.userDao = new();
        this.seriesDao = new();
    }

    [Test]
    public void Constructor_should_throw_exception_when_userDao_null()
    {
        var action = () => new EditSubscriptionRequestModelValidator(
            null!,
            this.seriesDao!.Object
        );
        Assert.That(Assert.Throws<ArgumentNullException>(() => action()), Has.Property(nameof(ArgumentNullException.ParamName)).EqualTo("userDAO"));
    }

    [Test]
    public void Constructor_should_throw_exception_when_seriesDAO_null()
    {
        var action = () => new EditSubscriptionRequestModelValidator(
            this.userDao!.Object,
            null!
        );
        Assert.That(Assert.Throws<ArgumentNullException>(() => action()), Has.Property(nameof(ArgumentNullException.ParamName)).EqualTo("seriesDAO"));
    }

    [Test]
    public async Task ValidateAsync_should_return_fail_when_model_is_null()
    {
        var result = await GetService().ValidateAsync(null!);
        Assert.That(result.IsValid, Is.False);
        TestAssert.HasSingleError(result.Errors, "UserId", "Field 'UserId' is empty.");
    }

    [Test]
    public async Task ValidateAsync_should_return_fail_when_userId_is_null()
    {
        var result = await GetService().ValidateAsync(new EditSubscriptionRequestModel());
        Assert.That(result.IsValid, Is.False);
        TestAssert.HasSingleError(result.Errors, "UserId", "Field 'UserId' is empty.");
    }

    [Test]
    public async Task ValidateAsync_should_return_fail_when_items_is_null()
    {
        var result = await GetService().ValidateAsync(new EditSubscriptionRequestModel() { UserId = "userId"});
        Assert.That(result.IsValid, Is.False);
        TestAssert.HasSingleError(result.Errors, "Items", "Field 'Items' is empty.");
    }

    [Test]
    public async Task ValidateAsync_should_return_fail_when_item_seriesName_is_null()
    {
        var result = await GetService().ValidateAsync(new EditSubscriptionRequestModel() { UserId = "userId", Items = [new SubscriptionItem()] });
        Assert.That(result.IsValid, Is.False);
        TestAssert.HasSingleError(result.Errors, "SeriesId", "Field 'SeriesId' is empty.");
    }

    [Test]
    public async Task ValidateAsync_should_return_fail_when_item_quality_is_null()
    {
        var result = await GetService().ValidateAsync(new EditSubscriptionRequestModel() { UserId = "userId", Items = [new SubscriptionItem() { SeriesId = "Series1" }] });
        Assert.That(result.IsValid, Is.False);
        TestAssert.HasSingleError(result.Errors, "Quality", "Field 'Quality' should be in [SD, 1080, MP4].");
    }

    [Test]
    public async Task ValidateAsync_should_return_fail_when_item_seriesName_is_invalid()
    {
        this.seriesDao!.Setup(x => x.LoadAsync("Series1")).ReturnsAsync(null as Series);
        var result = await GetService().ValidateAsync(new EditSubscriptionRequestModel() { UserId = "userId", Items = [new SubscriptionItem() { SeriesId = "Series1", Quality = "SD" }] });
        Assert.That(result.IsValid, Is.False);
        TestAssert.HasSingleError(result.Errors, "SeriesId", "Series 'Series1' does not exist.");
    }

    [Test]
    public async Task ValidateAsync_should_return_fail_when_item_userId_is_invalid()
    {
        var result = await GetService().ValidateAsync(new EditSubscriptionRequestModel() { UserId = "userId", Items = []});
        Assert.That(result.IsValid, Is.False);
        TestAssert.HasSingleError(result.Errors, "UserId", "User with Id 'userId' does not exist.");
    }

    [Test]
    public async Task ValidateAsync_should_return_success()
    {
        var testSeries = new Series(Guid.Parse("11111111-1111-1111-1111-111111111111"), string.Empty, DateTime.UtcNow, string.Empty, null, null, null, null, null, null, null, null, null);
        this.seriesDao!.Setup(x => x.LoadAsync()).ReturnsAsync([testSeries]);
        this.userDao!.Setup(x => x.LoadAsync("userId")).ReturnsAsync(new User(string.Empty, string.Empty));
        var result = await GetService().ValidateAsync(new EditSubscriptionRequestModel() { UserId = "userId", Items = [new SubscriptionItem() { SeriesId = "11111111-1111-1111-1111-111111111111", Quality = "SD" }] });
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors, Is.Empty);
    }

    private EditSubscriptionRequestModelValidator GetService() => new(
        this.userDao!.Object,
        this.seriesDao!.Object
    );
}
