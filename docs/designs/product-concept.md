# Station Zhasyl-1 Product Concept

## Status

This document records the agreed product direction. It is a design source, not a description of
implemented behavior.

## Product identity

The product is **Station Zhasyl-1**, displayed in Russian as `Станция «Жасыл-1»` and
transliterated as `Station Zhasyl-1` where Latin text is required. `Жасыл` means `green` in
Kazakh: life, growth, and a habitable future on the red planet.

There is no separate Zhasyl Academy, Zhasyl universe, or family of products carrying the same
name. The station is the single narrative frame and the product at `zhasyl.dev`.

## Premise

Station Zhasyl-1 is not merely a research outpost. Its long-term purpose is to make large,
permanent human settlements on Mars possible. Research matters because the settlement needs
food, health, materials, energy, communications, navigation, software, and reliable life-support
systems.

This premise provides a wide and durable reason to learn. A child may contribute through biology,
programming, mathematics, chemistry, materials science, robotics, astronomy, design, or a future
discipline without being forced through unrelated subjects.

The fiction must remain scientifically honest. Learner code may identify candidates, test a
model, or support a station decision; it must not be presented as proving medical safety,
discovering a medicine with one click, or replacing real laboratory validation.

## Kazakhstan identity

The station represents Kazakhstan's inclusive civic identity rather than one ethnicity. Its
recurring fictional crew should naturally include Kazakh, Russian, Ukrainian, Chechen, Korean,
German, Uighur, and other names found in Kazakhstan. Places, biographies, memories, and examples
may connect the crew to Pavlodar, Almaty, Astana, Karaganda, Kostanay, Shymkent, Atyrau, Aktau, and
other regions.

This should be expressed through credible characters and everyday detail, not a checklist of
ethnicities. Characters have stable responsibilities, expertise, limitations, relationships, and
personal histories. The station commander and laboratory specialists recur across missions so
that children gradually learn who runs the settlement and why their work matters.

All station leaders and specialists used as interactive AI personas are fictional. Real scientists
may appear in curated educational references, but a generative system must not impersonate a
living person.

## Learning language

The product deliberately avoids unnecessary school terminology.

| Conventional term | Station term | Meaning |
| --- | --- | --- |
| Course or subject area | Laboratory | An independently chosen field and ordered curriculum |
| Applied project or module | Mission | A substantial station problem with a story, competencies, and final product |
| Lesson | Station assignment | One focused learning session within a mission |
| Notebook | Scientific journal | Predictions, code, evidence, output, and reflection |
| Homework | Station assignment | Work that may be continued with a parent outside a session |
| Test | System check | A deterministic check of an observable result |
| Project progress | Mission readiness | The learner's progress inside one mission |

The hierarchy is:

```text
Station Zhasyl-1
└── Laboratory
    ├── Mission 1
    │   ├── Station assignment
    │   ├── Station assignment
    │   └── System check
    └── Mission 2 (after Mission 1)
```

## Product languages

Russian is the first content and interface language. Kazakh is a required product language, not
an optional rebranding exercise. The first implementation must allow the interface, authored
mission content, scientific journals, agent responses, search, and adult summaries to be selected
by locale.

A learner's mission, workspace, competency evidence, and progress are independent of language.
Changing from Russian to Kazakh must keep the same mission position and files. If a particular
assignment is not yet available in Kazakh, the product must say so clearly and offer an explicit
Russian fallback rather than silently mixing languages.

Kazakh content should be authored or reviewed by a fluent human with subject knowledge. Machine
translation may assist drafting but is not a publishable source of educational terminology or
age-appropriate voice.

## Choice and progress

A learner may participate in several laboratories at the same time. Laboratories are mutually
independent, but their core missions are ordered: later missions build on confirmed scientific
and programming knowledge from earlier missions in the same laboratory. No mission may require
completion of an unrelated laboratory.

The product must not expose a global station-completion percentage. Such a number becomes
meaningless when multiple children repeat a mission or new missions are added. Instead it may
show:

- personal mission readiness;
- concrete systems restored, built, or understood within that mission;
- competencies demonstrated by the learner;
- narrative consequences unlocked for that learner.

Station-wide story facts are authored canon, not a counter advanced collectively by all users.
Each learner experiences the story at their own pace. New required missions are normally appended
to a laboratory sequence; inserting content must not revoke access or readiness already earned by
existing learners.

## Initial laboratories

The first planned laboratories serve two different learners and remain fully independent:

- a bioinformatics laboratory whose initial mission is BioScout, aimed first at a nine-year-old
  who already understands conditions, loops, lists, and dictionaries;
- a materials laboratory combining Python, logic, mathematics, modelling, and safe investigation
  of mixtures and material properties, aimed first at an eleven-year-old beginner.

The materials path should use the learner's interest in mixing, texture, recipes, and changing
properties without giving unsafe household-chemistry instructions. Physical activities require
adult supervision and child-appropriate materials. Simulations must be labelled as models.

## Adult role

An adult may be a parent, an instructor, or both, but an instructor role and classes are not part
of the initial product model. Teaching can happen offline while the platform provides content,
the learner workspace, system checks, and concise adult-facing summaries.

The data model should not introduce classes speculatively. A future instructor feature can be
designed from observed needs rather than hidden behind an unused class abstraction.

## Product principles

- Lead with a meaningful station problem, then introduce the required theory and syntax.
- Keep each mission as one substantial applied problem; introduce concepts through its station
  assignments rather than splitting the problem into syntax-sized missions.
- Provide one canonical path and progressive hints instead of authored difficulty modes.
- Make the learner's code change what happens in the story.
- Preserve freedom to choose laboratories without completion pressure.
- Treat mistakes as evidence and part of investigation.
- Ask for a prediction before execution and an explanation after it.
- Separate fictional data, simplified models, observations, hypotheses, and conclusions.
- Produce a real, exportable artifact rather than a disposable quiz score.
- Design for children without making the interface childish.
- Treat Russian and Kazakh as presentations of the same learning model, not separate products.

## Related designs

- [Platform architecture](platform-architecture.md)
- [Learning workspaces and AI agent](learning-workspaces-and-ai-agent.md)
- [Initial laboratory mission map](../../content/ru/world/laboratory-mission-map.md)
- [Station interface design direction](interface-design-system.md)
- [MVP scope](mvp-scope.md)
- [First vertical slice plan](../plans/first-vertical-slice.md)
