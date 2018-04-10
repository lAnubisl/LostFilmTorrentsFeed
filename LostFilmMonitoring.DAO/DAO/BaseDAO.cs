namespace LostFilmMonitoring.DAO.DAO
{
    public class BaseDAO
    {
        private readonly string _connectionString;

        public BaseDAO(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected LostFilmDbContext OpenContext() => new LostFilmDbContext(_connectionString);
    }
}