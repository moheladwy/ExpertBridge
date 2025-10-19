# ExpertBridge Client

A modern, TypeScript-based React application built with Vite. This frontend interfaces with a .NET backend API to power ExpertBridge—a professional networking and job-matching platform with real-time features, robust authentication, and a rich, responsive UI.

## 🏗 Project Overview

ExpertBridge Client is the SPA frontend for the ExpertBridge platform. It provides:

- Authentication via Firebase (email/password and social providers)
- Feature-rich feed and post interactions
- User profiles and onboarding
- Job postings and job management flows
- Notifications, messaging scaffolding, and media handling
- API health guard for graceful startup when the backend is unavailable

The client integrates with a .NET backend over REST endpoints, using Redux Toolkit Query (RTK Query) for API state and caching. It is production-ready with TailwindCSS, Material UI, and Radix primitives for a cohesive design system, plus SPA routing via React Router v7.

## 🧩 Architecture Overview

The project follows a feature-first modular architecture, organized by domain under `src/features`, with shared libs in `src/lib`, and UI/route composition under `src/views` and `src/routes.tsx`.

Key characteristics:
- State management: Redux Toolkit + RTK Query
- API access: RTK Query `createApi` with fetchBaseQuery and auth header
- Routing: React Router v7, `createBrowserRouter`
- Authentication: Firebase Auth (token used as Bearer for backend)
- Styling: TailwindCSS + MUI + Radix UI primitives
- Health guard: Startup API health check with a user-friendly error page

### Directory Structure

```txt
client/
├── codegen-openapi-config.ts              # RTK Query OpenAPI codegen configuration
├── Dockerfile                             # Build-stage Dockerfile (build only)
├── netlify.toml                           # Netlify SPA redirects
├── public/
│   ├── _redirects                         # SPA fallback for static hosting
│   └── index.html
├── src/
│   ├── App.tsx                            # Root app layout and providers
│   ├── main.tsx                           # App bootstrap (Redux + Router + Theme)
│   ├── routes.tsx                         # Route definitions (React Router v7)
│   ├── api/
│   │   └── jobService.ts                  # Imperative job endpoints (non-RTKQ)
│   ├── app/
│   │   ├── store.ts                       # Redux store wiring
│   │   ├── hooks.ts                       # Typed Redux hooks
│   │   ├── listenerMiddleware.ts          # RTK listener middleware setup
│   │   └── withTypes.ts                   # App-typed createAsyncThunk
│   ├── contexts/
│   │   └── AuthPromptContext.tsx          # Global auth prompt control
│   ├── features/                          # Feature-first slices and types
│   │   ├── api/apiSlice.ts                # RTK Query base API slice
│   │   ├── auth/authSlice.ts              # Auth user store
│   │   ├── users/usersSlice.ts            # Users API (update current user)
│   │   ├── posts/, comments/, jobs/, ...  # Domain slices and types
│   │   └── api/appApiSlice.ts             # (Generated via OpenAPI; currently commented)
│   ├── hooks/
│   │   ├── useApiHealth.ts                # Startup API health check hook
│   │   ├── useCurrentAuthUser.ts          # Firebase current user bridge
│   │   └── ...                            # Utility hooks
│   ├── lib/
│   │   ├── api/
│   │   │   ├── endpoints.ts               # Base URLs and endpoint paths
│   │   │   └── interfaces.ts              # Shared backend data interfaces
│   │   ├── firebase/                      # Firebase initialization + helpers
│   │   │   └── index.ts                   # initializeApp + getAuth/getMessaging
│   │   └── util/
│   │       ├── config.ts                  # Config via Vite env (server URLs)
│   │       └── ...                        # Misc utils
│   ├── views/
│   │   ├── components/                    # UI building blocks
│   │   │   └── common/ui/ApiHealthGuard.tsx  # App-level health guard
│   │   └── pages/                         # Route pages organized by domain
│   │       ├── landing/, auth/, feed/, profile/, jobs/, jobPostings/, search/, notifications/, error/
│   └── assets/                            # Images, icons, etc.
├── tailwind.config.js                     # Tailwind configuration
├── vite.config.ts                         # Vite + React SWC config, path aliases
├── package.json                           # Scripts and dependencies
└── docs/
    ├── API_HEALTH_CHECK.md                # Health check system docs
    ├── API_HEALTH_CHECK_IMPLEMENTATION.md # Implementation summary
    └── HEALTH_CHECK_QUICK_START.md        # Quick start for health check
```

### How State, Routing, and API Calls Work

- State: The Redux store (`src/app/store.ts`) registers:
  - `apiSlice.reducer` (RTK Query) under `api`
  - `auth` reducer for the current user (profile info synced with backend)
  - Middleware: listener middleware (prepend) and RTK Query middleware (concat)

