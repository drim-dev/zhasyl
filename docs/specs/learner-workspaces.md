# Learner Workspaces

The first workspace slice gives each child profile one durable Python workspace per station
assignment. The browser remains the execution environment; PostgreSQL stores ownership and
version metadata, while private Azure Blob Storage stores immutable source snapshots.

## Storage and Identity

A workspace is identified by the child profile and the stable station assignment. Its first save
pins the published assignment revision that supplied the starter code. Every successful save
increments an integer version and creates a new immutable `.py` blob. Metadata records the blob
name, SHA-256 content hash, UTF-8 byte length, and save time.

Workspace APIs accept the current assignment revision UUID so authored display slugs never become
learning-state keys. A child can read only workspaces owned by the profile resolved from the
device session. The browser never sends a child UUID as authority.

## Browser Synchronisation

The Python workbench always writes edits to local storage first. It then behaves according to the
browser state:

- an unpaired browser remains fully usable and labels the draft as local;
- a paired browser loads the durable snapshot when the assignment opens;
- if the server has no snapshot but the browser has a local draft, that draft is promoted to the
  server automatically;
- if both are known to match, the durable server snapshot refreshes the local cache;
- if a local draft contains unsynchronised work, the application preserves it, synchronises it
  only when its base version still matches, and otherwise presents an explicit choice to load the
  newer Station version;
- edits on a paired browser are sent after an 800 ms quiet period;
- a network failure preserves the local draft and reports that synchronisation is unavailable;
- an optimistic-version conflict preserves the local draft rather than overwriting work from
  another device.

The editor is briefly disabled while restoration is resolved. A learner can download the current
source as the assignment's named Python file at any time after restoration.

## API Contracts

The browser calls same-origin BFF routes. The BFF reads the HttpOnly child-session cookie and
forwards only the opaque device token to the private API.

| Method | BFF and private API path | Result |
|---|---|---|
| GET | `/api/child/workspaces/{assignmentRevisionId}` | Current version, pinned revision, source, and save time; version zero when no workspace exists |
| PUT | `/api/child/workspaces/{assignmentRevisionId}` | Saves source with an expected version and returns the new version |

Source is limited to 200,000 UTF-8 bytes. A stale expected version returns RFC 7807 Problem
Details with HTTP 409 and `workspace:save:conflict`. Missing publication state returns 404.
Authentication and validation errors follow the shared platform contracts.

## Deliberate Boundaries

This slice synchronises the single Python file used by each initial assignment. It does not yet
provide a version-history UI, named snapshots, restore of an older version, multi-file projects,
file import, Jupyter notebooks, persisted run output, authoritative check evidence, or automated
cleanup of superseded blobs. Those additions extend the Workspace concept rather than replacing
it.
