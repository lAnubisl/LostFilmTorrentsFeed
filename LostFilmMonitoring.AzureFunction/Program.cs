// <copyright file="Program.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2023 Alexander Panfilenok
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
namespace LostFilmMonitoring.AzureFunction;

/// <summary>
/// Program entry class.
/// </summary>
public static class Program
{
    private static readonly Action<HostBuilderContext, IServiceCollection> RegisterDependencyInjection = (hostContext, services) =>
    {
        ConfigureLoggingAndTelemetry(services);
        services.AddLogging();
        services.AddSingleton<Common.ILogger, Logger>();
        services.AddTransient(r => new BlobServiceClient(
            new Uri($"https://{Env(EnvironmentVariables.MetadataStorageAccountName)}.blob.core.windows.net/"),
            new DefaultAzureCredential()));
        services.AddTransient(r => new TableServiceClient(
            new Uri($"https://{Env(EnvironmentVariables.MetadataStorageAccountName)}.table.core.windows.net/"),
            new TableSharedKeyCredential(
                Env(EnvironmentVariables.MetadataStorageAccountName),
                Env(EnvironmentVariables.MetadataStorageAccountKey))));
        services.AddTransient<IAzureBlobStorageClient, AzureBlobStorageClient>();
        services.AddTransient<IModelPersister, AzureBlobStorageModelPersister>();
        services.AddTransient<IDal, Dal>();
        services.AddTransient<ITorrentFileDao, AzureBlobStorageTorrentFileDao>();
        services.AddTransient<IFeedDao, AzureBlobStorageFeedDao>();
        services.AddTransient<IUserDao, AzureTableStorageUserDao>();
        services.AddTransient<ISeriesDao, AzureTableStorageSeriesDao>();
        services.AddTransient<IEpisodeDao, AzureTableStorageEpisodeDao>();
        services.AddTransient<ISubscriptionDao, AzureTableStorageSubscriptionDao>();
        services.AddTransient<IRssFeed, ReteOrgRssFeed>();
        services.AddTransient<IFileSystem, AzureBlobStorageFileSystem>();
        services.AddTransient<IConfigurationValuesProvider, EnvironmentConfigurationValuesProvider>();
        services.AddTransient<IConfiguration, Configuration>();
        services.AddTransient<IValidator<EditUserRequestModel>, EditUserRequestModelValidator>();
        services.AddTransient<IValidator<EditSubscriptionRequestModel>, EditSubscriptionRequestModelValidator>();
        services.AddTransient<ILostFilmClient, LostFilmClient>();
        services.AddTransient<ITmdbClient>(r => new TmdbClient(Env(EnvironmentVariables.TmdbApiKey), r.GetService<Common.ILogger>()!));
        services.AddHttpClient();
        services.AddTransient(sp =>
            new UpdateFeedsCommand(
                sp.GetService<Common.ILogger>() !,
                sp.GetService<IRssFeed>() !,
                sp.GetService<IDal>() !,
                sp.GetService<IConfiguration>() !,
                sp.GetService<IModelPersister>() !,
                sp.GetService<ILostFilmClient>() !,
                sp.GetService<ITmdbClient>() !,
                sp.GetService<IFileSystem>() !));
        services.AddTransient(sp =>
            new DownloadCoverImagesCommand(
                sp.GetService<Common.ILogger>() !,
                sp.GetService<IFileSystem>() !,
                sp.GetService<ISeriesDao>() !,
                sp.GetService<ITmdbClient>() !));
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
        IHostBuilder builder = new HostBuilder();
        builder = builder.ConfigureFunctionsWorkerDefaults(worker => worker.UseNewtonsoftJson());
        builder = builder.ConfigureOpenApi();
        builder = builder.ConfigureServices(RegisterDependencyInjection);
        IHost host = builder.Build();
        host.Run();
    }

    private static void ConfigureLoggingAndTelemetry(IServiceCollection services)
    {
        var resourceAttributes = new Dictionary<string, object>
        {
            { "service.instance.id", Environment.MachineName },
            { "service.version", "1.0.0" },
        };

        var resourceBuilder = ResourceBuilder
            .CreateDefault()
            .AddAttributes(resourceAttributes);

        services.AddSingleton<TracerProvider>(r =>
            Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(resourceBuilder)
                .SetSampler(new AlwaysOnSampler())
                .AddSource(ActivitySourceNames.ActivitySources)
                .AddAzureMonitorTraceExporter()
                .Build());

        services.AddSingleton<MeterProvider>(r =>
            Sdk.CreateMeterProviderBuilder()
                .SetResourceBuilder(resourceBuilder)
                .AddAzureMonitorMetricExporter()
                .Build());

        services.AddOpenTelemetry().UseFunctionsWorkerDefaults();
        services.AddSingleton<ILoggerFactory>(r =>
            LoggerFactory.Create(builder =>
                builder.AddOpenTelemetry(logging =>
                    {
                        logging.SetResourceBuilder(resourceBuilder);
                        logging.AddAzureMonitorLogExporter();
                    })));
    }

    private static string Env(string key) => Environment.GetEnvironmentVariable(key!)!;
}
