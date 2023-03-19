// <copyright file="Program.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.AzureFunction
{
    /// <summary>
    /// Program entry class.
    /// </summary>
    public static class Program
    {
        private static readonly Action<HostBuilderContext, IServiceCollection> RegisterDependencyInjection = (hostContext, services) =>
        {
            var storageAccountConnectionString = Environment.GetEnvironmentVariable("StorageAccountConnectionString") ?? throw new ArgumentException("Environment variable 'StorageAccountConnectionString' not set.");

            services.AddLogging();
            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton(r => new BlobServiceClient(storageAccountConnectionString));
            services.AddSingleton(r => new TableServiceClient(storageAccountConnectionString));
            services.AddSingleton<IAzureBlobStorageClient, AzureBlobStorageClient>();
            services.AddSingleton<IModelPersister, AzureBlobStorageModelPersister>();
            services.AddSingleton<IDal, Dal>();
            services.AddSingleton<ITorrentFileDao, AzureBlobStorageTorrentFileDao>();
            services.AddSingleton<IFeedDao, AzureBlobStorageFeedDao>();
            services.AddSingleton<IUserDao, AzureTableStorageUserDao>();
            services.AddSingleton<ISeriesDao, AzureTableStorageSeriesDao>();
            services.AddSingleton<IDictionaryDao, AzureTableStorageDictionaryDao>();
            services.AddSingleton<IEpisodeDao, AzureTableStorageEpisodeDao>();
            services.AddSingleton<ISubscriptionDao, AzureTableStorageSubscriptionDao>();
            services.AddSingleton<IRssFeed, ReteOrgRssFeed>();
            services.AddSingleton<IRssFeed, LostFilmRssFeed>();
            services.AddSingleton<IFileSystem, AzureBlobStorageFileSystem>();
            services.AddSingleton<IConfigurationValuesProvider, EnvironmentConfigurationValuesProvider>();
            services.AddSingleton<IConfiguration, Configuration>();
            services.AddSingleton<IValidator<EditUserRequestModel>, EditUserRequestModelValidator>();
            services.AddSingleton<IValidator<EditSubscriptionRequestModel>, EditSubscriptionRequestModelValidator>();
            services.AddSingleton<ILostFilmClient, LostFilmClient>();
            services.AddHttpClient();
            services.AddTransient(sp =>
                new UpdateFeedsCommand(
                    sp.GetService<ILogger>() !,
                    sp.GetServices<IRssFeed>().First(x => x.GetType().Name.Equals(nameof(ReteOrgRssFeed))) !,
                    sp.GetService<IDal>() !,
                    sp.GetService<IConfiguration>() !,
                    sp.GetService<IModelPersister>() !,
                    sp.GetService<ILostFilmClient>() !));
            services.AddTransient(sp =>
                new DownloadCoverImagesCommand(
                    sp.GetService<ILogger>() !,
                    sp.GetService<IFileSystem>() !,
                    sp.GetService<IConfiguration>() !,
                    sp.GetService<ISeriesDao>() !,
                    sp.GetService<ILostFilmClient>() !,
                    sp.GetService<IDictionaryDao>() !));
            services.AddTransient<ICommand<EditUserRequestModel, EditUserResponseModel>, SaveUserCommand>();
            services.AddTransient<ICommand<EditSubscriptionRequestModel, EditSubscriptionResponseModel>, SaveSubscriptionCommand>();
            services.AddTransient<ICommand<SignInRequestModel, SignInResponseModel>, SignInCommand>();
            services.AddTransient<ICommand<GetUserRequestModel, GetUserResponseModel>, GetUserCommand>();
        };

        /// <summary>
        /// Program entry point.
        /// </summary>
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults(worker => worker.UseNewtonsoftJson())
                .ConfigureOpenApi()
                .ConfigureServices(RegisterDependencyInjection)
                .Build();

            host.Run();
        }
    }
}
