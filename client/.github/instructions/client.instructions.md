---
applyTo: "**"
---

# ExpertBridge Client - AI Code Assistant Instructions

## Project Overview

ExpertBridge Client is a modern, production-ready React SPA (Single Page Application) built with TypeScript, Vite, and React 19. It serves as the frontend for a professional networking and job-matching platform, featuring real-time capabilities, Firebase authentication, and rich user interactions.

**Tech Stack:**

- **Build Tool:** Vite 6 with React SWC plugin
- **Framework:** React 19 with React Router v7
- **State Management:** Redux Toolkit with RTK Query
- **Authentication:** Firebase Auth (v11.2.0)
- **Styling:** Tailwind CSS v4 with shadcn/ui components
- **Language:** TypeScript 5.6
- **Deployment:** Docker + Nginx, Netlify-ready

## Architecture Principles

### 1. Feature-First Modular Architecture

The project follows a **domain-driven structure** under `src/features/`, with each feature as a self-contained module:

```
src/features/
├── api/          # Base RTK Query API slice
├── auth/         # Authentication state
├── posts/        # Posts feature (slice + types)
├── comments/     # Comments feature
├── profiles/     # User profiles
├── jobs/         # Job management
├── jobPostings/  # Job postings
├── messages/     # Messaging
├── notifications/# Notifications
├── search/       # Search functionality
├── tags/         # Tags management
├── media/        # Media handling
└── users/        # User management
```

**Each feature module contains:**

- `[feature]Slice.ts` - RTK Query endpoints via `apiSlice.injectEndpoints()`
- `types.ts` - TypeScript interfaces and types
- Transformers and adapters for data normalization

### 2. State Management Strategy

**Redux Toolkit + RTK Query** is the central state management solution:

- **Central Store:** `src/app/store.ts` configures the Redux store
- **API Layer:** `src/features/api/apiSlice.ts` provides the base RTK Query API
- **Slice Pattern:** Feature slices extend the base API using `injectEndpoints()`
- **Entity Adapters:** Use `createEntityAdapter` for normalized state (e.g., posts, comments)
- **Typed Hooks:** Always use typed hooks from `src/app/hooks.ts` (`useAppSelector`, `useAppDispatch`)

**Key Configuration:**

```typescript
// Base API with retry logic, token management, and tag invalidation
export const apiSlice = createApi({
  reducerPath: "api",
  baseQuery: baseQueryWithRetry,
  tagTypes: ["CurrentUser", "Post", "Comment", "Profile", "Tag", ...],
  keepUnusedDataFor: 300, // 5 minutes cache
  endpoints: (builder) => ({}),
});
```

### 3. Authentication System

**Centralized Firebase Auth Management:**

The project uses a **singleton auth state manager** to avoid duplicate listeners:

- **`AuthStateManager.ts`**: Single `onAuthStateChanged` listener for the entire app
- **`TokenManager.ts`**: Token caching, refresh logic, and React hook wrapper
- **Pattern:** Subscribe to centralized auth state, don't create new Firebase listeners

**Auth Flow:**

1. Firebase handles authentication (email/password, Google, etc.)
2. `AuthStateManager` maintains single listener and distributes state
3. `TokenManager` caches tokens with 60-second buffer before expiry
4. RTK Query `prepareHeaders` injects Bearer token from cache
5. 401 responses clear token cache and trigger re-auth

**Key Services:**

- `src/lib/services/AuthStateManager.ts` - Centralized auth state (use `useCurrentUser()` hook)
- `src/lib/services/TokenManager.ts` - Token management (use `tokenManager.getToken()`)
- `src/lib/firebase/` - Firebase initialization and auth utilities

### 4. Routing Architecture

**React Router v7** with `createBrowserRouter`:

- **Route Definitions:** `src/routes.tsx` (344 lines)
- **Lazy Loading:** Custom `lazyWithRetry()` utility with chunk recovery
- **Route Guards:** `ProtectedRoute` and `PublicRoute` components
- **Chunk Strategy:** Routes are grouped into logical chunks (auth, feed, profile, jobs)

**Code Splitting Strategy (from vite.config.ts):**

- `firebase` chunk: Firebase SDK
- `redux` chunk: Redux and RTK Query
- `ui` chunk: Radix UI, Lucide icons, Tailwind utilities
- `fonts` chunk: @fontsource/roboto
- `vendor` chunk: All other node_modules
- Page-level chunks: Lazy-loaded routes

### 5. Component Organization

**Three-tier component structure:**

