# TMDB API Client

## Purpose

Download TV series cover images from The Movie Database (TMDB) API.

## TmdbClient Pattern

```csharp
public class TmdbClient
{
    private readonly HttpClient httpClient;
    private readonly string apiKey;
    
    public async Task<byte[]?> DownloadCoverImageAsync(string seriesName)
    {
        // 1. Search for series by name
        var searchUrl = $"https://api.themoviedb.org/3/search/tv?api_key={apiKey}&query={seriesName}";
        var searchResults = await httpClient.GetFromJsonAsync<SearchResponse>(searchUrl);
        
        // 2. Get first result's poster path
        var posterPath = searchResults?.Results?.FirstOrDefault()?.PosterPath;
        if (posterPath == null) return null;
        
        // 3. Download image
        var imageUrl = $"https://image.tmdb.org/t/p/w500{posterPath}";
        return await httpClient.GetByteArrayAsync(imageUrl);
    }
}
```

## Usage

- Searches TMDB by series name (English original name)
- Downloads poster image at w500 size (500px wide)
- Returns null if series not found or no poster available

## Authentication

- Requires TMDB API key from environment variables
- API key passed as query parameter

## Notes

- Free TMDB API has rate limits
- Series names must match TMDB database (usually English titles)
- Used by `DownloadCoverImagesCommand` to fetch images for new series
