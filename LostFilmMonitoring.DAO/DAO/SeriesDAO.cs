// <copyright file="SeriesDAO.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.DAO
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LostFilmMonitoring.DAO.DomainModels;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Provides functionality for managing series.
    /// </summary>
    public class SeriesDAO : BaseDAO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesDAO"/> class.
        /// </summary>
        /// <param name="connectionString">Database connection string.</param>
        public SeriesDAO(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>
        /// Load series by name.
        /// </summary>
        /// <param name="name">Series name.</param>
        /// <returns>Series.</returns>
        public async Task<Series> LoadAsync(string name)
        {
            using (var ctx = this.OpenContext())
            {
                return await ctx.Series.FirstOrDefaultAsync(s => s.Name == name);
            }
        }

        /// <summary>
        /// Load series.
        /// </summary>
        /// <returns>All series.</returns>
        public async Task<List<Series>> LoadAsync()
        {
            using (var ctx = this.OpenContext())
            {
                return await ctx.Series.ToListAsync();
            }
        }

        /// <summary>
        /// Save series.
        /// </summary>
        /// <param name="series">Series to save.</param>
        /// <returns>Awaitable task.</returns>
        public async Task SaveAsync(Series series)
        {
            using (var ctx = this.OpenContext())
            {
                var existingSerial = await ctx.Series.FirstOrDefaultAsync(s => s.Name == series.Name);
                if (existingSerial == null)
                {
                    ctx.Add(series);
                }
                else
                {
                    existingSerial.LastEpisode = series.LastEpisode;
                    existingSerial.LastEpisodeName = series.LastEpisodeName;
                    existingSerial.LastEpisodeTorrentLinkSD = series.LastEpisodeTorrentLinkSD;
                    existingSerial.LastEpisodeTorrentLinkMP4 = series.LastEpisodeTorrentLinkMP4;
                    existingSerial.LastEpisodeTorrentLink1080 = series.LastEpisodeTorrentLink1080;
                }

                await ctx.SaveChangesAsync();
            }
        }
    }
}
