// <copyright file="Subscription.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.DAO.Interfaces.DomainModels
{
    /// <summary>
    /// Subscription.
    /// </summary>
    public sealed class Subscription : IEquatable<Subscription>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription"/> class.
        /// </summary>
        /// <param name="seriesName">Name of the series.</param>
        /// <param name="quality">Quality of the series.</param>
        public Subscription(string seriesName, string quality)
        {
            this.SeriesName = seriesName;
            this.Quality = quality;
        }

        /// <summary>
        /// Gets SeriesName.
        /// </summary>
        public string SeriesName { get; }

        /// <summary>
        /// Gets Quantity.
        /// </summary>
        public string Quality { get; }

        /// <summary>
        /// Filter selected subscriptions taking into account existing ones.
        /// </summary>
        /// <param name="selected">User selected subscriptions.</param>
        /// <param name="old">User existing subscriptions.</param>
        /// <returns>New subscriptions that were selected.</returns>
        public static IEnumerable<Subscription> Filter(Subscription[] selected, Subscription[] old)
        {
            var hs = new HashSet<Subscription>(old);
            return selected.Where(s => !hs.Contains(s));
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.SeriesName.GetHashCode(), this.Quality.GetHashCode());
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return this.Equals(obj as Subscription);
        }

        /// <inheritdoc/>
        public bool Equals(Subscription? other)
        {
            if (other == null)
            {
                return false;
            }

            return string.Equals(this.SeriesName, other.SeriesName, StringComparison.OrdinalIgnoreCase)
                && this.Quality == other.Quality;
        }
    }
}
