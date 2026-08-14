---
name: design-station-interface
description: Design and review Station Zhasyl-1 user interfaces. Use for UX flows, page layouts, React components, CSS or design tokens, MDX interactive components, scientific visualisations, responsive behaviour, accessibility, screenshots, and visual QA in the Next.js frontend.
---

# Design Station Interface

Create a light, calm, near-future station interface that helps a child read, investigate, and
write code without decorative science-fiction noise.

## Required reference

Before designing or changing UI, read
../../../docs/designs/interface-design-system.md completely. Treat it as the visual and
interaction source of truth. Also follow ../frontend-architecture/SKILL.md when implementation
touches the Next.js frontend.

If an implemented component pattern conflicts with the design document, do not silently copy it.
Preserve compatibility where necessary and report the inconsistency.

## Workflow

1. Identify the user: learner, adult, or both.
2. State the screen's single primary action and the evidence of success.
3. Separate narrative, scientific facts, learner work, and system output.
4. Reuse existing primitives and semantic tokens before adding a variant or dependency.
5. Design default, loading, empty, error, disabled, focus, save/offline, and narrow-screen states.
6. Implement the smallest responsive interaction that preserves keyboard and assistive-technology
   access.
7. Verify Russian and longer future Kazakh text without fixed-width labels or clipped content.
8. Review the result against the checklist in the design document.

## Non-negotiable direction

- Prefer pale mineral surfaces, dark readable text, generous space, and restrained green accents.
- Express futurism through precision, data, subtle motion, and dependable state feedback.
- Do not create a dark neon cockpit, game HUD, cartoon Mars theme, or generic school dashboard.
- Keep the experience serious but not adult, and welcoming but not childish.
- Keep mission prose and code visually dominant over navigation and decoration.
- Use one obvious primary action per screen.
- Do not use rankings, streak pressure, fake urgency, or global station progress.
- Do not rely on colour, animation, hover, or dragging alone.
- Respect reduced motion and a minimum 44-pixel interactive target.
- Use sourced real imagery, explanatory diagrams, and fictional station art for distinct purposes.

## Component decisions

Add a reusable primitive only when at least two concrete consumers need the same behaviour.
Prefer composition over a large variant matrix. Put semantic behaviour and accessibility in the
primitive; keep mission-specific story and data in feature components.

Use semantic design tokens rather than raw colour values in feature code. Do not introduce a UI
framework, icon family, chart library, or font dependency without checking the existing frontend
and demonstrating a concrete need.

For scientific visualisations:

- label axes, units, provenance, and uncertainty;
- distinguish observation from model output;
- provide a textual or tabular alternative for essential information;
- avoid gauges and 3D charts unless the represented quantity is genuinely spatial.

## Verification

For implementation work, verify at minimum:

- keyboard navigation and visible focus;
- accessible names and form labels;
- contrast and non-colour status cues;
- desktop, tablet, and narrow-screen layout;
- loading, error, save, offline, and system-check feedback;
- long Russian and representative Kazakh-length text;
- reduced-motion behaviour;
- no avoidable layout shift or unnecessary client-side JavaScript.

Report which states and viewport sizes were exercised. A visually appealing static happy path is
not a complete interface.
