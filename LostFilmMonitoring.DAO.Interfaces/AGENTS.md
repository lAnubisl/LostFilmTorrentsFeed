# Data Access Layer Interfaces

## Repository Pattern

Central `IDal` interface aggregates all DAO interfaces:

```csharp
public interface IDal
{
    IFeedDao Feed { get; }
    ISeriesDao Series { get; }
    ISubscriptionDao Subscription { get; }
    ITorrentFileDao TorrentFile { get; }
    IUserDao User { get; }
    IEpisodeDao Episode { get; }
}
```

## DAO Interface Pattern

```csharp
public interface IUserDao
{
    Task<User?> LoadAsync(string userId);
    Task SaveAsync(User user);
    Task DeleteAsync(string userId);
}
```

## Domain Models

Located in `DomainModels/` subdirectory:

- `User`: (Id, TrackerId)
- `Series`: (Id, Name, LastUpdateDate)
- `Episode`: (SeriesName, SeasonNumber, EpisodeNumber, Quality)
- `Subscription`: (UserId, SeriesName, Quality)
- `FeedItem`: RSS feed item with title, link, date, torrent info
- `TorrentFile`: (FileName, Content)

## Quality Constants

```csharp
public static class Quality
{
    public const string SD = "SD";
    public const string H1080 = "1080";
    public const string H720 = "MP4";
}
```

## Notes

- No implementation details in this project
- Only interfaces and domain models
- All async operations return `Task<T>`