- Routing: `src/routes.tsx` declares all routes via `createBrowserRouter`. Protected routes are wrapped in `ProtectedRoute`. Top-level routes include:
  - `/` (Landing), `/home`, `/posts/:postId`, `/jobs`, `/job/:jobPostingId`, etc.
  - Auth routes: `/login`, `/signup`, `/verify-email`
  - Profile routes: `/profile`, `/profile/:userId`
  - Search: `/search/p`, `/search/u`, `/search/jobs`
  - Error routes: `/service-unavailable` (health error page), catch-all `*` 404

- API: 
  - `apiSlice` uses `fetchBaseQuery` with `baseUrl = config.VITE_SERVER_URL + '/api'` and attaches the Firebase ID token to the `Authorization` header when available.
  - Domain slices inject endpoints into `apiSlice` (e.g., `usersSlice` with `updateUser` mutation).
  - `src/api/jobService.ts` contains a couple of imperative `fetch` calls for jobs.

- Health Guard:
  - On startup, `ApiHealthGuard` uses `useApiHealth` to call `${VITE_SERVER_URL}/health`.
  - If unhealthy, it renders a friendly error page and provides manual retry.
  - See `docs/API_HEALTH_CHECK.md` for full behavior and options.

## 🛠️ Technologies Used

- Application
  - React 19
  - React Router v7
  - TypeScript ~5.6
  - Vite ^6 (with `@vitejs/plugin-react-swc`)
- State & Data
  - Redux Toolkit ^2.5
  - RTK Query (with `@rtk-query/codegen-openapi` config present)
  - Dexie + dexie-react-hooks (IndexedDB)
- UI / Styling
  - TailwindCSS ^3.4 + `tailwindcss-animate`
  - Material UI v6 (`@mui/material`, `@mui/icons-material`)
  - Radix UI primitives (accordion, dialog, dropdown, select, tooltip, etc.)
  - Icons: heroicons, lucide-react
  - Vidstack for media player, Embla for carousel
  - sonner and react-hot-toast (notifications)
- Auth & Messaging
  - Firebase ^11 (Auth, Messaging)
- HTTP & Utils
  - axios ^1.8
  - date-fns ^4.1
  - class-variance-authority, clsx, tailwind-merge, cmdk, vaul, next-themes (installed; custom ThemeProvider is used)
- Tooling
  - ESLint ^9 with `typescript-eslint`, react hooks plugin, react-refresh plugin
  - PostCSS + Autoprefixer
  - Babel plugin react-compiler (beta – not wired into Vite by default here)

Note: Versions above reflect `package.json` at the time of writing.

## ⚙️ Setup Instructions (Manual)

Prerequisites:
- Node.js 20+ recommended (Dockerfile uses Node 22)
- npm

Development:
```bash
npm install
npm run dev
```

- App will start on http://localhost:5173 by default (Vite)

Build and Preview:
```bash
npm run build
npm run preview
```

