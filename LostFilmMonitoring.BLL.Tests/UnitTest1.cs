using LostFilmMonitoring.BLL.Implementations;
using LostFilmMonitoring.Common;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace LostFilmMonitoring.BLL.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var service = new LostFilmMonitoring.BLL.Implementations.LostFilmRegistrationService();
            var result = service.GetNewCaptcha().Result;
            var captcha = "";
            var result2 = service.Register(result.SessionKey, captcha).Result;
            Assert.Null(result);
        }

        [Fact]
        public void Test2()
        {
            var logger = new Mock<ILogger>();
            logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(logger.Object);
            var service = new LostFilmFeedService(logger.Object);
            var result = service.LoadFeedItems().Result;
        }
    }
}
