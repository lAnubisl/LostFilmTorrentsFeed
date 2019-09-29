using LostFilmMonitoring.BLL.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LostFilmMonitoring.BLL.Tests
{
    public class SerialCoverServiceTests
    {
        [Fact]
        public async Task SearchCoverTest()
        {
            var service = new SerialCoverService("C:/Torrents");
            await service.EnsureImageDownloaded("Блудный Сын");
        }
    }
}
