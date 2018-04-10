using LostFilmMonitoring.BLL;
using LostFilmMonitoring.BLL.Implementations;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace LostFilmMonitoring.Console
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; private set; }
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();

            var configurationService = new ConfigurationService();
            var userService = new PresentationService(configurationService);
            userService.RemoveOldUsers().Wait();
            var updater = new FeedService(configurationService);
            updater.Update().Wait();
            System.Console.WriteLine("Hello World!");
        }
    }
}
