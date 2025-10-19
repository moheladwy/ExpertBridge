# ExpertBridge

## Overview

ExpertBridge is an enterprise-grade social platform for expert professionals. It blends a professional network (profiles, posts, comments, tags, messaging, notifications) with a freelance marketplace (job postings, applications, offers, reviews) and AI-assisted features (content tagging, moderation, semantic search and recommendations).

The solution targets .NET 9 and follows Clean Architecture with clear separation of concerns, strong typing, async-first patterns, and rich observability. It runs locally via the .NET CLI, Docker (with pgvector for semantic embeddings), or as a composed distributed app using .NET Aspire.

## Architecture

-   Architectural style: Clean Architecture / Layered (Domain, Application, Infrastructure, Presentation), with cross-cutting extensions and background processing.
-   Composition/orchestration: .NET Aspire AppHost orchestrates API, Admin, Worker and dependencies (RabbitMQ, Redis, Seq, Ollama).
-   Persistence: PostgreSQL with pgvector extension for vector embeddings; EF Core 9 with retry policies, soft-delete, and global query filters.
-   Messaging: MassTransit over RabbitMQ for async pipelines and background processing.
-   Real-time: SignalR hub for notifications and messaging.
-   Caching: FusionCache with Redis as distributed cache backend.
-   Observability: OpenTelemetry (traces, metrics, logs) with Prometheus exporter and optional OTLP; Seq for structured logging.
-   Authentication & external services: Firebase Authentication, AWS S3 for media, Microsoft.Extensions.AI/Ollama for embeddings, Groq API for LLM tasks.

Dependency flow (high level):

-   Presentation: `ExpertBridge.Api` (Web API), `ExpertBridge.Admin` (Blazor Server)
-   Application: `ExpertBridge.Application` (domain services)
-   Domain/Core: `ExpertBridge.Core` (entities/DTOs/interfaces/queries)
-   Infrastructure: `ExpertBridge.Data` (EF Core), `ExpertBridge.Extensions` (cross-cutting), `ExpertBridge.GroqLibrary` (LLM integration), `ExpertBridge.Notifications` (SignalR)
-   Background: `ExpertBridge.Worker` (Quartz + MassTransit consumers)
-   Host: `ExpertBridge.Host` (Aspire AppHost)

## Project Structure

```
ExpertBridge.sln
├─ ExpertBridge.Api/                # ASP.NET Core Web API (controllers, middleware, swagger)
│  ├─ Extensions/                   # API wiring: EF, Firebase, caching, resilience, swagger, rate limiting
│  ├─ Middleware/                   # Global exception handling, auth gates
│  ├─ Program.cs                    # ASP.NET Core bootstrapping
│  └─ Dockerfile                    # API container image
├─ ExpertBridge.Admin/              # Blazor Server admin dashboard
│  ├─ Program.cs                    # Admin host + OpenTelemetry + caching + health checks
│  └─ Dockerfile
├─ ExpertBridge.Application/        # Application layer services (domain services, helpers, settings)
│  └─ ExpertBridge.Application.csproj
├─ ExpertBridge.Core/               # Domain entities, interfaces, DTOs, queries, exceptions
│  └─ ExpertBridge.Core.csproj
├─ ExpertBridge.Data/               # EF Core DbContext, migrations, soft-delete interceptor, DB DI
│  ├─ DatabaseContexts/ExpertBridgeDbContext.cs
│  ├─ Interceptors/
│  ├─ Migrations/
│  ├─ Extensions.cs                 # services.AddDatabase(...) (Npgsql + pgvector + retry)
│  └─ ExpertBridge.Data.csproj
├─ ExpertBridge.Extensions/         # Cross-cutting infra: caching, logging, OpenTelemetry, AWS, Firebase, broker, CORS, resilience
│  ├─ Caching/                      # FusionCache + Redis Hybrid cache
│  ├─ OpenTelemetry/                # Tracing + metrics + HttpClient defaults
│  ├─ MessageBroker/                # MassTransit + RabbitMQ registration
│  ├─ AWS/, Firebase/, HealthChecks/, Logging/, Embeddings/, CORS/, Resilience/
│  └─ ExpertBridge.Extensions.csproj
├─ ExpertBridge.Notifications/      # SignalR hub and notification abstractions
│  ├─ NotificationsHub.cs           # /api/notificationsHub
│  └─ ExpertBridge.Notifications.csproj
├─ ExpertBridge.GroqLibrary/        # Groq API clients, providers, settings
│  ├─ Clients/, Providers/, Settings/
│  └─ ExpertBridge.GroqLibrary.csproj
├─ ExpertBridge.Worker/             # Background worker: Quartz + MassTransit consumers
│  ├─ Consumers/                    # Post tagging, moderation, embeddings, interests processing
│  ├─ PeriodicJobs/                 # Quartz jobs
│  ├─ QuartzDatabase/               # Quartz persistent store setup
│  ├─ Program.cs                    # Host builder + migrations + services
│  └─ Dockerfile
├─ ExpertBridge.Host/               # .NET Aspire AppHost: orchestrates infra + projects
│  ├─ Program.cs                    # Adds RabbitMQ, Redis, Seq, Ollama; wires API/Admin/Worker
│  ├─ manifest.json                 # Aspire manifest
│  └─ aspirate-state.json
├─ ExpertBridge.Contract/           # Reserved for shared contracts (currently minimal)
├─ compose.yaml                     # Docker compose for API/Admin/Worker images (builds locally)
├─ postgresql-compose.yaml          # Standalone PostgreSQL (pgvector) + pgAdmin
├─ Directory.Packages.props         # Central NuGet versions
└─ Directory.Build.props            # Build/unified settings (net9, nullable, analysis)
```

