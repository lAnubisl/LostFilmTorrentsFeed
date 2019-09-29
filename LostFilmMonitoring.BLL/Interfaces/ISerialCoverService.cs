using LostFilmMonitoring.DAO.DomainModels;
using System.Threading.Tasks;

namespace LostFilmMonitoring.BLL.Interfaces
{
    public interface ISerialCoverService
    {
        Task EnsureImageDownloaded(string serial);
    }
}