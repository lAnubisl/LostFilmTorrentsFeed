# Domain Models

## Core Entities

**User:**
```csharp
public class User
{
    public string Id { get; set; }        // Generated GUID
    public string TrackerId { get; set; } // LostFilm.tv tracker ID
}
```

**Series:**
```csharp
public class Series
{
    public Guid Id { get; set; }
    public string Name { get; set; }              // Series name
    public DateTimeOffset LastUpdateDate { get; set; }
}
```

**Episode:**
```csharp
public class Episode
{
    public string SeriesName { get; set; }
    public int SeasonNumber { get; set; }
    public int EpisodeNumber { get; set; }
    public string Quality { get; set; }  // SD, 1080, MP4
}
```

**Subscription:**
```csharp
public class Subscription
{
    public string UserId { get; set; }
    public string SeriesName { get; set; }
    public string Quality { get; set; }  // SD, 1080, MP4
}
```

**FeedItem:**
```csharp
public class FeedItem
{
    public string Title { get; set; }
    public string Link { get; set; }
    public DateTimeOffset PublishDate { get; set; }
    public string TorrentUrl { get; set; }
    // Additional RSS feed properties
}
```

**TorrentFile:**
```csharp
public class TorrentFile
{
    public string FileName { get; set; }
    public byte[] Content { get; set; }
}
```

## Quality Constants

```csharp
public static class Quality
{
    public const string SD = "SD";
    public const string H1080 = "1080";  // High quality 1080p
    public const string H720 = "MP4";    // MP4 format (720p)
}
```

## Notes

- All domain models are POCOs (Plain Old CLR Objects)
- No validation logic in models (handled by validators)
- Quality values are restricted to SD, 1080, MP4
- DateTimeOffset used for proper timezone handling
