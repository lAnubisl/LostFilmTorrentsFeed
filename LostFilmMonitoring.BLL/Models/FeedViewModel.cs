using LostFilmMonitoring.DAO.DomainModels;
using System;
using System.Collections.Generic;

namespace LostFilmMonitoring.BLL.Models
{
    public class FeedViewModel
    {
        public FeedViewModel(SortedSet<FeedItem> feed, Guid userId)
        {
            Feed = feed;
            UserId = userId;
        }
        public SortedSet<FeedItem> Feed { get; }
        public Guid UserId { get; }
    }
}