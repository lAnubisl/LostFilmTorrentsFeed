namespace LostFilmTV.Client.Response;

/// <summary>
/// FeedItemResponse.
/// </summary>
public sealed class ReteOrgFeedItemResponse : FeedItemResponse
{
    private static readonly string[] QualityValues = { "MP4", "1080p", "SD" };

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
    /// Try to parse an RSS item XElement into a strongly-typed feed item.
    /// Returns true on success and sets <paramref name="result"/>; returns false and logs when parsing fails.
    /// </summary>
    /// <returns>True if parsing succeeded; false if parsing failed and the item should be skipped.</returns>
    /// <param name="xElement">XML element representing an <c>item</c>.</param>
    /// <param name="result">Parsed result if successful; otherwise null.</param>
    /// <param name="log">Optional logger action to receive diagnostics when parsing fails.</param>
    public static bool TryParseFromXElement(XElement xElement, out ReteOrgFeedItemResponse? result, Action<string>? log = null)
    {
        result = null;
        if (xElement == null)
        {
            log?.Invoke("xElement is null");
            return false;
        }

        var elements = xElement.Elements();
        if (elements == null || !elements.Any())
        {
            log?.Invoke("item element has no child elements");
            return false;
        }

        try
        {
            var link = elements.First(i => i.Name.LocalName == "link").Value;
            var publishDate = elements.First(i => i.Name.LocalName == "pubDate").Value;
            var title = elements.First(i => i.Name.LocalName == "title").Value;
            var description = elements.FirstOrDefault(i => i.Name.LocalName == "description")?.Value ?? string.Empty;

            var parsed = new ReteOrgFeedItemResponse();
            parsed.Link = link;
            parsed.TorrentId = GetTorrentId(parsed.Link);
            parsed.PublishDate = publishDate;
            parsed.PublishDateParsed = DateTime.ParseExact(parsed.PublishDate, "ddd, dd MMM yyyy HH:mm:ss +0000", CultureInfo.InvariantCulture);
            parsed.PublishDateParsed = DateTime.SpecifyKind(parsed.PublishDateParsed, DateTimeKind.Utc);
            parsed.Title = title;
            parsed.Description = description;

            if (!TryParseTitle(title, out string seriesNameRu, out string seriesNameEn, out string episodeName, out int seasonNumber, out int episodeNumber, out string? quality))
            {
                // parsing failed; include raw title in diagnostic and return null so outer logic can skip the item
                log?.Invoke($"Failed to parse title: '{title}'");
                result = null;
                return false;
            }

            parsed.SeriesNameRu = seriesNameRu;
            parsed.SeriesNameEn = seriesNameEn;
            parsed.EpisodeName = episodeName;
            parsed.SeasonNumber = seasonNumber;
            parsed.EpisodeNumber = episodeNumber;
            parsed.SeriesName = $"{parsed.SeriesNameRu} ({parsed.SeriesNameEn})";
            parsed.Quality = quality;

            result = parsed;
            return true;
        }
        catch (Exception ex)
        {
            log?.Invoke($"Exception while parsing item: {ex.Message}");
            return false;
        }
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return this.Title ?? string.Empty;
    }

    private static bool TryParseTitle(
        string title,
        out string seriesNameRu,
        out string seriesNameEn,
        out string episodeName,
        out int seasonNumber,
        out int episodeNumber,
        out string? quality)
    {
        seriesNameRu = string.Empty;
        seriesNameEn = string.Empty;
        episodeName = string.Empty;
        seasonNumber = 0;
        episodeNumber = 0;
        quality = null;

        // Example title:
        // "Внешние сферы (Outer Range). Неведомое (S01E07) [MP4]"
        //  ^seriesRu^ ^seriesEn^   ^episode^ ^season/episode^ ^quality^
        if (!TryParseSeriesNames(title, out seriesNameRu, out seriesNameEn, out int episodeStartIndex))
        {
            return false;
        }

        // Parse the episode block, beginning after the series name.
        if (!TryParseEpisodeDetails(title, episodeStartIndex, out episodeName, out seasonNumber, out episodeNumber, out int qualityStartIndex))
        {
            return false;
        }

        return TryParseQuality(title, qualityStartIndex, out quality);
    }

