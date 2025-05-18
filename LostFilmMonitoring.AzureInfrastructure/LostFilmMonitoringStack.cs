using System.Collections.Generic;
using LostFilmMonitoring.Common;
using Pulumi;
using Azure = Pulumi.AzureNative;
using Cloudflare = Pulumi.Cloudflare;

namespace LostFilmMonitoring.AzureInfrastructure;

public class LostFilmMonitoringStack : Pulumi.Stack
{
    private readonly Pulumi.Config config = new Pulumi.Config();
    private readonly Output<string> zoneId = Cloudflare.GetZone.Invoke(new Cloudflare.GetZoneInvokeArgs { Filter = new Cloudflare.Inputs.GetZoneFilterInputArgs { Name = "byalex.dev" } }).Apply(zone => zone.Id);

    // Storage Blob Data Contributor: https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#storage:~:text=ba92f5b4%2D2d11%2D453d%2Da403%2De96b0029c9fe

    public LostFilmMonitoringStack()
    {
        Pulumi.Output<Azure.Authorization.GetClientConfigResult> azureConfig = Azure.Authorization.GetClientConfig.Invoke();
        Azure.Resources.ResourceGroup rg = CreateResourceGroup();
        Azure.OperationalInsights.Workspace log = CreateLogAnalyticsWorkspace(rg);
        Azure.ApplicationInsights.Component appi = CreateApplicationInsights(rg, log);
        Azure.Web.AppServicePlan plan = CreatePlan(rg);
        Cloudflare.DnsRecord data_record = CreateDataRecord();
        Azure.Storage.StorageAccount metadata_st = CreateMetadataStorageAccount(rg, data_record);
        Azure.Storage.StorageAccount func_st = CreateFunctionStorageAccount(rg);
        Azure.Web.WebApp function = CreateAzureFunction(rg, func_st, plan, appi, metadata_st);
        Cloudflare.DnsRecord web_record = CreateWebRecord();
        Azure.Storage.StorageAccount web_st = CreateWebsiteStorageAccount(rg, web_record);
        Cloudflare.DnsRecord api_record = CreateApiRecord(function);
        Azure.Web.WebAppHostNameBinding api_custom_domain_binding = CreateApiCustomDomainBinding(rg, function, api_record);
        SetPermissions(function, metadata_st);
        // Export the Azure Function name and CDN endpoints
        FunctionName = function.Name;
        WebsiteStorageAccountName = web_st.Name;
        ApiDomain = api_record.Name;
        DataDomain = data_record.Name;
    }

    private Azure.Web.WebAppHostNameBinding CreateApiCustomDomainBinding( Azure.Resources.ResourceGroup rg, Azure.Web.WebApp function, Cloudflare.DnsRecord api_record)
    {
        var domainBinding = new Azure.Web.WebAppHostNameBinding("api_custom_domain_binding", new Azure.Web.WebAppHostNameBindingArgs
        {
            ResourceGroupName = rg.Name,
            Name = function.Name,
            SiteName = function.Name,
            HostName = config.Require("apidomain"),
            CustomHostNameDnsRecordType = Azure.Web.CustomHostNameDnsRecordType.CName,        
        }, new CustomResourceOptions 
        { 
            DependsOn = { api_record },
            IgnoreChanges = { "sslState", "thumbprint" }
        });

        return domainBinding;
    }

    private Cloudflare.DnsRecord CreateWebRecord()
    {
        var dataRecord = new Cloudflare.DnsRecord("web_cname_record", new Cloudflare.DnsRecordArgs
        {
            ZoneId = zoneId,
            Name = config.Require("webdomain"),
            Type = "CNAME",
            Content = $"{Locals.WebsiteStorageAccountName}.z6.web.core.windows.net",
            Proxied = true
        });

        var asverifyDataRecord = new Cloudflare.DnsRecord("asverify_web_cname_record", new Cloudflare.DnsRecordArgs
        {
            ZoneId = zoneId,
            Name = $"asverify.{config.Require("webdomain")}",
            Type = "CNAME",
            Content = $"asverify.{Locals.WebsiteStorageAccountName}.z6.web.core.windows.net",
            Proxied = false
        });

        return dataRecord;
    }

