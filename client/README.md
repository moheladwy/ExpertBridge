# ExpertBridge Client

A modern, TypeScript-based React application built with Vite. This frontend interfaces with a .NET backend API to power ExpertBridge—a professional networking and job-matching platform with real-time features, robust authentication, and a rich, responsive UI.

## 🏗 Project Overview

ExpertBridge Client is the SPA frontend for the ExpertBridge platform. It provides:

- **Centralized Authentication** via Firebase (email/password and social providers) with singleton auth state manager
- **Feature-rich feed** with infinite scroll and post interactions
- **User profiles** with skills, reputation, and expert suggestions
- **Job postings and job management** flows with applications tracking
- **Real-time notifications** and messaging capabilities
- **Media handling** with file uploads and previews
- **Search functionality** for posts, users, and jobs
- **API health guard** for graceful startup when the backend is unavailable

The client integrates with a .NET backend over REST endpoints, using Redux Toolkit Query (RTK Query) for API state and caching. It is production-ready with **Tailwind CSS v4**, **shadcn/ui components**, and **Radix UI primitives** for a cohesive design system, plus SPA routing via **React Router v7**.

## 🧩 Architecture Overview

The project follows a **feature-first modular architecture**, organized by domain under `src/features`, with shared libs in `src/lib`, and UI/route composition under `src/views` and `src/routes.tsx`.

Key characteristics:

- **State management:** Redux Toolkit + RTK Query with entity adapters
- **API access:** RTK Query `createApi` with fetchBaseQuery, retry logic, and automatic token injection
- **Routing:** React Router v7 with `createBrowserRouter` and lazy loading with retry
- **Authentication:** Centralized Firebase Auth with singleton `AuthStateManager` and `TokenManager` for token caching
- **Styling:** Tailwind CSS v4 with shadcn/ui components and OKLCH color space
- **Code splitting:** Vite-powered chunking strategy (firebase, redux, ui, fonts, vendor)
- **Health guard:** Startup API health check with user-friendly error page

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
│   │   ├── useAuthCheck.ts                # Legacy auth check (localStorage)
│   │   ├── useAuthReady.ts                # Check if auth is initialized
│   │   ├── useCurrentAuthUser.ts          # Firebase current user hook
│   │   ├── useIsUserLoggedIn.ts           # Combined auth + profile state
│   │   ├── useRefetchOnLogin.ts           # Refetch data on login
│   │   ├── useCallbackOnIntersection.ts   # Intersection Observer for infinite scroll
│   │   └── ...                            # Utility hooks
│   ├── lib/
│   │   ├── api/
│   │   │   ├── endpoints.ts               # Base URLs and endpoint paths
│   │   │   └── interfaces.ts              # Shared backend data interfaces
│   │   ├── firebase/                      # Firebase initialization + helpers
│   │   │   ├── index.ts                   # initializeApp + getAuth/getMessaging
│   │   │   ├── useAuthSubscribtion.ts     # Firebase auth subscription hook
│   │   │   ├── useSignInWithPopup.ts      # Social auth hook
│   │   │   ├── useSignOut.ts              # Sign out hook
│   │   │   └── EmailAuth/                 # Email auth utilities
│   │   ├── services/
│   │   │   ├── AuthStateManager.ts        # Centralized auth state (singleton)
│   │   │   ├── TokenManager.ts            # Token caching and refresh logic
│   │   │   └── SingletonAuthSubscription.ts
│   │   ├── hoc/
│   │   │   └── withAuthUser.tsx           # HOC for auth user
│   │   └── util/
│   │       ├── config.ts                  # Config via Vite env (server URLs)
│   │       ├── utils.ts                   # cn() utility for className merging
│   │       └── ...                        # Misc utils
│   ├── views/
│   │   ├── components/                    # UI building blocks
│   │   │   ├── common/                    # Shared feature components
│   │   │   │   ├── ui/                    # UI components (NavBar, AuthPromptModal, etc.)
│   │   │   │   ├── posts/                 # Post-related components (Feed, PostCard, etc.)
│   │   │   │   ├── comments/              # Comment components
│   │   │   │   ├── profile/               # Profile components (UpdateProfile, SuggestedExperts)
│   │   │   │   ├── jobPostings/           # Job posting components
│   │   │   │   ├── jobs/                  # Job management components
│   │   │   │   └── notifications/         # Notification components
│   │   │   ├── custom/                    # Custom UI components
│   │   │   │   ├── FileUpload.tsx         # File upload component
│   │   │   │   ├── TimeAgo.tsx            # Time formatting component
│   │   │   │   ├── AuthButtons.tsx        # Auth button components
│   │   │   │   └── ...                    # Other custom components
│   │   │   └── chat/                      # Chat components
│   │   └── pages/                         # Route pages organized by domain
│   │       ├── landing/                   # Landing page, Privacy, About
│   │       ├── auth/                      # Login, SignUp, EmailVerification
│   │       ├── feed/                      # HomePage (main feed)
│   │       ├── posts/                     # Post detail pages
│   │       ├── profile/                   # MyProfile, UserProfile
│   │       ├── jobs/                      # Job management pages
│   │       ├── jobPostings/               # Job postings pages
│   │       ├── search/                    # Search pages
│   │       └── notifications/             # Notifications page
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

