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
    using System;
    using Azure.Data.Tables;
    using Azure.Storage.Blobs;
    using LostFilmMonitoring.BLL;
    using LostFilmMonitoring.BLL.Commands;
    using LostFilmMonitoring.BLL.Interfaces;
    using LostFilmMonitoring.BLL.Models.Request;
    using LostFilmMonitoring.BLL.Models.Response;
    using LostFilmMonitoring.BLL.Validators;
    using LostFilmMonitoring.Common;
    using LostFilmMonitoring.DAO.Azure;
    using LostFilmMonitoring.DAO.Interfaces;
    using LostFilmTV.Client;
    using LostFilmTV.Client.RssFeed;
    using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Program entry class.
    /// </summary>
    public class Program
    {
        private static readonly Action<HostBuilderContext, IServiceCollection> RegisterDependencyInjection = (hostContext, services) =>
        {
            var storageAccountConnectionString = Environment.GetEnvironmentVariable("StorageAccountConnectionString") ?? throw new ArgumentException("Environment variable 'StorageAccountConnectionString' not set.");

            services.AddLogging();
            services.AddSingleton<ILogger, BLL.Logger>();
            services.AddSingleton<BlobServiceClient>(r => new BlobServiceClient(storageAccountConnectionString));
            services.AddSingleton<TableServiceClient>(r => new TableServiceClient(storageAccountConnectionString));
            services.AddSingleton<AzureBlobStorageClient>();
            services.AddSingleton<IModelPersister, AzureBlobStorageModelPersister>();
            services.AddSingleton<IDAL, DAL>();
            services.AddSingleton<ITorrentFileDAO, AzureBlobStorageTorrentFileDAO>();
            services.AddSingleton<IFeedDAO, AzureBlobStorageFeedDAO>();
            services.AddSingleton<IUserDAO, AzureTableStorageUserDAO>();
            services.AddSingleton<ISeriesDAO, AzureTableStorageSeriesDao>();
            services.AddSingleton<IEpisodeDAO, AzureTableStorageEpisodeDao>();
            services.AddSingleton<ISubscriptionDAO, AzureTableStorageSubscriptionDAO>();
            services.AddSingleton<IRssFeed, ReteOrgRssFeed>();
            services.AddSingleton<IFileSystem, AzureBlobStorageFileSystem>();
            services.AddSingleton<IConfiguration, Configuration>();
            services.AddSingleton<IValidator<EditUserRequestModel>, EditUserRequestModelValidator>();
            services.AddSingleton<IValidator<EditSubscriptionRequestModel>, EditSubscriptionRequestModelValidator>();
            services.AddSingleton<ILostFilmClient, LostFilmClient>();
            services.AddHttpClient();
            services.AddTransient<UpdateFeedsCommand>();
            services.AddTransient<DownloadCoverImagesCommand>();
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
