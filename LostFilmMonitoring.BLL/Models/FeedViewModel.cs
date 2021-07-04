// <copyright file="FeedViewModel.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2021 Alexander Panfilenok
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the 'Software'), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>

namespace LostFilmMonitoring.BLL.Models
{
    using System;
    using System.Collections.Generic;
    using LostFilmMonitoring.DAO.DomainModels;

    /// <summary>
    /// Represents data to be shown on 'My RSS Feed page'.
    /// </summary>
    public class FeedViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeedViewModel"/> class.
        /// </summary>
        /// <param name="feed">Feed items.</param>
        /// <param name="user">User.</param>
        public FeedViewModel(SortedSet<FeedItem> feed, User user)
        {
            this.Feed = feed;
            this.UserId = user.Id;
            this.If_session = user.Cookie;
            this.Uid = user.Uid;
            this.Usess = user.Usess;
            this.TrackerId = user.TrackerId;
        }

        /// <summary>
        /// Gets feed items.
        /// </summary>
        public SortedSet<FeedItem> Feed { get; }

        /// <summary>
        /// Gets user id.
        /// </summary>
        public Guid UserId { get; }

        /// <summary>
        /// Gets If_session.
        /// </summary>
        public string If_session { get; }

        /// <summary>
        /// Gets Uid.
        /// </summary>
        public string Uid { get; }

        /// <summary>
        /// Gets Usess.
        /// </summary>
        public string Usess { get; }

        /// <summary>
        /// Gets TrackerId.
        /// </summary>
        public string TrackerId { get; }
    }
}
