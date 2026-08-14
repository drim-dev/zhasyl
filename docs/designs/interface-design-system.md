# Station Interface Design Direction

## Status

This document is the visual and interaction source of truth for Station Zhasyl-1. It defines the
intended experience before frontend implementation. Concrete components and tokens may evolve
through prototypes and learner testing, but changes must preserve the principles here.

## Design promise

The interface should feel like a calm, precise learning instrument from a near-future Mars
station:

- light rather than visually heavy;
- futuristic through clarity, data, motion, and spatial precision;
- serious without feeling adult or institutional;
- welcoming to children without becoming childish;
- scientific without resembling a dense professional control room.

The target is **quiet confidence**, not spectacle. The station is a place where people live and
work every day, so its software should feel understandable, dependable, and humane.

## Avoided directions

Do not build:

- a dark neon science-fiction cockpit;
- a game HUD full of gauges, grids, and blinking alerts;
- a generic school dashboard with bright primary-colour cards;
- a glassmorphism showcase with low contrast and excessive blur;
- a cartoon Mars theme;
- an enterprise administration interface adapted superficially for children;
- an interface that creates urgency through timers, streaks, rankings, or fake emergencies.

Futurism comes from disciplined composition, responsive data, subtle depth, and purposeful
interaction. It does not come from decorative technical noise.

## Core principles

### One clear action

Each screen has one visually dominant next action. Secondary actions remain available but quiet.
A learner should understand what to do next without reading the whole page.

### Story, science, and code remain distinct

Use consistent visual regions for:

- station narrative and character messages;
- scientific facts and research provenance;
- learner predictions and reflections;
- editable code and data;
- deterministic system-check results.

Do not merge fiction, observation, model output, and scientific fact into one undifferentiated
card style.

### Visible progress without pressure

Show completed assignments, the current assignment, saved artifacts, and the next mission within
the active laboratory. Do not show global station completion, competitive ranks, streak loss, or
countdowns tied to the arrival of settlers.

### Child-centred, not childish

Use direct language, large readable type, limited choices, and immediate feedback. Avoid mascots
that interrupt work, oversized toy-like controls, praise for trivial clicks, and visual rewards
that compete with understanding.

### Performance is part of the aesthetic

A futuristic system should feel immediate and stable. Prefer server-rendered content, small client
islands, reserved layout space, restrained animation, and clear saved/offline states. Avoid
ornamental JavaScript and heavy background effects.

## Visual language

### Light and dark environments

The product supports light and dark themes from the first implementation. The first visit
follows the operating-system preference; an explicit learner choice is stored on that device.
Both themes are complete product states, not a primary design plus a recoloured fallback.

Light mode uses pale mineral surfaces with dark ink and restrained green accents. Dark mode uses
deep low-glare mineral surfaces, soft light text, and the same semantic hierarchy. Dark mode must
remain calm and readable; it must never become a neon cockpit, game HUD, or high-contrast science
fiction skin.

Use generous empty space, thin dividers, calm surfaces, and a small number of elevation levels in
both themes. Avoid large areas of saturated colour. Theme changes must preserve meaning, focus,
data distinctions, and status visibility without a flash of the wrong theme during page load.

### Light colour tokens

| Token | Initial value | Use |
| --- | --- | --- |
| canvas | #F4F7F4 | Main application background |
| surface | #FFFFFF | Reading and working surfaces |
| surface-subtle | #EAF0EC | Quiet grouping and inactive regions |
| text | #14201D | Primary text |
| text-muted | #60706B | Secondary text that still passes normal-text contrast |
| border | #D8E2DD | Dividers and component outlines |
| brand | #167052 | Primary actions and active navigation |
| brand-accent | #1F8A62 | Larger accents, charts, and selected surfaces |
| brand-soft | #DDF4E8 | Brand-tinted backgrounds |
| on-brand | #FFFFFF | Text and icons on brand surfaces |
| mars | #B45332 | Rare narrative or planetary accent |
| info | #2F6F9F | Informational state |
| warning | #8A5A12 | Warning state |
| danger | #A33A3A | Error or destructive state |
| focus | #2F6F9F | Keyboard focus ring |

### Dark colour tokens