### Project roles and relationships

-   ExpertBridge.Api

    -   Purpose: Public REST API, authentication, rate limiting, health endpoints, Swagger, SignalR hub mapping.
    -   Depends on: Application, Core, Data, Extensions, GroqLibrary, Notifications.
    -   Notables: Auto EF migrations on startup in Development; Prometheus endpoint; CORS policies; Serilog request logging.

-   ExpertBridge.Admin

    -   Purpose: Admin dashboard (Blazor Server) for management and operations.
    -   Depends on: Core, Data, Extensions.
    -   Notables: Health checks, OpenTelemetry, FusionCache; Radzen components.

-   ExpertBridge.Application

    -   Purpose: Domain/application services and business workflows.
    -   Depends on: Core, Data, Extensions, GroqLibrary, Notifications.
    -   Notables: Integration types and output formats for LLM workflows; validators; service composition.

-   ExpertBridge.Core

    -   Purpose: Domain model (entities, interfaces), DTOs (requests/responses), queries, exceptions.
    -   Notables: Soft-delete marker interface, timestamped entities, query extensions for projections.

-   ExpertBridge.Data

    -   Purpose: Persistence layer via EF Core (Npgsql + pgvector).
    -   Notables: `ExpertBridgeDbContext` with global soft-delete filter, timestamp stamping; retry policies; soft-delete interceptor; Postgres "vector" extension.

-   ExpertBridge.Extensions

    -   Purpose: Cross-cutting infrastructure (caching, logging, OpenTelemetry, Firebase, AWS S3, MassTransit/RabbitMQ, CORS, resilience, health checks, embeddings wiring).
    -   Notables: FusionCache with Redis as distributed cache; MassTransit registration; Prometheus exporter; HttpClient resilience and service discovery.

-   ExpertBridge.Notifications

    -   Purpose: SignalR hub and notification abstractions.
    -   Notables: Hub endpoint `/api/notificationsHub`, CORS integration for SignalR clients.

-   ExpertBridge.GroqLibrary

    -   Purpose: Groq client abstractions, settings, and providers for AI tasks (tagging, moderation, etc.).

-   ExpertBridge.Worker

    -   Purpose: Background processing host (Quartz + MassTransit consumers) for asynchronous pipelines.
    -   Depends on: Application, Data, Extensions.
    -   Notables: Quartz persistent store (PostgreSQL), consumers for tagging, embeddings, moderation; auto EF migrations on startup.

