using System;
using System.Collections.Generic;

namespace LostFilmMonitoring.DAO.DomainModels
{
    public class User
    {
        public Guid Id { get; set; }
        public string Cookie { get; set; }
        public string Usess { get; set; }
        public string Uid { get; set; }
        public DateTime LastActivity { get; set; }
        public List<Subscription> Subscriptions { get; set; }
    }
}