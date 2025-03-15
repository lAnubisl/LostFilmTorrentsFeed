using System.Collections.Generic;
using Pulumi;
using Azure = Pulumi.AzureNative;

public class LostFilmMonitoringStack : Pulumi.Stack
{
    private readonly string project = "lfmon";
    private readonly string environment = Pulumi.Deployment.Instance.StackName.ToLowerInvariant();
    private readonly Pulumi.Config config = new Pulumi.Config();

    // Storage Blob Data Contributor: https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#storage:~:text=ba92f5b4%2D2d11%2D453d%2Da403%2De96b0029c9fe

    public LostFilmMonitoringStack()
    {
        Pulumi.Output<Azure.Authorization.GetClientConfigResult> azureConfig = Azure.Authorization.GetClientConfig.Invoke();
        Azure.Resources.ResourceGroup rg = CreateResourceGroup();
        Azure.OperationalInsights.Workspace log = CreateLogAnalyticsWorkspace(rg);
        Azure.Insights.Component appi = CreateApplicationInsights(rg, log);
        Azure.Storage.StorageAccount func_st = CreateFunctionStorageAccount(rg);
        Azure.Web.AppServicePlan plan = CreatePlan(rg);
        Azure.Storage.StorageAccount metadata_st = CreateMetadataStorageAccount(rg);
        Azure.Web.WebApp function = CreateAzureFunction(rg, func_st, plan, appi, metadata_st);
        Azure.Storage.StorageAccount web_st = CreateWebsiteStorageAccount(rg);
        SetPermissions(function, metadata_st);

        // Export the Azure Function name
        FunctionName = function.Name;
    }

    private void SetPermissions(Azure.Web.WebApp function, Azure.Storage.StorageAccount metadata_st)
    {
        var blobDataContributorRole = new Azure.Authorization.RoleAssignment("func_metadata_blob_data_contributor", new Azure.Authorization.RoleAssignmentArgs
        {
            PrincipalId = function.Identity.Apply(identity => identity!.PrincipalId),
            PrincipalType = Azure.Authorization.PrincipalType.ServicePrincipal,
            RoleDefinitionId = GetRoleDefinitionId(RbacRoles.StorageBlobDataContributor),
            Scope = metadata_st.Id
        });

        var tableDataContributorRole = new Azure.Authorization.RoleAssignment("func_metadata_table_data_contributor", new Azure.Authorization.RoleAssignmentArgs
        {
            PrincipalId = function.Identity.Apply(identity => identity!.PrincipalId),
            PrincipalType = Azure.Authorization.PrincipalType.ServicePrincipal,
            RoleDefinitionId = GetRoleDefinitionId(RbacRoles.StorageTableDataContributor),
            Scope = metadata_st.Id
        });
    }

    private Pulumi.Output<string>? SubscriptionId;
    private Pulumi.Input<string> GetRoleDefinitionId(string roleId)
    {
        if (SubscriptionId == null)
        {
            SubscriptionId = Azure.Authorization.GetClientConfig.Invoke().Apply(config => config.SubscriptionId);
        }

        return Pulumi.Output.Format($"/subscriptions/{SubscriptionId}/providers/Microsoft.Authorization/roleDefinitions/{roleId}");
    }

    private Azure.Insights.Component CreateApplicationInsights(Azure.Resources.ResourceGroup rg, Azure.OperationalInsights.Workspace log)
    {
        return new Azure.Insights.Component("appi", new()
        {
            ApplicationType = Azure.Insights.ApplicationType.Other,
            FlowType = Azure.Insights.FlowType.Bluefield,
            Kind = "other",
            Location = rg.Location,
            RequestSource = Azure.Insights.RequestSource.Rest,
            ResourceGroupName = rg.Name,
            ResourceName = $"appi-{project}-{environment}",
            WorkspaceResourceId = log.Id,
            RetentionInDays = 30,
            SamplingPercentage = 100
        });
    }

    private Azure.OperationalInsights.Workspace CreateLogAnalyticsWorkspace(Azure.Resources.ResourceGroup rg)
    {
        return new Azure.OperationalInsights.Workspace("log", new()
        {
            WorkspaceName = $"log-{project}-{environment}",
            Location = rg.Location,
            ResourceGroupName = rg.Name,
            RetentionInDays = 30,
            Sku = new Azure.OperationalInsights.Inputs.WorkspaceSkuArgs
            {
                Name = Azure.OperationalInsights.WorkspaceSkuNameEnum.PerGB2018,
            }
        });
    }

    private Azure.Resources.ResourceGroup CreateResourceGroup()
    {
        return new Azure.Resources.ResourceGroup("rg", new Azure.Resources.ResourceGroupArgs
        {
            ResourceGroupName = $"rg-{project}-{environment}",
            Location = config.Require("location")
        });
    }

