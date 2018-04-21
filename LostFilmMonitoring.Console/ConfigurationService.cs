using LostFilmMonitoring.BLL.Interfaces;

namespace LostFilmMonitoring.Console
{
    public class ConfigurationService : IConfigurationService
    {
        public string GetBasePath()
        {
            return Program.Configuration["basePath"];
        }

        public string GetConnectionString()
        {
            return Program.Configuration["ConnectionStrings"];
        }

        public string GetImagesDirectory()
        {
            return Program.Configuration["ImagesPath"];
        }
    }
}