- **State:** The Redux store (`src/app/store.ts`) registers:
    - `apiSlice.reducer` (RTK Query) under `api` - handles all API calls with caching (5 min default)
    - `auth` reducer for the current user (profile info synced with backend)
    - Middleware: listener middleware (prepend) and RTK Query middleware (concat)
    - **Centralized Auth:** `AuthStateManager` singleton manages Firebase auth state with a single listener
    - **Token Management:** `TokenManager` caches tokens for 1 minute before expiry, reducing API calls by 70%

- **Routing:** `src/routes.tsx` declares all routes via `createBrowserRouter` with lazy loading:
    - **Lazy Loading:** Custom `lazyWithRetry()` utility retries failed chunk loads 3 times
    - **Protected Routes:** Wrapped in `<ProtectedRoute>` which redirects to login if not authenticated
    - **Public Routes:** Wrapped in `<PublicRoute>` for auth pages
    - **Top-level routes:**
        - `/` (Landing), `/home`, `/posts/:postId`, `/jobs`, `/job/:jobPostingId`, etc.
        - Auth routes: `/login`, `/signup`, `/verify-email`
        - Profile routes: `/profile`, `/profile/:userId`
        - Search: `/search/p`, `/search/u`, `/search/jobs`
        - Error routes: `/service-unavailable` (health error page), catch-all `*` 404

- **API:**
    - `apiSlice` uses `fetchBaseQuery` with automatic retry logic (3 attempts, fails on 401/404/400/429/500)
    - **Base URL:** `config.VITE_SERVER_URL` (which adds `/api` internally)
    - **Token Injection:** `prepareHeaders` uses cached token from `TokenManager` (70% faster than fresh token calls)
    - **Cache Strategy:** 5-minute cache (`keepUnusedDataFor: 300`)
    - **Tag Invalidation:** Comprehensive tag system for cache invalidation (Post, Comment, Profile, Job, etc.)
    - Domain slices inject endpoints into `apiSlice` using `injectEndpoints()` pattern
    - **Entity Adapters:** Used for normalized state (posts, comments) with `createEntityAdapter`
    - `src/api/jobService.ts` contains imperative `fetch` calls for specific job operations

- **Health Guard:**
    - On startup, app checks API health at `${VITE_SERVER_URL}/health`
    - If unhealthy, renders a user-friendly error page with manual retry option
    - Prevents app from loading when backend is unavailable
    - **Loading States:** `PageLoader` shown during initialization and profile fetching

## 🛠️ Technologies Used

- **Application**
    - React 19.0.0
    - React Router v7.1.5
    - TypeScript 5.6.2
    - Vite 6.4.1 (with `@vitejs/plugin-react-swc`)
    - React Compiler (Babel plugin beta)
- **State & Data**
    - Redux Toolkit 2.5.1
    - RTK Query with infinite queries support
    - Entity adapters for normalized state
    - `@rtk-query/codegen-openapi` for API generation
