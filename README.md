# ExpertBridge

An enterprise-grade professional networking and knowledge-sharing platform that combines social networking features with a freelance marketplace and advanced AI-powered capabilities. ExpertBridge enables expert professionals to showcase their expertise, connect with opportunities, and engage with a community while leveraging AI for content moderation, semantic search, and intelligent recommendations.

## Tech Stack Overview

-   **Backend:** .NET 10.0 (Clean Architecture, DDD, EF Core 10, MassTransit/RabbitMQ, SignalR, OpenTelemetry)
-   **Frontend:** React 19 + TypeScript 5.6 (Vite 6, Redux Toolkit + RTK Query, Tailwind CSS v4, shadcn/ui)
-   **Infrastructure:** PostgreSQL 18 + pgvector, Redis, RabbitMQ, Seq, Prometheus, Grafana, Ollama, Nginx
-   **AI/ML:** Microsoft.Extensions.AI + Ollama (embeddings), Groq SDK (LLM tasks)
-   **Auth:** Firebase Auth v11 (frontend), Firebase Admin SDK (backend JWT validation)
-   **Storage:** AWS S3 for media files
-   **Observability:** Serilog, OpenTelemetry, Prometheus, Seq

## Repository structure

Top-level folders you’ll use the most:

-   `server/` — .NET solution (API, Admin, Worker, Host) and compose files for local dev
-   `client/` — React SPA frontend
-   `deployment/` — Docker Compose stacks for production-like deployments (API/Admin/Worker, PostgreSQL/pgAdmin, RabbitMQ, Prometheus, Grafana, Nginx Proxy Manager, Ollama)
-   `docs/` — Documentation links and diagrams (PlantUML/sequence diagrams in subfolders)
-   `GP_Documentation/` — Class diagrams, ERDs, sequence diagrams, system component diagram

Example detailed back-end structure (see `server/README.md` for more):

```
ExpertBridge.sln
├─ ExpertBridge.Api/          # ASP.NET Core Web API (Swagger, health, SignalR)
├─ ExpertBridge.Admin/        # Blazor Server admin dashboard
├─ ExpertBridge.Application/  # Application layer services
├─ ExpertBridge.Core/         # Domain entities, DTOs, interfaces
├─ ExpertBridge.Data/         # EF Core + Npgsql + pgvector
├─ ExpertBridge.Extensions/   # Caching, logging, OpenTelemetry, AWS, Firebase, broker, etc.
├─ ExpertBridge.Notifications/# SignalR hub / abstractions
├─ ExpertBridge.GroqLibrary/  # Groq API clients
├─ ExpertBridge.Worker/       # Background jobs/consumers (Quartz + MassTransit)
└─ ExpertBridge.Host/         # .NET Aspire AppHost (local orchestration)
```

## Architecture overview

-   Clean Architecture with clear separation: Presentation → Application → Core → Infrastructure.
-   Composition via .NET Aspire to run API, Admin, Worker, and dependencies locally.
-   Persistence: PostgreSQL 18 + pgvector; EF Core 9 with soft-delete and retry policies.
-   Messaging: MassTransit on RabbitMQ; Background: Quartz persistent jobs.
-   Real-time: SignalR for notifications/messaging.
-   Caching: FusionCache + Redis.
-   Observability: OpenTelemetry (traces, metrics, logs), Prometheus exporter, Seq sink.
-   AI: Microsoft.Extensions.AI + Ollama for embeddings; Groq API for LLM tasks.

System component diagram and domain models are available under:

-   `GP_Documentation/SystemComponent/system-component.txt` (Mermaid)
-   `GP_Documentation/Class Diagram/Overall.txt` (PlantUML) and domain-focused diagrams
-   Additional sequence and architecture diagrams under `docs/`

## Quick Start

### Prerequisites

