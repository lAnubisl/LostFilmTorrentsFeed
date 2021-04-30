using Microsoft.EntityFrameworkCore;
namespace LostFilmMonitoring.DAO.DAO
{
    public class BaseDAO
    {
        private readonly string _connectionString;
        private readonly bool isMigrationApplied;

        public BaseDAO(string connectionString)
        {
            _connectionString = connectionString;
            if (!isMigrationApplied)
            {
                var ctx = new LostFilmDbContext(_connectionString);
                ctx.Database.Migrate();
                isMigrationApplied = true;
            }
        }

        protected LostFilmDbContext OpenContext() => new LostFilmDbContext(_connectionString);
    }
}
