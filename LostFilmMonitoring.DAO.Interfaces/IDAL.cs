// <copyright file="IDAL.cs" company="Alexander Panfilenok">
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
    public interface IDal
    {
        /// <summary>
        /// Gets an instance of IFeedDAO.
        /// </summary>
        IFeedDao Feed { get; }

        /// <summary>
        /// Gets an instance of ISeriesDAO.
        /// </summary>
        ISeriesDao Series { get; }

        /// <summary>
        /// Gets an instance of ISubscriptionDAO.
        /// </summary>
        ISubscriptionDao Subscription { get; }

        /// <summary>
        /// Gets an instance of ITorrentFileDAO.
        /// </summary>
        ITorrentFileDao TorrentFile { get; }

        /// <summary>
        /// Gets an instance of IUserDAO.
        /// </summary>
        IUserDao User { get; }

        /// <summary>
        /// Gets an instance of IEpisodeDAO.
        /// </summary>
        IEpisodeDao Episode { get; }
    }
}
