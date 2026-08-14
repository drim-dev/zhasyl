# Platform Architecture Direction

## Status

This document records agreed architectural boundaries before implementation. Details that have
not been validated through a vertical slice remain open.

## Repository and language

Station Zhasyl-1 is developed in one public monorepo, `drim-dev/zhasyl`. Code, identifiers,
technical documentation, operations, and repository metadata are English. Learner material,
parent or instructor guidance, mission narrative, and product copy are Russian.

Authored content lives in version-controlled Markdown/MDX and notebook files. The deployment
seeds versioned content into PostgreSQL so that the application can provide identity, progress,
saved work, search, and interactive experiences without making the database the authoring source.

MDX provides curated visualizations and interaction. Jupyter notebooks provide executable,
learner-owned investigation. They are complementary and should not be collapsed into one format.

## Application boundary

The intended stack is:

- ASP.NET Core backend organized by vertical slice;
- PostgreSQL and Entity Framework Core;
- Next.js App Router frontend and BFF;
- .NET Aspire for local orchestration and observability;
- JupyterLite and a browser Python runtime for initial executable workspaces;
- Azure Blob Storage for durable workspace artifacts;
- Azurite for local blob-storage development.

The request boundary is:

```text
Browser
  -> Next.js frontend and BFF
      -> private ASP.NET Core API
          -> PostgreSQL
          -> Azure Blob Storage
          -> model provider APIs
```

The browser does not call the private backend API or model providers directly. The BFF owns the
browser session and server-side calls. Model credentials never reach notebooks or learner code.

## Identity

Adults authenticate through social identity providers using the same general model as drim-dev.
A parent creates a child profile and pairs a child's device. The child then uses a simple device
session or PIN suitable for returning from another computer without managing an email account.

The parent owns consent and may inspect, export, or delete the child's saved work and AI history.
Teacher accounts, classes, invitations, and school administration are outside the initial scope.

## Content model

The domain hierarchy is:

```text
Laboratory -> Mission -> StationAssignment
```

Authored content has stable English machine identifiers and localized display text. The seeding
process must be idempotent, preserve learner state, and create a new authored-content version when
meaningful content changes. Progress points to stable identifiers and versioned completion
evidence rather than Markdown paths alone.

### Localization boundary

Russian (`ru`) is the initial locale and Kazakh (`kk`) is required in the product architecture
from the first vertical slice. Domain entities are locale-neutral. Localized content is attached
to stable identifiers such as `bioinformatics`, `bioscout`, and `fasta-quality-check`; titles and
translated slugs are never identities.

Authored files follow the same logical hierarchy under `content/ru/` and `content/kk/`. Each
localized mission version is seeded and published independently, because translations may be
reviewed and released at different times. A localization manifest or validation step must report
missing, outdated, and structurally incompatible translations.

The application must support:

- an adult account locale and a separate preferred learning locale per child;
- locale-aware routes and explicit language switching;
- UI message catalogs separate from authored MDX and notebooks;
- per-locale search indexes and metadata;
- locale-aware formatting for dates, numbers, and plural forms;
- Russian fallback only when explicitly indicated to the user;
- localized static accessibility labels, validation messages, and Problem Details presentation;
- language-independent telemetry, authorization, system checks, and progress.

Backend contracts return stable codes and structured values, not preformatted English or Russian
business messages. The BFF and content renderer select presentation text. Server logs and
technical diagnostics remain English.

MDX components and notebook scaffolding use stable English component, variable, and file names.
Visible prose, captions, starter comments intended for the learner, test explanations, and sample
data labels belong to the localized content version. Switching language never renames or forks a
learner's existing source files.

Protected adult material is an authorization concern even when authored beside public content.
The initial public repository cannot provide secrecy for committed answers. If genuinely private
answers become a requirement, they must live outside the public repository or be generated and
stored through a protected process. UI authorization must not be presented as source secrecy.

## Learner state

The backend saves:

- enrollment in independently chosen missions;
- assignment status and deterministic check results;
- competency evidence and learner reflections;
- workspace metadata and durable versions;
- concise AI-session summaries and safety records;
- parent-visible progress summaries.

There is no global station completion value. New content must not reduce a learner's previously
earned readiness.

## Deployment

Local development is orchestrated only through Aspire. Production deployment targets the
existing Kubernetes cluster and follows the established drim.dev Argo CD pattern through a new,
separate GitOps repository for Station Zhasyl-1.

The application repository does not contain cluster secrets. Production object storage uses the
existing Azure storage capability after its exact account and operational policy are confirmed.
Azurite is the default local substitute; no MinIO dependency is required.

## Initial vertical slice

The first useful slice should prove the complete path rather than scaffold every future role:

1. A parent signs in and opens a child profile.
2. The child starts one station assignment.
3. Versioned Russian MDX content is loaded from seeded content.
4. The child edits and runs a browser workspace.
5. Work is saved and restored from another session.
6. A deterministic system check records evidence.
7. The learning agent gives bounded help using the actual workspace context.
8. The parent receives a concise session summary.

The slice uses Russian content, but its identifiers, routes, persistence, and message catalogs
must pass a localization review showing that a Kazakh version can be added without a schema or
workspace migration.

## Explicitly deferred

- classes and school administration;
- a general teacher workflow;
- arbitrary server-side code execution;
- local desktop agents and CLI integration;
- automatic GitHub publication by children;
- voice interaction;
- a global station-progress mechanic.

## Related designs

- [Product concept](product-concept.md)
- [Learning workspaces and AI agent](learning-workspaces-and-ai-agent.md)
