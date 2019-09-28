using LostFilmMonitoring.DAO.DomainModels;
using System.Threading.Tasks;

namespace LostFilmMonitoring.BLL
{
    public interface ISerialCoverService
    {
        Task EnsureImageDownloaded(FeedItem feedItem);
    }
}