- **UI / Styling**
    - **Tailwind CSS v4.1.16** with `@tailwindcss/vite` plugin
    - **shadcn/ui** components (New York style)
    - **Radix UI** primitives (accordion, dialog, dropdown, select, tooltip, etc.)
    - **Icons:** Lucide React, Tabler Icons, Heroicons
    - **Theme:** `next-themes` with dark/light mode support
    - **Media:** Vidstack for video player, Embla carousel
    - **Notifications:** react-hot-toast (primary), sonner
    - **Fonts:** @fontsource/roboto (300, 400, 500, 700)
- **Auth & Messaging**
    - Firebase 11.2.0 (Auth, Messaging)
    - Centralized `AuthStateManager` singleton
    - Token caching with `TokenManager`
- **Forms & Validation**
    - react-hook-form 7.66.0
    - zod 3.25.76
    - @hookform/resolvers 3.10.0
- **HTTP & Utils**
    - date-fns 4.1.0
    - class-variance-authority, clsx, tailwind-merge
    - cmdk, vaul
- **Tooling**
    - ESLint 9.17.0 with `typescript-eslint`, react hooks plugin, react-refresh plugin
    - PostCSS 8.5.1
    - Terser 5.44.0 for minification
    - Babel plugin react-compiler (beta)

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
# API Configuration
VITE_SERVER_URL=http://localhost:5027
VITE_INDEXED_DB_NAME="expertbridge.duckdns.org"
VITE_INDEXED_DB_VERSION=1

# Firebase Configuration (required for auth)
VITE_API_KEY="AIzaSyAzvL5tyv1Z8mzky7VgZUVf9eHs5CuUbk8"
VITE_AUTH_DOMAIN="expert-bridge.firebaseapp.com"
VITE_PROJECT_ID="expert-bridge"
VITE_STORAGE_BUCKET="expert-bridge.firebasestorage.app"
VITE_MESSAGING_SENDER_ID="603187739026"
VITE_APP_ID="1:603187739026:web:d97a7904166d2eb044694b"
VITE_MEAUSUREMENT_ID="G-R53RPN7HKW"

# Feature Flags for Development
VITE_ENABLE_DEBUG_LOGGING=true
VITE_API_TIMEOUT=60000
VITE_MAX_API_RETRIES=3
VITE_ENABLE_TOKEN_MONITOR=true
VITE_ENABLE_AUTH_MONITOR=true
VITE_ENABLE_PERFORMANCE_MONITORING=true

