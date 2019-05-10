using LostFilmMonitoring.DAO.DomainModels;
using System.IO;

namespace LostFilmMonitoring.BLL.Interfaces.Models
{
    public class RssItemViewModel
    {
        public RssItemViewModel(TorrentFile torrentFile)
        {
            Body = torrentFile.Stream;
            FileName = torrentFile.FileName;
            ContentType = "application/x-bittorrent";
        }

        public Stream Body { get; }
        public string FileName { get; }
        public string ContentType { get; }
    }
}