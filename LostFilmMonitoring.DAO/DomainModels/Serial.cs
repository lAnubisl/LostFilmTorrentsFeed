using System;

namespace LostFilmMonitoring.DAO.DomainModels
{
    public class Serial
    {
        public string Name { get; set; }
        public DateTime LastEpisode { get; set; }
        public string LastEpisodeName { get; set; }
        public string LastEpisodeTorrentLinkSD { get; set; }
        public string LastEpisodeTorrentLinkMP4 { get; set; }
        public string LastEpisodeTorrentLink1080 { get; set; }
    }
}