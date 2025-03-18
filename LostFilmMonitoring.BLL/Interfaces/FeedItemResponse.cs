// <copyright file="FeedItemResponse.cs" company="Alexander Panfilenok">
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

namespace LostFilmMonitoring.BLL.Interfaces;

/// <summary>
/// FeedItemResponse interface.
/// </summary>
public class FeedItemResponse : IComparable<FeedItemResponse>
{
    /// <summary>
    /// Gets or sets Series Name (Russian Part only).
    /// </summary>
    public string? SeriesNameRu { get; set; }

    /// <summary>
    /// Gets or sets Series Name (English Part only).
    /// </summary>
    public string? SeriesNameEn { get; set; }

    /// <summary>
    /// Gets or sets Series Name.
    /// </summary>
    public string? SeriesName { get; set; }

    /// <summary>
    /// Gets or sets Episode Name.
    /// </summary>
    public string? EpisodeName { get; set; }

    /// <summary>
    /// Gets or sets Episode Number.
    /// </summary>
    public int? EpisodeNumber { get; set; }

    /// <summary>
    /// Gets or sets Season Number..
    /// </summary>
    public int? SeasonNumber { get; set; }

    /// <summary>
    /// Gets or sets Quality.
    /// </summary>
    public string? Quality { get; set; }

    /// <summary>
    /// Gets or sets Title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets Link.
    /// </summary>
    public string? Link { get; set; }

    /// <summary>
    /// Gets or sets Description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets PublishDateParsed.
    /// </summary>
    public DateTime PublishDateParsed { get; set; }

    /// <summary>
    /// Gets or sets PublishDate.
    /// </summary>
    public string? PublishDate { get; set; }

    /// <summary>
    /// Gets or sets TorrentId.
    /// </summary>
    public string? TorrentId { get; set; }

    /// <inheritdoc/>
    int IComparable<FeedItemResponse>.CompareTo(FeedItemResponse? that)
    {
        if (that == null)
        {
            return 1;
        }

        if (this.PublishDateParsed < that.PublishDateParsed)
        {
            return 1;
        }

        if (this.PublishDateParsed > that.PublishDateParsed)
        {
            return -1;
        }

        return string.Compare(this.Title, that.Title, StringComparison.OrdinalIgnoreCase);
    }
}