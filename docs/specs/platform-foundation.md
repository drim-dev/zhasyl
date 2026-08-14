# Platform Foundation

The platform foundation serves a Russian station overview through a server-rendered web
application backed by a private API. It exposes the two initial laboratories and their first
planned missions without authentication or persisted learner state.

## Data Model

| Entity | Key Fields | Notes |
|---|---|---|
| Station overview | stationId, stationName, locale, location, briefing | Read-only localized response |
| Laboratory summary | id, name, purpose, specialist, firstMission | IDs are locale-neutral |
| Mission summary | id, name, problem, status | Describes the first mission in a laboratory |

No platform entities are persisted in the current foundation.

## API Endpoints

| Method | Path | Auth | Description |
|---|---|---|---|
| GET | /api/station/overview?locale=ru | Anonymous | Returns the published station and initial laboratory overview |
| GET | /health | Anonymous in development | Reports readiness |
| GET | /alive | Anonymous in development | Reports process liveness |

## Key Behaviors

- Russian (ru) is the only published API locale.
- An omitted locale defaults to Russian.
- An unpublished locale returns HTTP 400 Problem Details with the stable error code
  content:locale:read:not_published.
- Unexpected API failures return HTTP 500 Problem Details with a trace identifier and the stable
  error code platform:request:execute:unexpected_failure.
- The browser accesses the product through Next.js. The station page reads the API only on the
  server.
- The overview presents the bioinformatics and materials laboratories as independent choices.
- BioScout and Sealant No. 17 are shown as the first planned missions.
- The page provides explicit loading, load-error, and not-found states.
- The page supports keyboard navigation and desktop, tablet, and narrow-screen layouts.
- The interface provides complete light and dark themes through Station semantic design tokens.
- The first visit follows the operating-system theme; a manual theme choice is stored on the device and applied before the first paint.

## Content Structure

The station overview copy is application-owned localized data. Mission learning content remains
under content/{locale}/ and is not rendered or seeded by the current foundation.

## Authentication and Persistence

Authentication, child pairing, PostgreSQL, blob storage, and learner workspace persistence are
not present in the current foundation.
