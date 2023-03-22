// <copyright file="EditSubscriptionRequestModelValidatorTests.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2023 Alexander Panfilenok
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the 'Software'), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>

namespace LostFilmMonitoring.BLL.Tests.Validation;

[ExcludeFromCodeCoverage]
public class EditSubscriptionRequestModelValidatorTests
{
    private Mock<IUserDao> userDao;
    private Mock<ISeriesDao> seriesDao;
    private Mock<Common.ILogger> logger;

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
            this.seriesDao.Object
        );
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("userDAO");
    }

    [Test]
    public void Constructor_should_throw_exception_when_seriesDAO_null()
    {
        var action = () => new EditSubscriptionRequestModelValidator(
            this.userDao.Object,
            null!
        );
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("seriesDAO");
    }

    [Test]
    public async Task ValidateAsync_should_return_fail_when_model_is_null()
    {
        var result = await GetService().ValidateAsync(null!);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Key.Should().Be("UserId");
        result.Errors.First().Value.Should().Be("Field 'UserId' is empty.");
    }

    [Test]
    public async Task ValidateAsync_should_return_fail_when_userId_is_null()
    {
        var result = await GetService().ValidateAsync(new EditSubscriptionRequestModel());
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Key.Should().Be("UserId");
        result.Errors.First().Value.Should().Be("Field 'UserId' is empty.");
    }

    [Test]
    public async Task ValidateAsync_should_return_fail_when_items_is_null()
    {
        var result = await GetService().ValidateAsync(new EditSubscriptionRequestModel() { UserId = "userId"});
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Key.Should().Be("Items");
        result.Errors.First().Value.Should().Be("Field 'Items' is empty.");
    }

    [Test]
    public async Task ValidateAsync_should_return_fail_when_item_seriesName_is_null()
    {
        var result = await GetService().ValidateAsync(new EditSubscriptionRequestModel() { UserId = "userId", Items = new SubscriptionItem[] { new SubscriptionItem() } });
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Key.Should().Be("SeriesName");
        result.Errors.First().Value.Should().Be("Field 'SeriesName' is empty.");
    }

    [Test]
    public async Task ValidateAsync_should_return_fail_when_item_quality_is_null()
    {
        var result = await GetService().ValidateAsync(new EditSubscriptionRequestModel() { UserId = "userId", Items = new SubscriptionItem[] { new SubscriptionItem() { SeriesName = "Series1" } } });
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Key.Should().Be("Quality");
        result.Errors.First().Value.Should().Be("Field 'Quality' should be in [SD, 1080, MP4].");
    }

    [Test]
    public async Task ValidateAsync_should_return_fail_when_item_seriesName_is_invalid()
    {
        this.seriesDao.Setup(x => x.LoadAsync("Series1")).ReturnsAsync(null as Series);
        var result = await GetService().ValidateAsync(new EditSubscriptionRequestModel() { UserId = "userId", Items = new SubscriptionItem[] { new SubscriptionItem() { SeriesName = "Series1", Quality = "SD" } } });
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Key.Should().Be("SeriesName");
        result.Errors.First().Value.Should().Be("Series 'Series1' does not exist.");
    }

    [Test]
    public async Task ValidateAsync_should_return_fail_when_item_userId_is_invalid()
    {
        var result = await GetService().ValidateAsync(new EditSubscriptionRequestModel() { UserId = "userId", Items = new SubscriptionItem[0]});
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Key.Should().Be("UserId");
        result.Errors.First().Value.Should().Be("User with Id 'userId' does not exist.");
    }

    [Test]
    public async Task ValidateAsync_should_return_success()
    {
        var testSeries = new Series(string.Empty, DateTime.UtcNow, string.Empty, null, null, null, null, null, null, null, null, null);
        this.seriesDao.Setup(x => x.LoadAsync("Series1")).ReturnsAsync(testSeries);
        this.userDao.Setup(x => x.LoadAsync("userId")).ReturnsAsync(new User(string.Empty, string.Empty));
        var result = await GetService().ValidateAsync(new EditSubscriptionRequestModel() { UserId = "userId", Items = new SubscriptionItem[] { new SubscriptionItem() { SeriesName = "Series1", Quality = "SD" } } });
        result.IsValid.Should().BeTrue();
        result.Errors.Should().HaveCount(0);
    }

    private EditSubscriptionRequestModelValidator GetService() => new(
        this.userDao.Object,
        this.seriesDao.Object
    );
}
