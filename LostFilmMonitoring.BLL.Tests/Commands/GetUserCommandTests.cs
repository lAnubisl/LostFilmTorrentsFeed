﻿// <copyright file="GetUserCommand.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.BLL.Tests.Commands;

[ExcludeFromCodeCoverage]
internal class GetUserCommandTests
{
    private Mock<IUserDao> userDao;
    private Mock<Common.ILogger> logger;

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
        var action = () => new GetUserCommand(null!, this.logger.Object);
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("userDao");
    }

    [Test]
    public void Constructor_should_throw_exception_when_logger_null()
    {
        var action = () => new GetUserCommand(this.userDao.Object, null!);
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("logger");
    }

    [Test]
    public void Constructor_should_throw_exception_when_logger_createScope_null()
    {
        this.logger.Setup(x => x.CreateScope(It.IsAny<string>())).Returns((null as Common.ILogger)!);
        var action = () => new GetUserCommand(this.userDao.Object, this.logger.Object);
        action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("logger");
    }

    [Test]
    public async Task ExecuteAsync_should_return_error_when_request_is_null()
    {
        var command = new GetUserCommand(this.userDao.Object, this.logger.Object);
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
        var command = new GetUserCommand(this.userDao.Object, this.logger.Object);
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
        this.userDao.Setup(x => x.LoadAsync(It.IsAny<string>())).ReturnsAsync(null as User);
        var command = new GetUserCommand(this.userDao.Object, this.logger.Object);
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
        this.userDao.Setup(x => x.LoadAsync(It.IsAny<string>())).ReturnsAsync(new User(string.Empty, testTrackerIdValue));
        var command = new GetUserCommand(this.userDao.Object, this.logger.Object);
        var response = await command.ExecuteAsync(new GetUserRequestModel { UserId = "123" });
        response.TrackerId.Should().Be(testTrackerIdValue);
        response.ValidationResult.Should().NotBeNull();
        response.ValidationResult.IsValid.Should().BeTrue();
        response.ValidationResult.Errors.Count.Should().Be(0);
    }
}
