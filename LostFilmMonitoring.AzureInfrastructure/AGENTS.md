# Pulumi Infrastructure as Code

## Stack Pattern

- C# Pulumi project managing Azure resources
- Separate stacks for `dev` and `prod` environments
- Configuration in `Pulumi.{stack}.yaml` files

## Key Resources

- Resource Group, Log Analytics, Application Insights
- App Service Plan (Consumption Y1), Function App
- Storage Accounts: metadata (data/images/feeds), function runtime, static website
- Cloudflare DNS records and custom domain bindings
- RBAC role assignments for managed identity

## Authentication Strategy

- **Blob Storage**: Managed identity (RBAC) via `DefaultAzureCredential`
- **Table Storage**: Storage account keys for performance (lower latency)

## Deployment Files

- `Program.cs`: Main stack definition
- `*.yaml`: Environment-specific configurations
- Use `config.RequireSecret()` for sensitive values

## Common Operations

```csharp
// Create blob container with public access
new Azure.Storage.BlobContainer("images", new()
{
    ResourceGroupName = rg.Name,
    AccountName = storageAccount.Name,
    ContainerName = "images",
    PublicAccess = Azure.Storage.PublicAccess.Blob,
});

// Assign RBAC role
new Authorization.RoleAssignment("func-storage", new()
{
    PrincipalId = functionApp.Identity.Apply(i => i!.PrincipalId),
    RoleDefinitionId = "/providers/Microsoft.Authorization/roleDefinitions/...",
    Scope = storageAccount.Id,
});
```