    /// <summary>
    /// Extracts the Russian and English series names and returns the index
    /// immediately after the title separator so episode parsing can begin.
    /// </summary>
    /// <example>
    /// Input: "Внешние сферы (Outer Range). Неведомое (S01E07) [MP4]"
    /// Output: seriesNameRu="Внешние сферы", seriesNameEn="Outer Range",
    /// episodeStartIndex points to the first non-whitespace character after ".".
    /// </example>
    private static bool TryParseSeriesNames(
        string title,
        out string seriesNameRu,
        out string seriesNameEn,
        out int episodeStartIndex)
    {
        seriesNameRu = string.Empty;
        seriesNameEn = string.Empty;
        episodeStartIndex = 0;

        if (string.IsNullOrWhiteSpace(title))
        {
            return false;
        }

        int englishOpenIndex = title.IndexOf(" (", StringComparison.InvariantCulture);
        if (englishOpenIndex <= 0)
        {
            return false;
        }

        int englishCloseIndex = title.IndexOf(')', englishOpenIndex + 2);
        if (englishCloseIndex < 0)
        {
            return false;
        }

        // Russian series name is everything before " (".
        seriesNameRu = title.Substring(0, englishOpenIndex).Trim();

        // English series name is inside the first parentheses.
        seriesNameEn = title.Substring(englishOpenIndex + 2, englishCloseIndex - englishOpenIndex - 2).Trim();

        int afterEnglish = englishCloseIndex + 1;
        if (afterEnglish >= title.Length || title[afterEnglish] != '.')
        {
            return false;
        }

        // Skip the dot and any spaces after it.
        afterEnglish++;
        while (afterEnglish < title.Length && char.IsWhiteSpace(title[afterEnglish]))
        {
            afterEnglish++;
        }

        episodeStartIndex = afterEnglish;
        return true;
    }

    /// <summary>
    /// Extracts episode name, season and episode numbers, and returns the index
    /// where optional quality information begins.
    /// </summary>
    /// <example>
    /// Input (startIndex points at "Неведомое ..."):
    /// "Неведомое (S01E07) [MP4]"
    /// Output: episodeName="Неведомое", seasonNumber=1, episodeNumber=7,
    /// qualityStartIndex points to the space before "[MP4]".
    /// </example>
    private static bool TryParseEpisodeDetails(
        string title,
        int startIndex,
        out string episodeName,
        out int seasonNumber,
        out int episodeNumber,
        out int qualityStartIndex)
    {
        episodeName = string.Empty;
        seasonNumber = 0;
        episodeNumber = 0;
        qualityStartIndex = 0;

        // Find the last occurrence of " (S" because the title may contain extra parentheses earlier.
        int seasonOpenIndex = title.LastIndexOf(" (S", StringComparison.InvariantCultureIgnoreCase);
        if (seasonOpenIndex < startIndex)
        {
            return false;
        }

        // Episode name is everything between the series section and the season marker.
        episodeName = title.Substring(startIndex, seasonOpenIndex - startIndex).Trim();
        if (string.IsNullOrEmpty(episodeName))
        {
            return false;
        }

        int seasonNumberStart = seasonOpenIndex + 3;
        int episodeSeparatorIndex = title.IndexOf('E', seasonNumberStart);
        if (episodeSeparatorIndex < 0)
        {
            return false;
        }

        string seasonText = title.Substring(seasonNumberStart, episodeSeparatorIndex - seasonNumberStart);

        int episodeNumberStart = episodeSeparatorIndex + 1;
        int episodeCloseIndex = title.IndexOf(')', episodeNumberStart);
        if (episodeCloseIndex < 0)
        {
            return false;
        }

        string episodeText = title.Substring(episodeNumberStart, episodeCloseIndex - episodeNumberStart);
        if (!int.TryParse(seasonText, NumberStyles.None, CultureInfo.InvariantCulture, out seasonNumber) ||
            !int.TryParse(episodeText, NumberStyles.None, CultureInfo.InvariantCulture, out episodeNumber))
        {
            return false;
        }

        // The quality block begins after the closing ")" and may be empty.
        qualityStartIndex = episodeCloseIndex + 1;
        return true;
    }

    /// <summary>
    /// Parses optional quality metadata from the tail of the title.
    /// </summary>
    /// <example>
    /// Input fragment: "[MP4]" => quality = "MP4"
    /// Input fragment: "[1080p]" => quality = "1080".
    /// </example>
    private static bool TryParseQuality(
        string title,
        int startIndex,
        out string? quality)
    {
        quality = null;
        if (startIndex >= title.Length)
        {
            // No quality section present.
            return true;
        }

        string remainder = title[startIndex..].Trim();
        if (string.IsNullOrEmpty(remainder))
        {
            // Trailing whitespace only.
            return true;
        }

        // Quality must be wrapped in square brackets, e.g. [MP4] or [1080p].
        if (!remainder.StartsWith('[') ||
            !remainder.EndsWith(']'))
        {
            return false;
        }

        string qualityText = remainder[1..^1];
        if (!QualityValues.Contains(qualityText, StringComparer.InvariantCultureIgnoreCase))
        {
            return false;
        }

        quality = qualityText.Replace("1080p", "1080", StringComparison.InvariantCultureIgnoreCase);
        return true;
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
