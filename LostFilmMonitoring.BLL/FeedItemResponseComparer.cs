// <copyright file="FeedItemResponseComparer.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.BLL;

/// <summary>
/// FeedItemResponseComparer class.
/// </summary>
public class FeedItemResponseComparer : IEqualityComparer<FeedItemResponse>
{
    /// <summary>
    /// Equals method.
    /// </summary>
    /// <param name="x">First item.</param>
    /// <param name="y">Second item.</param>
    /// <returns>bool.</returns>
    public bool Equals(FeedItemResponse? x, FeedItemResponse? y)
    {
        if (x == null && y == null)
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        if (!string.Equals(x.Title, y.Title, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.Equals(x.Link, y.Link, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.Equals(x.PublishDate, y.PublishDate, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// GetHashCode method.
    /// </summary>
    /// <param name="obj">Item.</param>
    /// <returns>int.</returns>
    public int GetHashCode(FeedItemResponse obj)
    {
        return HashCode.Combine(obj.Title, obj.Link, obj.PublishDate);
    }
}