| Token | Initial value | Use |
| --- | --- | --- |
| canvas | #0F1715 | Low-glare application background |
| surface | #16211E | Reading and working surfaces |
| surface-subtle | #1C2A26 | Quiet grouping and inactive regions |
| text | #EDF5F1 | Primary text |
| text-muted | #AABBB5 | Secondary text |
| border | #30423C | Dividers and component outlines |
| brand | #65D0A3 | Primary actions and active navigation |
| brand-accent | #7CDBB1 | Larger accents, charts, and selected surfaces |
| brand-soft | #1F3D32 | Brand-tinted backgrounds |
| on-brand | #0B1713 | Text and icons on brand surfaces |
| mars | #E28B6C | Rare narrative or planetary accent |
| info | #83B8DF | Informational state and focus |
| warning | #E2B96C | Warning state |
| danger | #F09292 | Error or destructive state |
| focus | #83B8DF | Keyboard focus ring |

Green represents life, growth, active work, and the station identity. Mars orange is a supporting
accent, not a second brand colour. Status colours must always be paired with text or an icon.

### Typography

Use typography that supports Cyrillic, future Kazakh content, code, and scientific notation.

- **Display and navigation:** Manrope, with a system sans-serif fallback.
- **Body and mission prose:** Inter, with a system sans-serif fallback.
- **Code and data:** JetBrains Mono, with a system monospace fallback.

Initial scale:

| Role | Size / line height | Notes |
| --- | --- | --- |
| Hero | 40 / 48 | Marketing or laboratory introduction only |
| Page title | 30 / 38 | One per page |
| Section title | 22 / 30 | Major content section |
| Component title | 18 / 26 | Cards and panels |
| Mission prose | 18 / 30 | Main learner reading experience |
| Interface body | 16 / 24 | Controls and navigation |
| Supporting text | 14 / 20 | Never use for essential instructions |
| Code | 15 / 23 | Adjustable in the workbench |

Keep mission prose near 60–72 characters per line. Do not use uppercase for long labels. Use
weight and spacing before introducing additional colours.

### Spacing and geometry

Use a 4-pixel base grid with a restrained spacing scale:

4, 8, 12, 16, 24, 32, 48, 64

- Control height: at least 44 pixels.
- Input and button radius: 10 pixels.
- Card and panel radius: 14 pixels.
- Modal and large-sheet radius: 18 pixels.
- Pills are reserved for short statuses, filters, and compact metadata.
- Borders are usually 1 pixel.
- Shadows are soft and rare; hierarchy should primarily come from spacing and surface colour.

Avoid making every paragraph a card. Cards group a distinct action, object, or state.

### Iconography and imagery

Use one consistent outline icon family. Icons clarify labels but do not replace unfamiliar text.
Do not use emoji as primary controls.

Station imagery should show believable lived-in technology, people, terrain, and scale. Clearly
separate:

- real scientific imagery and its source;
- diagrams or visual models;
- fictional illustrations of Station Zhasyl-1.

Do not decorate every assignment with a generic Mars image. Use an image only when it establishes
place, explains a system, or advances the story.

## Motion

Motion should communicate spatial change, causality, or system state.

- Use 120–220 ms transitions for controls and panels.
- Use slightly longer transitions only for major workspace changes.
- Prefer opacity and small transforms over large movement.
- Do not animate continuously in the background.
- Pause non-essential motion when the document is hidden.
- Respect prefers-reduced-motion.
- Never use flashing alerts.

A successful system check may reveal its evidence progressively, but it should not become a
celebration that delays the learner.

## Product layouts

### Station shell

The shell provides quiet orientation:

- current laboratory;
- current mission and assignment;
- durable save state;
- access to the scientific journal;
- account or child-profile access appropriate to the current user.

On desktop, use a compact side rail or header plus a broad content area. On mobile, preserve the
primary action and current context; do not compress the desktop navigation into a wall of icons.

### Laboratory view

Show:

- the laboratory's purpose;
- the current available mission;
- completed missions and their artifacts;
- later missions as understandable previews;
- required starting knowledge;
- the specialist responsible for the laboratory.

The sequence should feel like growing capability, not a locked game map.

### Mission reader

Use a calm reading column for story and theory. Station messages, researcher notes, research
provenance, predictions, and safety notes receive distinct but related treatments.

Keep the next practical action visible after a theory section. Long scientific references belong
in expandable researcher material, not in the primary reading flow.

### Learning workbench

The workbench may combine:

- mission context;
- scientific journal;
- file tree;
- editor or notebook;
- run and system-check output;
- progressive hints.

On wide screens, use a resizable split layout. On narrow screens, use clear tabs or modes and
preserve unsaved work when switching. Do not display four small panels simultaneously.

The editor is the visual focus while coding. Narrative panels should remain accessible without
competing with the code.

### Adult view

Adult summaries are concise and factual. They should show what the learner built, evidence of
understanding, hints used, and where help may be useful. Avoid surveillance-like timelines and
raw AI transcripts as the default presentation.

## Priority components for the first vertical slice

