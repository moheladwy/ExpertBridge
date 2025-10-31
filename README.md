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
- Composition via .NET Aspire to run API, Admin, Worker, and dependencies locally.
- Persistence: PostgreSQL 18 + pgvector; EF Core 9 with soft-delete and retry policies.
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

### Run everything with .NET Aspire (recommended)

Starts RabbitMQ, Redis, Seq, Ollama, Postgresql Database, and the API/Admin/Worker projects together.

```bash
# From repository root
dotnet run --project server/ExpertBridge.Host/ExpertBridge.Host.csproj --profile http
```

Key endpoints (development defaults):
- API Swagger: `/swagger`
- Health: `/health` (liveness `/alive`)
- SignalR hub: `/api/notificationsHub`


## Frontend (client/)

Frontend in a separate terminal:

- React 19, React Router v7, Redux Toolkit + RTK Query
- TailwindCSS + MUI; Auth via Firebase; IndexedDB via Dexie
- Health guard: calls `${VITE_SERVER_URL}/health` on startup. See `client/docs/`.

```bash
cd client
npm install
npm run dev
```

Environment variables (create `client/.env`):

```dotenv
VITE_SERVER_URL=http://localhost:5027
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
