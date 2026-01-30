namespace LostFilmMonitoring.BLL;

/// <summary>
/// Usefull extensions.
/// </summary>
public static class Extensions
{
    private static readonly HashSet<char> ForbiddenPrimaryKeyCharacters = new () { '/', '\\', '?', '#', '\t', '\r', '\n', '+' };

    /// <summary>
    /// Generates torrent link for user's feed.
    /// </summary>
    /// <param name="baseUrl">Base website URL.</param>
    /// <param name="userId">User Id.</param>
    /// <param name="torrentFileName">Torrent Id.</param>
    /// <returns>Torrent link.</returns>
    internal static string GenerateTorrentLink(string baseUrl, string userId, string torrentFileName)
    {
        return $"{baseUrl}/{Constants.MetadataStorageContainerUserTorrents}/{userId}/{torrentFileName}.torrent";
    }

    /// <summary>
    /// Replaces trackers for an instance of <see cref="BencodeNET.Torrents.Torrent"/>.
    /// </summary>
    /// <param name="torrent">Instance of <see cref="BencodeNET.Torrents.Torrent"/>.</param>
    /// <param name="announces">Array of trackers.</param>
    internal static void FixTrackers(this BencodeNET.Torrents.Torrent torrent, string[] announces)
    {
        torrent.Trackers = announces.Select(a => new List<string>() { a } as IList<string>).ToList();
    }

    /// <summary>
    /// Map instance of <see cref="BencodeNET.Torrents.Torrent"/> to <see cref="TorrentFile"/>.
    /// </summary>
    /// <param name="torrent">Instance of <see cref="BencodeNET.Torrents.Torrent"/>.</param>
    /// <returns>Instance of <see cref="TorrentFile"/>.</returns>
    internal static TorrentFile ToTorrentFile(this BencodeNET.Torrents.Torrent torrent)
        => new (torrent.DisplayNameUtf8 ?? torrent.DisplayName, torrent.ToStream());

    /// <summary>
    /// Clean forbidden characters in a collection of <see cref="FeedItemResponse"/>.
    /// </summary>
    /// <param name="feedItemResponses">Collection of <see cref="FeedItemResponse"/>.</param>
    internal static void CleanForbiddenCharacters(this IEnumerable<FeedItemResponse> feedItemResponses)
    {
        foreach (var feedItemResponse in feedItemResponses)
        {
            feedItemResponse.CleanForbiddenCharacters();
        }
    }

    /// <summary>
    /// Clean forbidden characters in an instance of <see cref="FeedItemResponse"/>.
    /// </summary>
    /// <param name="feedItemResponse">Instance of <see cref="FeedItemResponse"/>.</param>
    internal static void CleanForbiddenCharacters(this FeedItemResponse feedItemResponse)
    {
        feedItemResponse.Title = ReplaceForbiddenCharacters(feedItemResponse.Title) !;
        feedItemResponse.EpisodeName = ReplaceForbiddenCharacters(feedItemResponse.EpisodeName);
        feedItemResponse.SeriesName = ReplaceForbiddenCharacters(feedItemResponse.SeriesName);
        feedItemResponse.SeriesNameEn = ReplaceForbiddenCharacters(feedItemResponse.SeriesNameEn);
        feedItemResponse.SeriesNameRu = ReplaceForbiddenCharacters(feedItemResponse.SeriesNameRu);
    }

    /// <summary>
    /// Generate instance of <see cref="BencodeNET.Torrents.Torrent"/> from a <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">An instance of <see cref="Stream"/> to torrent file.</param>
    /// <returns>Instance of <see cref="BencodeNET.Torrents.Torrent"/>.</returns>
    internal static BencodeNET.Torrents.Torrent ToTorrentDataStructure(this Stream stream)
    {
        var parser = new BencodeNET.Torrents.TorrentParser(BencodeNET.Torrents.TorrentParserMode.Tolerant);
        var torrent = parser.Parse(new BencodeNET.IO.BencodeReader(stream));
        torrent.IsPrivate = false;
        stream.Position = 0;
        return torrent;
    }

