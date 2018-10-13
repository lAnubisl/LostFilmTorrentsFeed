using LostFilmMonitoring.DAO.DomainModels;
using System.Collections.Generic;
using System.Linq;

namespace LostFilmMonitoring.BLL.Models
{
    public class IndexModel
    {
        public IndexModel() { }

        public IndexModel(IList<Serial> serials, User user)
        {
            AllSerials = serials
                .OrderByDescending(s => s.LastEpisode)
                .Select(s => new SerialViewModel { Name = s.Name, Escaped = s.Name.EscapePath() })
                .ToArray();
            if(user != null)
            {
                SelectedItems = user.Subscriptions?.Select(s => new SelectedFeedItem()
                {
                    Serial = s.Serial,
                    Quality = s.Quality
                }).ToArray();
            }
        }

        public SerialViewModel[] AllSerials { get; }

        public SelectedFeedItem[] SelectedItems { get; set; }
    }
}