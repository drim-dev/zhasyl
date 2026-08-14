# AGENTS.md

## Product

The user-facing product is **Station Zhasyl-1** (`Станция «Жасыл-1»`), a story-driven
learning platform for children and families. The station exists to prepare and build large,
permanent human settlements on Mars. It is the setting and guide through programming, science,
mathematics, engineering, and other fields; do not introduce a separate Zhasyl brand, academy,
or fictional universe above it.

The learning hierarchy is:

- laboratories are broad subject areas;
- missions are substantial learning paths with a story and final product;
- station assignments are individual learning sessions;
- scientific journals are executable notebooks and reflections;
- system checks verify observable outcomes.

BioScout is an initial mission in the bioinformatics laboratory. MatterLab is a working name for
an initial mission in the materials laboratory, not a separate product.

This is a single monorepo. Do not introduce cross-repository dependencies or assume that a
mission, adult guide, backend service, or frontend application lives elsewhere.

## Language policy

- Write code, identifiers, file and directory names, comments, tests, commit messages, README
  files, architecture documents, plans, ADRs, and operational documentation in English.
- Russian is the first authored product locale; Kazakh is a required future product locale and
  must be supported by the architecture from the first implementation.
- Write learner-facing content, adult guides, missions, theory, quizzes, hints, laboratory
  journals, and user-facing product copy in the content locale.
- Keep code identifiers inside localized station assignments in English.
- Store localized authored content under `content/{locale}/`; initially `content/ru/`, with
  `content/kk/` using the same stable content identifiers and hierarchy when Kazakh content is
  introduced.
- Use English slugs and stable machine identifiers; use localized display titles.
- Do not mix prose languages inside one technical document unless quoting learner-facing copy.
- Never use localized display text as a database key, route identity, analytics dimension, or
  progress identifier.
- Do not treat machine translation as publishable Kazakh learning content. Kazakh terminology,
  scientific accuracy, and natural phrasing require human review.

Use `.agents/skills/writing-russian-content/SKILL.md` before writing or editing Russian content.

## Architecture

The intended stack is:

- .NET and ASP.NET Core for the private backend API.
- PostgreSQL with Entity Framework Core for persistent data.
- Next.js with TypeScript and the App Router for the frontend and BFF.
- .NET Aspire for local orchestration, service discovery, configuration, health checks, and
  observability.
- JupyterLite for browser-based learner notebooks where a mission needs executable Python.

The request flow is:

```text
Browser -> Next.js frontend/BFF -> private ASP.NET Core API -> PostgreSQL
                         |
                         +-> static mission assets and JupyterLite
```

The browser must not call the ASP.NET Core API directly. Next.js Server Components, Server
Actions, and route handlers may call it on the server side. Keep the backend private in deployed
environments. Do not add CORS as a substitute for the BFF boundary.

Aspire is the only local orchestrator. Do not add Docker Compose for normal development unless
the user explicitly changes this decision.

## Target repository structure

Follow the established structure once scaffolding begins:

```text
Zhasyl.AppHost/                 # Aspire orchestration
Zhasyl.ServiceDefaults/         # Shared resilience and telemetry defaults
backend/
  src/Zhasyl.Api/               # ASP.NET Core API
  tests/Zhasyl.Api.Tests/       # Backend tests
frontend/                       # Next.js application and BFF
content/
  ru/                            # First authored locale
    laboratories/               # Labs, missions, and station assignments
    adult-guides/                # Parent or instructor material and spoilers
  kk/                            # Future Kazakh content with the same stable identifiers
docs/
  specs/                        # English current-state module specifications
  designs/                      # English design documents
  plans/                        # English implementation and test plans
```

Do not create speculative projects or layers. Add a directory when the product has a concrete
need for it.

## Backend conventions

- Organize the API by vertical slice. Keep each feature cohesive: endpoint, request, validator,
  handler, and response belong together unless size makes a split clearly better.
- Treat backend validation as authoritative. Frontend validation exists for user experience.
- Return RFC 7807 Problem Details for API errors.
- Keep business rules out of controllers, route glue, and the BFF.
- Use EF Core code-first migrations and review every generated migration before applying it.
- Use `AsNoTracking()` and projections for read-only queries.
- Prefer component tests through HTTP with a real PostgreSQL dependency over mocked repository
  tests.