-   ExpertBridge.Host (Aspire)

    -   Purpose: Local orchestration of infrastructure and projects (RabbitMQ, Redis, Seq, Ollama + API/Admin/Worker).
    -   Notables: Data volumes for persistence; optional OTLP exporter; GPU support for Ollama; publishes connection strings into projects.

-   ExpertBridge.Contract
    -   Purpose: Placeholder for shared contracts (currently minimal); can hold shared abstractions for events/clients.

## Technologies Used

-   Platform: .NET 9, ASP.NET Core, Blazor Server
-   Data & ORM: PostgreSQL (pgvector/pg17), Entity Framework Core 9, Npgsql
-   Messaging & Jobs: MassTransit (RabbitMQ), Quartz (persistent store in PostgreSQL)
-   Real-time: SignalR
-   Caching: FusionCache + Redis (HybridCache)
-   AI/LLM: Microsoft.Extensions.AI, Ollama embeddings, Groq API
-   Auth & Media: Firebase Authentication (JWT), AWS S3 for storage
-   Observability: OpenTelemetry (traces/metrics/logs), Prometheus exporter, Seq sink
-   Health & Resilience: HealthChecks (Redis/Postgres), Microsoft Resilience, Service Discovery, Rate Limiting
-   Tooling: Swashbuckle/Swagger, Refit, FluentValidation, Bogus (seed data)
-   Orchestration: .NET Aspire AppHost
-   Containers: Dockerfiles per project; docker compose files for app and PostgreSQL

## Getting Started

### Prerequisites

-   .NET SDK 9.0+
-   Docker and Docker Compose (for containers)
-   Optional local services if not using Aspire/Docker:
    -   PostgreSQL 17 with `pgvector` extension enabled
    -   Redis
    -   RabbitMQ
    -   Seq (optional, for logs)
    -   Ollama (optional, for local embeddings)

### Configuration

Configuration is read from `appsettings.json` / `appsettings.Development.json` and environment variables. Key sections include:

-   ConnectionStrings
    -   `Postgresql`: PostgreSQL connection string
    -   `Redis`: Redis connection string
    -   `Seq`: Seq ingestion endpoint
    -   `QuartzDatabase`: PostgreSQL connection for Quartz job store
-   MessageBroker
    -   `MessageBrokerCredentials`: `{ Host, Username, Password }` for RabbitMQ
-   Firebase
    -   Credentials and auth settings (see `ExpertBridge.Api/FirebaseOAuthCredentialsExpertBridge.json` and related settings)
-   AWS
    -   `AwsSettings` section for S3 access
-   AI
    -   `AiSettings`, `GroqSettings`, and Ollama connection
-   Rate limiting
    -   `ExpertBridgeRateLimitSettings`

Use user-secrets or environment variables for secrets in development.

### Run everything via .NET Aspire (recommended for local)

Run the AppHost to start the full distributed app (RabbitMQ, Redis, Seq, Ollama) and the three projects:

```bash
# from repository root
dotnet run --project ExpertBridge.Host/ExpertBridge.Host.csproj --profile http
```

The AppHost will:

-   Start containers for RabbitMQ, Redis, Seq, and Ollama (with model `snowflake-arctic-embed2:latest`)
-   Start API, Admin, and Worker projects and wire their connection strings

### Run projects individually (manual)

1. Start infrastructure (option A: Docker Compose for PostgreSQL + pgAdmin)

```bash
docker compose -f postgresql-compose.yaml up -d
```

-   Postgres: `localhost:5432` (user/password: `root`/`root`)
-   pgAdmin: http://localhost:5050 (default creds in `.postgres.env`)

2. Configure environment variables (examples)

