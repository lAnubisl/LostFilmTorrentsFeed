// <copyright file="IFeedDAO.cs" company="Alexander Panfilenok">
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
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LostFilmMonitoring.DAO.Interfaces.DomainModels;

    /// <summary>
    /// Provides functionality for managing user's feeds in storage.
    /// </summary>
    public interface IFeedDAO
    {
        /// <summary>
        /// Loads users rss feed.
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <returns>Rss feed content.</returns>
        Task<string> LoadFeedRawAsync(Guid userId);

        /// <summary>
        /// Delete users rss feed.
        /// </summary>
        /// <param name="userId">UserId.</param>
        /// <returns>Awaitable task.</returns>
        Task DeleteAsync(Guid userId);

        /// <summary>
        /// Loads user's rss feed in form of items.
        /// </summary>
        /// <param name="userId">UserId.</param>
        /// <returns>Set of FeedItems.</returns>
        Task<SortedSet<FeedItem>> LoadUserFeedAsync(Guid userId);

        /// <summary>
        /// Loads base rss feed.
        /// </summary>
        /// <returns>Set of FeedItems.</returns>
        Task<SortedSet<FeedItem>> LoadBaseFeedAsync();

        /// <summary>
        /// Save user's feed.
        /// </summary>
        /// <param name="userId">UserId.</param>
        /// <param name="items">FeedItems to save.</param>
        /// <returns>Awaitable task.</returns>
        Task SaveUserFeedAsync(Guid userId, FeedItem[] items);

        /// <summary>
        /// Save base feed.
        /// </summary>
        /// <param name="items">FeedItems to save.</param>
        /// <returns>Awaitable task.</returns>
        Task SaveBaseFeedAsync(FeedItem[] items);
    }
}
