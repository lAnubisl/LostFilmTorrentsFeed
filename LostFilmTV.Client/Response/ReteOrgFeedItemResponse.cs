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

    private static readonly TimeSpan RegexTimeout = TimeSpan.FromMilliseconds(100);

    private static readonly RegexOptions RegexOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.Singleline;

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

        var match = Regex.Match(this.Title, RegexPattern, RegexOptions, RegexTimeout);
        if (!match.Success)
        {
            match = Regex.Match(this.Title, RegexPattern2, RegexOptions, RegexTimeout);
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
        int index = reteOrgUrl.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
        {
            return null;
        }

        return reteOrgUrl[(index + marker.Length) ..];
    }
}
