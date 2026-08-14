---
name: frontend-architecture
description: Use when working on the Next.js frontend — pages, layouts, React components, data fetching, Server vs Client Components, Server Actions vs BFF API routes, or TypeScript in the frontend. Provides App Router structure, the Server-Components-first rule, and data-access patterns (project)
---

# Frontend Architecture (Next.js)

The frontend is Next.js (App Router) with TypeScript in strict mode. All UI MUST follow `frontend/DESIGN_SYSTEM.md`.

## When to Use This Skill

Use when adding or changing anything in `frontend/`:

- Pages, layouts, route groups
- React components (server or client)
- Data fetching and mutations
- BFF API routes
- TypeScript types in the frontend

## App Router Structure

- **`app/`**: pages and layouts (Next.js App Router).
- **Route groups**: `(auth)`, `(dashboard)`, etc. for layout organization.
- **Layouts**: `layout.tsx` (shared layout), `loading.tsx` (loading state), `error.tsx` (error boundary).

## Component Organization

- **`components/ui/`**: base/primitive components (Button, Input, Card, …).
- **`components/`**: feature-specific and shared components. Co-locate related components in feature folders when appropriate.

**ALL components MUST adhere to `frontend/DESIGN_SYSTEM.md`:** stone palette for light mode, gray for dark backgrounds/borders, universal focus ring on all interactive elements, defined button sizes (`md`/`lg`), typography with explicit line-heights, consistent spacing/shadows/radius. Never deviate without explicit approval.

Focus ring pattern: `focus:outline-none focus:ring-2 focus:ring-brand-500 dark:focus:ring-brand-600 focus:ring-offset-2 dark:focus:ring-offset-gray-950`

**Form controls MUST be associated with a label.** Every `<input>`/`<select>`/`<textarea>` needs a `<label htmlFor={id}>` with a matching `id` (or wrap the control in its `<label>`, or set `aria-label`). A bare sibling `<label>` is NOT associated — it breaks screen readers AND accessible-name queries (`getByLabel`, `getByRole('textbox', { name })`) in tests. Accessibility and testability are the same requirement here — write the identifier every time.

## Server vs Client Components

**CRITICAL RULE: ALWAYS TRY SERVER COMPONENTS FIRST.** Better performance, security, SEO, and less JS shipped.

Server Components can: fetch data directly from backend (no BFF), do server-side auth checks, access env vars securely, be async and await data directly.

**ONLY use Client Components (`'use client'`) when you MUST have:**
- Interactivity (onClick, onChange, event handlers)
- React state or effects (useState, useEffect, useContext)
- Browser-only APIs (localStorage, window, navigator, …)

Keep Client Components small — extract only the interactive parts.

**Preferred pattern:**
```
page.tsx (Server Component)
  ├─ fetch data from backend directly
  ├─ pass data to client components as props
  └─ InteractiveTable.tsx (Client Component — only the interactive table)
       └─ uses Server Actions for mutations
```

**Anti-pattern (avoid):**
```
❌ page.tsx ('use client')
     └─ useEffect to fetch from BFF
          └─ BFF route that proxies to backend
```

### Separate data-fetching from presentation (for testability)

Keep the async data-fetching Server Component a **thin shell**; put rendering in a **pure view** that takes data as props.

- **Shell** (`page.tsx` / async component): auth + fetch + delegate. No inline `.map`/markup over fetched data.
- **View** (`XxxView.tsx`): pure, `props → JSX`. Stays a Server Component unless it needs interactivity. Independently unit-testable with RTL (fixture props → DOM) — no DB, no browser.

```
GuideSectionPage (async Server Component)     ← shell: requireAuth() + fetch + delegate
  └─ <GuideSectionView section={section} />    ← pure view: props → JSX, RTL-testable
```

**Why:** an async Server Component that fetches AND renders inline traps its render logic — RTL/jsdom cannot cleanly render async Server Components, so inline rendering is only reachable by full E2E. Extracting the view makes render logic testable without a DB. Same instinct as separating I/O from logic on the backend.

**Scope:** default for NEW components. Do NOT retro-refactor existing pages wholesale — extract a view only when you're already changing/hardening that slice.

## Server Actions vs API Routes (BFF Layer)

**Prefer Server Actions for mutations:**
- `actions.ts` files with `'use server'`
- Call backend directly with user context from `requireAuth()` / `requireRole()`
- `revalidatePath()` to refresh data after mutations
- No BFF route needed

**API Routes (BFF) — ONLY when necessary** (`app/api/`):
- Client Components need to fetch data (rare — prefer Server Components)
- External webhooks call your API
- You need a public API endpoint

When you do create BFF routes: mirror backend endpoint structure; handle auth, request transformation, error handling.

## API Communication — Backend Access Patterns

**Server Components → Backend (direct):** call backend API directly, use `getServerSession()` for auth, pass user context via headers. Fewer hops, better performance. Default for page-load data.

**Client Components → BFF → Backend:** Client Components cannot call backend directly (browser security); the Next.js API route acts as the BFF and handles auth, transformation, errors. Use when a Client Component needs data (forms, dynamic updates).

## Frontend Feature Workflow

1. **ALWAYS start with a Server Component** — `app/[route]/page.tsx` (NO `'use client'`).
2. Fetch data directly from backend in the Server Component using `fetch()` with user headers.
3. Create Server Actions in `actions.ts` for mutations (NO BFF route).
4. Extract ONLY interactive parts into Client Components.
5. Pass data from Server Component to Client Components via props.
6. Client Components use Server Actions for mutations.
7. Create shared types in `types/` if needed.
8. Follow `frontend/DESIGN_SYSTEM.md` for ALL UI.

## TypeScript Guidelines

- Strict mode is ALWAYS enabled.
- No `any` (use `unknown` if truly dynamic).
- Prefer interfaces for object shapes.
- Use discriminated unions for variants.
- Explicit return types on functions.
- Define shared types in the `types/` directory.

## Gotchas

- **TypeScript paths**: configure path aliases in both `tsconfig` and Next.js.
- **App Router**: Server Components are the default — reach for `'use client'` only when required.
- **BFF auth**: NextAuth.js session cookies must be configured correctly.
- **Network isolation**: backend is not internet-accessible — only the BFF reaches it (no CORS needed).
