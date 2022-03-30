using System;
using LostFilmMonitoring.Common;
using LostFilmMonitoring.DAO.Azure;
using LostFilmMonitoring.DAO.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LostFilmMonitoring.Updater.AzureFunction
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(RegisterDependencyInjection)
                .Build();

            host.Run();
        }

        private static readonly Action<HostBuilderContext, IServiceCollection> RegisterDependencyInjection = (hostContext, services) =>
        {
            services.AddSingleton<AzureBlobStorageClient>(r => new AzureBlobStorageClient(Settings.StorageAccountConnectionString, r.GetService<ILogger>()));
            services.AddSingleton<ITorrentFileDAO, AzureBlobStorageTorrentFileDAO>();
            services.AddSingleton<IFeedDAO, AzureBlobStorageFeedDAO>();
            // configure services
        };
    }
}
