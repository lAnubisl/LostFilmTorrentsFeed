# Response Models

## ReteOrgFeedItemResponse

Parsed RSS feed item from LostFilm.tv:

```csharp
public class ReteOrgFeedItemResponse
{
    public string Title { get; set; }           // Full title
    public string OriginalTitle { get; set; }   // English name
    public string SeriesName { get; set; }      // Russian name
    public string Link { get; set; }            // Episode page URL
    public string TorrentUrl { get; set; }      // Torrent download link
    public DateTimeOffset PublishDate { get; set; }
    public string Quality { get; set; }         // SD, 1080, MP4
    public int SeasonNumber { get; set; }
    public int EpisodeNumber { get; set; }
}
```

## TorrentFileResponse

Downloaded torrent file:

```csharp
public class TorrentFileResponse
{
    public string FileName { get; set; }  // Extracted from Content-Disposition header
    public byte[] Content { get; set; }   // Binary torrent file data
}
```

## Usage

```csharp
// Parse RSS feed
var feed = new ReteOrgRssFeed();
var items = await feed.LoadFeedItemsAsync(url, cookie);

// Download torrent
var client = new LostFilmClient();
var torrent = await client.DownloadTorrentFileAsync(items[0].TorrentUrl, cookie);
```

## Notes

- All response models are DTOs (Data Transfer Objects)
- Quality values normalized to constants: SD, 1080, MP4
- Dates converted to DateTimeOffset for proper timezone handling