    private Cloudflare.DnsRecord CreateDataRecord()
    {
        var dataRecord = new Cloudflare.DnsRecord("data_cname_record", new Cloudflare.DnsRecordArgs
        {
            ZoneId = zoneId,
            Name = config.Require("datadomain"),
            Type = "CNAME",
            Content = $"{Locals.MetadataStorageAccountName}.blob.core.windows.net",
            Proxied = true
        });

        var asverifyDataRecord = new Cloudflare.DnsRecord("asverify_data_cname_record", new Cloudflare.DnsRecordArgs
        {
            ZoneId = zoneId,
            Name = $"asverify.{config.Require("datadomain")}",
            Type = "CNAME",
            Content = $"asverify.{Locals.MetadataStorageAccountName}.blob.core.windows.net",
            Proxied = false
        });

        return dataRecord;
    }

    private Cloudflare.DnsRecord CreateApiRecord(Azure.Web.WebApp function)
    {
        var txt_record = new Cloudflare.DnsRecord("api_txt_record", new Cloudflare.DnsRecordArgs
        {
            ZoneId = zoneId,
            Name = $"asuid.{config.Require("apidomain")}",
            Type = "TXT",
            Content = function.CustomDomainVerificationId.Apply(id => $"\"{id}\"")
        });
        
        return new Cloudflare.DnsRecord("api", new Cloudflare.DnsRecordArgs
        {
            ZoneId = zoneId,
            Name = config.Require("apidomain"),
            Type = "CNAME",
            Content = function.DefaultHostName,
            Proxied = true
        });
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

    private Azure.ApplicationInsights.Component CreateApplicationInsights(Azure.Resources.ResourceGroup rg, Azure.OperationalInsights.Workspace log)
    {
        return new Azure.ApplicationInsights.Component("appi", new()
        {
            ApplicationType = Azure.ApplicationInsights.ApplicationType.Other,
            FlowType = Azure.ApplicationInsights.FlowType.Bluefield,
            Kind = "other",
            Location = rg.Location,
            RequestSource = Azure.ApplicationInsights.RequestSource.Rest,
            ResourceGroupName = rg.Name,
            ResourceName = Locals.ApplicationInsightsName,
            WorkspaceResourceId = log.Id,
            RetentionInDays = 30,
            SamplingPercentage = 100
        });
    }

    private Azure.OperationalInsights.Workspace CreateLogAnalyticsWorkspace(Azure.Resources.ResourceGroup rg)
    {
        return new Azure.OperationalInsights.Workspace("log", new()
        {
            WorkspaceName = Locals.LogAnalyticsWorkspaceName,
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
            ResourceGroupName = Locals.ResourceGroupName,
            Location = config.Require("location")
        });
    }

    private Azure.Storage.StorageAccount CreateMetadataStorageAccount(Azure.Resources.ResourceGroup rg, Cloudflare.DnsRecord data_record)
    {
        var storageAccount = new Azure.Storage.StorageAccount("sametadata", new Azure.Storage.StorageAccountArgs
        {
            ResourceGroupName = rg.Name,
            AccountName = Locals.MetadataStorageAccountName,
            Sku = new Azure.Storage.Inputs.SkuArgs
            {
                Name = Azure.Storage.SkuName.Standard_LRS,
            },
            Kind = Azure.Storage.Kind.StorageV2,
            AllowBlobPublicAccess = true,
            EnableHttpsTrafficOnly = false,
            AllowSharedKeyAccess = true,
            DefaultToOAuthAuthentication = true,
            CustomDomain = new Azure.Storage.Inputs.CustomDomainArgs
            {
                Name = data_record.Name.Apply(name => name),
                UseSubDomainName = true
            } 
        });

        var corsRule = new Azure.Storage.BlobServiceProperties("cors_stmetadata", new Azure.Storage.BlobServicePropertiesArgs
        {
            AccountName = storageAccount.Name,
            BlobServicesName = "default",
            ResourceGroupName = rg.Name,
            Cors = new Azure.Storage.Inputs.CorsRulesArgs
            {
                CorsRules = 
                {
                    new Azure.Storage.Inputs.CorsRuleArgs
                    {
                        AllowedOrigins = MetadataAllowedOrigins(),
                        AllowedMethods = {"GET", "OPTIONS"},
                        AllowedHeaders = {"*"},
                        ExposedHeaders = {"*"},
                        MaxAgeInSeconds = 3600
                    }
                }
            }
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
        
        return storageAccount;
    }

    private Azure.Storage.StorageAccount CreateWebsiteStorageAccount(Azure.Resources.ResourceGroup rg, Cloudflare.DnsRecord web_record)
    {
        var storageAccount = new Azure.Storage.StorageAccount("saweb", new Azure.Storage.StorageAccountArgs
        {
            ResourceGroupName = rg.Name,
            AccountName = Locals.WebsiteStorageAccountName,
            Sku = new Azure.Storage.Inputs.SkuArgs
            {
                Name = Azure.Storage.SkuName.Standard_LRS,
            },
            Kind = Azure.Storage.Kind.StorageV2,
            EnableHttpsTrafficOnly = false,
            AllowSharedKeyAccess = false,
            DefaultToOAuthAuthentication = true,
            CustomDomain = new Azure.Storage.Inputs.CustomDomainArgs
            {
                Name = web_record.Name.Apply(name => name),
                UseSubDomainName = true
            }
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
            AccountName = Locals.FunctionAppStorageAccountName,
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
            Name = Locals.AppServicePlanName,
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
        Azure.ApplicationInsights.Component appi,
        Azure.Storage.StorageAccount metadata_st)
    {
        return new Azure.Web.WebApp("function", new Azure.Web.WebAppArgs
        {
            ResourceGroupName = rg.Name,
            Kind = "FunctionApp",
            Name = Locals.FunctionAppName,
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
                    { "AzureWebJobsDisableHomepage", "true" },
                    { EnvironmentVariables.MetadataStorageAccountName, metadata_st.Name },
                    { EnvironmentVariables.MetadataStorageAccountKey, GetAccessKey(rg.Name, metadata_st.Name) },
                    { EnvironmentVariables.BaseUrl, config.Require("baseurl") },
                    { EnvironmentVariables.BaseFeedCookie, config.RequireSecret("basefeedcookie") },
                    { EnvironmentVariables.BaseLinkUID, config.RequireSecret("baselinkuid") },
                    { EnvironmentVariables.TorrentTrackers, config.Require("torrenttrackers") },
                    { EnvironmentVariables.TmdbApiKey, config.RequireSecret("tmdbapikey") },
                    { "FUNCTIONS_WORKER_RUNTIME", "dotnet-isolated" },
                    { "FUNCTIONS_EXTENSION_VERSION", "~4" },
                    { "AzureWebJobsFeatureFlags", "EnableWorkerIndexing" },
                    { "WEBSITE_ENABLE_SYNC_UPDATE_SITE", "true" },
                    { "SCM_DO_BUILD_DURING_DEPLOYMENT", "false" },
                }),
                Cors = new Azure.Web.Inputs.CorsSettingsArgs
                {
                    AllowedOrigins = AzureFunctionAllowedOrigins(),
                    SupportCredentials = true
                }
            }
        }, new CustomResourceOptions { 
            IgnoreChanges = { 
                "hostNameSslStates",
                "enabledHostNames"
            }
         });
    }

    private string[] AllowedOrigins()
    {
        var result = new List<string>
        {
            $"https://{config.Require("webdomain")}"
        };
        
        if (Locals.Environment == "dev")
        {
            result.Add("https://localhost:11443");
        }

        return [.. result];
    }

    private string[] MetadataAllowedOrigins() => AllowedOrigins();

    private string[] AzureFunctionAllowedOrigins() => AllowedOrigins();

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

    [Output]
    public Output<string> WebsiteStorageAccountName { get; set; }

    [Output]
    public Output<string> ApiDomain { get; set; }

    [Output]
    public Output<string> DataDomain { get; set; }
}