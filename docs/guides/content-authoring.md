# Content Authoring

Station learning content is authored as MDX in `content/` and seeded into PostgreSQL.
Learner-facing text belongs in the publication language; paths and stable slugs remain English
and locale-neutral.

## Layout

~~~text
content/
  station/overview.ru.mdx
  laboratories/
    bioinformatics/
      overview.ru.mdx
      missions/01-bioscout.ru.mdx
~~~

Adding Kazakh does not require a schema or code change. Add matching `.kk.mdx` documents with the
same station, laboratory, and mission slugs and `locale: kk`.

## Frontmatter

Every file starts with YAML frontmatter and declares `schema: zhasyl.content/v1`.

Station documents require `kind`, `slug`, `locale`, `title`, `location`, and `briefing`.
Laboratory documents additionally require `station`, positive `order`, `purpose`, `specialist`,
and `isPublished`. Mission documents require `laboratory`, positive `order`, `problem`, `status`,
and a non-empty MDX body.

~~~mdx
---
schema: zhasyl.content/v1
kind: mission
laboratory: bioinformatics
slug: bioscout
locale: ru
order: 1
title: "BioScout: код Красной планеты"
problem: В агрокомплексе обнаружены признаки неизвестной болезни растений.
status: Подготовка первого задания
isPublished: true
---

# BioScout: код Красной планеты

Mission content begins here.
~~~

## Publication and Revisions

The repository file is the authored source; PostgreSQL is the runtime source. On application
startup the seeder hashes each normalized source file.

- The same hash keeps the current revision unchanged.
- A changed mission file creates a new immutable numbered revision.
- The previous revision stays in the database for learner-state references and auditability.
- Only the current published revision is returned by the private content API.
- Removing a source file does not currently delete persisted content.

Interactive MDX components must eventually come from an explicit renderer allow-list. Raw MDX
must never be evaluated as arbitrary server or browser code.
