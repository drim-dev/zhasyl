# AGENTS.md

## Product

The user-facing product is **Station Zhasyl-1** (`Станция «Жасыл-1»`), a story-driven
learning platform for children and families. The station exists to prepare and build large,
permanent human settlements on Mars. It is the setting and guide through programming, science,
mathematics, engineering, and other fields; do not introduce a separate Zhasyl brand, academy,
or fictional universe above it.

The learning hierarchy is:

- laboratories are broad subject areas and ordered curricula;
- missions are substantial applied projects with a story, final product, and a defined position
  inside one laboratory;
- station assignments are individual learning sessions and incremental steps inside a mission;
- scientific journals are executable notebooks and reflections;
- system checks verify observable outcomes.

Laboratories are independent, and a learner may work in several at once. Core missions inside one
laboratory are sequential because later missions build on confirmed scientific and programming
knowledge. A mission remains one substantial station problem; do not split it into separate
missions merely to introduce a file format, syntax feature, algorithm, or scientific concept.

BioScout is the first mission in the bioinformatics laboratory. Its plant-disease investigation
remains one complete mission. MatterLab is a working name for
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
- Support complete light and dark themes through semantic tokens. Follow the system preference on first use, preserve an explicit device choice, and verify both themes.

Before changing `frontend/`, read both:

1. `.agents/skills/frontend-architecture/SKILL.md`
2. `.agents/skills/design-station-interface/SKILL.md`

Follow `docs/designs/interface-design-system.md` for visual and interaction decisions. Also read
the validation and error-handling skills for forms and request flows.

## Learning content

- Every mission has a continuous story, a concrete final product, and observable progress after
  each station assignment.
- Author one canonical assignment path and one set of completion criteria. Use progressive hints
  and optional post-completion investigations instead of authored difficulty modes.
- Introduce scientific and programming knowledge incrementally through station assignments while
  preserving the mission as one substantial applied problem.
- Keep laboratories mutually independent. Order core missions within a laboratory, and do not
  create prerequisites across laboratories.
- Each station assignment explains what the learner is trying to achieve and why the idea matters
  before introducing syntax.
- Keep mandatory theory discussable in 10–15 minutes. Put deeper explanations in an optional
  researcher note.
- Ask the learner to predict an outcome before running code.
- Separate fictional data, simplified models, hypotheses, computed results, and scientific facts.
- Adult guidance must include expected results, common errors, progressive hints, a small
  recovery example within the same path, and an optional post-completion investigation.
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

## Agent-ready MVP

The MVP must be complete and usable without an LLM or learning agent. Preserve an agent-ready
architecture through ordinary product boundaries rather than speculative AI abstractions:

- Author mission theory, the canonical assignment sequence, progressive hints, checks, and
  completion criteria in versioned content. An agent may retrieve or explain authored material
  later, but does not own the curriculum.
- Keep mission progression deterministic. The backend opens the next core mission from confirmed
  completion of the preceding mission in that laboratory; an agent must not reorder the
  curriculum or invent alternate difficulty paths.
- Implement mission context, workspace snapshots, system checks, authored hints, reflections,
  competency evidence, and activity history as cohesive application use cases. Future agent tools
  adapt to those use cases; they never query the database or blob storage directly.
- Return structured result and evidence codes from checks. Localized learner messages are a
  presentation concern and must not be the only machine-readable result.
- Record which actor produced a material action or observation (`learner`, `adult`, `system`, and
  later `agent`) without adding agent-specific ownership to core learning entities.
- Require explicit resource scope when workspace content is read. Design snapshots so a future
  agent can receive one selected file, notebook cell, error, or check result without receiving an
  entire account or device.
- Keep model providers, prompts, conversations, streaming, and token accounting outside the MVP.
  Add them only with the first agent-backed vertical slice.
- Keep the deterministic experience available when the agent feature is disabled or unavailable.

Do not add empty `IAgentService` interfaces, placeholder chat tables, provider SDKs, generic event
buses, or an MCP/tool framework merely to prepare for the future. The reusable seam is the tested
application behavior, not a speculative infrastructure layer.

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
