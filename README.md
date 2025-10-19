# ExpertBridge

An enterprise-grade social platform for expert professionals that blends a professional network (profiles, posts, comments, messaging, notifications) with a freelance marketplace (job postings, hiring, reviews) and AI-assisted features (content tagging, moderation, semantic search, recommendations).

- Backend: .NET 9 (Clean Architecture, EF Core, MassTransit/RabbitMQ, SignalR, OpenTelemetry)
- Frontend: React 19 + TypeScript (Vite, Redux Toolkit + RTK Query, Tailwind, MUI)
- Infra: PostgreSQL (pgvector), Redis, RabbitMQ, Seq, Prometheus, Grafana, Ollama, Nginx Proxy Manager


## Repository structure

Top-level folders you’ll use the most:

- `server/` — .NET solution (API, Admin, Worker, Host) and compose files for local dev
- `client/` — React SPA frontend
- `deployment/` — Docker Compose stacks for production-like deployments (API/Admin/Worker, PostgreSQL/pgAdmin, RabbitMQ, Prometheus, Grafana, Nginx Proxy Manager, Ollama)
- `docs/` — Documentation links and diagrams (PlantUML/sequence diagrams in subfolders)
- `GP_Documentation/` — Class diagrams, ERDs, sequence diagrams, system component diagram

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

- Clean Architecture with clear separation: Presentation → Application → Core → Infrastructure.
- Composition via .NET Aspire to run API, Admin, Worker and dependencies locally.
- Persistence: PostgreSQL 17 + pgvector; EF Core 9 with soft-delete and retry policies.
- Messaging: MassTransit on RabbitMQ; Background: Quartz persistent jobs.
- Real-time: SignalR for notifications/messaging.
- Caching: FusionCache + Redis.
- Observability: OpenTelemetry (traces, metrics, logs), Prometheus exporter, Seq sink.
- AI: Microsoft.Extensions.AI + Ollama for embeddings; Groq API for LLM tasks.

System component diagram and domain models are available under:

- `GP_Documentation/SystemComponent/system-component.txt` (Mermaid)
- `GP_Documentation/Class Diagram/Overall.txt` (PlantUML) and domain-focused diagrams
- Additional sequence and architecture diagrams under `docs/`


## Quickstart

Pick one path to run locally.

### Option A — Run everything with .NET Aspire (recommended)

Starts RabbitMQ, Redis, Seq, Ollama and the API/Admin/Worker projects together.

```bash
# From repository root
dotnet run --project server/ExpertBridge.Host/ExpertBridge.Host.csproj --profile http
```

Key endpoints (development defaults):
- API Swagger: `/swagger`
- Health: `/health` (liveness `/alive`)
- SignalR hub: `/api/notificationsHub`


### Option B — Run services manually

1) Start PostgreSQL (+pgAdmin) for local dev

```bash
# From repository root
docker compose -f server/postgresql-compose.yaml up -d
```

2) Export minimal env vars for the API/Worker (adjust as needed)

```bash
export ConnectionStrings__Postgresql="Host=localhost;Port=5432;Database=expertbridge;Username=root;Password=root"
export ConnectionStrings__Redis="localhost:6379"
export ConnectionStrings__Seq="http://localhost:5341"
export ConnectionStrings__QuartzDatabase="Host=localhost;Port=5432;Database=quartz;Username=root;Password=root"
export MessageBrokerCredentials__Host="amqp://localhost"
export MessageBrokerCredentials__Username="guest"
export MessageBrokerCredentials__Password="guest"
```

3) Build and run the API (and others as needed)

```bash
dotnet restore
dotnet build
dotnet run --project server/ExpertBridge.Api/ExpertBridge.Api.csproj
```

Frontend in a separate terminal:

```bash
cd client
npm install
npm run dev
```

By default the SPA runs on http://localhost:5173 and calls `${VITE_SERVER_URL}` for API/health.


## Frontend (client/)

- React 19, React Router v7, Redux Toolkit + RTK Query
- TailwindCSS + MUI; Auth via Firebase; IndexedDB via Dexie
- Health guard: calls `${VITE_SERVER_URL}/health` on startup. See `client/docs/`.

Local development:

```bash
cd client
npm install
npm run dev
```

Environment variables (create `client/.env`):

```dotenv
VITE_SERVER_URL=http://localhost:5027        # or your API base
VITE_API_KEY=your_firebase_api_key
VITE_AUTH_DOMAIN=your-project.firebaseapp.com
VITE_PROJECT_ID=your-project-id
VITE_STORAGE_BUCKET=your-project.appspot.com
VITE_MESSAGING_SENDER_ID=1234567890
VITE_APP_ID=1:1234567890:web:abcdef123456
VITE_MEASUREMENT_ID=G-XXXXXXXXXX
```

More details: `client/README.md` (routing, state, codegen, Docker options).


## Backend (server/)

