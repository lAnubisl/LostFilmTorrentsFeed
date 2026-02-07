namespace LostFilmMonitoring.AzureFunction;

/// <summary>
/// Program entry class.
/// </summary>
public static class Program
{
    private static readonly Action<HostBuilderContext, IServiceCollection> RegisterDependencyInjection = (_, services) =>
    {
        services
            .AddOpenTelemetry()
            .UseFunctionsWorkerDefaults()
            .WithTracing(b => b.AddSource(ActivitySourceNames.ActivitySources))
            .UseAzureMonitorExporter();
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
        services.AddTransient<ICommand<DAO.Interfaces.DomainModels.Series>>(sp => new DownloadCoverImageCommand(sp.GetService<Common.ILogger>()!, sp.GetService<IFileSystem>()!, sp.GetService<ITmdbClient>()!));
        services.AddTransient(sp =>
            new UpdateFeedsCommand(
                sp.GetService<Common.ILogger>() !,
                sp.GetService<IRssFeed>() !,
                sp.GetService<IDal>() !,
                sp.GetService<IConfiguration>() !,
                sp.GetService<IModelPersister>() !,
                sp.GetService<ILostFilmClient>() !,
                sp.GetService<ICommand<DAO.Interfaces.DomainModels.Series>>()!));
        services.AddTransient(sp =>
            new DownloadCoverImagesCommand(
                sp.GetService<Common.ILogger>() !,
                sp.GetService<ISeriesDao>() !,
                sp.GetService<ICommand<DAO.Interfaces.DomainModels.Series>>() !));
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

    private static string Env(string key) => Environment.GetEnvironmentVariable(key!)!;
}
