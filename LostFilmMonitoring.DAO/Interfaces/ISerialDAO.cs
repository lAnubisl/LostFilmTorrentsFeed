using System.Collections.Generic;
using System.Threading.Tasks;
using LostFilmMonitoring.DAO.DomainModels;

namespace LostFilmMonitoring.DAO.Interfaces
{
    public interface ISerialDAO
    {
        Task<List<Serial>> LoadAsync();
        Task<Serial> LoadAsync(string name);
        Task SaveAsync(Serial serial);
    }
}