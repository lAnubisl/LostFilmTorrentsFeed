using System.Collections.Generic;
using System.Threading.Tasks;
using LostFilmMonitoring.DAO.DomainModels;

namespace LostFilmMonitoring.DAO.Interfaces
{
    public interface ISerialDAO
    {
        Task<List<Series>> LoadAsync();
        Task<Series> LoadAsync(string name);
        Task SaveAsync(Series serial);
    }
}