    /// <summary>
    /// Map instance of <see cref="FeedItemResponse"/> to <see cref="Episode"/>.
    /// </summary>
    /// <param name="feedItem">Instance of <see cref="FeedItemResponse"/>.</param>
    /// <returns>Instance of <see cref="Episode"/>.</returns>
    internal static Episode? ToEpisode(this FeedItemResponse feedItem)
    {
        var torrentId = feedItem.TorrentId;
        if (string.IsNullOrEmpty(feedItem.SeriesName)
            || string.IsNullOrEmpty(feedItem.EpisodeName)
            || feedItem.EpisodeNumber == null
            || feedItem.SeasonNumber == null
            || string.IsNullOrEmpty(feedItem.Quality)
            || string.IsNullOrEmpty(torrentId))
        {
            return null;
        }

        return new (
            feedItem.SeriesName,
            feedItem.EpisodeName,
            feedItem.SeasonNumber.Value,
            feedItem.EpisodeNumber.Value,
            torrentId,
            feedItem.Quality);
    }

    /// <summary>
    /// Map instance of <see cref="FeedItemResponse"/> to <see cref="Series"/>.
    /// </summary>
    /// <param name="feedItem">Instance of <see cref="FeedItemResponse"/>.</param>
    /// <returns>Instance of <see cref="Series"/>.</returns>
    internal static Series? ToSeries(this FeedItemResponse feedItem)
    {
        if (feedItem == null || string.IsNullOrEmpty(feedItem.SeriesName))
        {
            return null;
        }

        return new (
            Guid.Empty,
            feedItem.SeriesName,
            feedItem.PublishDateParsed,
            $"{feedItem.SeriesName}. {feedItem.EpisodeName} (S{feedItem.SeasonNumber:D2}E{feedItem.EpisodeNumber:D2}) ",
            ParseLink(feedItem, Quality.SD),
            ParseLink(feedItem, Quality.H720),
            ParseLink(feedItem, Quality.H1080),
            ParseSeasonNumber(feedItem, Quality.H1080),
            ParseSeasonNumber(feedItem, Quality.H720),
            ParseSeasonNumber(feedItem, Quality.SD),
            ParseEpisodeNumber(feedItem, Quality.H1080),
            ParseEpisodeNumber(feedItem, Quality.H720),
            ParseEpisodeNumber(feedItem, Quality.SD));
    }

    /// <summary>
    /// Calculates if there are any updates in <paramref name="newItems"/> in comparison to <paramref name="oldItems"/>.
    /// </summary>
    /// <param name="newItems">Instance of <see cref="SortedSet{IFeedItemResponse}"/> representing new items.</param>
    /// <param name="oldItems">Instance of <see cref="SortedSet{IFeedItemResponse}"/> representing old items.</param>
    /// <returns>returns true in case when <paramref name="newItems"/> has updates in comparison to <paramref name="oldItems"/>, otherwise false.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="newItems"/> must not be null, <paramref name="oldItems"/> must not be null.</exception>
    internal static bool HasUpdates(SortedSet<FeedItemResponse>? newItems, SortedSet<FeedItemResponse>? oldItems)
    {
        if (newItems == null)
        {
            throw new ArgumentNullException(nameof(newItems));
        }

        if (oldItems == null)
        {
            throw new ArgumentNullException(nameof(oldItems));
        }

        if (newItems.Count != oldItems.Count)
        {
            return true;
        }

        var newEnum = newItems.GetEnumerator();
        var oldEnum = oldItems.GetEnumerator();

        var comparer = new FeedItemResponseEqualityComparer();
        while (newEnum.MoveNext() && oldEnum.MoveNext())
        {
            if (!comparer.Equals(newEnum.Current, oldEnum.Current))
            {
                return true;
            }
        }

        return false;
    }

    private static string? ReplaceForbiddenCharacters(string? str)
        => str == null
            ? null
            : new (str.ToCharArray().Where(c => !ForbiddenPrimaryKeyCharacters.Contains(c)).ToArray());

    private static Stream ToStream(this BencodeNET.Torrents.Torrent torrent)
    {
        var ms = new MemoryStream();
        torrent.EncodeTo(ms);
        ms.Position = 0;
        return ms;
    }

    private static string? ParseLink(FeedItemResponse feedItem, string quality)
        => feedItem.Quality == quality ? feedItem.Link : null;

    private static int? ParseEpisodeNumber(FeedItemResponse feedItem, string quality)
        => feedItem.Quality == quality ? feedItem.EpisodeNumber : null;

    private static int? ParseSeasonNumber(FeedItemResponse feedItem, string quality)
        => feedItem.Quality == quality ? feedItem.SeasonNumber : null;
}