Highlights:
- API (ASP.NET Core), Admin (Blazor Server), Worker (Quartz + MassTransit)
- EF Core 9 with migrations (applied automatically in Development)
- OpenTelemetry + Prometheus + Seq
- Dockerfiles per project and `compose.yaml` for services

Run via Aspire (recommended) or see manual steps above. For EF migrations, examples are in `server/README.md`.


## Deployment (Docker Compose)

Composable stacks are under `deployment/`:

- `deployment/postgresql/docker-compose.yml` — PostgreSQL (pgvector), pgAdmin, exporter
- `deployment/rabbitmq/docker-compose.yaml` — RabbitMQ management
- `deployment/prometheus/prometheus-compose.yml` — Prometheus
- `deployment/grafana/grafana-compose.yml` — Grafana
- `deployment/nginx/docker-compose.yaml` — Nginx Proxy Manager
- `deployment/ollama/docker-compose.yaml` — Ollama
- `deployment/api/docker-compose.yml` — App stack (API, Admin, Worker, Seq, Aspire dashboard)

Typical bring-up order:

```bash
# One-time: shared network used by stacks
docker network create shared || true

# Datastores & infra
docker compose -f deployment/postgresql/docker-compose.yml up -d
docker compose -f deployment/rabbitmq/docker-compose.yaml up -d
docker compose -f deployment/prometheus/prometheus-compose.yml up -d
docker compose -f deployment/grafana/grafana-compose.yml up -d
docker compose -f deployment/nginx/docker-compose.yaml up -d
docker compose -f deployment/ollama/docker-compose.yaml up -d

# Application services
docker compose -f deployment/api/docker-compose.yml up -d
```

Environment files:

- `deployment/postgresql/.env` — supplies `POSTGRES_USER`, `POSTGRES_PASSWORD`, `PGADMIN_*`
- `deployment/api/.env` — supplies the app settings (connection strings, Firebase, AWS S3, OTEL, etc.)

Example snippets (adjust for your environment):

```dotenv
# deployment/postgresql/.env
POSTGRES_USER=root
POSTGRES_PASSWORD=root
PGADMIN_DEFAULT_EMAIL=admin@example.com
PGADMIN_DEFAULT_PASSWORD=change-me
DB_HOST=postgres
DB_NAME=expertbridge
```

```dotenv
# deployment/api/.env (partial)
ASPNETCORE_ENVIRONMENT=Production
OTEL_EXPORTER_OTLP_ENDPOINT=http://aspire-dashboard:18889
HTTP_PORTS=8080

# Connection strings (containers communicate via Docker networks)
ConnectionStrings__Postgresql=Host=postgres;Port=5432;Database=expertbridge;Username=root;Password=root
ConnectionStrings__QuartzDatabase=Host=postgres;Port=5432;Database=quartz;Username=root;Password=root
ConnectionStrings__Redis=ExpertBridgeRedis:6379
ConnectionStrings__rabbitmq=amqp://rabbitmq:5672
ConnectionStrings__Seq=http://seq:5341

# RabbitMQ credentials
MessageBroker__Host=amqp://rabbitmq
MessageBroker__Username=guest
MessageBroker__Password=guest

# Firebase (example placeholders)
Firebase__ProjectId=your-project-id
Firebase__ClientEmail=service-account@your-project.iam.gserviceaccount.com
Firebase__PrivateKey=-----BEGIN PRIVATE KEY-----\n...\n-----END PRIVATE KEY-----\n

# AWS S3
AwsS3__Region=us-east-1
AwsS3__BucketName=your-bucket
AwsS3__AwsKey=AKIA...
AwsS3__AwsSecret=...
AwsS3__BucketUrl=https://your-bucket.s3.amazonaws.com
```

Notes:
- Stacks assume an external Docker network named `shared`.
- Volumes are bound to local folders as declared in each compose file.


## Observability

- OpenTelemetry is enabled across services; the app publishes Prometheus metrics and can export OTLP.
- Seq is included for structured logs (default ports 4002 UI, 5341 ingestion in `deployment/api`).
- Prometheus and Grafana stacks are available under `deployment/`.


## Documentation & diagrams

- General docs links: `docs/README.md`
- Client health check: `client/docs/`
- Architecture and domain diagrams: `docs/` and `GP_Documentation/`
	- System component (Mermaid): `GP_Documentation/SystemComponent/system-component.txt`
	- Class diagrams (PlantUML): `GP_Documentation/Class Diagram/*.txt`
	- Sequence diagrams: `GP_Documentation/SequenceDiagrams/` and `docs/SequenceDiagrams/`


## Contributing

- Follow Clean Architecture conventions and established naming.
- Prefer async patterns, DI, and focused services.
- Frontend follows a feature-first structure under `client/src/features`.
- Run linters before pushing:

```bash
cd client && npm run lint
```


## License

Unless a license file is added to the repository, all rights are reserved by the authors/owners. Source files in the server may include an MIT header; consider adding a top-level `LICENSE` if open-sourcing.