- Preview server runs on port 4173 by default (http://localhost:4173)

Quick command list (as requested):
```bash
npm install
npm run dev
npm run build
docker build -t react-client .
docker run -p 3000:3000 react-client
```

Important: The current Dockerfile is build-only and does not run a server. See “Setup Instructions (Docker)” below for correct container runtime options.

## 🐳 Setup Instructions (Docker)

The provided `Dockerfile` performs a production build only:

- It installs dependencies
- Injects Vite env args
- Runs `npm run build`
- It does not expose a port or run a web server

You have two good options to serve the built app in a container:

### Option A: Nginx (recommended for production)

Use a multi-stage Dockerfile to copy `dist/` into an Nginx image:

```dockerfile
# Stage 1: Build
FROM node:22-alpine AS build
WORKDIR /client
COPY package*.json ./
RUN npm ci
COPY . .
# Env needed at build-time for Vite
ARG VITE_SERVER_URL
ARG VITE_API_KEY
ARG VITE_AUTH_DOMAIN
ARG VITE_PROJECT_ID
ARG VITE_STORAGE_BUCKET
ARG VITE_MESSAGING_SENDER_ID
ARG VITE_APP_ID
ARG VITE_MEASUREMENT_ID
# (Optional) IndexedDB settings if used:
# ARG VITE_INDEXED_DB_NAME
# ARG VITE_INDEXED_DB_VERSION
RUN npm run build

# Stage 2: Serve with Nginx
FROM nginx:alpine
COPY --from=build /client/dist /usr/share/nginx/html
# SPA fallback
COPY public/_redirects /usr/share/nginx/html/_redirects
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

Build and run:
```bash
docker build -t react-client .
docker run -p 3000:80 react-client
```

Access at http://localhost:3000

### Option B: Node + `serve` (simple runtime)

```dockerfile
# Stage 1: Build
FROM node:22-alpine AS build
WORKDIR /client
COPY package*.json ./
RUN npm ci
COPY . .
ARG VITE_SERVER_URL
# ...other VITE_ args as needed...
RUN npm run build

# Stage 2: Serve static build
FROM node:22-alpine
WORKDIR /app
RUN npm i -g serve
COPY --from=build /client/dist ./dist
EXPOSE 3000
CMD ["serve", "-s", "dist", "-l", "3000"]
```

Build and run:
```bash
docker build -t react-client .
docker run -p 3000:3000 react-client
```

Note on the current Dockerfile:
- It ends after `npm run build` and won’t start a server when run.
- To use the quick command `docker run -p 3000:3000 react-client`, adopt Option B above or use Nginx with `-p 3000:80`.

## 🔧 Environment Variables

All runtime config is injected at build-time via Vite (must be prefixed with `VITE_`). Create a `.env` file in the project root for local development:

```env
# Backend base URL (no trailing slash)
VITE_SERVER_URL=https://api.example.com

# Optional HTTPS endpoint for direct use if needed
VITE_API_HTTPS_BASE_URL=https://api.example.com

# Firebase (required for auth)
VITE_API_KEY=your_firebase_api_key
VITE_AUTH_DOMAIN=your-project.firebaseapp.com
VITE_PROJECT_ID=your-project-id
VITE_STORAGE_BUCKET=your-project.appspot.com
VITE_MESSAGING_SENDER_ID=1234567890
VITE_APP_ID=1:1234567890:web:abcdef123456
VITE_MEASUREMENT_ID=G-XXXXXXXXXX

# Optional IndexedDB (if used with Dexie)
VITE_INDEXED_DB_NAME=expertbridge
VITE_INDEXED_DB_VERSION=1
```

Used in code:
- `VITE_SERVER_URL` is used to build API endpoints:
  - Health: `${VITE_SERVER_URL}/health`
  - API base: `${VITE_SERVER_URL}/api`
- Firebase config: consumed in `src/lib/firebase/index.ts`

Important Docker note:
- Ensure the env name is spelled correctly. In the code it’s `VITE_MEASUREMENT_ID` (with sure spelling), so ARG/ENV in Docker must use the exact same name. A misspelling will result in undefined at runtime.

## 🔍 Testing

No testing framework is currently configured in `package.json`. You can add Vitest/Jest/Cypress as needed.

Linting is set up:
```bash
npm run lint
```

ESLint configuration (`eslint.config.js`) includes:
- `typescript-eslint` recommended rules
- React hooks plugin
- Less restrictive defaults for DX (e.g., no-console allowed)

## 🧭 Deployment

- Static hosting: Build with `npm run build` and deploy the `dist/` folder.
- SPA routing:
  - `public/_redirects` and `netlify.toml` enable SPA fallback (200 to `/index.html`).
- Netlify:
  - Build command: `npm run build`
  - Publish directory: `dist`
  - Single Page App redirects are configured via `netlify.toml` and `_redirects`.
- Docker:
  - Use the Nginx or Node+serve multi-stage Dockerfiles above.

## 🔌 API Integration Notes

- Base slice: `src/features/api/apiSlice.ts` uses RTK Query with automatic auth header injection using Firebase ID token.
- Endpoint tags cover: `CurrentUser`, `Post`, `Comment`, `Profile`, `JobPosting`, `Job`, etc.
- Codegen: `codegen-openapi-config.ts` is present and configured for `@rtk-query/codegen-openapi`.
  - Schema: `expertbridge-openapi.json`
  - Output: `src/features/api/appApiSlice.ts`
  - To generate hooks (example command):
    ```bash
    npx @rtk-query/codegen-openapi codegen-openapi-config.ts
    ```
  - Note: The generated file is currently commented; wire it up by injecting endpoints into `apiSlice` and exporting the hooks you need.

## 👨‍💻 Contributing

- Use feature branches and open PRs for review.
- Keep domain logic inside `src/features/<domain>`; prefer RTK Query for data fetching and caching.
- Run linting before pushing:
  ```bash
  npm run lint
  ```
- Follow the established aliasing and import style:
  - `@` → `src/`
  - `@assets`, `@views` are configured in `vite.config.ts`

## 📜 License and Credits

- No LICENSE file is included in this repository at this time. Unless a license is added, all rights are reserved by the authors/owners.
- © 2025 ExpertBridge. Repository owner: moheladwy.

## ℹ️ Additional Notes

- Default dev server: http://localhost:5173
- Preview server after build: http://localhost:4173
- Health check documentation is available under `docs/` and integrated at app root via `ApiHealthGuard`.
- MUI and Tailwind are used together: MUI is configured to inject styles first via `StyledEngineProvider` and Tailwind drives utility classes and design tokens (see `tailwind.config.js`).
- IndexedDB via Dexie is included; configure `VITE_INDEXED_DB_*` if you wire up persistence.

If you need a production-ready Docker image immediately, prefer the Nginx option above for best static serving performance and SPA routing behavior.
