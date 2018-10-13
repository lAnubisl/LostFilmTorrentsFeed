using System;
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
    }
}
