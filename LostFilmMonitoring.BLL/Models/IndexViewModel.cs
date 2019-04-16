using LostFilmMonitoring.DAO.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LostFilmMonitoring.BLL.Models
{
    public class IndexModel
    {
        private readonly SelectedFeedItem[] selectedItems;

        public IndexModel(IList<Serial> serials, User user)
        {
            if (user != null)
            {
                selectedItems = user.Subscriptions?.Select(s => new SelectedFeedItem()
                {
                    Serial = s.Serial,
                    Quality = s.Quality
                }).ToArray();
            }

            KnownUser = user != null;
            Last24HoursItems = Filter(serials, s => s.LastEpisode >= DateTime.Now.AddHours(-24));
            Last7DaysItems = Filter(serials, s => s.LastEpisode < DateTime.Now.AddHours(-24) && s.LastEpisode >= DateTime.Now.AddDays(-7));
            OlderItems = Filter(serials, s => s.LastEpisode < DateTime.Now.AddDays(-7));
        }

        private SerialViewModel[] Filter(IList<Serial> serials, Func<Serial, bool> predicate)
        {
            return serials
                .Where(predicate)
                .OrderByDescending(s => s.LastEpisode)
                .Select(s => new SerialViewModel(s, selectedItems))
                .ToArray();
        }

        public bool KnownUser { get; }

        public SerialViewModel[] Last24HoursItems { get; }

        public SerialViewModel[] Last7DaysItems { get; }

        public SerialViewModel[] OlderItems { get; }
    }
}