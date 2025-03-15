using System.Threading.Tasks;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        return await Pulumi.Deployment.RunAsync<LostFilmMonitoringStack>();
    }
}
