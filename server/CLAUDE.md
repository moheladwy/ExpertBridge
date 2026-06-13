# CLAUDE.md — server/

.NET 10 solution (`ExpertBridge.slnx`), Clean Architecture + DDD. SDK pinned in `global.json`.
Build settings in `Directory.Build.props` (nullable + implicit usings on, analyzers `All`).
Package versions are centrally managed in `Directory.Packages.props`.

## Commands
```bash
dotnet build      # builds ExpertBridge.slnx (XML solution format, not .sln)
dotnet test       # ExpertBridge.Tests.Unit — xUnit + Shouldly, AAA pattern
dotnet format
# Local dev (Aspire spins up Postgres/Redis/RabbitMQ/Ollama/Seq):
dotnet run --project ExpertBridge.Host/ExpertBridge.Host.csproj --launch-profile http   # dashboard :15888
dotnet watch --project ExpertBridge.Api    # API only; requires infra already running
```

EF Core migrations (auto-applied on startup in Development):
```bash
dotnet ef migrations add <Name> --project ExpertBridge.Data --startup-project ExpertBridge.Api
dotnet ef database update      --project ExpertBridge.Data --startup-project ExpertBridge.Api
```

## Projects
- `ExpertBridge.Host` — .NET Aspire AppHost (local orchestration; entry point for dev).
- `ExpertBridge.Api` — ASP.NET Core REST API. Firebase JWT auth, Scalar docs `/scalar/v1`, health `/health` + `/alive`.
- `ExpertBridge.Admin` — Blazor Server admin (Radzen UI, ASP.NET Core Identity, separate DbContext).
- `ExpertBridge.Worker` — MassTransit consumers + Quartz jobs; runs the AI pipeline (tagging, NSFW, embeddings).
- `ExpertBridge.Application` — application services. `ExpertBridge.Core` — domain entities/DTOs/interfaces.
- `ExpertBridge.Data` — EF Core + Npgsql + pgvector (migrations live here).
- `ExpertBridge.Extensions` — cross-cutting wiring (caching, OpenTelemetry, AWS, Firebase, broker).
- `ExpertBridge.Contract`, `ExpertBridge.Notifications` (SignalR), `ExpertBridge.Tests.Unit`.

## Conventions
- Dependency flow: Presentation → Application → Core ← Infrastructure (Data/Extensions). Core depends on nothing.
- **Controllers return DTOs, never EF entities.** Services are stateless, injected via DI, async for all I/O.
- Soft-delete via `ISoftDeletable` + global query filters; timestamps set by interceptor.
- Offload long/AI work by publishing domain events → MassTransit/RabbitMQ → `ExpertBridge.Worker` consumers.
- Naming: `{Domain}Service`, `{Resource}Controller`, `{Action}{Resource}Request`, `{Resource}Response`, `{Action}Consumer`.
- Config precedence: env vars > user secrets (dev) > `appsettings.{Env}.json` > `appsettings.json`.

## More
`README.md` and `.github/instructions/ExpertBridgeServerIntructions.instructions.md` (full guidelines).
