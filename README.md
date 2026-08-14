# Station Zhasyl-1

Station Zhasyl-1 is a story-driven learning product where children learn programming and science
by solving practical problems for a future settlement on Mars.

The repository contains a runnable product slice: a private ASP.NET Core API, PostgreSQL-backed
versioned Russian MDX, a Next.js frontend/BFF, two mission readers with interactive scientific
visualizations, and browser Python workspaces for the first BioScout and Sealant No. 17
assignments. Local orchestration uses .NET Aspire.

## Prerequisites

- .NET SDK 10.0.300
- Node.js 24.9.0
- npm 11.6.0
- Docker Desktop or another Docker-compatible runtime

The exact .NET and Node versions are declared in `global.json` and `.nvmrc`.

## Install

~~~bash
make restore
~~~

## Run Locally

~~~bash
make dev
~~~

Aspire starts PostgreSQL 17, Azurite, the API, and the frontend. It injects private service and
storage addresses, applies EF Core migrations, seeds MDX content, and exposes the browser
application at <http://localhost:3000>. The dashboard URL and login token are printed to the
terminal. The local dashboard and its internal control endpoints use HTTP, so no development
certificate is required. PostgreSQL and Azurite use named development volumes so their state
survives restarts.

The frontend automatically copies the pinned Pyodide runtime into generated public assets before
`dev` and `build`. Python then runs in a browser Web Worker without an external runtime CDN.

From the Station overview, open either laboratory and its first assignment:

- Bioinformatics: **BioScout — Check the signal from the agricultural complex**;
- Materials: **Sealant No. 17 — Balance formula No. 17**.

Each assignment includes Russian theory, an interactive visualization, prediction, editable and
runnable Python, deterministic checks, three hints, and journal questions. The code draft is
currently saved in that browser only. Accounts, server synchronisation, cross-device restore, and
JupyterLite are not implemented yet.

The browser does not call the ASP.NET Core API directly. Mission and assignment pages are rendered
on the Next.js server from private API responses. The interface follows the operating-system
colour preference on first use and preserves an explicit light or dark choice on the device.

## Verify

~~~bash
make verify
make test-e2e
make format-check
~~~

`make test-e2e` starts the complete application through Aspire, checks desktop, tablet, mobile,
keyboard, and theme journeys, and executes working Python solutions for both assignments.

## Repository Map

~~~text
Zhasyl.AppHost/          Aspire local orchestration
Zhasyl.ServiceDefaults/  Health, resilience, telemetry, and service discovery defaults
backend/src/Zhasyl.Api/  Private ASP.NET Core API
backend/tests/           Backend component and validator tests
frontend/                Next.js BFF, mission reader, visuals, and browser workspace
content/{locale}/        Localized station, laboratory, mission, and assignment MDX
docs/designs/            Product and architecture decisions
docs/specs/              Current implemented behaviour
docs/plans/              Implementation and validation plans
~~~

Code and technical documentation are written in English. Learner-facing materials are authored in
their publication locale, beginning with Russian. See
[Content authoring](docs/guides/content-authoring.md) for the frontmatter, MDX component, safety,
and revision contracts.
