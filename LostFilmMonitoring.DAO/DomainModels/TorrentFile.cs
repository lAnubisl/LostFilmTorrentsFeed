using System.IO;

namespace LostFilmMonitoring.DAO.DomainModels
{
    public class TorrentFile
    {
        public string FileName { get; set; }
        public Stream Stream { get; set; }
    }
}