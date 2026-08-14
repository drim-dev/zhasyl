# Learning Workspaces and AI Agent

## Decision

Station Zhasyl-1 is browser-first. Learners create folders, design programs, write code, inspect
errors, and keep scientific journals in the browser. The product must teach real project
structure without requiring Python, Git, an editor, or an AI CLI to be installed for the first
missions.

The AI capability is a bounded learning agent, not a generic chat assistant and not an autonomous
coding agent. Its objective is a working artifact that the learner understands and can recreate
in a related situation.

The MVP is intentionally complete without this agent. The agent is a later enhancement over
stable learning, workspace, and evidence capabilities; it must not become a prerequisite for
opening content, writing code, receiving authored help, running checks, saving progress, or
finishing a mission.

## Single authored path in the MVP

Every practical assignment has one stable identity, one canonical authored path, and one set of
completion criteria. The MVP does not publish separate guided, starter, or open versions.

A mission remains a substantial applied station problem. Its ordered station assignments
introduce the required scientific and programming knowledge in small steps while preserving one
continuous narrative and final artifact. Core missions are ordered within a laboratory because
later missions build on confirmed competencies and verified artifacts from earlier missions.
Laboratories remain independent and may be pursued in parallel.

Mission metadata declares required, introduced, practised, and optional advanced competencies.
The content owns progressive hints, core and optional theory, transfer questions, system checks,
and completion criteria. A small recovery example may help a learner return to the same path, and
an optional investigation may follow the core result; neither creates a separate difficulty mode.

The MVP can therefore teach a complete mission when no model is configured. A future agent may
explain the current step or choose the next progressive hint, but it must not reorder the
curriculum, invent alternate paths, or become the authority for mission progression.

## Agent-ready seams

The MVP exposes ordinary application capabilities that a future agent can use through narrow tool
adapters:

- resolve versioned mission and assignment context;
- list and read an explicitly scoped workspace resource;
- read the latest run output or traceback;
- run a deterministic system check and receive structured result codes;
- retrieve an authored hint by stable identifier and level;
- read confirmed competency evidence;
- record a provisional observation without promoting it to confirmed evidence;
- request adult help;
- build a factual adult summary.

The browser UI and future tools call the same authorized use cases. The tool layer is added with
the first agent slice; the MVP does not contain empty agent interfaces, provider SDKs, prompt
tables, conversation tables, or a generic tool framework.

The learning workbench reserves a support surface beside the editor. In the MVP it presents
authored theory, hints, checks, and reflection prompts. A future mentor panel can use the same
context and layout without replacing the editor or changing workspace persistence. Do not ship an
empty chat placeholder.

Workspace disclosure is resource-scoped from the beginning. A learner action can identify the
current file, selected notebook cell, latest error, or check result. The future agent receives
only those authorized resources rather than a whole workspace or device.

### Activity and evidence

Persist facts that are already valuable without AI:

- assignment session started and completed;
- authored hint requested and its level;
- workspace version saved;
- program run and structured check result;
- learner reflection submitted;
- competency evidence confirmed by deterministic or rubric-based checks.

Records use stable identifiers and explicit actor or source values. `agent` can be added as a new
actor later without changing the meaning or ownership of core entities. Agent observations remain
provisional until combined with deterministic results, explanation evidence, or a transfer task.

## Workspace model

Use `Workspace` as the durable product concept rather than making `Notebook` the root entity. A
workspace may be presented in two initial modes.

### Scientific journal

The journal is a Jupyter notebook used for predictions, small experiments, tables, charts,
evidence, and reflection. It is especially suitable for mathematics, data exploration,
bioinformatics, and materials simulations.

### Station project

The project workspace provides a constrained browser IDE with a file tree, code editor, runner,
test output, and mission panel. It supports real folders and files such as:

```text
virus-scanner/
├── README.md
├── data/
│   └── samples.fasta
├── src/
│   ├── parser.py
│   └── analyzer.py
└── tests/
    └── test_analyzer.py
```

The transition from a journal to a project is pedagogical, not a platform migration. Reusable
logic moves from experimental notebook cells into normal Python modules while the journal remains
the place for investigation and reports.

## Execution and persistence

Initial Python runs in the browser through the JupyterLite/Pyodide family of technologies. This
provides a consistent, safe environment without per-learner server containers. Missions must use
packages and APIs supported by the chosen browser runtime and be verified against a pinned build.

Browser-local storage is a working cache, never the sole durable copy. The application
automatically synchronizes versioned workspace snapshots to the backend and blob storage. A
learner can resume from a paired computer and can explicitly:

- save a named version;
- restore an earlier version;
- download the workspace;
- import supported files;
- later export a repository-ready project.

Server-side isolated execution may be added when a concrete mission needs native packages,
processes, or network services. It is not required for the initial product.

## Agent context

The agent is assembled from small, explicit components:

```text
Shared tutor policy
+ under-18 safety policy
+ fictional character profile
+ mission facts and rubric
+ learner evidence
+ explicitly shared workspace context
+ narrowly permitted tools
```

The agent may represent a fictional station specialist appropriate to the current laboratory.
Characters change voice and domain perspective; they do not replace shared pedagogy, safety, or
tool authorization. The interface must disclose that the specialist's messages are generated by
AI from mission materials.

The learner controls workspace disclosure through actions such as `Share this file`, `Share this
error`, or `Ask for a hint`. The product may automatically include the current mission state and
system-check output, but must not imply that the agent can see the child's computer.

