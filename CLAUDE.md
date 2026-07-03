# CLAUDE.md

ExpertBridge — professional-networking + freelance-marketplace platform with AI features
(content moderation, semantic search, recommendations). .NET 10 backend + React 19 SPA.

## Layout
- `server/` — .NET 10 solution (`ExpertBridge.slnx`), Clean Architecture. Build/test/run from here. See `server/CLAUDE.md`.
- `client/` — React 19 + TypeScript SPA (Vite, Redux Toolkit + RTK Query, Tailwind v4, shadcn/ui). See `client/CLAUDE.md`.
- `deployment/` — production-like Docker Compose stacks (Prometheus, Grafana, Nginx, etc.).
- `docs/`, `GP_Documentation/` — architecture, ERD, sequence & class diagrams.
- Versions are authoritative in `client/package.json` and `server/Directory.Packages.props`
  (the root README's version list lags — trust the manifests).

## Commands
Full stack (all infra + apps) from repo root:
```bash
docker compose up -d        # frontend :80, API :5027, Admin :5028, Aspire :18888
docker compose logs -f api  # service-name: api | client | worker | admin
```

Backend (local dev, recommended — Aspire orchestrates Postgres/Redis/RabbitMQ/Ollama/Seq):
```bash
cd server
dotnet run --project ExpertBridge.Host/ExpertBridge.Host.csproj --launch-profile http
# Aspire dashboard :15888 ;  or: aspire run  (.aspire/settings.json points at the Host)
dotnet build      # solution is ExpertBridge.slnx (XML solution format)
dotnet test       # ExpertBridge.Tests.Unit — xUnit + Shouldly, AAA pattern
dotnet format
```

Frontend:
```bash
cd client
npm install
npm run dev       # Vite dev server :5173
npm run build     # tsc -b && vite build
npm run lint      # eslint .  (no test runner configured yet)
```

EF Core migrations (auto-applied on startup in Development):
```bash
cd server
dotnet ef migrations add <Name> --project ExpertBridge.Data --startup-project ExpertBridge.Api
dotnet ef database update      --project ExpertBridge.Data --startup-project ExpertBridge.Api
```

## Gotchas (project-specific)
- **.NET SDK is pinned to 10.0** in `server/global.json` (`rollForward: latestMajor`, prerelease allowed). The solution is `.slnx`, not `.sln`.
- **Frontend auth:** never create new Firebase `onAuthStateChanged` listeners — use the `useCurrentUser()` hook / `AuthStateManager`, and `tokenManager.getToken()` for cached tokens.
- **Frontend data:** always go through RTK Query (no raw `fetch`); extend the base `apiSlice` via `injectEndpoints()` and set `providesTags`/`invalidatesTags`.
- **`VITE_` env vars are embedded at BUILD time**, not runtime — the client Dockerfile needs them passed as `ARG`. All client env vars must be `VITE_`-prefixed.
- **Backend:** controllers return DTOs, never EF entities; soft-delete is via `ISoftDeletable` + global query filters; AI work (tagging, NSFW detection, embeddings) is offloaded to `ExpertBridge.Worker` via MassTransit/RabbitMQ domain events.
- **Codacy:** `.github/instructions/codacy.instructions.md` asks for Codacy MCP analysis after edits — only relevant if you have the Codacy MCP server connected.

## Deeper docs (read before large changes; not auto-loaded by Claude)
- Root `README.md`, `server/README.md`, `client/README.md`
- `.github/instructions/expertbridge.instructions.md` (full coding standards, ~38 KB)
- `server/.github/instructions/…` and `client/.github/instructions/…` (layer/feature rules)
