using System.Threading.Tasks;

namespace LostFilmMonitoring.AzureInfrastructure;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        return await Pulumi.Deployment.RunAsync<LostFilmMonitoringStack>();
    }
}
