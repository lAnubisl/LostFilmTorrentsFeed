using LostFilmMonitoring.BLL.Interfaces;
using Microsoft.Extensions.Configuration;

namespace LostFilmMonitoring.Web
{
    public class ConfigurationService : IConfigurationService
    {
        public string GetBasePath()
        {
            return Startup.Configuration.GetSection("AppSettings")["basePath"];
        }

        public string GetConnectionString()
        {
            return Startup.Configuration.GetConnectionString("default");
        }
    }
}