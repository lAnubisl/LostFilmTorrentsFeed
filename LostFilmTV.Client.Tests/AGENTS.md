# LostFilm.tv Client Tests

## Testing Strategy

Test RSS feed parsing and client behavior using sample data files.

## Test Data

Located in `TestData/` subdirectory:
- `BaseFeed1.xml`: Sample LostFilm RSS feed
- `LostFilmFeed1.xml`: Alternative feed format

## RSS Feed Parsing Tests

```csharp
[Test]
public void ReteOrgRssFeed_should_parse_valid_feed()
{
    // Arrange
    var xml = Helper.GetResourceStream("BaseFeed1.xml");
    var feed = new ReteOrgRssFeed();
    
    // Act
    var items = feed.LoadFeedItems(xml);
    
    // Assert
    items.Should().HaveCount(expected);
    items[0].Title.Should().Contain("Series Name");
    items[0].Quality.Should().Be("1080");
}
```

## Helper Class

`Helper.cs` provides utilities for loading test resources:

```csharp
public static class Helper
{
    public static Stream GetResourceStream(string filename)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"LostFilmTV.Client.Tests.TestData.{filename}";
        return assembly.GetManifestResourceStream(resourceName);
    }
}
```

## Test Patterns

- Verify title parsing extracts series name correctly
- Verify quality extraction (SD, 1080, MP4)
- Verify publish date parsing
- Verify torrent link extraction

## Notes

- All test resources embedded in assembly
- Tests use actual LostFilm RSS XML structure
