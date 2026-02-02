# GitHub Copilot — Repository Instructions (LostFilm RSS Feed)

Purpose: Help GitHub Copilot generate context-aware, correct, and secure code for this repository.

Summary
- Project: LostFilm RSS Feed service (C# .NET 8, Azure Functions isolated worker).
- Key folders: LostFilmMonitoring.AzureFunction (functions), LostFilmMonitoring.BLL (business logic/commands), LostFilmMonitoring.DAO.Azure (storage), LostFilmMonitoring.AzureInfrastructure (Pulumi), LostFilmMonitoring.Web (frontend).

What Copilot should assume
- Target framework: .NET 8.
- Patterns: Command pattern for business logic; DAOs for storage; DI registration in `Program.cs`.
- Azure: Blob storage uses managed identity (RBAC); Table storage uses storage account keys for performance.
- Pulumi used for IaC in `LostFilmMonitoring.AzureInfrastructure` (C# Pulumi).
- Tests: NUnit + Moq + FluentAssertions; tests live in `*.Tests` projects.

Coding conventions (brief)
- Indent: 4 spaces; Allman brace style.
- Use `var` when the type is obvious.
- Using directives: outside namespace, `System` first.
- Naming: PascalCase for public members, `I` prefix for interfaces, DAO classes end with `Dao`.
- Async methods end with `Async`.
- Validate constructor arguments with `?? throw new ArgumentNullException(nameof(...))` and scope loggers via `logger.CreateScope(...)`.
- Public APIs require XML docs.
- Use `CommonSerializationOptions.Default` for JSON serialization.

Where to add code
- New commands: add to [LostFilmMonitoring.BLL/Commands](LostFilmMonitoring.BLL/Commands) and register in `Program.cs` of the Azure Function project.
- New DAOs: define interface in [LostFilmMonitoring.DAO.Interfaces](LostFilmMonitoring.DAO.Interfaces) and implementation in [LostFilmMonitoring.DAO.Azure](LostFilmMonitoring.DAO.Azure).
- New Azure Functions: add to [LostFilmMonitoring.AzureFunction/Functions](LostFilmMonitoring.AzureFunction/Functions) and provide OpenAPI attributes.
- Frontend changes: modify [LostFilmMonitoring.Web](LostFilmMonitoring.Web).

Common code patterns Copilot should follow
- Commands implement `ICommand<TRequest, TResponse>` or `ICommand` and return `ValidationResult` on invalid input.
- Use `ModelBinder.Bind<T>(req)` in functions to deserialize request models.
- DAO classes should inherit from `BaseAzureTableStorageDao` when applicable.
- Use `Task.WhenAll` for parallelizable work; avoid `ConfigureAwait(false)` in functions.
- Use ActivitySource for tracing and `ILogger` for structured logs.

Build & test commands
- Build solution: `dotnet build LostFilmMonitoring.sln --configuration Release`
- Run tests: `dotnet test --configuration Release`
- Run/host functions locally (watch): use the `watch` task in workspace tasks or `func host start` in `LostFilmMonitoring.AzureFunction/bin/Debug/net8.0`.
- Pulumi preview/apply: `pulumi preview --stack dev` and `pulumi up --stack dev` inside `LostFilmMonitoring.AzureInfrastructure`.

Secrets & safety
- Do not hardcode secrets or credentials. Use Pulumi secrets (`pulumi config set --secret ...`) or environment variables provided by CI.
- Do not suggest committing keys, connection strings, or API keys to the repo.

CI/CD
- GitHub Actions workflows exist under `.github/workflows` and expect Pulumi to run before backend/frontend deploy jobs.
- Tests must be run and reported to test reporter steps in workflows.

PR guidance
- Keep PRs focused and small; update unit tests for new behavior.
- Add XML docs for new public APIs.
- Ensure StyleCop rules are followed and unit tests pass locally.

Helpful prompts examples (for user to give Copilot)
- "Create a new ICommand implementation `XCommand` that validates input, uses `IUserDao` and returns `XResponseModel` following project patterns."
- "Add a DAO interface `INewDao` and an Azure Table Storage implementation `AzureTableStorageNewDao` inheriting from `BaseAzureTableStorageDao`."
- "Generate an Azure Function `NewFunction` that accepts POST JSON, binds the model with `ModelBinder.Bind<T>`, calls a command, and returns serialized JSON response with OpenAPI attributes."

Files to inspect for more context
- [LostFilmMonitoring.BLL](LostFilmMonitoring.BLL) — command implementations and validators
- [LostFilmMonitoring.AzureFunction](LostFilmMonitoring.AzureFunction) — function entry points and DI
- [LostFilmMonitoring.AzureInfrastructure](LostFilmMonitoring.AzureInfrastructure) — Pulumi stacks and resource conventions

Maintenance notes for Copilot
- Prefer existing helpers and constants (e.g., `Constants`, `CommonSerializationOptions`).
- Reuse existing DTOs and models where possible to avoid duplication.
- When proposing infrastructure changes, prefer `Locals` naming conventions and Pulumi `config` usage for environment values.

---

This file is intentionally concise. For full project architecture and rules, see the repository's `.cursor` rules (project-architecture.mdc, infrastructure-deployment.mdc, csharp-conventions.mdc) in the `.cursor/rules` directory.
