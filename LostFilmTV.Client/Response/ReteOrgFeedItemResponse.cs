// <copyright file="ReteOrgFeedItemResponse.cs" company="Alexander Panfilenok">
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

namespace LostFilmTV.Client.Response;

/// <summary>
/// FeedItemResponse.
/// </summary>
public sealed class ReteOrgFeedItemResponse : FeedItemResponse
{
    // Уокер (Walker). То, чего раньше не было (S03E06) [1080p]
    private const string RegexPattern = @"(?<SeriesNameRu>.+) \((?<SeriesNameEng>.+)\)\. (?<EpisodeNameRu>.+) \(S(?<SeasonNumber>[0-9]+)E(?<EpisodeNumber>[0-9]+)\) \[(?<Quality>MP4|1080p|SD)\]";

    // Периферийные устройства (The Peripheral). А как же Боб?. (S01E05)
    private const string RegexPattern2 = @"(?<SeriesNameRu>.+) \((?<SeriesNameEng>.+)\)\. (?<EpisodeNameRu>.+) \(S(?<SeasonNumber>[0-9]+)E(?<EpisodeNumber>[0-9]+)\)";

    /// <summary>
    /// Initializes a new instance of the <see cref="ReteOrgFeedItemResponse"/> class.
    /// This constructor is required for JSON deserializer.
    /// </summary>
    public ReteOrgFeedItemResponse()
    {
        this.Title = string.Empty;
        this.Link = string.Empty;
        this.Description = string.Empty;
        this.PublishDate = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReteOrgFeedItemResponse"/> class.
    /// </summary>
    /// <param name="xElement">element.</param>
    internal ReteOrgFeedItemResponse(XElement xElement)
        : this()
    {
        var elements = xElement.Elements();
        if (elements == null || !elements.Any())
        {
            return;
        }

        this.Link = elements.First(i => i.Name.LocalName == "link").Value;
        this.TorrentId = GetTorrentId(this.Link);
        this.PublishDate = elements.First(i => i.Name.LocalName == "pubDate").Value;
        this.PublishDateParsed = DateTime.ParseExact(this.PublishDate, "ddd, dd MMM yyyy HH:mm:ss +0000", CultureInfo.InvariantCulture);
        this.PublishDateParsed = DateTime.SpecifyKind(this.PublishDateParsed, DateTimeKind.Utc);
        this.Title = elements.First(i => i.Name.LocalName == "title").Value;
        this.Description = elements.FirstOrDefault(i => i.Name.LocalName == "description")?.Value ?? string.Empty;

        var match = Regex.Match(this.Title, RegexPattern);
        if (!match.Success)
        {
            match = Regex.Match(this.Title, RegexPattern2);
            if (!match.Success)
            {
                return;
            }
        }

        this.SeriesNameRu = match.Groups["SeriesNameRu"].Value;
        this.SeriesNameEn = match.Groups["SeriesNameEng"].Value;
        this.EpisodeName = match.Groups["EpisodeNameRu"].Value;
        this.SeasonNumber = int.Parse(match.Groups["SeasonNumber"].Value);
        this.EpisodeNumber = int.Parse(match.Groups["EpisodeNumber"].Value);
        this.SeriesName = $"{this.SeriesNameRu} ({this.SeriesNameEn})";
        if (match.Groups.ContainsKey("Quality"))
        {
            this.Quality = match.Groups["Quality"].Value.Replace("1080p", "1080");
        }
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return this.Title ?? string.Empty;
    }

    private static string? GetTorrentId(string? reteOrgUrl)
    {
        // http://tracktor.in/rssdownloader.php?id=33572
        if (string.IsNullOrEmpty(reteOrgUrl))
        {
            return null;
        }

        string marker = "rssdownloader.php?id=";
        int index = reteOrgUrl.IndexOf(marker);
        if (index < 0)
        {
            return null;
        }

        return reteOrgUrl[(index + marker.Length) ..];
    }
}
