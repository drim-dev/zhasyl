# CLAUDE.md

Project instructions live in a single tool-neutral file. Read it and follow it:

@AGENTS.md

Notes for Claude Code specifically:

- Project skills live in `.agents/skills/<name>/SKILL.md` and are exposed to the skill loader
  through the `.claude/skills` symlink. Both paths point at the same files; edit them under
  `.agents/skills/`.
- Run builds, tests, and checks through the `Makefile` targets (`make verify`, `make test`,
  `make format-check`) rather than reinventing command lines.
