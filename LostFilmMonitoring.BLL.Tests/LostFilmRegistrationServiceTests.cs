using LostFilmMonitoring.BLL.Implementations;
using LostFilmMonitoring.Common;
using Moq;
using Xunit;

namespace LostFilmMonitoring.BLL.Tests
{
    public class LostFilmRegistrationServiceTests
    {
        [Fact]
        public void GetUidShouldReturnUid()
        {
            var logger = new Mock<ILogger>();
            logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(logger.Object);
            var service = new LostFilmRegistrationService(logger.Object);
            var uid = service.GetUserIds("c6052eae0ab12fad29329c1fce2a6964.1874597").Result;
        }
    }
}
