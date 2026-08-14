# Content Authoring

Station learning content is authored as MDX under a locale root and seeded into PostgreSQL.
Learner-facing text belongs in the publication language; paths and stable slugs remain English
and locale-neutral.

## Layout

~~~text
content/
  ru/
    station/overview.mdx
    laboratories/
      bioinformatics/
        overview.mdx
        missions/
          bioscout/
            overview.mdx
            assignments/
              01-check-sequence.mdx
      materials/
        overview.mdx
        missions/
          sealant-17/
            overview.mdx
            assignments/
              01-balance-formula.mdx
~~~

A future reviewed Kazakh publication uses the same hierarchy under `content/kk/` and the same
station, laboratory, mission, and assignment slugs. A path is an authoring convention, not a
runtime identity; frontmatter defines the parent relationships.

## Frontmatter

Every file starts with YAML frontmatter and declares `schema: zhasyl.content/v1`.

- Station documents require `kind`, `slug`, `locale`, `title`, `location`, and `briefing`.
- Laboratory documents require `station`, positive `order`, `purpose`, `specialist`, and
  `isPublished`.
- Mission documents require `laboratory`, positive `order`, `problem`, `status`, and a non-empty
  MDX body.
- Assignment documents require `laboratory`, `mission`, positive `order`, `objective`, positive
  `estimatedMinutes`, and a non-empty MDX body.

~~~mdx
---
schema: zhasyl.content/v1
kind: assignment
laboratory: bioinformatics
mission: bioscout
slug: check-sequence
locale: ru
order: 1
title: Проверь сигнал из агрокомплекса
objective: Прочитать FASTA и найти ошибочные символы.
estimatedMinutes: 60
isPublished: true
---

<StationMessage author="Лариса Ким" role="Руководитель лаборатории биоинформатики">
Сообщение станции.
</StationMessage>
~~~

## Safe MDX Components

MDX is evaluated only for repository-authored, reviewed content. The renderer rejects imports,
exports, JavaScript expressions, raw HTML, unknown JSX tags, and expression-valued properties.
Component properties must be literal strings.

Available learning components are:

- `StationMessage` with `author` and `role`;
- `ResearcherNote` with `title`;
- `Prediction` with `prompt`;
- `Hint` with `level` and `title`;
- `SystemCriteria` with `title`;
- `JournalPrompt` with `title`;
- `PythonWorkbench` with a registered `assignment` configuration.

Current mission-specific visualizations are `SequenceInspector` and `MixtureExplorer`.

Every visualization must be wrapped in `Figure`:

~~~mdx
<Figure caption="What the visual teaches." source="Source or model label">
  <SequenceInspector />
</Figure>
~~~

A visual must explain a relationship or process, support light and dark themes, remain operable by
keyboard, and include a table or textual alternative when it carries essential information.
Decorative imagery does not belong in lesson MDX.

## Learning Structure

Each station assignment should contain, in learner-facing language:

1. a concrete station problem and message from the responsible specialist;
2. why the result matters;
3. the mandatory theory needed for the current step, normally 10–15 minutes;
4. an explicit boundary between real science, synthetic data, and the fictional Station model;
5. a prediction before code execution;
6. an executable task and observable system criteria;
7. three progressive hints;
8. scientific-journal reflection questions;
9. the consequence for the Station;
10. primary, official, or reviewed research provenance.

Russian text uses `ё` where appropriate, Russian quotation marks, age-appropriate sentences, and
English stable code identifiers. A program may identify candidates or data errors; it must not
claim a diagnosis or validated material performance without the required evidence.

## Publication and Revisions

The repository file is the authored source; PostgreSQL is the runtime source. On startup the
seeder hashes each normalized source file.

- The same hash keeps the current revision unchanged.
- A changed mission or assignment creates a new immutable numbered revision.
- Mission and assignment versions advance independently.
- Prior revisions remain available for future learner-state references and auditability.
- Only the current published revision is returned by the private content API.
- Removing a source file does not currently delete or unpublish persisted content.

Interactive component names and workbench configuration keys are part of the renderer contract.
Changing them requires updating the allow-list, tests, and existing authored content together.