The selected learning locale is explicit agent context. The shared tutor and safety policies are
language-neutral in behavior, while character voice, mission facts, examples, terminology, and
responses come from the reviewed localized mission version. The agent may explain an English code
identifier in Russian or Kazakh, but must not translate identifiers inside the learner's files.

Agent memory stores structured concepts, evidence codes, and misconception identifiers rather
than only localized prose. This allows a learner to change language without losing continuity.
Parent summaries use the adult's preferred locale, which may differ from the child's learning
locale. AI-generated translation is not a substitute for missing reviewed mission content.

## Agent tools

The initial tool set is narrow and mostly read-only:

- get the current mission and assignment context;
- list the station workspace tree;
- read an explicitly shared file or notebook selection;
- read the latest program output or traceback;
- run a deterministic mission check;
- retrieve relevant authored theory;
- read previously confirmed learning evidence;
- record a structured session summary;
- request adult help.

The agent cannot access arbitrary local files, execute a shell, install packages, use unrestricted
network access, modify source files, publish work, or communicate with other people. Suggested
changes may be displayed as a small example or diff, but the learner applies them.

These tools are adapters over the application capabilities described above. Adding them must not
change content formats, workspace storage, deterministic check contracts, competency identities,
or mission completion rules.

## Pedagogical loop

The default intervention is progressive:

1. Ask the learner to describe the attempt or predict the result.
2. Point to an observation, failing example, or error location.
3. Recall the relevant concept.
4. Suggest pseudocode or one next action.
5. Show a tiny analogous example.
6. Offer a partial scaffold only after repeated attempts.

After the code works, the agent asks for an explanation, prediction, or transfer variation. A
deterministic system check establishes whether the artifact behaves correctly. The agent uses a
rubric to collect evidence of understanding but is not the sole authority for correctness or
mission completion.

The agent should not optimize for conversation length or user attachment. Product evaluation
should measure:

- proportion of code written by the learner;
- deepest hint level required;
- ability to explain the result;
- success on a related transfer task;
- recovery from errors;
- repeated misconceptions resolved over time.

## Modern model strategy

Do not copy old, highly scripted Nazar prompts literally. Improved models need clear domain
context, constraints, tools, approval boundaries, and success criteria, but not a complete script
for every conversational turn. Deterministic behavior belongs in application code and structured
state rather than repeated prose.

Preserve the valuable Nazar mechanisms:

- provider abstraction;
- immutable model and prompt versions;
- streaming responses;
- token budgets and rate limits;
- content-linked sessions;
- administrative review and analytics.

Add capabilities that the original chat model did not have:

- live, permissioned workspace context;
- deterministic tool results;
- cross-session learning evidence;
- structured actions and hint levels;
- representative regression evaluations.

Model and reasoning settings are selected through evaluations on real learner traces, not by
assuming that the largest model or longest prompt is best. Required evaluation cases include
solution leakage, age-appropriate language, mission-grounded factual accuracy, frustration,
unsafe disclosure, prompt injection, correct tool permissions, and effective use of notebook and
project context. The suite must contain equivalent Russian and Kazakh cases before Kazakh launch,
including code-switching and terminology consistency.

## Child safety and privacy

The initial learners are under thirteen, so data minimization is part of the architecture:

- the model receives no name, email, exact age, or unnecessary family information;
- requests use a stable pseudonymous safety identifier;
- parent consent and clear AI disclosure are required;
- the parent can inspect, export, and delete AI history;
- input and output pass through safety controls;
- the agent does not solicit personal details or encourage emotional dependency;
- conversations are not represented as secret from the parent;
- provider-side storage is disabled where supported and history is managed by the application;
- production use must satisfy the provider's current under-18 and data-retention requirements.

Safety cannot rely on the system prompt alone. Authorization, tool schemas, moderation, retention,
and escalation are enforced outside the model. Requirements must be rechecked against current
provider policies and applicable Kazakhstan law before public launch.

## Adult summary

The parent or offline instructor receives a concise debrief rather than a raw transcript by
default:

- what the learner built;
- what they explained independently;
- the highest hint level used;
- recurring misconceptions;
- whether adult help would be useful;
- a suggested next activity.

## Future local workflow

Browser-first does not mean browser-only forever. The intended progression is:

1. scientific journal in the browser;
2. multi-file project in the browser;
3. downloadable project on the learner's computer;
4. local editor, Git, and GitHub;
5. an optional Station Zhasyl-1 CLI or repository integration;
6. an advanced Codex or Claude Code workflow for older learners.

A future exported workspace may contain `AGENTS.md`, a mission manifest, learner journal, source,
and tests. Local integration must be separately consented and scoped to that repository; it must
not grant the station access to the rest of the computer.

## Initial validation slice

First validate the agent-free MVP with one BioScout assignment and one materials-laboratory
assignment. Each slice includes the canonical authored path, progressive hints, a browser
workspace, persistence, structured deterministic checks, an explanation checkpoint, activity
evidence, and
an adult summary.

The first later agent slice reuses one of those completed assignments unchanged. It adds tool
adapters, conversation persistence, safety controls, and a mentor panel. Compare a lean tool-aware
prompt with the older scripted style using learning outcomes and cost rather than subjective chat
quality.

Agent readiness is accepted only when:

- the complete mission remains usable with the agent disabled;
- agent tools wrap existing authorized application use cases;
- adding the agent requires no rewrite of mission content, workspace persistence, system checks,
  progress, competency identifiers, or localization;
- existing activity records provide enough context for a useful first agent session;
- agent observations cannot bypass deterministic completion and evidence rules.

## Related designs

- [Product concept](product-concept.md)
- [Platform architecture](platform-architecture.md)