    private Azure.Storage.StorageAccount CreateMetadataStorageAccount(Azure.Resources.ResourceGroup rg)
    {
        var storageAccount = new Azure.Storage.StorageAccount("sametadata", new Azure.Storage.StorageAccountArgs
        {
            ResourceGroupName = rg.Name,
            AccountName = $"stmeta{project}{environment}",
            Sku = new Azure.Storage.Inputs.SkuArgs
            {
                Name = Azure.Storage.SkuName.Standard_LRS,
            },
            Kind = Azure.Storage.Kind.StorageV2,
            AllowBlobPublicAccess = true,
        });

        var usertorrents = new Azure.Storage.BlobContainer("usertorrents", new Azure.Storage.BlobContainerArgs
        {
            ResourceGroupName = rg.Name,
            AccountName = storageAccount.Name,
            ContainerName = Constants.MetadataStorageContainerUserTorrents,
            PublicAccess = Azure.Storage.PublicAccess.Blob,
        });

        var rssfeeds = new Azure.Storage.BlobContainer("rssfeeds", new Azure.Storage.BlobContainerArgs
        {
            ResourceGroupName = rg.Name,
            AccountName = storageAccount.Name,
            ContainerName = Constants.MetadataStorageContainerRssFeeds,
            PublicAccess = Azure.Storage.PublicAccess.Blob,
        });

        var models = new Azure.Storage.BlobContainer("models", new Azure.Storage.BlobContainerArgs
        {
            ResourceGroupName = rg.Name,
            AccountName = storageAccount.Name,
            ContainerName = Constants.MetadataStorageContainerModels,
            PublicAccess = Azure.Storage.PublicAccess.Blob,
        });

        var images = new Azure.Storage.BlobContainer("images", new Azure.Storage.BlobContainerArgs
        {
            ResourceGroupName = rg.Name,
            AccountName = storageAccount.Name,
            ContainerName = Constants.MetadataStorageContainerImages,
            PublicAccess = Azure.Storage.PublicAccess.Blob,
        }); 

        var baseTorrents = new Azure.Storage.BlobContainer("basetorrents", new Azure.Storage.BlobContainerArgs
        {
            ResourceGroupName = rg.Name,
            AccountName = storageAccount.Name,
            ContainerName = Constants.MetadataStorageContainerBaseTorrents,
            PublicAccess = Azure.Storage.PublicAccess.None,
        });

        var dictionary = new Azure.Storage.Table("dictionary", new Azure.Storage.TableArgs
        {
            ResourceGroupName = rg.Name,
            AccountName = storageAccount.Name,
            TableName = Constants.MetadataStorageTableNameDictionary,
        });

        var episodes = new Azure.Storage.Table("episodes", new Azure.Storage.TableArgs
        {
            ResourceGroupName = rg.Name,
            AccountName = storageAccount.Name,
            TableName = Constants.MetadataStorageTableNameEpisodes,
        });

        var series = new Azure.Storage.Table("series", new Azure.Storage.TableArgs
        {
            ResourceGroupName = rg.Name,
            AccountName = storageAccount.Name,
            TableName = Constants.MetadataStorageTableNameSeries,
        });

        var subscriptions = new Azure.Storage.Table("subscriptions", new Azure.Storage.TableArgs
        {
            ResourceGroupName = rg.Name,
            AccountName = storageAccount.Name,
            TableName = Constants.MetadataStorageTableNameSubscriptions,
        });

        var users = new Azure.Storage.Table("users", new Azure.Storage.TableArgs
        {
            ResourceGroupName = rg.Name,
            AccountName = storageAccount.Name,
            TableName = Constants.MetadataStorageTableNameUsers,
        });
        
        return storageAccount;
    }

    private Azure.Storage.StorageAccount CreateWebsiteStorageAccount(Azure.Resources.ResourceGroup rg)
    {
        var storageAccount = new Azure.Storage.StorageAccount("saweb", new Azure.Storage.StorageAccountArgs
        {
            ResourceGroupName = rg.Name,
            AccountName = $"stweb{project}{environment}",
            Sku = new Azure.Storage.Inputs.SkuArgs
            {
                Name = Azure.Storage.SkuName.Standard_LRS,
            },
            Kind = Azure.Storage.Kind.StorageV2,
        });

        // Enable static website hosting
        var staticWebsite = new Azure.Storage.StorageAccountStaticWebsite("staticWebsite", new Azure.Storage.StorageAccountStaticWebsiteArgs
        {
            AccountName = storageAccount.Name,
            ResourceGroupName = rg.Name,
            IndexDocument = "index.html",
            Error404Document = "404.html"
        });

        return storageAccount;
    }

