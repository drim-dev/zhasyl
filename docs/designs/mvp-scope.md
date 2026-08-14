# MVP Scope

## Status

This document defines the planned first releasable product. It is a target, not a description of
implemented behaviour.

## Product outcome

The MVP proves that two children with different starting knowledge can learn programming through
substantial, scientifically grounded Station Zhasyl-1 missions without installing a local
development environment or using an AI agent.

The release contains:

- the bioinformatics laboratory with the complete BioScout plant-disease mission;
- the materials laboratory with the complete Sealant No. 17 mission;
- one canonical assignment path per mission;
- durable browser workspaces, deterministic checks, progressive hints, and adult summaries;
- Russian content with locale-neutral product state ready for reviewed Kazakh content.

## Primary journeys

### Adult starts a learner

1. An adult signs in with a supported social identity provider.
2. The adult creates a child profile with only the information required by the product.
3. The product displays a short pairing code or PIN.
4. The child pairs a browser and receives a renewable device session.
5. The adult can revoke the device, inspect progress, export work, or delete the child profile.

### Learner completes an assignment

1. The child opens an independently chosen laboratory.
2. The product shows the current available mission and previews later missions.
3. The child opens the next station assignment.
4. The child reads the station problem and only the theory required for the current step.
5. The child records a prediction and works in a browser notebook or project.
6. The child runs code and a deterministic system check.
7. The product saves the workspace, check evidence, reflection, and assignment state.
8. The child sees the consequence of the work and the next assignment.
9. The same work can be resumed from another paired browser.

### Adult reviews a session

The adult sees:

- what artifact changed;
- what the learner explained or demonstrated;
- which progressive hints were used;
- which checks passed or still need work;
- whether an offline conversation or practical activity may help.

The summary is factual and concise. It is not a raw event log or surveillance view.

## Content scope

### Bioinformatics laboratory

The first mission is **BioScout: Code of the Red Planet**. The entire plant-disease investigation
remains one mission. Its assignments introduce FASTA, data quality, sequence comparison,
mutations, proteins, candidate markers, Python files, functions, modules, tests, visualisation,
and reporting as steps toward one final BioScout application.

The mission uses synthetic narrative data first and small, sourced open sequences where they add
real scientific value. Software output identifies candidates and uncertainty; it does not claim
to diagnose disease or replace laboratory validation.

### Materials laboratory

The first mission is **Sealant No. 17**. A beginner investigates synthetic material formulations
and gradually learns variables, numbers, conditions, loops, functions, tables, files,
visualisation, measurement, proportions, uncertainty, and model limitations.

The mission does not provide hazardous household chemistry instructions. A simulation is labelled
as a model. Any optional physical activity requires an adult, child-appropriate materials, and
explicit safety guidance.

## Functional scope

### Identity and consent

- social authentication for adults;
- child profiles without child email accounts;
- pairing codes or PINs for child browsers;
- revocable device sessions;
- adult-owned consent, export, and deletion;
- separate adult and learner locales.

### Authored content

- Markdown and MDX authored in the repository;
- stable English identifiers and localized display content;
- idempotent seeding into PostgreSQL;
- independent versioning and publication per locale;
- ordered missions and assignments;
- research-provenance cards and scientific-dossier metadata;
- protected presentation of adult guidance without claiming repository secrecy.

### Learning experience

- laboratory, mission, and assignment navigation;
- one canonical path and progressive authored hints;
- story, theory, researcher notes, predictions, reflections, and system checks;
- no authored difficulty modes;
- deterministic mission progression;
- no cross-laboratory prerequisites;
- no global station completion value.

### Workspaces

- JupyterLite/Pyodide scientific journals for executable investigation;
- a constrained browser project experience when assignments need files and modules;
- automatic durable workspace versions in application storage;
- explicit download and export;
- import of mission-supported data files;
- visible saving, saved, offline, synchronisation-failed, running, and stopped states.

### Evidence and adult summaries

- structured system-check results and stable evidence codes;
- assignment completion state;
- confirmed competency evidence;
- hint-use records;
- learner reflections;
- factual adult session summaries.

### Operations

- local orchestration through .NET Aspire;
- private ASP.NET Core API behind the Next.js BFF;
- PostgreSQL;
- Azure Blob Storage in production and Azurite locally;
- health checks, structured logging, traces, and deployment-ready configuration;
- deployment through a separate Argo CD repository to the existing Kubernetes cluster.

## Experience requirements

The interface follows the Station design direction:

- light, calm, precise, and near-future;
- serious without becoming adult or institutional;
- readable and accessible for children;
- responsive on desktop, tablet, and phone;
- coding optimised for larger screens without hiding reading or saved work on phones;
- WCAG 2.2 AA target;
- Russian-first layouts that tolerate longer reviewed Kazakh content;
- no dark neon cockpit, cartoon Mars theme, game HUD, streak pressure, or fake urgency.

## Agent-ready boundary

The MVP has no model-provider integration and remains complete without an agent. Application use
cases for assignment context, scoped workspace reads, checks, hints, evidence, and summaries are
designed so a later bounded agent can call them through authorised adapters.

The MVP does not contain placeholder chat UI, provider SDKs, prompt tables, conversation tables,
empty agent interfaces, or a generic tool framework.

## Explicitly out of scope

- AI learning agent;
- classes and school administration;
- general teacher workflows;
- child-to-child interaction;
- public child profiles, rankings, or shared progress;
- global station readiness;
- voice interaction;
- arbitrary server-side code execution;
- automatic GitHub publication by children;
- local desktop agent or CLI;
- visual content authoring interface;
- complete Kazakh mission publication before human review;
- more than the first complete mission in each initial laboratory.

## Release acceptance

The MVP is releasable when:

- both complete missions work without an AI agent;
- an adult and child can complete the primary journeys;
- work survives browser restart and can resume on a second paired device;
- every assignment has observable completion criteria and progressive hints;
- the two missions demonstrate both notebook and multi-file project work;
- scientific claims and datasets have reviewed provenance;
- Russian content is complete and the same identifiers can accept Kazakh content without
  migration;
- critical journeys pass automated component and end-to-end tests;
- the deployed system exposes health and operational diagnostics without exposing the private API;
- child privacy, consent, retention, and current provider requirements have been reviewed before
  public access.

## Open implementation decisions

Resolve these through the first vertical slice rather than speculation:

- supported social identity providers and authentication library;
- pairing-code lifetime and child session renewal policy;
- exact authored-content manifest and frontmatter schema;
- JupyterLite integration and pinned browser package set;
- browser project editor and runner boundary;
- workspace snapshot granularity and retention;
- first charting requirement and whether it justifies a dependency;
- font delivery strategy;
- exact Azure storage account and production lifecycle policy;
- initial deployment hostnames and GitOps repository naming.
