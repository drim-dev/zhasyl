# First Vertical Slice Plan

## Objective

Deliver the smallest end-to-end, agent-free learning experience that validates the product
architecture with real content from both initial laboratories.

The slice is successful when a paired child browser can open one BioScout assignment and one
Sealant No. 17 assignment, run Python, receive a deterministic check, save work durably, resume it,
and produce a factual adult summary.

## Current Implementation Checkpoint

The agent-free content and execution checkpoint is implemented:

- both first assignments render from independently versioned Russian MDX revisions;
- both assignments include theory, scientific boundaries, prediction, three hints, reflection,
  primary-source provenance, and an applied Station consequence;
- accessible interactive visualizations render through a constrained MDX component allow-list;
- locally served Pyodide runs editable Python in a terminable browser worker;
- deterministic preview checks use stable codes and observable output;
- a draft restores after refresh in the same browser;
- Playwright covers both assignments, both themes, and desktop, tablet, and mobile reading layouts.

The complete vertical slice is not finished. Adult authentication, child pairing, durable backend
workspace versions, second-device restore, authoritative check evidence, reflections, progression,
JupyterLite, and the adult summary remain in the plan below. Local browser storage and client-side
preview checks must not be treated as substitutes for those application use cases.

## Slice boundaries

### Included

- local Aspire orchestration;
- Next.js frontend and BFF;
- private ASP.NET Core API;
- PostgreSQL and Azurite;
- one adult sign-in path;
- child profile and browser pairing;
- seeded Russian laboratory, mission, and assignment content;
- one assignment from each initial mission;
- browser Python execution;
- durable workspace save and resume;
- one progressive hint sequence per assignment;
- structured system checks and evidence;
- adult session summary;
- locale-neutral identifiers;
- responsive and accessible Station UI.

### Excluded

- complete BioScout and Sealant No. 17 missions;
- AI agent and model-provider calls;
- classes, teacher administration, and social features;
- arbitrary server execution;
- production deployment;
- Kazakh authored content beyond structural test fixtures;
- generic component or workflow frameworks not required by the two assignments.

## Content inputs

Before implementation of the content renderer is considered stable, author a thin but real draft
for each assignment.

### BioScout assignment

The assignment should include:

- the first plant-disease signal;
- a message from Larisa Kim;
- a short explanation of DNA sequence representation;
- a small synthetic sequence with an invalid symbol;
- a prediction prompt;
- executable Python for validating symbols;
- a deterministic check with multiple examples;
- three progressive hints;
- a scientific-journal reflection;
- a story consequence and saved artifact.

### Sealant No. 17 assignment

The assignment should include:

- a request from Zarema Dadaeva;
- clearly fictional material components and properties;
- a short explanation of proportions and model limitations;
- a prediction prompt;
- executable Python variables and arithmetic;
- a deterministic check that proportions form a valid whole;
- three progressive hints;
- a reflection explaining the chosen formulation;
- a story consequence and saved artifact.

These drafts are product fixtures, not throwaway demo copy. They may be revised after learner
testing but use the intended content model and scientific-provenance fields.

## Implementation order

### 1. Repository and toolchain

- create the .NET solution, Aspire AppHost, and ServiceDefaults projects;
- create the ASP.NET Core API and component-test project;
- create the Next.js App Router frontend with strict TypeScript;
- pin SDK and package-manager versions;
- add repository-level build, format, and test entry points;
- preserve the single-monorepo boundary.

Exit condition: all applications start through Aspire and expose healthy placeholder endpoints.

### 2. Application topology

- add PostgreSQL to Aspire;
- add Azurite for local blob storage;
- keep the backend private and expose the browser only through Next.js;
- configure service discovery, health checks, logs, and traces;
- prove frontend-to-BFF-to-API communication.

Exit condition: the browser displays a server-rendered health-backed Station page without direct
browser access to the API.

### 3. Identity and pairing

- implement one adult social sign-in path;
- create the minimum child profile;
- issue a short-lived pairing code or PIN;
- exchange it for a revocable child device session;
- authorise adult and child journeys separately;
- cover expiration, revocation, and invalid-code states.

