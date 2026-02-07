# Azure Table Storage Entities

This directory contains entity definitions for Azure Table Storage. Each entity represents a different table in the storage account.

## Table Storage Key Strategy

Azure Table Storage uses a composite key consisting of:
- **PartitionKey**: Used for data distribution and grouping related entities together
- **RowKey**: Unique identifier within a partition

Together, `PartitionKey + RowKey` form a unique primary key for each entity.

## 1. UserTableEntity

**Table Name**: `users`

**Purpose**: Stores user accounts with LostFilm.tv tracker credentials.

### Key Structure

| Key          | Value                          | Example                                |
|--------------|--------------------------------|----------------------------------------|
| PartitionKey | User ID                        | `df1410d2-d23a-4217-a326-c8877e0555c1` |
| RowKey       | User ID (same as PartitionKey) | `df1410d2-d23a-4217-a326-c8877e0555c1` |

**Note**: Both PartitionKey and RowKey are set to the user's ID. This creates a 1:1 mapping and allows direct lookups by user ID.

### Additional Properties

- `TrackerId`: User's tracker ID from LostFilm.tv (used in torrent announce URLs)
  - Example: `1b07a52cb12a12945e15cca756f83789`

### Query Patterns

- **Load by User ID**: Direct entity lookup using `userId` for both PartitionKey and RowKey
- **Delete by User ID**: Direct entity deletion using `userId`

### Mapping

```csharp
// Domain Model → Table Entity
UserTableEntity
{
    PartitionKey = user.Id,
    RowKey = user.Id,
    TrackerId = user.TrackerId,
    Timestamp = user.CreatedAt ?? DateTime.UtcNow
}

// Table Entity → Domain Model
User
{
    Id = entity.RowKey,
    TrackerId = entity.TrackerId,
    CreatedAt = entity.Timestamp.Value.DateTime
}
```

## 2. SeriesTableEntity

**Table Name**: `series`

**Purpose**: Stores TV series metadata including last episode information and quality-specific details.

### Key Structure

| Key          | Value                              | Example            |
|--------------|------------------------------------|--------------------|
| PartitionKey | Series Name                        | `The Walking Dead` |
| RowKey       | Series Name (same as PartitionKey) | `The Walking Dead` |

**Note**: Using series name for both keys enables direct lookups by series name and groups all data for a series together.

### Additional Properties

- `Id`: Unique GUID identifier for the series
- `Name`: Series name (duplicated for convenience)
- `LastEpisode`: DateTime of the last episode
- `LastEpisodeName`: Name of the last episode
- `LastEpisodeTorrentLinkSD`: Torrent link for SD quality
- `LastEpisodeTorrentLinkMP4`: Torrent link for MP4 (720p) quality
- `LastEpisodeTorrentLink1080`: Torrent link for 1080p quality
- `SeasonNumberSD`: Latest season number available in SD
- `SeasonNumberMP4`: Latest season number available in MP4
- `SeasonNumber1080`: Latest season number available in 1080p
- `EpisodeNumberSD`: Latest episode number available in SD
- `EpisodeNumberMP4`: Latest episode number available in MP4
- `EpisodeNumber1080`: Latest episode number available in 1080p

### Query Patterns

- **Load by Series Name**: Direct entity lookup using `seriesName` for both keys
- **List All Series**: Table scan (used for building index)

### Mapping

```csharp
// Domain Model → Table Entity
SeriesTableEntity
{
    PartitionKey = series.Name,
    RowKey = series.Name,
    Id = series.Id,
    Name = series.Name,
    LastEpisode = series.LastEpisode,
    LastEpisodeName = series.LastEpisodeName,
    LastEpisodeTorrentLink1080 = series.LastEpisodeTorrentLink1080,
    // ... other quality-specific properties
}

// Table Entity → Domain Model
Series
{
    Id = entity.Id,
    Name = entity.Name,
    LastEpisode = entity.LastEpisode,
    LastEpisodeName = entity.LastEpisodeName,
    // ... quality-specific properties
}
```

## 3. EpisodeTableEntity

**Table Name**: `episodes`

**Purpose**: Stores individual episode records for tracking which episodes have been processed.

### Key Structure

| Key          | Value       | Example            |
|--------------|-------------|--------------------|
| PartitionKey | Series Name | `The Walking Dead` |
| RowKey       | Torrent ID  | `123456`           |

