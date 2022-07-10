// <copyright file="SaveUserCommandTests.cs" company="Alexander Panfilenok">
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
    internal class SaveUserCommandTests
    {
        private Mock<IUserDao> userDao;
        private Mock<IFeedDao> feedDao;
        private Mock<IModelPersister> persister;
        private Mock<ILogger> logger;

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
            var action = () => new SaveUserCommand(null!, this.logger.Object, this.persister.Object, this.feedDao.Object);
            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("userDao");
        }

        [Test]
        public void Constructor_should_throw_exception_when_logger_null()
        {
            var action = () => new SaveUserCommand(this.userDao.Object, null!, this.persister.Object, this.feedDao.Object);
            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("logger");
        }

        [Test]
        public void Constructor_should_throw_exception_when_persister_null()
        {
            var action = () => new SaveUserCommand(this.userDao.Object, this.logger.Object, null!, this.feedDao.Object);
            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("persister");
        }

        [Test]
        public void Constructor_should_throw_exception_when_feedDao_null()
        {
            var action = () => new SaveUserCommand(this.userDao.Object, this.logger.Object, this.persister.Object, null!);
            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("feedDao");
        }

        [Test]
        public void Constructor_should_throw_exception_when_logger_createScope_null()
        {
            this.logger.Setup(x => x.CreateScope(It.IsAny<string>())).Returns((null as ILogger)!);
            var action = () => new SaveUserCommand(this.userDao.Object, this.logger.Object, this.persister.Object, this.feedDao.Object);
            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("logger");
        }

        [Test]
        public async Task ExecuteAsync_should_return_error_when_request_is_null()
        {
            var command = new SaveUserCommand(this.userDao.Object, this.logger.Object, this.persister.Object, this.feedDao.Object);
            var response = await command.ExecuteAsync(null);
            response.UserId.Should().BeNull();
            response.ValidationResult.Should().NotBeNull();
            response.ValidationResult.IsValid.Should().BeFalse();
            response.ValidationResult.Errors.Count.Should().Be(1);
            response.ValidationResult.Errors.First().Key.Should().Be("model");
            response.ValidationResult.Errors.First().Value.Should().Be(ErrorMessages.RequestNull);
        }

        [Test]
        public async Task ExecuteAsync_should_return_error_when_request_trackerId_is_null()
        {
            var command = new SaveUserCommand(this.userDao.Object, this.logger.Object, this.persister.Object, this.feedDao.Object);
            var response = await command.ExecuteAsync(new EditUserRequestModel());
            response.UserId.Should().BeNull();
            response.ValidationResult.Should().NotBeNull();
            response.ValidationResult.IsValid.Should().BeFalse();
            response.ValidationResult.Errors.Count.Should().Be(1);
            response.ValidationResult.Errors.First().Key.Should().Be(nameof(EditUserRequestModel.TrackerId));
            response.ValidationResult.Errors.First().Value.Should().Be(string.Format(ErrorMessages.FieldEmpty, nameof(EditUserRequestModel.TrackerId)));
        }

        [Test]
        public async Task ExecuteAsync_should_generate_userId_for_new_user()
        {
            var trackerIdValue = "TrackerId";
            var command = new SaveUserCommand(this.userDao.Object, this.logger.Object, this.persister.Object, this.feedDao.Object);
            var response = await command.ExecuteAsync(new EditUserRequestModel() { UserId = null, TrackerId = trackerIdValue });
            response.UserId.Should().NotBeNull();
        }

        [Test]
        public async Task ExecuteAsync_should_update_existing_user()
        {
            var trackerIdValue = "TrackerId";
            var userIdValue = "UserId";
            var command = new SaveUserCommand(this.userDao.Object, this.logger.Object, this.persister.Object, this.feedDao.Object);
            var response = await command.ExecuteAsync(new EditUserRequestModel() { UserId = userIdValue, TrackerId = trackerIdValue });
            response.UserId.Should().Be(userIdValue);
        }

        [Test]
        public async Task ExecuteAsync_should_trigger_userDao_saveAsync()
        {
            var trackerIdValue = "TrackerId";
            var userIdValue = "UserId";
            var command = new SaveUserCommand(this.userDao.Object, this.logger.Object, this.persister.Object, this.feedDao.Object);
            var response = await command.ExecuteAsync(new EditUserRequestModel() { UserId = userIdValue, TrackerId = trackerIdValue });
            this.userDao.Verify(x => x.SaveAsync(It.Is<User>(x => x.Id == userIdValue && x.TrackerId == trackerIdValue)), Times.Once);
        }

        [Test]
        public async Task ExecuteAsync_should_trigger_persister_persistAsync()
        {
            var trackerIdValue = "TrackerId";
            var userIdValue = "UserId";
            var command = new SaveUserCommand(this.userDao.Object, this.logger.Object, this.persister.Object, this.feedDao.Object);
            var response = await command.ExecuteAsync(new EditUserRequestModel() { UserId = userIdValue, TrackerId = trackerIdValue });
            this.persister.Verify(x => x.PersistAsync($"subscription_{userIdValue}", Array.Empty<SubscriptionItem>()), Times.Once);
        }

        [Test]
        public async Task ExecuteAsync_should_trigger_feedDAO_saveUserFeedAsync()
        {
            var trackerIdValue = "TrackerId";
            var userIdValue = "UserId";
            var command = new SaveUserCommand(this.userDao.Object, this.logger.Object, this.persister.Object, this.feedDao.Object);
            var response = await command.ExecuteAsync(new EditUserRequestModel() { UserId = userIdValue, TrackerId = trackerIdValue });
            this.feedDao.Verify(x => x.SaveUserFeedAsync(userIdValue, Array.Empty<FeedItem>()), Times.Once);
        }
    }
}