- Add isolated validator tests for validation rules.

Before changing a backend feature, read these skills:

1. `.agents/skills/vertical-slice-architecture/SKILL.md`
2. `.agents/skills/component-testing/SKILL.md`
3. `.agents/skills/validation/SKILL.md`
4. `.agents/skills/error-handling/SKILL.md`

Also read `.agents/skills/database-ef-core/SKILL.md` for data-layer work.

## Frontend and BFF conventions

- Enable TypeScript strict mode. Do not use `any`; use a precise type or `unknown` with narrowing.
- Prefer Server Components. Add Client Components only for browser APIs, state, effects, or user
  interaction.
- Use Server Actions for server-owned mutations when they fit. Use route handlers when a browser
  client needs an HTTP endpoint or when the BFF must proxy a request.
- Keep secrets and backend addresses on the server.
- Pass backend Problem Details through the BFF without inventing a second error contract.
- Make every interactive control keyboard accessible and visibly focusable.
- Associate every form control with an accessible label.
- Design for children without making the interface childish: clear hierarchy, readable type,
  limited choices, and immediate feedback.
- Support phone, tablet, and desktop layouts.

Read `.agents/skills/frontend-architecture/SKILL.md` before changing `frontend/`. Also read the
validation and error-handling skills for forms and request flows.

## Learning content

- Every mission has a continuous story, a concrete final product, and observable progress after
  each station assignment.
- Each station assignment explains what the learner is trying to achieve and why the idea matters
  before introducing syntax.
- Keep mandatory theory discussable in 10–15 minutes. Put deeper explanations in an optional
  researcher note.
- Ask the learner to predict an outcome before running code.
- Separate fictional data, simplified models, hypotheses, computed results, and scientific facts.
- Adult guidance must include expected results, common errors, progressive hints, an easier path,
  and an extension path.
- Never publish real personal, medical, or genomic data from children.
- Do not provide child-directed instructions for hazardous household chemistry. Any physical
  experiment requires adult supervision, child-appropriate materials, explicit safety guidance,
  and authoritative sourcing.

Use `.agents/skills/content-visualization/SKILL.md` when a visual materially improves an assignment.
Do not add decorative diagrams that do not teach a relationship or process.

## JupyterLite

- Treat notebooks as laboratory journals for prediction, experimentation, evidence, and
  reflection.
- Keep reusable production logic in normal Python modules, not only in notebook cells.
- Make starter notebooks deterministic and safe to rerun from top to bottom.
- Do not place credentials or private service endpoints in browser notebooks.
- Synchronize learner work to durable application storage and provide an explicit download/export
  path; browser storage alone is not a durable backup.

## Code quality

- Prefer the smallest complete solution. Apply YAGNI.
- Prefer readable code over clever abstractions.
- Match surrounding conventions before introducing a new pattern.
- Comments explain why a non-obvious constraint exists; they do not narrate what code does.
- Names describe purpose, not implementation history. Avoid `New`, `Old`, `Legacy`, `Wrapper`,
  and similar temporal or pattern-padding names.
- Do not add a dependency until the existing platform cannot reasonably solve the problem.
- Do not suppress warnings or failing tests without explaining and resolving the cause.

Read `.agents/skills/code-style/SKILL.md` for naming and comment examples.

## Testing and verification

- Run the narrowest relevant tests while iterating, then run the affected project suites.
- Backend: component tests, validator tests, and focused unit tests for pure algorithms.
- Frontend: React Testing Library for components and Playwright for critical full-stack journeys.
- Content: validate links, frontmatter, code snippets, downloadable files, and notebook execution.
- Full-stack tests must start through an isolated Aspire test profile with disposable data.
- Never delete or weaken a test merely to make a build pass.
- Report commands run and any verification that remains outstanding.

## Specifications

Keep current-state module specifications in `docs/specs/`. Specs describe behavior and contracts,
not implementation history. Use `.agents/skills/spec-maintenance/SKILL.md` after changing a
documented module.

## Git and change safety

- Do not commit, push, publish, deploy, purchase services, or change DNS unless the user asks.
- Preserve unrelated user changes.
- Never use destructive Git commands to discard work.
- Discuss major architecture changes before implementing them. Routine work within the decisions
  in this file does not require a new architecture discussion.
