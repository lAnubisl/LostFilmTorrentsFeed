// <copyright file="IEpisodeDAO.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.Interfaces;

/// <summary>
/// Provides functionality for managing episode.
/// </summary>
public interface IEpisodeDao
{
    /// <summary>
    /// Save series.
    /// </summary>
    /// <param name="episode">Episode to save.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task SaveAsync(Episode episode);

    /// <summary>
    /// Check if suche episode already exist.
    /// </summary>
    /// <param name="seriesName">Name of the series.</param>
    /// <param name="seasonNumber">The season number.</param>
    /// <param name="episideNumber">The eposide number.</param>
    /// <param name="quality">The quality.</param>
    /// <returns>True is such episode exists. Otherwise false.</returns>
    Task<bool> ExistsAsync(string seriesName, int seasonNumber, int episideNumber, string quality);
}