Exit condition: an adult can pair and revoke a browser without giving the child an email account.

### 4. Versioned content

- define stable laboratory, mission, and assignment identifiers;
- define the minimal localized content manifest;
- seed Russian Markdown or MDX idempotently;
- preserve content versions referenced by learner state;
- report missing or structurally incompatible locale content;
- render the two real assignment drafts.

Exit condition: both assignments render from seeded content and a repeat seed does not duplicate
or invalidate learner state.

### 5. Station interface foundation

Implement only the primitives required by the two assignments:

- application shell;
- laboratory and mission context;
- station message;
- researcher note and provenance card;
- prediction and reflection prompt;
- notebook or code workbench;
- progressive hint panel;
- system-check result;
- save, offline, and synchronisation state;
- adult summary.

Use semantic tokens from the Station interface design direction. Verify desktop, tablet, and
narrow-screen layouts before adding visual variants.

Exit condition: the two assignments share a coherent interface without becoming identical pages.

### 6. Browser execution and workspace persistence

- pin JupyterLite/Pyodide and the allowed package set;
- run the two assignment exercises without network or credentials;
- represent notebooks and project files under one Workspace concept;
- cache locally and synchronise durable versions to the backend and blob storage;
- support explicit download;
- preserve work across refresh and a second paired browser;
- expose running, stopped, saving, saved, offline, and failed-sync states.

Exit condition: a learner can edit, run, save, resume, and download each assignment workspace.

### 7. Checks, evidence, and progression

- implement deterministic check contracts with stable result codes;
- render localized explanations separately from result codes;
- record check runs, hint levels, reflections, and confirmed evidence;
- complete the assignment only from observable criteria;
- keep later mission unlocking deterministic and agent-independent.

Exit condition: checks produce the same result for the same workspace, and saved evidence restores
with the session.

### 8. Adult summary

- summarise the changed artifact, check evidence, reflection, and deepest hint used;
- exclude raw workspace content that is not needed;
- keep the summary factual and localisable;
- cover the no-progress and needs-adult-help states.

Exit condition: the adult can understand what happened without watching the session.

### 9. End-to-end validation

Automate and manually exercise:

1. adult sign-in;
2. child profile creation;
3. pairing success, expiry, invalid code, and revocation;
4. opening both assignment types;
5. keyboard-only reading and workbench navigation;
6. Python run success and failure;
7. each progressive hint;
8. deterministic check failure and success;
9. save, refresh, offline, failed synchronisation, and recovery;
10. resume from a second paired browser;
11. adult summary;
12. Russian UI with representative longer Kazakh fixture text;
13. desktop, tablet, and narrow-screen layouts;
14. reduced-motion behaviour.

## Test strategy

- backend component tests use a real disposable PostgreSQL dependency through Aspire;
- validators receive focused tests for boundaries and invalid state;
- pure check algorithms receive focused unit tests;
- frontend views receive React Testing Library coverage;
- critical user journeys use Playwright;
- content validation checks manifests, links, frontmatter, code snippets, data files, and notebook
  execution;
- accessibility checks combine automated rules with keyboard and screen-reader-oriented review.

## Design checkpoints

Review UI after:

1. the Station shell and assignment reader;
2. the first executable workbench;
3. save, error, offline, and check-result states;
4. adult summary;
5. responsive integration of the full slice.

Do not defer all visual review to the end. A static happy-path screenshot is not acceptance.

## Learner validation

After the technical slice is reliable:

- run 4–6 early BioScout assignments with Ilya;
- run 4–6 early Sealant No. 17 assignments with Liza;
- record time, confusion, hint use, independent explanation, transfer performance, and desire to
  continue;
- revise assignment length and concept density before expanding either complete mission.

Do not interpret successful code execution alone as successful learning.

## Completion handoff

At slice completion, provide:

- commands for local startup and tests;
- architecture and content specs reflecting implemented behaviour;
- sample adult and child accounts or a documented local bootstrap;
- known limitations;
- learner-test findings;
- the recommended order for completing the two full missions.
