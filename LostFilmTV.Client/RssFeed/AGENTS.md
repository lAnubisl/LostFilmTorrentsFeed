# RSS Feed Parsers

## Base RSS Feed Parser

```csharp
public abstract class BaseRssFeed<T> where T : FeedItemResponse
{
    public IReadOnlyCollection<T> LoadFeedItems(Stream stream)
    {
        var doc = XDocument.Load(stream);
        var items = doc.Descendants("item");
        return items.Select(CreateFeedItemResponse).ToList();
    }
    
    protected abstract T CreateFeedItemResponse(XElement item);
}
```

## ReteOrgRssFeed Implementation

Parses LostFilm.tv RSS feed from rete.org mirror:

```csharp
public class ReteOrgRssFeed : BaseRssFeed<ReteOrgFeedItemResponse>
{
    protected override ReteOrgFeedItemResponse CreateFeedItemResponse(XElement item)
    {
        return new ReteOrgFeedItemResponse
        {
            Title = item.Element("title")?.Value,
            Link = item.Element("link")?.Value,
            PublishDate = ParseDate(item.Element("pubDate")?.Value),
            OriginalTitle = ParseOriginalTitle(item.Element("title")?.Value),
            Quality = ParseQuality(item.Element("title")?.Value),
            // ...
        };
    }
}
```

## LostFilm RSS Format

**Title Format:** `Series Name (Original Name). Season X Episode Y (Quality)`

Example: `Звёздный путь: Дискавери (Star Trek: Discovery). Пилот (1080p)`

**Parsing Rules:**
- Extract Russian name before `(`
- Extract English name between `(` and `)`
- Extract quality from last `(` before `)`
- Quality values: SD, 1080, MP4

## Notes

- RSS feed uses XML format (RSS 2.0)
- All dates in RFC 822 format
- Enclosure element contains torrent download link
- Parser handles malformed XML gracefully (skips invalid items)
