// <copyright file="SeriesViewModel.cs" company="Alexander Panfilenok">
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
    using System.Linq;
    using LostFilmMonitoring.DAO.DomainModels;

    /// <summary>
    /// Represents a single series shown on home page.
    /// </summary>
    public class SeriesViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesViewModel"/> class.
        /// </summary>
        /// <param name="series">Series.</param>
        /// <param name="selectedFeedItems">Selected series.</param>
        public SeriesViewModel(Series series, SelectedFeedItem[] selectedFeedItems)
        {
            this.Name = series.Name;
            var selectedItem = selectedFeedItems?.FirstOrDefault(i => i.SeriesName == series.Name);
            this.Selected = selectedItem != null;
            this.Quantity = selectedItem?.Quality;
        }

        /// <summary>
        /// Gets a value indicating whether seris is selected by user.
        /// </summary>
        public bool Selected { get; }

        /// <summary>
        /// Gets Series name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets series quality.
        /// </summary>
        public string Quantity { get; }

        /// <summary>
        /// Gets series title.
        /// </summary>
        public string Title
        {
            get
            {
                var index = this.Name.IndexOf('(');
                if (index > 0)
                {
                    return this.Name.Substring(0, index);
                }

                return this.Name;
            }
        }

        /// <summary>
        /// Gets series escaped name.
        /// </summary>
        public string Escaped => this.Name.EscapePath();
    }
}
