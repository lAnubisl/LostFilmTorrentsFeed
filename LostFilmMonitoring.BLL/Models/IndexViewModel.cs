using LostFilmMonitoring.DAO.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LostFilmMonitoring.BLL.Models
{
    public class RegistrationViewModel
    {
        public RegistrationViewModel() { }

        public RegistrationViewModel(IList<Serial> serials, User user)
        {
            AllSerials = serials
                .OrderByDescending(s => s.LastEpisode)
                .Select(s => new SerialViewModel { Name = s.Name, Escaped = s.Name.EscapePath() })
                .ToArray();
            if(user != null)
            {
                UserId = user.Id;
                Cookie = user.Cookie;
                SelectedItems = user.Subscriptions?.Select(s => new SelectedFeedItem()
                {
                    Serial = s.Serial,
                    Quality = s.Quality
                }).ToArray();
            }
        }

        public SerialViewModel[] AllSerials { get; }

        public SelectedFeedItem[] SelectedItems { get; set; }

        public Guid UserId { get; set; }

        public string Cookie { get; set; }
    }

    public class SerialViewModel
    {
        public string Name { get; set; }
        public string Escaped { get; set; }
    }

    public class SelectedFeedItem
    {
        public string Serial { get; set; }

        public string Quality { get; set; }
    }
}