```bash
# zsh examples
export ConnectionStrings__Postgresql="Host=localhost;Port=5432;Database=expertbridge;Username=root;Password=root"
export ConnectionStrings__Redis="localhost:6379"
export ConnectionStrings__Seq="http://localhost:5341"
export ConnectionStrings__QuartzDatabase="Host=localhost;Port=5432;Database=quartz;Username=root;Password=root"
export MessageBrokerCredentials__Host="amqp://localhost"
export MessageBrokerCredentials__Username="guest"
export MessageBrokerCredentials__Password="guest"
```

3. Restore, build and run

```bash
dotnet restore
dotnet build
dotnet run
```

-   API Swagger UI is enabled in Development and exposes health endpoints:
    -   Swagger: `/swagger`
    -   Health: `/health`, Liveness: `/alive`
    -   Prometheus scraping endpoint is mapped
-   SignalR hub: `/api/notificationsHub`

## Database and EF Core

The API and Worker apply EF Core migrations automatically at startup in Development. To manage migrations manually:

Create a migration (example):

```bash
# Add a migration in the Data project using the API as startup
dotnet ef migrations add InitialCreate \
  --project ExpertBridge.Data/ExpertBridge.Data.csproj \
  --startup-project ExpertBridge.Api/ExpertBridge.Api.csproj \
  --context ExpertBridge.Data.DatabaseContexts.ExpertBridgeDbContext
```

Update the database:

```bash
dotnet ef database update \
  --project ExpertBridge.Data/ExpertBridge.Data.csproj \
  --startup-project ExpertBridge.Api/ExpertBridge.Api.csproj \
  --context ExpertBridge.Data.DatabaseContexts.ExpertBridgeDbContext
```

Notes:

-   Ensure `ConnectionStrings:Postgresql` is set (environment or appsettings) before running migrations.
-   Quartz uses its own connection string (`ConnectionStrings:QuartzDatabase`) and is configured in the Worker.
-   The DbContext enables the PostgreSQL `vector` extension and applies a global query filter for soft-deleted entities.

## Docker

You can build and run the application services in containers.

Build images:

```bash
# From repository root
# API
docker build -f ExpertBridge.Api/Dockerfile -t expertbridge-api:dev .

# Admin
docker build -f ExpertBridge.Admin/Dockerfile -t expertbridge-admin:dev .

# Worker
docker build -f ExpertBridge.Worker/Dockerfile -t expertbridge-worker:dev .
```

Run via compose (application services):

```bash
# This builds and runs API/Admin/Worker images defined in compose.yaml
docker compose up -d --build
```

-   API exposed by default at host port 5027 (mapped to container 8080). Adjust `compose.yaml` as needed.
-   Ensure your environment variables and connection strings are supplied to the containers (e.g., `-e`, `.env`, or compose `environment:` blocks) for Postgres/Redis/RabbitMQ/Seq/Ollama.

Run PostgreSQL + pgAdmin separately:

```bash
docker compose -f postgresql-compose.yaml up -d
```

## Observability

-   OpenTelemetry is enabled for logging, metrics, and tracing.
-   Prometheus scraping endpoint is mapped; set `OTEL_EXPORTER_OTLP_ENDPOINT` to export OTLP if needed.
-   Serilog writes to console, file, and Seq (configure `ConnectionStrings:Seq`).

## Testing

A test toolchain is configured (xUnit, Microsoft.NET.Test.Sdk, coverlet). If/when test projects are added, run:

```bash
dotnet test
```

## Contributing

-   Follow Clean Architecture guidelines and existing naming conventions.
-   Prefer async patterns, DI, and small focused services.
-   Add or update XML documentation on public APIs and keep cross-cutting logic in `ExpertBridge.Extensions`.

## License

Source files include an MIT license header. If using this repository publicly, consider adding a top-level `LICENSE` file with the full MIT text.

## Notes

-   .NET Aspire Host is the easiest way to run all components locally with dependencies.
-   EF Core migrations are applied automatically on API/Worker startup in Development, but commands above are provided for explicit control.
-   Docker `compose.yaml` references Dockerfiles for API/Admin/Worker; ensure image environment variables are set for external services (PostgreSQL/Redis/RabbitMQ/Seq/Ollama/Firebase/AWS).