Build a small coherent component set rather than a generic UI library:

1. station application shell;
2. laboratory and mission cards;
3. station message;
4. researcher note;
5. research provenance card;
6. prediction and reflection prompt;
7. scientific data table;
8. code or notebook workbench;
9. system-check result;
10. progressive hint panel;
11. save and offline indicator;
12. adult session summary.

Each component must define default, loading, empty, error, disabled, focus, and narrow-screen
states where applicable.

## Data visualisation

Visualisations are scientific instruments, not decoration.

- Always label axes, units, categories, and data provenance.
- Distinguish observed data, model output, and uncertainty visually and in text.
- Never rely on colour alone.
- Provide a table or textual summary when the chart carries essential information.
- Use stable colours for the same entities within one mission.
- Let the learner inspect values where interaction adds understanding.
- Do not use 3D charts or simulated instrument gauges without a real spatial reason.

## Accessibility

The implementation target is WCAG 2.2 AA.

- All interactions work with keyboard and assistive technology.
- Focus is visible and never removed without an equivalent.
- Controls have programmatic names and associated labels.
- Touch targets are at least 44 by 44 pixels.
- Text and essential graphics meet contrast requirements.
- Status is never expressed by colour alone.
- Error messages identify the problem and the next action.
- Dragging has a non-drag alternative.
- Zoom and larger text do not hide core actions.
- Motion respects user preferences.
- Code, charts, and notebooks receive accessible names and alternatives appropriate to their
  function.

Accessibility is part of the initial component contract, not a later audit.

## Responsive and localisation rules

Design first for desktop and tablet learning, then ensure essential reading, journals, and progress
work on phones. Complex coding may recommend a larger screen without blocking access to content
or saved work.

Russian and Kazakh text may differ substantially in length. Components must:

- avoid fixed-width text labels;
- allow wrapping without clipping;
- reserve space for longer plurals and status messages;
- keep machine identifiers and code unchanged;
- support locale-aware number and date formatting;
- avoid embedding visible text in images;
- test mixed Cyrillic and Latin content.

## Feedback and system states

Always make these states explicit:

- saving;
- saved;
- working offline;
- synchronisation failed;
- code is running;
- code stopped;
- system check passed;
- system check found a specific problem;
- content or workspace could not load;
- session expired.

Use calm factual language. A failed check is evidence about the current program, not a judgement
about the learner.

## Implementation direction

Use semantic CSS custom properties as the stable design-token boundary. Components consume
semantic tokens rather than raw colours.

    :root,
    [data-theme="light"] {
      --color-canvas: #f4f7f4;
      --color-surface: #ffffff;
      --color-surface-subtle: #eaf0ec;
      --color-text: #14201d;
      --color-text-muted: #60706b;
      --color-border: #d8e2dd;
      --color-brand: #167052;
      --color-brand-accent: #1f8a62;
      --color-brand-soft: #ddf4e8;
      --color-on-brand: #ffffff;
      --color-mars: #b45332;
      --color-focus: #2f6f9f;
      --radius-control: 0.625rem;
      --radius-panel: 0.875rem;
    }

    [data-theme="dark"] {
      --color-canvas: #0f1715;
      --color-surface: #16211e;
      --color-surface-subtle: #1c2a26;
      --color-text: #edf5f1;
      --color-text-muted: #aabbb5;
      --color-border: #30423c;
      --color-brand: #65d0a3;
      --color-brand-accent: #7cdbb1;
      --color-brand-soft: #1f3d32;
      --color-on-brand: #0b1713;
      --color-mars: #e28b6c;
      --color-focus: #83b8df;
    }

Resolve the system preference before the first paint. Store an explicit device choice and do not
let a later operating-system change override it. The toggle has a stable accessible name, while
its tooltip describes
the resulting action.

Do not choose a component framework merely to obtain a visual style. Evaluate dependencies when
the first vertical slice identifies a concrete need. Whatever styling approach is selected must
preserve server-first rendering, semantic tokens, accessibility, and the design direction here.

## Review checklist

Before accepting a new screen or component, verify:

- Is the primary learner action obvious?
- Does the screen feel calm and visually lightweight in both themes?
- Is the futuristic quality functional rather than decorative?
- Can story, science, learner work, and system evidence be distinguished?
- Is the learner reading or coding at a comfortable size and line length?
- Are loading, error, save, offline, and check states covered?
- Does it work by keyboard and on a narrow screen?
- Does it tolerate longer Russian and Kazakh text?
- Is animation purposeful and reduced-motion safe?
- Does the interface avoid pressure, competition, and fake urgency?
- Could any ornament, panel, or dependency be removed without losing understanding?