**Key Strategy**: 
- **PartitionKey = Series Name**: Groups all episodes of a series together for efficient queries
- **RowKey = Torrent ID**: Unique identifier for each episode/quality combination

This design allows efficient queries like "Get all episodes for The Walking Dead" or "Check if episode with TorrentId X exists".

### Additional Properties

- `EpisodeName`: Full episode name (e.g., "S01E01")
- `SeasonNumber`: Season number (integer)
- `EpisodeNumber`: Episode number (integer)
- `Quality`: Quality setting (SD, MP4, 1080)

### Query Patterns

- **Check Episode Exists**: Query by PartitionKey (series name) and RowKey (torrent ID)
- **List Episodes by Series**: Query by PartitionKey (series name)
- **Prevent Duplicates**: Before processing a new episode, check if it already exists

### Mapping

```csharp
// Domain Model → Table Entity
EpisodeTableEntity
{
    PartitionKey = episode.SeriesName,
    RowKey = episode.TorrentId,
    EpisodeName = episode.EpisodeName,
    SeasonNumber = episode.SeasonNumber,
    EpisodeNumber = episode.EpisodeNumber,
    Quality = episode.Quality,
    Timestamp = DateTime.UtcNow
}

// Table Entity → Domain Model
Episode
{
    SeriesName = entity.PartitionKey,
    TorrentId = entity.RowKey,
    EpisodeName = entity.EpisodeName,
    SeasonNumber = entity.SeasonNumber,
    EpisodeNumber = entity.EpisodeNumber,
    Quality = entity.Quality
}
```

## 4. SubscriptionTableEntity

**Table Name**: `subscriptions`

**Purpose**: Stores user subscriptions to TV series with quality preferences.

### Key Structure

| Key          | Value       | Example                                |
|--------------|-------------|----------------------------------------|
| PartitionKey | Series Name | `The Walking Dead`                     |
| RowKey       | User ID     | `df1410d2-d23a-4217-a326-c8877e0555c1` |

**Key Strategy**:
- **PartitionKey = Series Name**: Groups all subscribers of a series together
- **RowKey = User ID**: Unique subscriber within the series partition

This design is optimized for the query pattern: "Find all users subscribed to a specific series" which is critical when a new episode is released and user feeds need to be updated.

### Additional Properties

- `Quality`: Requested quality for this subscription (SD, MP4, or 1080)

### Query Patterns

- **Find Users by Series**: Query by PartitionKey (series name) to get all subscribers when new episode is available
- **Load User Subscriptions**: Query by RowKey across all partitions (less efficient, used only when user edits subscriptions)
- **Update Subscription**: Delete all user subscriptions, then save new ones

### Mapping

```csharp
// Domain Model → Table Entity
SubscriptionTableEntity
{
    PartitionKey = subscription.SeriesName,
    RowKey = userId,
    Quality = subscription.Quality,
    Timestamp = DateTime.UtcNow
}

// Table Entity → Domain Model
Subscription
{
    SeriesName = entity.PartitionKey,
    Quality = entity.Quality
}
```

## Design Rationale

### Why Different Key Strategies?

1. **UserTableEntity** (User ID, User ID):
   - Direct lookups by user ID
   - No need for grouping or relationships
   - Simple 1:1 mapping

2. **SeriesTableEntity** (Series Name, Series Name):
   - Direct lookups by series name
   - Series names are unique identifiers
   - No need for complex partitioning

3. **EpisodeTableEntity** (Series Name, Torrent ID):
   - Groups episodes by series for efficient queries
   - Torrent ID provides uniqueness within series
   - Enables "get all episodes for series" queries

4. **SubscriptionTableEntity** (Series Name, User ID):
   - **Critical for performance**: When new episode arrives, need to quickly find ALL users subscribed to that series
   - Optimized for the most frequent query: "Who subscribed to The Walking Dead?"
   - Reverse lookup (all subscriptions for a user) is less frequent, only when editing subscriptions

### Quality Constants

The `Quality` property uses these standard values:
- `SD`: Standard Definition
- `MP4`: 720p (H.264 MP4)
- `1080`: 1080p Full HD

## Mapper Class

The `Mapper.cs` class in the parent directory provides bidirectional conversion between:
- Domain models (defined in `LostFilmMonitoring.DAO.Interfaces/DomainModels/`)
- Table entities (defined in this directory)

All mapping logic is centralized in this static class to ensure consistency across DAOs.