```
src/
├── components/         # Shared/reusable components
│   ├── ui/            # shadcn/ui primitives (resizable-navbar, etc.)
│   ├── loaders/       # Loading states (PageLoader, ComponentLoader)
│   └── errors/        # Error boundaries and fallbacks
├── views/
│   ├── components/    # Feature-specific components
│   │   ├── common/    # Shared feature components (posts, comments, profile, etc.)
│   │   ├── custom/    # Custom UI components (FileUpload, TimeAgo, etc.)
│   │   └── chat/      # Chat-specific components
│   └── pages/         # Page-level components (route targets)
│       ├── auth/      # Login, SignUp, EmailVerification
│       ├── feed/      # HomePage (main feed)
│       ├── posts/     # Post detail pages
│       ├── profile/   # MyProfile, UserProfile
│       ├── jobs/      # Job management pages
│       ├── jobPostings/# Job postings pages
│       └── search/    # Search pages
```

**Component Patterns:**

- **Container/Presenter:** Separate data fetching from presentation
- **Compound Components:** For complex UI (e.g., Navbar with NavBody, NavItems, etc.)
- **Custom Hooks:** Extract reusable logic (e.g., `useRefetchOnLogin`, `useAuthCheck`)

### 6. Styling System

**Tailwind CSS v4 + shadcn/ui** with custom design tokens:

- **Configuration:** `components.json` defines shadcn/ui setup
- **Theme System:** `next-themes` for dark/light mode
- **CSS Variables:** Design tokens in `src/index.css` (OKLCH color space)
- **Utility Function:** `cn()` utility in `src/lib/util/utils.ts` combines `clsx` and `tailwind-merge`

**Design Token Structure:**

```css
:root {
  --radius: 0.65rem;
  --background: oklch(1 0 0);
  --foreground: oklch(0.141 0.005 285.823);
  --card, --primary, --secondary, --accent, --muted, --destructive...
}
```

**Component Styling Pattern:**

```tsx
import { cn } from "@/lib/util/utils";

<div
	className={cn(
		"base-classes",
		variant && variantClasses[variant],
		className
	)}
/>;
```

## Coding Standards & Best Practices

### TypeScript Guidelines

1. **Strict Mode Enabled:** All code must pass TypeScript strict checks
2. **Interface Naming:**
    - Props interfaces: `ComponentNameProps` (e.g., `PostCardProps`)
    - Response types: `[Entity]Response` (e.g., `PostResponse`)
    - Request types: `[Action][Entity]Request` (e.g., `AddPostRequest`)
3. **Type Safety:**
    - Use typed Redux hooks from `src/app/hooks.ts`
    - Define return types for all functions
    - Avoid `any` unless absolutely necessary (ESLint allows but discouraged)
4. **Import Aliases:**
    - Use `@/` for `./src/` imports
    - Use `@assets/` for assets
    - Use `@views/` for views

### Component Guidelines

1. **Functional Components:** Always use function components with hooks
2. **Props Interface:** Define props interface above the component
3. **Default Exports:** Use default exports for page components, named exports for utilities
4. **Memoization:** Use `React.memo()` for expensive re-renders, `useMemo/useCallback` when needed
5. **Error Boundaries:** Wrap route components in `<ErrorBoundary>`
6. **Loading States:** Use `PageLoader` for page-level, `ComponentLoader` for components

**Example Component Structure:**

```tsx
import { cn } from "@/lib/util/utils";
import { useAppSelector } from "@/app/hooks";

interface PostCardProps {
	post: Post;
	onAction?: () => void;
	className?: string;
}

const PostCard = ({ post, onAction, className }: PostCardProps) => {
	// Hooks first
	const user = useAppSelector((state) => state.auth.user);

	// Event handlers
	const handleClick = () => {
		onAction?.();
	};

	// Render
	return (
		<div className={cn("base-classes", className)}>
			{/* Component JSX */}
		</div>
	);
};

export default PostCard;
```

### RTK Query Patterns

1. **Slice Creation:** Extend base `apiSlice` using `injectEndpoints()`
2. **Naming Convention:**
    - Slice file: `[feature]Slice.ts`
    - Endpoints: `get[Entity]`, `add[Entity]`, `update[Entity]`, `delete[Entity]`
    - Generated hooks: `use[Get|Add|Update|Delete][Entity]Query|Mutation`
3. **Tag Invalidation:** Always define `providesTags` and `invalidatesTags`
4. **Transformers:** Use transformer functions to normalize API responses
5. **Infinite Queries:** Use `builder.infiniteQuery` for paginated feeds

