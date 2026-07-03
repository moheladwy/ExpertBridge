# CLAUDE.md — client/

React 19 + TypeScript SPA. Vite + SWC, Redux Toolkit + RTK Query, Tailwind v4, shadcn/ui (New York),
Firebase Auth, react-router v7. Authoritative versions live in `package.json` (the root README lags).

## Commands
```bash
npm install
npm run dev      # Vite dev server :5173
npm run build    # tsc -b && vite build  (type errors fail the build)
npm run lint     # eslint .
npm run preview  # serve the production build :4173
```
No test runner is configured yet (lint only).

## Structure (feature-first)
- `src/features/<feature>/` — RTK Query slices + types per domain: `auth`, `posts`, `comments`,
  `jobs`, `jobPostings`, `messages`, `notifications`, `profiles`, `users`, `search`, `tags`, `media`.
- `src/api/` — base `apiSlice` (Firebase token injection, retry, tag types).
- `src/views/` — `pages/` (routed) and `components/` (UI). `src/routes.tsx` wires routes.
- `src/hooks/`, `src/lib/`, `src/contexts/`, `src/utils/`.
- Path aliases (`vite.config.ts`): `@/` → `src/`, `@views/` → `src/views/`.

## Conventions
- **API calls go through RTK Query**, never raw `fetch`. Extend the base `apiSlice` with
  `injectEndpoints()`; set `providesTags` / `invalidatesTags` for cache invalidation.
- **Auth:** use `useCurrentUser()` (from `AuthStateManager`) — never add a new Firebase
  `onAuthStateChanged` listener. Use `tokenManager.getToken()` (1-min cache) over `getIdToken()`.
- Wrap lazy routes with `lazyWithRetry()`; wrap protected pages in `<ProtectedRoute>`.
- Use `cn()` for className merging; typed Redux hooks `useAppSelector` / `useAppDispatch`.
- shadcn/ui components: `npx shadcn@latest add <component>`.

## Adding env vars (4 places — they're embedded at BUILD time)
1. `.env` with a `VITE_` prefix → 2. `docker-compose.yml` `build.args` → 3. `Dockerfile` `ARG` →
4. `Dockerfile` `ENV`. Access via `import.meta.env.VITE_<NAME>`.

## More
`README.md`, `client/docs/`, and `.github/instructions/client.instructions.md` (full guidelines).
