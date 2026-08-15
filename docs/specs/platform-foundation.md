# Platform Foundation

The platform serves localized Station content through a server-rendered Next.js application
backed by a private ASP.NET Core API. PostgreSQL is the runtime source of truth;
repository-owned MDX files are idempotently seeded on startup.

## Data Model

| Entity | Stable identity | Localized or versioned data |
|---|---|---|
| Station | UUID and locale-neutral slug | Name, location, and briefing per locale |
| Laboratory | UUID, station, slug, order, publication state | Name, purpose, and specialist per locale |
| Mission | UUID, laboratory, slug, order, publication state | Immutable numbered revisions per locale |
| Mission revision | UUID, mission, locale, version | Name, problem, status, raw MDX, hash, and publication timestamps |
| Station assignment | UUID, mission, slug, order, publication state | Immutable numbered revisions per locale |
| Assignment revision | UUID, assignment, locale, version | Name, objective, estimated minutes, raw MDX, hash, and publication timestamps |
| Adult account | UUID | Normalized email and timestamps |
| OAuth identity | UUID, provider and provider subject | Provider email and last sign-in time |
| Child profile | UUID and owning adult | Display name and learning locale |
| Pairing code | UUID and child profile | Hashed code, expiry, and consumption time |
| Child device session | UUID and child profile | Hashed token, device label, expiry, and revocation time |
| Learner workspace | UUID, child profile, and stable assignment | Pinned assignment revision, current version, and timestamps |
| Workspace snapshot | UUID, workspace, and version | Private blob name, content hash, byte length, and save time |

Exactly one mission revision and one assignment revision may be current for each stable identity
and locale. Future learner evidence can pin revision UUIDs rather than silently changing when
authored content is updated.

## API Endpoints

| Method | Path | Auth | Description |
|---|---|---|---|
| GET | /api/station/overview?locale=ru | Anonymous | Returns the published station and laboratory overview |
| GET | /api/laboratories/{laboratorySlug}/missions/{missionSlug}?locale=ru | Anonymous | Returns the current mission revision and ordered assignment summaries |
| GET | /api/laboratories/{laboratorySlug}/missions/{missionSlug}/assignments/{assignmentSlug}?locale=ru | Anonymous | Returns the current assignment revision |
| POST | /api/auth/oauth-sign-in | Private BFF | Resolves or creates an adult from a social identity |
| GET/POST | /api/adult/children | Adult | Lists or creates child profiles |
| POST | /api/adult/children/{childId}/pairing-codes | Adult | Issues a one-use pairing code |
| DELETE | /api/adult/children/{childId}/devices/{deviceId} | Adult | Revokes a paired browser |
| POST | /api/child/pair | Anonymous, rate-limited | Exchanges a code for a device session |
| GET | /api/child/session | Child device | Resolves the current child profile |
| GET/PUT | /api/child/workspaces/{assignmentRevisionId} | Child device | Restores or version-saves assignment source |
| GET | /health | Anonymous in development | Reports readiness |
| GET | /alive | Anonymous in development | Reports process liveness |

The content endpoints are private application APIs consumed by the Next.js BFF. Authentication
and pairing contracts are specified in [Identity and Device Pairing](identity-and-device-pairing.md).

## Authored Content and MDX

- Russian (`ru`) is the only authored and published locale.
- Locale-neutral station, laboratory, mission, and assignment IDs do not contain translated text.
- Authored files follow the logical hierarchy under `content/{locale}/`.
- Mission and assignment pages are server-rendered from the exact revision returned by the API.
- Repository-authored MDX is trusted content, but it is still constrained before evaluation.
- The AST validator rejects ESM imports, JavaScript expressions, raw HTML, expression-valued
  attributes, and unknown JSX components.
- The renderer exposes only Station learning primitives and explicitly registered visualizations.
- External links accept HTTP or HTTPS only and open with an isolated browsing context.
- GitHub-flavoured Markdown tables render as semantic tables.

The current component allow-list includes station messages, researcher notes, prediction prompts,
three-level hints, system criteria, journal prompts, figures, two mission visualizations, and the
Python workbench. Visualizations use semantic HTML controls, work in both themes, and provide a
table or textual alternative for essential information.

Arbitrary administrative or learner-authored MDX must not pass through this renderer. Adding a
content authoring UI requires a separate sanitisation and trust design.

## Browser Python Workspace

Both published assignments contain a runnable Python workspace:

- Pyodide runs inside a native module Web Worker, not on the UI thread;
- the Python runtime, standard library, and WebAssembly files are served by the application;
- `npm run sync:pyodide` copies the pinned package assets before development and production builds;
- a run can be stopped by terminating the worker and is terminated automatically after 45 seconds;
- stdout, stderr, and Python exceptions appear in the assignment output panel;
- stable check codes are evaluated deterministically from source and observed output;
- the current draft is cached in browser local storage under a stable assignment key;
- an unpaired browser restores its local draft;
- a paired browser automatically saves immutable source versions to private Blob Storage with
  ownership and version metadata in PostgreSQL;
- a second paired browser restores the latest durable source;
- optimistic version checks prevent silent cross-device overwrites;
- the current source can be downloaded as a Python file.

This remains a single-file executable product slice. It does not yet expose version history,
multi-file projects, persisted run output, authoritative evidence, or JupyterLite notebooks. See
[Learner Workspaces](learner-workspaces.md) for synchronisation and conflict behavior.

## Interface

- The overview presents bioinformatics and materials as independent laboratory choices.
- Each laboratory card links to a mission reader and its ordered assignments.
- BioScout and Sealant No. 17 each publish one complete first assignment draft.
- Mission and assignment routes provide loading, unexpected-error, and not-found states.
- The interface supports keyboard navigation and desktop, tablet, and narrow-screen layouts.
- Light and dark themes use shared Station semantic tokens.
- The first visit follows the operating-system theme; a manual choice is stored on the device and
  applied without a hydration mismatch.
- Figures can be expanded through a keyboard-accessible dialog and remain usable at narrow widths.

## Content Seeding

Startup seeding runs in dependency order: station, laboratories, missions, then assignments.

- Station and laboratory translations are updated in place by locale.
- Mission and assignment metadata are updated without changing stable identities.
- An unchanged content hash does not create another revision.
- Changed frontmatter or MDX creates the next immutable numbered revision and keeps prior revisions.
- Mission and assignment revisions are versioned independently.
- Invalid frontmatter, empty versioned bodies, non-positive order or estimated time, and missing
  parent references stop initialization.
- Source files are copied into API build and publish output.
- Removing a source file does not currently unpublish persisted content.

## Local Infrastructure

Aspire orchestrates PostgreSQL 17 and Azure Storage emulation through Azurite. Both use named
development volumes. The API receives the `zhasyl` PostgreSQL connection string and `blobs`
storage reference through Aspire service discovery. The local AppHost, dashboard, telemetry, and
resource service endpoints use HTTP and do not require a development certificate. Blob storage
stores private learner workspace snapshots.

Normal development uses named PostgreSQL and Azurite volumes. Playwright starts the same AppHost
with ephemeral infrastructure, preventing test identities and workspace blobs from contaminating
the developer's persisted local state.

## Not Implemented Yet

- authoritative check-run and evidence persistence;
- progress and mission unlocking;
- reflections stored outside the learner's own notes;
- adult session summaries;
- JupyterLite scientific journals;
- production OAuth application configuration, recovery, and consent records;
- reviewed Kazakh interface catalogs and authored content.
