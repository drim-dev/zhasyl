---
name: spec-maintenance
description: Use when updating module specs after feature implementation or design changes — guides reading current spec, identifying changes, and updating to reflect current state only
---

# Spec Maintenance

Use this skill after implementing a feature or applying a design change to update the corresponding module spec in `docs/specs/`.

## Workflow

1. **Identify the module spec** to update based on which domain was changed (e.g., Skills → `docs/specs/skills.md`)
2. **Read the current spec** to understand what's documented
3. **Read the design doc or code changes** that triggered the update — understand what changed
4. **Update the spec** to reflect the current state:
   - Add new entities, endpoints, or behaviors
   - Modify existing entries that changed
   - Remove anything that was deleted or superseded
5. **Verify the ~300 line cap** — if over, condense tables or merge related behaviors
6. **Verify no history or rationale crept in** — specs describe current state only, never "was changed from X to Y" or "this was added because..."
7. **Verify the template structure** is maintained (see below)

## Rules

- **Current state only** — no history, no rationale, no references to design docs or PRs
- **No implementation details** — no mention of MediatR, FluentValidation, EF Core, or internal patterns
- **~300 lines max** per module spec — use tables for structured data, bullets for behaviors
- **Tables for data model and endpoints** — bullets for key behaviors
- **Reference code files** for complex logic rather than re-explaining

## Module Spec Template

```markdown
# Module Name

Brief purpose (2-3 sentences).

## Data Model

| Entity | Key Fields | Notes |
|--------|-----------|-------|

## API Endpoints

| Method | Path | Auth | Description |
|--------|------|------|-------------|

## Key Behaviors

- Bullet list of important business rules and flows

## Content Structure

(Only for MDX-based modules — describes file layout and frontmatter)

## Admin Operations

(Brief summary of admin-specific features, if any)
```

## Section Guidelines

**Data Model table:**
- One row per entity
- Key Fields: list the important fields (Id, Slug, FKs, status fields, timestamps)
- Notes: constraints, relationships, enum values

**API Endpoints table:**
- Group by auth level or feature area if the module is large (use sub-headings)
- Auth column: Anonymous, Authenticated, Admin, Owner/Admin, Instructor/Admin
- Description: one-line summary of what the endpoint does

**Key Behaviors:**
- Business rules and invariants
- State machine transitions
- Idempotency guarantees
- Auto-triggered side effects (e.g., "section completion auto-completes skill")
- Access control nuances not captured in the endpoints table

**Content Structure** (if applicable):
- MDX file layout and path conventions
- What's stored in DB vs MDX
- Custom MDX components used

**Admin Operations:**
- Brief summary of admin-specific workflows
- Restrictions (e.g., "cannot delete if user submissions exist")
