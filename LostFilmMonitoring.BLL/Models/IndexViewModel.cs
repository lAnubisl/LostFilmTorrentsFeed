// <copyright file="IndexViewModel.cs" company="Alexander Panfilenok">
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
    using System.Linq;
    using LostFilmMonitoring.DAO.DomainModels;

    /// <summary>
    /// Represents information to be shown in home screen.
    /// </summary>
    public class IndexViewModel
    {
        private readonly SelectedFeedItem[] selectedItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexViewModel"/> class.
        /// </summary>
        /// <param name="series">Series.</param>
        /// <param name="user">Current user.</param>
        public IndexViewModel(IList<Series> series, User user)
        {
            if (user != null)
            {
                this.selectedItems = user.Subscriptions?.Select(s => new SelectedFeedItem()
                {
                    SeriesName = s.SeriesName,
                    Quality = s.Quality,
                }).ToArray();
            }

            this.KnownUser = user != null;
            this.Last24HoursItems = this.Filter(series, s => s.LastEpisode >= DateTime.Now.AddHours(-24));
            this.Last7DaysItems = this.Filter(series, s => s.LastEpisode < DateTime.Now.AddHours(-24) && s.LastEpisode >= DateTime.Now.AddDays(-7));
            this.OlderItems = this.Filter(series, s => s.LastEpisode < DateTime.Now.AddDays(-7));
        }

        /// <summary>
        /// Gets a value indicating whether user is logged in.
        /// </summary>
        public bool KnownUser { get; }

        /// <summary>
        /// Gets episodes updated within last 24 hours.
        /// </summary>
        public SeriesViewModel[] Last24HoursItems { get; }

        /// <summary>
        /// Gets episodes updated within last 7 days.
        /// </summary>
        public SeriesViewModel[] Last7DaysItems { get; }

        /// <summary>
        /// Gets episodes older than 7 days..
        /// </summary>
        public SeriesViewModel[] OlderItems { get; }

        private SeriesViewModel[] Filter(IList<Series> series, Func<Series, bool> predicate)
        {
            return series
                .Where(predicate)
                .OrderByDescending(s => s.LastEpisode)
                .Select(s => new SeriesViewModel(s, this.selectedItems))
                .ToArray();
        }
    }
}
