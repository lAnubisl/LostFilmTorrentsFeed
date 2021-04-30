using LostFilmMonitoring.BLL.Implementations;
using LostFilmMonitoring.Common;
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
            var service = new SerialCoverService("C:/Torrents", new Logger(nameof(SerialCoverService)));
            await service.EnsureImageDownloaded("Блудный Сын");
        }
    }
}
