# LostFilm.tv API Client

## Purpose

HTTP client for interacting with LostFilm.tv:
- Parse RSS feed from rete.org mirror
- Download torrent files
- Extract series and episode metadata

## Key Classes

- `LostFilmClient`: Main HTTP client for downloading torrents and RSS
- `ReteOrgRssFeed`: RSS feed parser for LostFilm feed format
- `ReteOrgFeedItemResponse`: Parsed feed item model
- `TorrentFileResponse`: Torrent file download result

## LostFilmClient Pattern

```csharp
public class LostFilmClient : ILostFilmClient
{
    public async Task<TorrentFileResponse> DownloadTorrentFileAsync(string url, string cookie)
    {
        using var response = await httpClient.GetAsync(url);
        var content = await response.Content.ReadAsByteArrayAsync();
        return new TorrentFileResponse
        {
            FileName = ExtractFileName(response),
            Content = content
        };
    }
}
```

## RSS Feed Parsing

```csharp
public class ReteOrgRssFeed : BaseRssFeed<ReteOrgFeedItemResponse>
{
    protected override ReteOrgFeedItemResponse CreateFeedItemResponse(XElement item)
    {
        // Parse XML elements: title, link, pubDate, enclosure
        return new ReteOrgFeedItemResponse
        {
            Title = item.Element("title")?.Value,
            Link = item.Element("link")?.Value,
            PublishDate = ParseDate(item.Element("pubDate")?.Value),
            // ...
        };
    }
}
```

## Feed Item Format

LostFilm RSS format:
- Title: "Series Name (Original Name). Description (Quality)"
- Link: Episode page URL
- Enclosure: Torrent download link

## Notes

- All HTTP operations are async
- Requires authentication cookie for downloads
- Handles RSS XML parsing errors gracefully