**Example Slice:**

```typescript
export const postsApiSlice = apiSlice.injectEndpoints({
	endpoints: (builder) => ({
		getPosts: builder.query<Post[], void>({
			query: () => "/posts",
			transformResponse: (response: PostResponse[]) =>
				response.map(postResponseTransformer),
			providesTags: (result) =>
				result
					? [
							...result.map(({ id }) => ({
								type: "Post" as const,
								id,
							})),
							"Post",
						]
					: ["Post"],
		}),
		addPost: builder.mutation<Post, AddPostRequest>({
			query: (body) => ({
				url: "/posts",
				method: "POST",
				body,
			}),
			invalidatesTags: ["Post"],
		}),
	}),
});

export const { useGetPostsQuery, useAddPostMutation } = postsApiSlice;
```

### Custom Hooks Guidelines

1. **Naming:** Always prefix with `use` (e.g., `useAuthCheck`, `useRefetchOnLogin`)
2. **Location:** Place in `src/hooks/` for shared hooks
3. **Dependencies:** Always include all dependencies in hook dependency arrays
4. **Return Pattern:** Use array destructuring for multiple values, object for named returns
5. **Auth Hooks:** Use centralized auth hooks (`useCurrentUser`, `useAuthReady`)

**Common Hooks:**

- `useCurrentUser()` - Get current Firebase user from AuthStateManager
- `useAuthReady()` - Check if auth is initialized
- `useIsUserLoggedIn()` - Get logged-in state and profile loading state
- `useRefetchOnLogin(refetch)` - Refetch data when user logs in
- `useCallbackOnIntersection(callback)` - Intersection Observer for infinite scroll

### Error Handling

1. **Error Boundaries:** Wrap all route components in `<ErrorBoundary>`
2. **API Errors:** RTK Query handles errors in `error` field, display with toast
3. **Toast Notifications:** Use `react-hot-toast` for user feedback
4. **Loading States:** Always handle `isLoading`, `isFetching`, `isError` states
5. **Retry Logic:** Base query has automatic retry (3 attempts), fails on 401/404/400/429/500

### Performance Optimization

1. **Code Splitting:** Use `lazyWithRetry()` for route-level components
2. **Chunk Strategy:** Vite config splits into firebase, redux, ui, fonts, vendor
3. **Memoization:** Use `createSelector` for derived Redux state
4. **Entity Adapters:** Use for normalized state (posts, comments, etc.)
5. **Cache Strategy:** RTK Query caches for 5 minutes (`keepUnusedDataFor: 300`)
6. **Token Caching:** TokenManager caches tokens for 1 minute before expiry

### Environment Variables

**All environment variables must be prefixed with `VITE_`:**

```bash
# API Configuration
VITE_SERVER_URL=http://localhost:5027
VITE_INDEXED_DB_NAME="expertbridge.duckdns.org"
VITE_INDEXED_DB_VERSION=1

# Firebase Configuration
VITE_API_KEY="..."
VITE_AUTH_DOMAIN="expert-bridge.firebaseapp.com"
VITE_PROJECT_ID="expert-bridge"
VITE_STORAGE_BUCKET="..."
VITE_MESSAGING_SENDER_ID="..."
VITE_APP_ID="..."
VITE_MEAUSUREMENT_ID="..."

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

**Access in code:**

```typescript
import.meta.env.VITE_SERVER_URL;
```

### Docker & Deployment

**Multi-stage Dockerfile:**

1. **Build Stage:** Node Alpine, `npm ci`, `npm run build`
2. **Serve Stage:** Nginx Alpine, copy build artifacts, configure nginx

**Environment Variables in Docker:**

- All `VITE_*` vars must be passed as `ARG` in Dockerfile
- Set as `ENV` during build stage (Vite embeds at build time)
- Defined in `docker-compose.yml` under `build.args`

**Nginx Configuration:**

- SPA fallback: `try_files $uri $uri/ /index.html`
- Health check endpoint: `/health`
- Gzip compression enabled

## File Naming Conventions

- **Components:** PascalCase (e.g., `PostCard.tsx`, `AuthButtons.tsx`)
- **Hooks:** camelCase with `use` prefix (e.g., `useAuthCheck.ts`)
- **Utilities:** camelCase (e.g., `config.ts`, `utils.ts`)
- **Types:** PascalCase (e.g., `types.ts` exports `Post`, `PostResponse`)
- **Slices:** camelCase with feature name (e.g., `postsSlice.ts`, `authSlice.ts`)

## Import Order

Follow this import order (separated by blank lines):

1. React and React libraries
2. Third-party libraries
3. Internal absolute imports (`@/`)
4. Relative imports
5. Types (if separate)
6. CSS/Styles

```tsx
import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";

