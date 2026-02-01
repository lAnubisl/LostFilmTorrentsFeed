namespace LostFilmMonitoring.AzureInfrastructure;

public class Locals
{
    public static readonly string Project = "lfmon";
    public static readonly string Environment = Pulumi.Deployment.Instance.StackName.ToLowerInvariant();
    public static string MetadataStorageAccountName => $"stmeta{Project}{Environment}";
    public static string AppServicePlanName => $"plan-{Project}-{Environment}";
    public static string FunctionAppName => $"func-{Project}-{Environment}";
    public static string FunctionAppStorageAccountName => $"stfunc{Project}{Environment}";
    public static string WebsiteStorageAccountName => $"stweb{Project}{Environment}";
    public static string LogAnalyticsWorkspaceName => $"log-{Project}-{Environment}";
    public static string ApplicationInsightsName => $"appi-{Project}-{Environment}";
    public static string ResourceGroupName => $"rg-{Project}-{Environment}";
}
