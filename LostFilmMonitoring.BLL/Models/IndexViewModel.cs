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
            AllSerials = serials.Select(s => s.Name).ToArray();
            SelectedItems = new SelectedFeedItem[1] { new SelectedFeedItem { Serial = "Миллиарды (Billions).", Quality = "1080" } };
            if(user != null)
            {
                UserId = user.Id;
                Cookie = string.Empty; // do not allow to see it
                SelectedItems = user.Subscriptions.Select(s => new SelectedFeedItem()
                {
                    Serial = s.Serial,
                    Quality = s.Quality
                }).ToArray();
            }
        }

        public string[] AllSerials { get; }

        public SelectedFeedItem[] SelectedItems { get; set; }

        public Guid UserId { get; set; }

        public string Cookie { get; set; }
    }

    public class SelectedFeedItem
    {
        public string Serial { get; set; }

        public string Quality { get; set; }
    }
}