import { toast } from "react-hot-toast";

import { useAppSelector } from "@/app/hooks";
import { Post } from "@/features/posts/types";
import { cn } from "@/lib/util/utils";

import PostCard from "./PostCard";

import "./styles.css";
```

## Testing Strategy

**Current State:** No test files exist yet.

**Future Testing Approach:**

- Unit tests: Jest + React Testing Library
- Integration tests: RTK Query hooks, auth flow
- E2E tests: Playwright or Cypress
- Test location: Co-located with components (`[Component].test.tsx`)

## Common Pitfalls & Solutions

### 1. Multiple Auth Listeners

**Problem:** Creating multiple `onAuthStateChanged` listeners causes memory leaks.
**Solution:** Always use `useCurrentUser()` from `AuthStateManager`, never create new listeners.

### 2. Token Refresh

**Problem:** Calling `getIdToken()` on every request is slow.
**Solution:** Use `tokenManager.getToken()` which caches tokens for 1 minute.

### 3. Chunk Load Failures

**Problem:** Code splitting can fail if chunks aren't available.
**Solution:** Use `lazyWithRetry()` wrapper which retries failed chunk loads 3 times.

### 4. API Health on Startup

**Problem:** Frontend loads before backend is ready.
**Solution:** App checks API health on mount, shows user-friendly error if backend is down.

### 5. Route Protection

**Problem:** Unauthenticated users accessing protected routes.
**Solution:** Use `<ProtectedRoute>` wrapper which redirects to login if not authenticated.

## When Making Changes

### Adding a New Feature

1. Create feature folder in `src/features/[feature]/`
2. Define types in `types.ts`
3. Create RTK Query slice in `[feature]Slice.ts`
4. Add tag types to `apiSlice.ts` if needed
5. Create UI components in `src/views/components/common/[feature]/`
6. Create page components in `src/views/pages/[feature]/`
7. Add routes in `src/routes.tsx`
8. Export hooks from slice

### Adding a New UI Component

1. If shadcn/ui component: Use `npx shadcn-ui@latest add [component]`
2. If custom: Create in `src/views/components/custom/` or feature folder
3. Define props interface above component
4. Use `cn()` utility for className merging
5. Export as default for pages, named export for utilities
6. Document props with JSDoc if complex

### Adding Environment Variables

1. Add to `.env` file with `VITE_` prefix
2. Add to `docker-compose.yml` under `build.args`
3. Add `ARG` declaration in `Dockerfile`
4. Add to `ENV` statement in `Dockerfile`
5. Access via `import.meta.env.VITE_[NAME]`

### Modifying API Integration

1. Update types in `src/features/[feature]/types.ts`
2. Update slice in `src/features/[feature]/[feature]Slice.ts`
3. Update tag invalidation if needed
4. Test with dev backend (`npm run dev`)

## Commands Reference

```bash
# Development
npm run dev              # Start Vite dev server (port 5173)
npm run build            # Build for production
npm run preview          # Preview production build
npm run lint             # Run ESLint

# Docker
docker-compose up --build         # Build and run in Docker
docker-compose down              # Stop containers

# Environment
cp .env.example .env            # Create local .env file
```

## Additional Resources

- **Vite Docs:** https://vitejs.dev/
- **React Router v7:** https://reactrouter.com/
- **Redux Toolkit:** https://redux-toolkit.js.org/
- **RTK Query:** https://redux-toolkit.js.org/rtk-query/overview
- **Firebase Auth:** https://firebase.google.com/docs/auth
- **shadcn/ui:** https://ui.shadcn.com/
- **Tailwind CSS:** https://tailwindcss.com/

## Questions to Ask When Unsure

1. **State Management:** Should this be local state or Redux? (Use Redux for shared/persisted state)
2. **Auth:** Am I creating a new auth listener? (No, use `useCurrentUser()`)
3. **API Call:** Should I use RTK Query or fetch? (Always use RTK Query for API calls)
4. **Styling:** Should I use inline styles or className? (Always use className with Tailwind)
5. **Routing:** Is this a new page or a component? (Pages go in `views/pages/`, components in `views/components/`)
6. **Types:** Where do types belong? (In `types.ts` within the feature folder)

---

**Last Updated:** November 23, 2025
**Project Version:** 0.0.0
**React Version:** 19.0.0
**TypeScript Version:** 5.6.2