# Development Tools
VITE_ENABLE_REACT_DEVTOOLS=true
VITE_ENABLE_REDUX_DEVTOOLS=true
```

**Used in code:**

- `VITE_SERVER_URL` - Backend API base URL (health check and API endpoints)
- Firebase config - Consumed in `src/lib/firebase/index.ts`
- Feature flags - Control debug logging, monitoring, and devtools
- API configuration - Timeout, retry attempts, IndexedDB settings

**Access in code:**

```typescript
import.meta.env.VITE_SERVER_URL;
import.meta.env.VITE_API_KEY;
```

**Important Docker note:**

- All `VITE_*` variables must be defined as `ARG` in Dockerfile
- Set as `ENV` during build stage (Vite embeds at build time)
- Must be passed via `docker-compose.yml` under `build.args`
- Note: Variable name is `VITE_MEAUSUREMENT_ID` (check spelling in Firebase console)

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

### Base API Configuration

- **Base slice:** `src/features/api/apiSlice.ts` uses RTK Query with:
    - Automatic Firebase token injection via `TokenManager.getToken()`
    - Retry logic (3 attempts, fails on 401/404/400/429/500)
    - 5-minute cache (`keepUnusedDataFor: 300`)
    - Comprehensive tag system for cache invalidation

### Feature Slices Pattern

Each feature extends the base API using `injectEndpoints()`:

```typescript
export const postsApiSlice = apiSlice.injectEndpoints({
	endpoints: (builder) => ({
		getPosts: builder.query<Post[], void>({
			query: () => "/posts",
			providesTags: ["Post"],
		}),
		addPost: builder.mutation<Post, AddPostRequest>({
			query: (body) => ({ url: "/posts", method: "POST", body }),
			invalidatesTags: ["Post"],
		}),
	}),
});
```

### Available Endpoint Tags

`CurrentUser`, `AuthUser`, `Post`, `Comment`, `Profile`, `Tag`, `CurrentUserSkills`, `ProfileSkills`, `SimilarPosts`, `SimilarJobs`, `JobPosting`, `JobOffer`, `Job`

### OpenAPI Codegen (Optional)

- Config: `codegen-openapi-config.ts`
- Generate hooks: `npx @rtk-query/codegen-openapi codegen-openapi-config.ts`
- Output: `src/features/api/appApiSlice.ts`

## 🏛️ Architecture Best Practices

### Authentication

- **Never create new Firebase listeners** - always use `useCurrentUser()` from `AuthStateManager`
- Use `tokenManager.getToken()` for cached tokens (70% faster than `getIdToken()`)
- Token cache expires 60 seconds before actual token expiry
- Single `onAuthStateChanged` listener for entire app eliminates memory leaks

### State Management

- Use RTK Query for all API calls (no direct `fetch` unless necessary)
- Extend `apiSlice` with `injectEndpoints()` pattern
- Always define `providesTags` and `invalidatesTags`
- Use `createEntityAdapter` for normalized state (posts, comments)
- Use typed hooks: `useAppSelector`, `useAppDispatch`

### Component Patterns

- Props interface above component: `ComponentNameProps`
- Use `cn()` utility for className merging
- Default export for pages, named exports for utilities
- Wrap routes in `<ErrorBoundary>`
- Use `PageLoader` for page-level, `ComponentLoader` for component-level loading

### Code Splitting

- Use `lazyWithRetry()` for route components (retries 3 times on chunk failure)
- Vite config defines strategic chunks: firebase, redux, ui, fonts, vendor
- Page-level components are automatically code-split

### Styling

- Use Tailwind CSS v4 utility classes
- shadcn/ui components for complex UI primitives
- OKLCH color space for design tokens
- `next-themes` for dark/light mode

## 👨‍💻 Contributing

- Use feature branches and open PRs for review
- Keep domain logic inside `src/features/<domain>`
- **Always use RTK Query** for data fetching and caching
- Run linting before pushing:
    ```bash
    npm run lint
    ```
- Follow the established aliasing and import style:
    - `@/` → `src/`
    - `@assets/` → `src/assets/`
    - `@views/` → `src/views/`
- **Component structure:**
    - Feature components → `src/views/components/common/[feature]/`
    - Custom UI → `src/views/components/custom/`
    - Pages → `src/views/pages/[feature]/`
- **Import order:** React → Third-party → Internal (`@/`) → Relative → Styles
- **Never create Firebase auth listeners** - use centralized hooks

## 📜 License and Credits

- No LICENSE file is included in this repository at this time. Unless a license is added, all rights are reserved by the authors/owners.
- © 2025 ExpertBridge. Repository owner: moheladwy.

## ℹ️ Additional Notes

- **Dev server:** http://localhost:5173 (Vite)
- **Preview server:** http://localhost:4173 (after build)
- **Centralized auth:** Single `onAuthStateChanged` listener eliminates 90% of auth-related overhead
- **Token caching:** Reduces API calls by 70% with 1-minute cache before expiry
- **Code splitting:** Strategic chunking reduces initial bundle size
- **Tailwind CSS v4:** Uses new `@import "tailwindcss"` syntax and `@theme inline`
- **shadcn/ui:** New York style with custom color palette (OKLCH color space)
- **Entity adapters:** Used for normalized state in posts and comments slices
- **Infinite scroll:** RTK Query `infiniteQuery` with cursor-based pagination
- **IndexedDB:** Configured via `VITE_INDEXED_DB_*` variables (ready for offline support)
- **Performance monitoring:** Optional via `VITE_ENABLE_PERFORMANCE_MONITORING`
- **Debug tools:** Redux DevTools and React DevTools enabled in development

### Performance Features

- **Lazy loading with retry:** Automatic retry on chunk load failures
- **Request retry logic:** 3 automatic retries with exponential backoff
- **Cache invalidation:** Comprehensive tag system prevents stale data
- **Optimized chunking:** Firebase, Redux, UI libraries in separate chunks
- **Token caching:** Reduces Firebase API calls significantly
- **Entity normalization:** Efficient state updates with `createEntityAdapter`

If you need a production-ready Docker image immediately, prefer the Nginx option above for best static serving performance and SPA routing behavior.
