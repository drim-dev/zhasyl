# Station Zhasyl-1

Station Zhasyl-1 is a story-driven learning product where children learn programming and science
by solving practical problems for a future settlement on Mars.

The repository is an early product foundation. It currently contains the world and product
documentation, a private ASP.NET Core API, a Next.js frontend/BFF, and local orchestration
through .NET Aspire.

## Prerequisites

- .NET SDK 10.0.300
- Node.js 24.9.0
- npm 11.6.0
- Docker Desktop or another Docker-compatible runtime
- an HTTPS development certificate is recommended for the Aspire dashboard

The exact .NET and Node versions are declared in global.json and .nvmrc.

## Install

~~~bash
make restore
~~~

## Run locally

~~~bash
make dev
~~~

Aspire starts PostgreSQL 17, Azurite, the API, and the frontend. It injects private service and
storage addresses, applies EF Core migrations, seeds MDX content, and exposes the browser
application at http://localhost:3000. The dashboard URL and login token are printed to the
terminal. PostgreSQL and Azurite use named development volumes so their state survives restarts.

The browser does not call the ASP.NET Core API directly. The station overview is rendered on the
Next.js server from the private API response. The interface follows the operating-system colour
preference on first use and preserves an explicit light or dark theme choice on the device.

## Verify

~~~bash
make verify
make test-e2e
make format-check
~~~

make test-e2e starts the complete application through Aspire and exercises desktop, tablet,
mobile, and keyboard journeys.

## Repository map

~~~text
Zhasyl.AppHost/          Aspire local orchestration
Zhasyl.ServiceDefaults/  Health, resilience, telemetry, and service discovery defaults
backend/src/Zhasyl.Api/  Private ASP.NET Core API
backend/tests/           Backend component and validator tests
frontend/                Next.js frontend and BFF
content/                 Localized station, laboratory, and mission MDX source
docs/designs/            Planned product and architecture decisions
docs/specs/              Current implemented behavior
docs/plans/              Implementation and validation plans
~~~

Code and technical documentation are written in English. Learner-facing materials are authored
in their publication locale, beginning with Russian.

See [Content authoring](docs/guides/content-authoring.md) for the frontmatter contract and revision
behavior.
