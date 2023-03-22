// <copyright file="IndexViewModel.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.BLL.Models.ViewModel;

/// <summary>
/// Represents information to be shown in home screen.
/// </summary>
public class IndexViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IndexViewModel"/> class.
    /// </summary>
    /// <param name="series">Series.</param>
    public IndexViewModel(ICollection<Series> series)
    {
        this.Items = series.OrderByDescending(s => s.LastEpisode).Select(s => new IndexViewItemModel(s)).ToArray();
        this.Last24HoursItems = Filter(series, s => s.LastEpisode >= DateTime.Now.AddHours(-24));
        this.Last7DaysItems = Filter(series, s => s.LastEpisode < DateTime.Now.AddHours(-24) && s.LastEpisode >= DateTime.Now.AddDays(-7));
        this.OlderItems = Filter(series, s => s.LastEpisode < DateTime.Now.AddDays(-7));
    }

    /// <summary>
    /// Gets or sets episodes updated within last 24 hours.
    /// </summary>
    public string[] Last24HoursItems { get; set; }

    /// <summary>
    /// Gets or sets episodes updated within last 7 days.
    /// </summary>
    public string[] Last7DaysItems { get; set; }

    /// <summary>
    /// Gets or sets episodes older than 7 days..
    /// </summary>
    public string[] OlderItems { get; set; }

    /// <summary>
    /// Gets or sets items to be shown in home screen.
    /// </summary>
    public IndexViewItemModel[] Items { get; set; }

    private static string[] Filter(ICollection<Series> series, Func<Series, bool> predicate)
    {
        return series
            .Where(predicate)
            .OrderByDescending(s => s.LastEpisode)
            .Select(s => s.Name)
            .ToArray();
    }
}
