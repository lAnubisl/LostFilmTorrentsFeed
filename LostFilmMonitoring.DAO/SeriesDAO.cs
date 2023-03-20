﻿// <copyright file="SeriesDAO.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2023 Alexander Panfilenok
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
    using System.Linq;
    using System.Threading.Tasks;
    using LostFilmMonitoring.Common;
    using LostFilmMonitoring.DAO.Interfaces;
    using LostFilmMonitoring.DAO.Interfaces.DomainModels;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Provides functionality for managing series.
    /// </summary>
    public class SeriesDAO : BaseDAO, ISeriesDAO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesDAO"/> class.
        /// </summary>
        /// <param name="configuration">IConfiguration.</param>
        public SeriesDAO(IConfiguration configuration)
            : base(configuration.SqlServerConnectionString)
        {
        }

        public Task DeleteAsync(Series series)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<Interfaces.DomainModels.Series> LoadAsync(string name)
        {
            using var ctx = this.OpenContext();
            return Mapper.Map(await ctx.Series.FirstOrDefaultAsync(s => s.Name == name));
        }

        /// <inheritdoc/>
        public async Task<Interfaces.DomainModels.Series[]> LoadAsync()
        {
            using var ctx = this.OpenContext();
            return (await ctx.Series.ToArrayAsync()).Select(Mapper.Map).ToArray();
        }

        /// <inheritdoc/>
        public async Task SaveAsync(Interfaces.DomainModels.Series series)
        {
            using var ctx = this.OpenContext();
            var existingSeries = await ctx.Series.FirstOrDefaultAsync(s => s.Name == series.Name);
            if (existingSeries == null)
            {
                ctx.Add(series);
            }
            else
            {
                existingSeries.LastEpisode = series.LastEpisode;
                existingSeries.LastEpisodeName = series.LastEpisodeName;
                existingSeries.LastEpisodeTorrentLinkSD = series.LastEpisodeTorrentLinkSD;
                existingSeries.LastEpisodeTorrentLinkMP4 = series.LastEpisodeTorrentLinkMP4;
                existingSeries.LastEpisodeTorrentLink1080 = series.LastEpisodeTorrentLink1080;
            }

            await ctx.SaveChangesAsync();
        }
    }
}
