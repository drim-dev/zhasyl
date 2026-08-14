# Platform Foundation

The platform foundation serves localized Station content through a server-rendered Next.js
application backed by a private ASP.NET Core API. PostgreSQL is the runtime source of truth;
repository-owned MDX files are idempotently seeded on startup.

## Data Model

| Entity | Stable identity | Localized or versioned data |
|---|---|---|
| Station | UUID and locale-neutral slug | Name, location, and briefing per locale |
| Laboratory | UUID, station, slug, order, publication state | Name, purpose, and specialist per locale |
| Mission | UUID, laboratory, slug, order, publication state | Immutable numbered revisions per locale |
| Mission revision | UUID, mission, locale, version | Name, problem, status, raw MDX, hash, and publication timestamps |

Exactly one mission revision may be current for a mission and locale. A learner workspace can
later pin the revision UUID rather than silently changing when authored content is updated.

## API Endpoints

| Method | Path | Auth | Description |
|---|---|---|---|
| GET | /api/station/overview?locale=ru | Anonymous | Returns the published station and laboratory overview |
| GET | /api/laboratories/{laboratorySlug}/missions/{missionSlug}?locale=ru | Anonymous | Returns the current published MDX revision |
| GET | /health | Anonymous in development | Reports readiness |
| GET | /alive | Anonymous in development | Reports process liveness |

The content endpoints are private application APIs consumed by the Next.js BFF. Authentication
will be added before learner state is introduced.

## Key Behaviors

- Russian (`ru`) is the only authored and published locale.
- An omitted locale defaults to Russian.
- Locale-neutral station, laboratory, mission, and revision IDs do not contain translated text.
- A syntactically valid future locale such as `kk` requires no code change; it returns HTTP 404
  with `content:locale:read:not_published` until matching content is seeded.
- Invalid locale and slug values return HTTP 400 validation Problem Details with stable codes.
- Missing or unpublished mission content returns HTTP 404 with
  `content:mission:read:not_found`.
- Unexpected API failures return HTTP 500 Problem Details with a trace identifier and the stable
  error code `platform:request:execute:unexpected_failure`.
- The browser accesses the product through Next.js. The station page reads the API only on the
  server.
- The overview presents the bioinformatics and materials laboratories as independent choices.
- BioScout and Sealant No. 17 are shown as the first published mission shells.
- The page provides explicit loading, load-error, and not-found states.
- The page supports keyboard navigation and desktop, tablet, and narrow-screen layouts.
- The interface provides complete light and dark themes through Station semantic design tokens.
- The first visit follows the operating-system theme; a manual theme choice is stored on the
  device and applied before the first paint.

## Content Seeding

Authoring files live under `content/` and declare the `zhasyl.content/v1` frontmatter schema.
Startup seeding runs in dependency order: station, laboratories, missions, then mission revisions.

- Station and laboratory translations are updated in place by locale.
- Mission metadata is updated in place without changing its stable identity.
- An unchanged mission content hash does not create another revision.
- Changed frontmatter or MDX creates the next numbered revision and keeps prior revisions.
- Invalid frontmatter, empty mission bodies, or missing parent references stop initialization.
- Source files are copied into API build and publish output, so the same seeder works in local and
  deployed environments.

## Local Infrastructure

Aspire orchestrates PostgreSQL 17 and Azure Storage emulation through Azurite. Both use named
development volumes. The API receives the `zhasyl` PostgreSQL connection string and `blobs`
storage reference through Aspire service discovery. Blob storage is wired but not consumed until
learner workspace persistence is implemented.

## Authentication and Learner Persistence

Social authentication, child pairing, learner workspaces, progress, and blob-backed files are not
present in this foundation.
