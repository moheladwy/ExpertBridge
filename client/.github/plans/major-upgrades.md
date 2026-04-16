# Major Package Upgrades Plan

**Packages:** `vite` 7→8 · `typescript` 5→6 · `eslint` 9→10 · `@eslint/js` 9→10  
**Date Created:** 2026-04-16  
**Status:** Pending confirmation

---

## Overview

Four packages remain after the safe upgrades. Each has breaking changes that require
targeted code edits. The order of execution matters: TypeScript first (smallest scope),
then ESLint, then Vite last (largest scope).

---

## Package 1 — `typescript` ~5.9.3 → ~6.0.2

### What breaks
| Breaking Change | Impact on this project |
|---|---|
| `baseUrl` deprecated — no longer a module resolution root | **Directly used** in `tsconfig.app.json` |
| `strict` now defaults to `true` | Already `true` — no change needed |
| `target` defaults to current-year ES | Already explicitly set to `ES2020` — no change needed |
| `types` defaults to `[]` | No `@types/node` used in browser code — no change needed |
| `moduleResolution: node` deprecated | Using `bundler` — no change needed |
| `--module amd/umd/systemjs` removed | Using `ESNext` — no change needed |
| `--esModuleInterop false` no longer allowed | Not set to false — no change needed |
| `outFile` removed | Not used — no change needed |

### Files to change

**`tsconfig.app.json`** — 1 edit

Remove `"baseUrl": "."`. The `paths` entries already use explicit relative prefixes
(`"./src/*"`) so path mapping continues to work without `baseUrl`.

```jsonc
// BEFORE
"baseUrl": ".",
"paths": {
  "@/*": ["./src/*"]
}

// AFTER
"paths": {
  "@/*": ["./src/*"]
}
```

---

## Package 2 — `eslint` ^9.39.2 → ^10.2.0 · `@eslint/js` ^9.39.2 → ^10.0.1

### What breaks
| Breaking Change | Impact on this project |
|---|---|
| Old `.eslintrc` format removed | **Not affected** — already using flat config (`eslint.config.js`) |
| `eslint:recommended` adds 3 new rules | **May surface new lint errors** — see below |
| JSX references now tracked | May fix false positives in `no-unused-vars` for JSX — benign |
| `eslint-env` comments now errors | Not used in codebase — no change needed |
| Node.js < v20.19 dropped | Dev environment is fine |
| `name` property added to core configs | No action needed |

### New `eslint:recommended` rules (3 additions)

These three rules are newly enabled in v10 and could surface errors in source files:

1. **`no-unassigned-vars`** — variables declared but never assigned  
2. **`no-useless-assignment`** — assignments whose value is never read  
3. **`preserve-caught-error`** — caught error variables must be used (no empty `catch (e) {}` blocks)

### Files to change

**`eslint.config.js`** — 0 structural changes needed.

The config already uses flat format and will work with ESLint 10 as-is.  
After upgrading, run `npm run lint` to discover any violations from the 3 new rules.
If violations exist, they will be fixed in source files individually (not by disabling rules).

---

## Package 3 — `vite` ^7.3.2 → ^8.0.8

### What breaks
| Breaking Change | Impact on this project |
|---|---|
| `build.rollupOptions` renamed to `build.rolldownOptions` | **Directly used** in `vite.config.ts` — object form of `manualChunks` also removed |
| `build.commonjsOptions` is now a no-op | **Directly used** — must be removed |
| Bundler changed to Rolldown (esbuild → Oxc) | Transparent for this project — no esbuild config used |
| CSS minification changed to Lightning CSS | Transparent — no `build.cssMinify` override set |
| `esbuild` is no longer a direct Vite dependency | `build.minify: "terser"` is used, so `terser` is the minifier — unaffected |
| CJS interop change | Using ESM throughout — low risk |
| `import.meta.hot.accept` URL form removed | Not used — no change needed |

### Files to change

**`vite.config.ts`** — 2 edits

#### Edit 1 — Remove `commonjsOptions` (now a no-op)

```typescript
// REMOVE this entire block:
// CommonJS options
commonjsOptions: {
    include: [/node_modules/],
    transformMixedEsModules: true,
},
```

#### Edit 2 — Rename `rollupOptions` → `rolldownOptions`

The function form of `manualChunks` still works in Vite 8 (with a deprecation warning)
and produces identical output, so the chunk splitting logic is preserved as-is. Only the
top-level key name changes.

```typescript
// BEFORE
rollupOptions: {
    output: { ... }
}

// AFTER
rolldownOptions: {
    output: { ... }
}
```

> **Note on `manualChunks` function form:** The function-based `manualChunks` is deprecated
> in Vite 8 but still functional. Migrating to Rolldown's `codeSplitting` API is a
> separate, optional future task and is NOT part of this upgrade plan — it would require
> significant rework of the chunk strategy.

---

## Execution Order

```
Step 1  ncu -u --filter "typescript"          → update package.json
Step 2  Edit tsconfig.app.json                → remove baseUrl
Step 3  npm install
Step 4  npx tsc --noEmit                      → verify no TS errors

Step 5  ncu -u --filter "eslint,@eslint/js"   → update package.json
Step 6  npm install
Step 7  npm run lint                          → check for new rule violations
Step 8  Fix any lint errors in source files

Step 9  ncu -u --filter "vite"                → update package.json
Step 10 Edit vite.config.ts                   → remove commonjsOptions, rename rollupOptions
Step 11 npm install
Step 12 npm run build                         → verify production build succeeds
Step 13 npm run dev                           → smoke-test dev server
```

---

## Risk Assessment

| Package | Risk | Reason |
|---|---|---|
| `typescript` 5→6 | **Low** | Single `baseUrl` removal in tsconfig |
| `eslint` 9→10 | **Low** | Flat config already in use; 3 new rules may add warnings |
| `vite` 7→8 | **Medium** | Bundler internals changed; build output could differ subtly |

---

## Files Modified Summary

| File | Changes |
|---|---|
| `package.json` | Version bumps for 4 packages |
| `tsconfig.app.json` | Remove `"baseUrl": "."` |
| `vite.config.ts` | Remove `commonjsOptions`; rename `rollupOptions` → `rolldownOptions` |
| `eslint.config.js` | No structural changes (verify after lint run) |
