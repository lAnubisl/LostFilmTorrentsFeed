// <copyright file="Class1.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2021 Alexander Panfilenok
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

namespace LostFilmMonitoring.BLL.Tests.Commands
{
    [ExcludeFromCodeCoverage]
    internal class SignInCommandTests
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
        public async Task ExecuteAsync_should_return_success_false_when_request_is_null()
        {
            var command = new SignInCommand(this.userDao.Object, this.logger.Object);
            var response = await command.ExecuteAsync(null);
            response.Success.Should().BeFalse();
        }

        [Test]
        public async Task ExecuteAsync_should_return_success_false_when_request_userId_is_null()
        {
            var command = new SignInCommand(this.userDao.Object, this.logger.Object);
            var response = await command.ExecuteAsync(new Models.Request.SignInRequestModel());
            response.Success.Should().BeFalse();
        }

        [Test]
        public async Task ExecuteAsync_should_return_success_false_when_user_not_found()
        {
            this.userDao.Setup(x => x.LoadAsync(It.IsAny<string>())).ReturnsAsync(null as User);
            var command = new SignInCommand(this.userDao.Object, this.logger.Object);
            var response = await command.ExecuteAsync(new Models.Request.SignInRequestModel { UserId = "123" });
            response.Success.Should().BeFalse();
        }

        [Test]
        public async Task ExecuteAsync_should_return_success_true_when_user_found()
        {
            this.userDao.Setup(x => x.LoadAsync(It.IsAny<string>())).ReturnsAsync(new User(string.Empty, string.Empty));
            var command = new SignInCommand(this.userDao.Object, this.logger.Object);
            var response = await command.ExecuteAsync(new Models.Request.SignInRequestModel { UserId = "123" });
            response.Success.Should().BeTrue();
        }

        [Test]
        public void Constructor_should_throw_exception_when_userDao_null()
        {
            var action = () => new SignInCommand(null!, this.logger.Object);
            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("userDao");
        }

        [Test]
        public void Constructor_should_throw_exception_when_logger_null()
        {
            var action = () => new SignInCommand(this.userDao.Object, null!);
            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("logger");
        }

        [Test]
        public void Constructor_should_throw_exception_when_logger_createScope_null()
        {
            this.logger.Setup(x => x.CreateScope(It.IsAny<string>())).Returns((null as Common.ILogger)!);
            var action = () => new SignInCommand(this.userDao.Object, this.logger.Object);
            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("logger");
        }
    }
}
