# Common Utilities

## Purpose

Shared constants, interfaces, and utilities used across all projects.

## Key Files

- `Constants.cs`: Storage container/table names, blob paths
- `EnvironmentVariables.cs`: Environment variable key names
- `ActivitySourceNames.cs`: OpenTelemetry activity source names
- `CommonSerializationOptions.cs`: JSON serialization settings
- `ILogger.cs`: Custom logging interface abstraction
- `IConfiguration.cs`: Configuration abstraction

## Usage Patterns

```csharp
// Constants
var containerName = Constants.MetadataStorageContainerImages;
var tableName = Constants.MetadataStorageTableUsers;

// Environment variables
var storageAccount = Environment.GetEnvironmentVariable(EnvironmentVariables.MetadataStorageAccountName);

// Serialization
var json = JsonSerializer.Serialize(model, CommonSerializationOptions.Default);

// Logging
logger.Info("Message");
logger.Error("Error", exception);
var scoped = logger.CreateScope("ClassName");
```

## Notes

- All constants use PascalCase
- No business logic in this project
- Only abstractions and shared values
