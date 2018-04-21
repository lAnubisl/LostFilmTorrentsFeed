using System;

namespace LostFilmMonitoring.DAO.DomainModels
{
    public class Serial
    {
        public string Name { get; set; }
        public DateTime LastEpisode { get; set; }
        public string LastEpisodeLink { get; set; }
    }
}