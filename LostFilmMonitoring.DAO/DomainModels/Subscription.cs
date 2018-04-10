using System;

namespace LostFilmMonitoring.DAO.DomainModels
{
    public class Subscription
    {
        public Guid UserId { get; set; }
        public string Serial { get; set; }
        public string Quality { get; set; }

        public User User { get; set; }
    }
}