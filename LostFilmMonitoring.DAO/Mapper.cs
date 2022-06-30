// <copyright file="Mapper.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.Sql
{
    using System;
    using LostFilmMonitoring.DAO.Sql.DomainModels;

    internal static class Mapper
    {
        internal static Series Map(Interfaces.DomainModels.Series series)
            => new ()
            {
                LastEpisode = series.LastEpisode,
                LastEpisodeName = series.LastEpisodeName,
                LastEpisodeTorrentLink1080 = series.LastEpisodeTorrentLink1080,
                LastEpisodeTorrentLinkMP4 = series.LastEpisodeTorrentLinkMP4,
                LastEpisodeTorrentLinkSD = series.LastEpisodeTorrentLinkSD,
                Name = series.Name,
            };

        internal static Interfaces.DomainModels.Series Map(Series series)
            => new Interfaces.DomainModels.Series(
                series.Name,
                series.LastEpisode,
                series.LastEpisodeName,
                series.LastEpisodeTorrentLinkSD,
                series.LastEpisodeTorrentLinkMP4,
                series.LastEpisodeTorrentLink1080,
                null, null, null, null, null, null);

        internal static Interfaces.DomainModels.Subscription Map(Subscription s)
            => new (s.SeriesName, s.Quality);

        internal static Subscription Map(Interfaces.DomainModels.Subscription subscription, Guid userId)
        {
            return new Subscription
            {
                Quality = subscription.Quality,
                SeriesName = subscription.SeriesName,
                UserId = userId,
            };
        }

        internal static User Map(Interfaces.DomainModels.User user)
        {
            return new User
            {
                Id = Guid.Parse(user.Id),
                TrackerId = user.TrackerId,
            };
        }

        internal static Interfaces.DomainModels.User Map(User user)
            => new Interfaces.DomainModels.User(user.Id.ToString(), user.TrackerId);
    }
}
