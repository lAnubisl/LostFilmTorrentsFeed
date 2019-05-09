using LostFilmMonitoring.BLL.Implementations.RssFeedService;
using LostFilmMonitoring.Common;
using Moq;
using Xunit;

namespace LostFilmMonitoring.BLL.Tests
{
    public class ReteOrgRssFeedServiceTests
    {
        [Fact]
        public void LoadFeedItemsShouldReturnFeedItems()
        {
            var logger = new Mock<ILogger>();
            logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(logger.Object);
            var service = new ReteOrgRssFeedService(logger.Object);
            var result = service.LoadFeedItems().Result;
            Assert.NotNull(result);
            Assert.True(result.Count == 15);
        }
    }
}