    private Azure.Storage.StorageAccount CreateFunctionStorageAccount(Azure.Resources.ResourceGroup rg)
    {
        return new Azure.Storage.StorageAccount("sa", new Azure.Storage.StorageAccountArgs
        {
            ResourceGroupName = rg.Name,
            AccountName = $"stfunc{project}{environment}",
            Sku = new Azure.Storage.Inputs.SkuArgs
            {
                Name = Azure.Storage.SkuName.Standard_LRS,
            },
            Kind = Azure.Storage.Kind.StorageV2,
        });
    }

    private Azure.Web.AppServicePlan CreatePlan(Azure.Resources.ResourceGroup rg)
    {
        return new Azure.Web.AppServicePlan("plan", new Azure.Web.AppServicePlanArgs
        {
            ResourceGroupName = rg.Name,
            Name = $"plan-{project}-{environment}",
            Location = rg.Location,
            Sku = new Azure.Web.Inputs.SkuDescriptionArgs
            {
                Name = "Y1",
                Tier = "Dynamic",
                Size = "Y1",
                Family = "Y",
                Capacity = 0
            },
            Kind = "functionapp",
            Reserved = true // linux
        });
    }

    private Azure.Web.WebApp CreateAzureFunction(
        Azure.Resources.ResourceGroup rg,
        Azure.Storage.StorageAccount st,
        Azure.Web.AppServicePlan plan,
        Azure.Insights.Component appi,
        Azure.Storage.StorageAccount metadata_st)
    {
        return new Azure.Web.WebApp("function", new Azure.Web.WebAppArgs
        {
            ResourceGroupName = rg.Name,
            Kind = "FunctionApp",
            Name = $"func-{project}-{environment}",
            Location = rg.Location,
            ServerFarmId = plan.Id,
            Identity = new Azure.Web.Inputs.ManagedServiceIdentityArgs
            {
                Type = Azure.Web.ManagedServiceIdentityType.SystemAssigned
            },
            SiteConfig = new Azure.Web.Inputs.SiteConfigArgs
            {
                LinuxFxVersion = "DOTNET-ISOLATED|8.0",
                AppSettings = GetAppSettings(new Dictionary<Pulumi.Input<string>, Pulumi.Input<string>>
                {
                    { "APPLICATIONINSIGHTS_CONNECTION_STRING", appi.ConnectionString },
                    { "AzureWebJobsStorage", GetConnectionString(rg.Name, st.Name) },
                    { EnvironmentVariables.MetadataStorageAccountName, metadata_st.Name },
                    { EnvironmentVariables.MetadataStorageAccountKey, GetAccessKey(rg.Name, metadata_st.Name) },
                    { "FUNCTIONS_WORKER_RUNTIME", "dotnet-isolated" },
                    { "FUNCTIONS_EXTENSION_VERSION", "~4" },
                    { "AzureWebJobsFeatureFlags", "EnableWorkerIndexing" },
                    { "WEBSITE_ENABLE_SYNC_UPDATE_SITE", "true" },
                    { "SCM_DO_BUILD_DURING_DEPLOYMENT", "false" },
                }),
            }
        });
    }

    private static Pulumi.InputList<Azure.Web.Inputs.NameValuePairArgs> GetAppSettings(Dictionary<Pulumi.Input<string>, Pulumi.Input<string>> settings)
    {
        var result = new Pulumi.InputList<Azure.Web.Inputs.NameValuePairArgs>();
        foreach (var setting in settings)
        {
            result.Add(new Azure.Web.Inputs.NameValuePairArgs
            {
                Name = setting.Key,
                Value = setting.Value
            });
        }

        return result;
    }

    private static Pulumi.Output<string> GetAccessKey(Pulumi.Input<string> resourceGroupName, Pulumi.Input<string> accountName)
    {
        var storageAccountKeys = Azure.Storage.ListStorageAccountKeys.Invoke(new Azure.Storage.ListStorageAccountKeysInvokeArgs
        {
            ResourceGroupName = resourceGroupName,
            AccountName = accountName
        });

        return storageAccountKeys.Apply(keys =>
        {
            var primaryStorageKey = keys.Keys[0].Value;
            return Pulumi.Output.Format($"{primaryStorageKey}");    
        });
    }

    private static Pulumi.Output<string> GetConnectionString(Pulumi.Input<string> resourceGroupName, Pulumi.Input<string> accountName)
    {
        var storageAccountKeys = Azure.Storage.ListStorageAccountKeys.Invoke(new Azure.Storage.ListStorageAccountKeysInvokeArgs
        {
            ResourceGroupName = resourceGroupName,
            AccountName = accountName
        });

        return storageAccountKeys.Apply(keys =>
        {
            var primaryStorageKey = keys.Keys[0].Value;
            return Pulumi.Output.Format($"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={primaryStorageKey};EndpointSuffix=core.windows.net");
        });
    }

    [Output]
    public Output<string> FunctionName { get; set; }
}