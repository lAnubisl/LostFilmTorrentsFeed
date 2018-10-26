using LostFilmMonitoring.BLL.Interfaces;
using LostFilmMonitoring.DAO.DAO;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace LostFilmMonitoring.Console
{
    public class ConfigurationService : IConfigurationService
    {
        private IDictionary<string, string> _settings;

        public string BaseFeedCookie()
        {
            if (_settings == null)
            {
                _settings = new SettingsDAO(GetConnectionString()).GetSettings().ToDictionary(s => s.Name, s => s.Value);
            }

            return _settings["BaseFeedCookie"];
        }

        public string GetBasePath()
        {
            return Program.Configuration.GetSection("AppSettings")["basePath"];
        }

        public string GetConnectionString()
        {
            return Program.Configuration.GetConnectionString("default");
        }

        public string GetImagesDirectory()
        {
            return Program.Configuration.GetSection("AppSettings")["ImagesPath"];
        }
    }
}