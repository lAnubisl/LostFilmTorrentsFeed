using LostFilmMonitoring.DAO.DomainModels;
using System.Linq;

namespace LostFilmMonitoring.DAO.DAO
{
    public class SettingsDAO : BaseDAO
    {
        public SettingsDAO(string connectionString) : base(connectionString)
        {
        }

        public Setting[] GetSettings()
        {
            using (var ctx = OpenContext())
            {
                return ctx.Settings.ToArray();
            }
        }
    }
}