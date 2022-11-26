﻿// <copyright file="DAL.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.Interfaces
{
    /// <summary>
    /// Responsible for providing access to all aspects of Data-Access-Layer.
    /// </summary>
    public class Dal : IDal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Dal"/> class.
        /// </summary>
        /// <param name="feedDAO">IFeedDAO.</param>
        /// <param name="seriesDAO">ISeriesDAO.</param>
        /// <param name="subscriptionDAO">ISubscriptionDAO.</param>
        /// <param name="torrentFileDAO">ITorrentFileDAO.</param>
        /// <param name="userDAO">IUserDAO.</param>
        /// <param name="episodeDAO">IEpisodeDAO.</param>
        public Dal(IFeedDao feedDAO, ISeriesDao seriesDAO, ISubscriptionDao subscriptionDAO, ITorrentFileDao torrentFileDAO, IUserDao userDAO, IEpisodeDao episodeDAO)
        {
            this.Feed = feedDAO ?? throw new ArgumentNullException(nameof(feedDAO));
            this.Subscription = subscriptionDAO ?? throw new ArgumentNullException(nameof(subscriptionDAO));
            this.TorrentFile = torrentFileDAO ?? throw new ArgumentNullException(nameof(torrentFileDAO));
            this.Series = seriesDAO ?? throw new ArgumentNullException(nameof(seriesDAO));
            this.User = userDAO ?? throw new ArgumentNullException(nameof(userDAO));
            this.Episode = episodeDAO ?? throw new ArgumentNullException(nameof(episodeDAO));
        }

        /// <inheritdoc/>
        public IFeedDao Feed { get; }

        /// <inheritdoc/>
        public ISeriesDao Series { get; }

        /// <inheritdoc/>
        public ISubscriptionDao Subscription { get; }

        /// <inheritdoc/>
        public ITorrentFileDao TorrentFile { get; }

        /// <inheritdoc/>
        public IUserDao User { get; }

        /// <inheritdoc/>
        public IEpisodeDao Episode { get; }
    }
}