-   **Docker & Docker Compose** (for Option 1)
-   **.NET SDK 10.0** ([download](https://dotnet.microsoft.com/download/dotnet/10.0)) (for Option 2)
-   **Node.js 20+** (for Option 2)
-   **Git**

### Option 1: Docker Compose (Recommended)

Run the entire stack (frontend, backend, and all infrastructure) with a single command.

**Steps:**

1. **Set up environment variables:**

    - Copy `.env.example` to `.env` in the repository root
    - Fill in all required values (Firebase, AWS S3, Groq API keys, database credentials, etc.)

2. **Run the stack:**

    ```bash
    # From repository root
    docker compose up -d
    ```

3. **Access the application:**
    - **Frontend**: http://localhost:80
    - **API**: http://localhost:5027
    - **Admin Dashboard**: http://localhost:5028
    - **Aspire Dashboard**: http://localhost:18888
    - **pgAdmin**: http://localhost:5050
    - **RabbitMQ Management**: http://localhost:15672

**Manage the stack:**

```bash
# Stop all services
docker compose down

# View logs
docker compose logs -f [service-name]
# Examples: api, client, worker, admin

# Rebuild after changes
docker compose up -d --build
```

### Option 2: Manual Development Setup

Run backend and frontend separately for active development with hot reload.

**Backend Setup:**

Follow the detailed instructions in [`server/README.md`](server/README.md) for:

-   Infrastructure setup (PostgreSQL, Redis, RabbitMQ, Ollama)
-   Environment configuration and user secrets
-   Running with .NET Aspire or individual projects

**Frontend Setup:**

Follow the detailed instructions in [`client/README.md`](client/README.md) for:

-   Node.js dependency installation
-   Firebase environment variables configuration
-   Vite development server setup

**Quick commands:**

```bash
# Backend (with Aspire - recommended)
cd server
dotnet run --project ExpertBridge.Host/ExpertBridge.Host.csproj --launch-profile http

# Frontend (separate terminal)
cd client
npm install
npm run dev
```

**Key endpoints (Option 2):**

-   **Frontend**: http://localhost:5173 (Vite dev server)
-   **API**: http://localhost:5027
-   **Admin Dashboard**: http://localhost:5028
-   **Aspire Dashboard**: http://localhost:15888

## Frontend Details

### Technology Stack

-   **Framework**: React 19.0.0 with React Compiler (Babel plugin beta)
-   **Language**: TypeScript 5.6.2 with strict mode
-   **Build Tool**: Vite 6 with React SWC plugin
-   **Routing**: React Router v7 with lazy loading and chunk retry
-   **State Management**: Redux Toolkit 2.5.1 + RTK Query
-   **Authentication**: Firebase Auth v11.2.0 with centralized state management
-   **Styling**: Tailwind CSS v4.1.16 + shadcn/ui components (New York style)
-   **UI Components**: Radix UI primitives, Lucide icons
-   **Forms**: react-hook-form 7.66.0 + Zod validation
-   **Notifications**: react-hot-toast for user feedback
-   **Theme**: next-themes with dark/light mode support

### Key Features

**Centralized Authentication:**

-   Single `AuthStateManager` eliminates 90% of auth overhead
-   `TokenManager` caches tokens with 1-minute expiry (70% faster than direct calls)
-   Never create new Firebase listeners - always use provided hooks

**Performance Optimizations:**

-   Strategic code splitting: firebase, redux, ui, fonts, vendor chunks
-   Custom `lazyWithRetry()` for route components (3 retry attempts)
-   Entity adapters for normalized state (posts, comments)
-   5-minute RTK Query cache with tag-based invalidation

**API Integration:**

-   Base `apiSlice` with automatic Firebase token injection
-   Retry logic (3 attempts, smart failure handling)
-   Comprehensive tag system for cache invalidation
-   Health check on startup with user-friendly error pages

**Development Experience:**

-   Hot module replacement with Vite
-   TypeScript strict mode with typed Redux hooks
-   ESLint with React hooks and TypeScript rules
-   Path aliases: `@/` → `src/`, `@views/` → `src/views/`

### Running the Frontend

```bash
cd client

# Install dependencies
npm install

# Development server (http://localhost:5173)
npm run dev

# Production build
npm run build

# Preview production build (http://localhost:4173)
npm run preview

# Lint code
npm run lint
```

### Docker Deployment

Multi-stage Dockerfile with Nginx:

```bash
cd client
docker build -t expertbridge-client .
docker run -p 3000:80 expertbridge-client
```

**Note:** All `VITE_*` environment variables must be provided at build time as `ARG` in Dockerfile.

### Environment Configuration

All variables must be prefixed with `VITE_` for Vite to embed them:

```env
# API Configuration
VITE_SERVER_URL=http://localhost:5027
VITE_INDEXED_DB_NAME="expertbridge.duckdns.org"
VITE_INDEXED_DB_VERSION=1

# Firebase (required)
VITE_API_KEY="your-api-key"
VITE_AUTH_DOMAIN="your-project.firebaseapp.com"
VITE_PROJECT_ID="your-project"
VITE_STORAGE_BUCKET="your-project.appspot.com"
VITE_MESSAGING_SENDER_ID="123456789"
VITE_APP_ID="1:123456789:web:abcdef"
VITE_MEASUREMENT_ID="G-XXXXXXXXX"

# Feature Flags
VITE_ENABLE_DEBUG_LOGGING=true
VITE_API_TIMEOUT=60000
VITE_MAX_API_RETRIES=3
VITE_ENABLE_TOKEN_MONITOR=true
VITE_ENABLE_AUTH_MONITOR=true
VITE_ENABLE_PERFORMANCE_MONITORING=true
VITE_ENABLE_REACT_DEVTOOLS=true
VITE_ENABLE_REDUX_DEVTOOLS=true
```

**Important:** Variables are embedded at build time, not runtime. Use `.env.example` as a template.

More details: `client/README.md`

## Backend Details

### Core Projects

**ExpertBridge.Api** (REST API)

-   ASP.NET Core 10.0 with controllers
-   Firebase JWT authentication
-   Scalar/OpenAPI documentation at `/scalar/v1`
-   Health checks: `/health` (liveness: `/alive`)
-   Response caching with profiles
-   Global exception middleware
-   Automatic migrations in Development

**ExpertBridge.Admin** (Blazor Server)

-   Admin dashboard with Radzen UI
-   ASP.NET Core Identity authentication
-   Content moderation interface
-   User management and analytics
-   Separate admin DbContext

**ExpertBridge.Worker** (Background Processing)

-   MassTransit message consumers
-   Quartz.NET scheduled jobs with PostgreSQL persistence
-   AI processing pipeline:
    -   `PostProcessingPipelineConsumer` - Orchestrates multi-step processing
    -   `PostTaggingConsumer` - AI-powered content tagging
    -   `InappropriateContentDetectionConsumer` - NSFW detection
    -   `PostEmbeddingConsumer` - Vector embeddings generation
    -   `NotificationSendingPipelineConsumer` - Notification delivery
    -   `UserInterestsUpdatedConsumer` - Interest tracking

**ExpertBridge.Host** (.NET Aspire)

-   Orchestrates all services for local development
-   Infrastructure management (PostgreSQL, Redis, RabbitMQ, Seq, Ollama)
-   Service discovery and health monitoring
-   Aspire dashboard at http://localhost:15888

### Key Technologies

**Data Access:**

-   Entity Framework Core 10.0 with Npgsql
-   PostgreSQL 18 with pgvector extension
-   Soft-delete interceptor with global query filters
-   Timestamp management interceptor
-   Z.EntityFramework.Extensions for bulk operations

**Messaging:**

-   MassTransit on RabbitMQ
-   Event-driven architecture with domain events
-   Automatic consumer registration
-   Retry policies and error handling

**Caching:**

-   FusionCache with hybrid strategy (memory + Redis)
-   Distributed caching for multi-instance deployments
-   Adaptive cache expiration
-   Fail-safe with stale data fallback

**Observability:**

-   OpenTelemetry (traces, metrics, logs)
-   Serilog with structured logging
-   Seq for log aggregation (ports 4002 UI, 5341 ingestion)
-   Prometheus metrics export
-   Health checks for all dependencies

**AI/ML:**

-   Microsoft.Extensions.AI abstractions
-   Ollama for embeddings (model: `snowflake-arctic-embed2:latest`)
-   Groq SDK for LLM tasks (content moderation, categorization)
-   Pgvector for semantic search

**Resilience:**

-   Polly policies (retry, circuit breaker, timeout)
-   Microsoft.Extensions.Resilience pipelines
-   Automatic retry on transient failures

### Database Migrations

```bash
# Navigate to server directory
cd server

# Add migration
dotnet ef migrations add MigrationName \
  --project ExpertBridge.Data \
  --startup-project ExpertBridge.Api

# Update database
dotnet ef database update \
  --project ExpertBridge.Data \
  --startup-project ExpertBridge.Api

# Remove last migration
dotnet ef migrations remove \
  --project ExpertBridge.Data \
  --startup-project ExpertBridge.Api
```

**Note:** Migrations are automatically applied in Development environment on startup.

### Running Individual Services

```bash
cd server

# Run API only (requires PostgreSQL, Redis, RabbitMQ)
dotnet run --project ExpertBridge.Api

# Run Admin dashboard
dotnet run --project ExpertBridge.Admin

# Run Worker service
dotnet run --project ExpertBridge.Worker

# Run with watch mode (hot reload)
dotnet watch --project ExpertBridge.Api
```

### Configuration

**Backend uses hierarchical configuration:**

1. Environment variables (highest priority)
2. User secrets (development only)
3. `appsettings.{Environment}.json`
4. `appsettings.json` (lowest priority)

**Key configuration sections:**

-   `ConnectionStrings`: PostgreSQL connection
-   `Firebase`: Admin SDK credentials
-   `AWS`: S3 bucket configuration
-   `RabbitMQ`: Message broker settings
-   `Redis`: Cache configuration
-   `NsfwThresholds`: AI moderation thresholds
-   `Ollama`: Embedding service configuration

**Development secrets:**

```bash
# Set user secrets (ExpertBridge.Api or ExpertBridge.Data)
cd ExpertBridge.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=expertbridge;Username=postgres;Password=your_password"
```

More details: `server/README.md`

## Observability & Monitoring

### Logging

**Serilog** with structured logging to multiple sinks:

-   Console (colored output with timestamps)
-   File (rolling daily logs in `Logs/` directory)
-   Seq (centralized log aggregation)
-   OpenTelemetry (distributed tracing integration)

**Log Enrichment:**

-   Environment name (Development/Production)
-   Thread ID and Process ID
-   Correlation IDs for distributed tracing
-   User context (when authenticated)

**Seq Dashboard:**

-   UI: http://localhost:4002
-   Ingestion: http://localhost:5341
-   Query and filter structured logs
-   Alert on error patterns

### Tracing

**OpenTelemetry** distributed tracing:

-   Traces HTTP requests across services
-   Database query tracing (EF Core)
-   Message broker operations (MassTransit)
-   External API calls (S3, Firebase, Groq)
-   SignalR real-time connections

**Trace Exporters:**

-   OTLP (OpenTelemetry Protocol) for standard collectors
-   Console (development)
-   Aspire dashboard integration

### Metrics

**Prometheus** metrics export:

-   ASP.NET Core metrics (request duration, error rates)
-   Runtime metrics (GC, memory, threads)
-   Database connection pool metrics
-   Custom business metrics (posts created, jobs posted, etc.)
-   Message broker metrics (messages processed, queue depth)

**Prometheus Endpoint:** http://localhost:5027/metrics (API)

**Grafana Dashboards:**

-   Available in `deployment/grafana/`
-   Pre-configured dashboards for:
    -   Application performance monitoring
    -   Infrastructure health
    -   Business KPIs

### Health Checks

**Comprehensive health monitoring:**

-   PostgreSQL connectivity
-   Redis connectivity
-   RabbitMQ connectivity
-   Disk space
-   Memory usage

**Health Endpoints:**

-   `/health` - Full health report
-   `/alive` - Simple liveness check (fast response)

**Health Check UI:**

-   Integrated in Aspire dashboard
-   Shows dependency status in real-time

### Aspire Dashboard

**Development orchestration dashboard:**

-   URL: http://localhost:15888
-   Features:
    -   Service status and logs
    -   Resource monitoring (CPU, memory)
    -   Distributed tracing visualization
    -   Metrics charts
    -   Environment variables viewer

### Production Monitoring

**Deployment includes:**

-   Prometheus compose: `deployment/prometheus/prometheus-compose.yml`
-   Grafana compose: `deployment/grafana/grafana-compose.yml`
-   Seq compose: `deployment/api/docker-compose.yml` (with Seq service)

**Setup:**

```bash
# Start Prometheus
cd deployment/prometheus
docker compose -f prometheus-compose.yml up -d

# Start Grafana
cd deployment/grafana
docker compose -f grafana-compose.yml up -d

# Access Grafana at http://localhost:3000
# Default credentials: admin/admin
```

## Documentation

### Project Documentation

-   **Root README**: Overview and quick start (this file)
-   **Server README**: `server/README.md` - Backend architecture, setup, and development
-   **Client README**: `client/README.md` - Frontend architecture, setup, and deployment
-   **API Documentation**: Scalar at `/scalar/v1` (when API is running)

### Architecture Documentation

**System Architecture:**

-   **System Component Diagram**: `GP_Documentation/SystemComponent/system-component.txt` (Mermaid)
-   **Detailed Architecture**: `docs/ArchitectureDiagrams/DetailedSysArchDiagram.puml` (PlantUML)

**Domain Models:**

-   **Overall Class Diagram**: `GP_Documentation/Class Diagram/Overall.txt` (PlantUML)
-   Domain-specific diagrams in `GP_Documentation/Class Diagram/`
-   **ERD**: `GP_Documentation/ERD/` (Entity Relationship Diagrams)

**Sequence Diagrams:**

Located in `docs/SequenceDiagrams/` and `GP_Documentation/SequenceDiagrams/`:

-   User Registration and Authentication
-   Content Creation and Post Interaction
-   Content Moderation Workflow
-   Content Sharing
-   Job Hiring and Payment Workflow
-   Contractor Workflow
-   Community Engagement and Voting
-   Dispute Resolution
-   Search and Discovery
-   Cross-Platform Experience

**Use Case Diagrams:**

Located in `docs/UseCaseDiagrams/`:

-   Authentication
-   Content Management
-   Job Management
-   Contractor Features
-   AI Features
-   Moderation

### Frontend-Specific Documentation

**Located in `client/docs/`:**

-   `API_HEALTH_CHECK.md` - Health check system details
-   `CENTRALIZED_AUTH_GUIDE.md` - Authentication architecture
-   `HEALTH_CHECK_QUICK_START.md` - Quick setup guide

### AI Assistant Instructions

**For AI code assistants (GitHub Copilot, Cursor, etc.):**

-   **General Instructions**: `.github/instructions/expertbridge.instructions.md`
-   **Server Instructions**: `server/.github/instructions/ExpertBridgeServerIntructions.instructions.md`
-   **Client Instructions**: `client/.github/instructions/client.instructions.md`

These files provide comprehensive coding guidelines, architectural patterns, and best practices for the project.

## Development Workflow

### Code Quality Standards

**Backend (.NET):**

```bash
cd server

# Build solution
dotnet build

# Run tests
dotnet test

# Format code
dotnet format

# Run with hot reload
dotnet watch --project ExpertBridge.Api
```

**Frontend (React):**

```bash
cd client

# Install dependencies
npm install

# Development server with hot reload
npm run dev

# Lint code
npm run lint

# Build for production
npm run build

# Preview production build
npm run preview
```

### Architectural Guidelines

**Backend:**

-   Follow Clean Architecture layers (Domain → Application → Infrastructure → Presentation)
-   Use Domain-Driven Design patterns (aggregate roots, value objects)
-   Always use async/await for I/O operations
-   Apply marker interfaces for cross-cutting concerns (`ISoftDeletable`, `ISafeContent`)
-   Services are stateless and injected via DI
-   Return DTOs from controllers, never domain entities
-   Publish domain events for async processing
-   Document all public APIs with XML comments

**Frontend:**

-   Follow feature-first structure under `src/features/`
-   Always use RTK Query for API calls (no direct `fetch`)
-   **Never create new Firebase auth listeners** - use `useCurrentUser()` hook
-   Use `tokenManager.getToken()` for cached tokens (70% faster)
-   Extend base `apiSlice` with `injectEndpoints()` pattern
-   Define `providesTags` and `invalidatesTags` for cache management
-   Use `cn()` utility for className merging
-   Wrap routes in `<ErrorBoundary>`
-   Use typed Redux hooks (`useAppSelector`, `useAppDispatch`)

### Naming Conventions

**Backend:**

-   Entities: Singular PascalCase (`Post`, `Comment`, `User`)
-   Services: `{Domain}Service` (`PostService`, `TaggingService`)
-   Controllers: `{Resource}Controller` (`PostsController`)
-   Requests: `{Action}{Resource}Request` (`CreatePostRequest`)
-   Responses: `{Resource}Response` (`PostResponse`)
-   Consumers: `{Action}Consumer` (`PostTaggingConsumer`)

**Frontend:**

-   Components: PascalCase (`PostCard`, `AuthButtons`)
-   Hooks: camelCase with `use` prefix (`useAuthCheck`, `useRefetchOnLogin`)
-   Slices: camelCase with feature (`postsSlice.ts`, `authSlice.ts`)
-   Types: PascalCase (`Post`, `PostResponse`)
-   Files: Match export name

### Testing

**Backend:**

-   Unit tests: Test business logic in isolation
-   Integration tests: Use `WebApplicationFactory<Program>`
-   Follow AAA pattern (Arrange, Act, Assert)
-   Use Shouldly for assertions
-   Mock external dependencies

**Frontend:**

-   Testing framework to be added (Vitest/Jest recommended)
-   ESLint configured for code quality

### Git Workflow

1. Create feature branch from `main`
2. Follow naming conventions
3. Run linters before committing
4. Ensure all tests pass
5. Open PR for review

### Code Review Checklist

-   [ ] Naming conventions followed
-   [ ] Type safety enforced
-   [ ] Error handling implemented
-   [ ] Validation applied
-   [ ] Logging added
-   [ ] Tests written/updated
-   [ ] Documentation added
-   [ ] No secrets in code
-   [ ] Linting passes
-   [ ] Build succeeds

## Deployment

### Docker Deployment

**Full Stack:**

```bash
cd server
docker compose -f compose.yaml up --build
```

**Frontend Only:**

```bash
cd client
docker build -t expertbridge-client .
docker run -p 3000:80 expertbridge-client
```

### Production Considerations

**Backend:**

-   Use environment variables for all secrets
-   Enable HTTPS with valid certificates
-   Configure rate limiting
-   Set up log aggregation (Seq)
-   Monitor with Prometheus + Grafana
-   Configure health checks for orchestrator
-   Use Redis for distributed caching
-   Scale Worker instances for high load

**Frontend:**

-   Build with production environment variables
-   Serve via Nginx with gzip compression
-   Configure CDN for static assets
-   Enable HTTPS only
-   Set appropriate CSP headers
-   Use `.env.production` for production config

**Infrastructure:**

-   PostgreSQL: Enable SSL, configure replication
-   Redis: Enable persistence, configure clustering
-   RabbitMQ: Enable management plugin, configure clustering
-   S3: Configure bucket policies and CORS

### Environment Variables

**Backend (see `server/README.md`):**

-   Connection strings
-   Firebase Admin SDK credentials
-   AWS S3 configuration
-   RabbitMQ connection
-   Redis connection
-   Ollama endpoint
-   Groq API key

**Frontend (see `client/README.md`):**

-   All variables prefixed with `VITE_`
-   Firebase client configuration
-   API base URL
-   Feature flags

## Contributing

We welcome contributions! Please follow these guidelines:

1. **Read the documentation** - Familiarize yourself with the architecture
2. **Follow coding standards** - Check `.github/instructions/` for guidelines
3. **Write tests** - Aim for >80% code coverage
4. **Document your changes** - Update README and add XML/JSDoc comments
5. **Run linters** - Ensure code quality before committing
6. **Create focused PRs** - One feature/fix per pull request

**Before submitting:**

-   Run backend tests: `cd server && dotnet test`
-   Run frontend linter: `cd client && npm run lint`
-   Verify build succeeds: `dotnet build` and `npm run build`
-   Update documentation if needed

## License

Unless a license file is added to the repository, all rights are reserved by the authors/owners.

© 2025 ExpertBridge. Repository owner: moheladwy.

## Support & Community

-   **Issues**: Report bugs or request features via GitHub Issues
-   **Discussions**: Use GitHub Discussions for questions and ideas
-   **Documentation**: Check `docs/` and project-specific READMEs

## Acknowledgments

Built with modern technologies:

-   .NET 10.0 and ASP.NET Core team
-   React, Redux Toolkit, and Vite teams
-   Open source community (EF Core, MassTransit, Serilog, etc.)
-   Firebase, AWS, and infrastructure providers
