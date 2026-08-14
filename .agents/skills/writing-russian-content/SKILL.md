---
name: writing-russian-content
description: Use when writing or editing any Russian learner-facing content or UI copy for Zhasyl, including stories, lessons, theory, missions, quizzes, hints, lab journals, safety notes, and teacher guides. Keep prose natural, age-appropriate, scientifically honest, and distinct from the English codebase.
---

# Writing Russian Learning Content

Write all learner-facing and teacher-facing course material in Russian. Keep code, identifiers,
file names, comments, tests, README files, architecture documents, and other technical artifacts
in English.

## Voice

- Write as a thoughtful Russian-speaking teacher, not as translated English documentation.
- Address the learner directly and respectfully. Avoid baby talk and false excitement.
- Prefer short sentences and one new idea at a time.
- Explain every necessary technical term on first use.
- Lead with purpose: what happened, what must be discovered, and why the concept helps.
- Separate observed facts, model assumptions, hypotheses, and conclusions.
- Use `ё` where it prevents ambiguity and standard Russian typography (`«ёлочки»`, em dash).

## Lesson structure

When authoring a lesson, keep this order unless the lesson design requires otherwise:

1. Continue the story and recall the previous result.
2. State the mission in one concrete sentence.
3. Explain the real-world purpose.
4. Introduce only the biology, mathematics, logic, or materials-science theory needed now.
5. Introduce the required Python concept.
6. Ask for a prediction before code runs.
7. Give the practical task and observable success criteria.
8. Provide optional extensions separately.
9. End with laboratory-journal questions and a story consequence.

Keep the core theory suitable for 10–15 minutes of discussion. Put deeper material in a clearly
labelled `Справка исследователя` section.

## Terminology

- Prefer a living Russian term when one exists.
- Give a genuine English industry term in parentheses once on first use when helpful.
- Do not transliterate ordinary English words merely because they appear in source material.
- Keep Python syntax, FASTA identifiers, function names, and file formats unchanged.
- Use stable translations consistently across the whole course and glossary.

## Scientific honesty and safety

- Label fictional organisms, ingredients, properties, and simplified models explicitly.
- Never present a simulation result as a laboratory fact.
- Do not claim that software discovers a medicine, proves safety, or replaces experimental
  validation.
- Do not provide child-directed recipes involving hazardous household chemicals.
- Any real-world experiment must use child-appropriate materials, require adult supervision,
  list safety constraints, and cite an authoritative source when safety could be disputed.
- Do not include real personal, medical, or genomic data from children.

## Teacher guides

Teacher-facing material remains in Russian and must include:

- the learning objective;
- prerequisite knowledge;
- the expected result;
- common misconceptions and likely coding errors;
- three progressively stronger hints;
- an easier path and an extension path;
- questions that guide without writing the solution for the learner;
- any safety or scientific-accuracy caveats.

## Mandatory self-check

After writing Russian content, verify:

- Does it sound originally written in Russian?
- Is the language suitable for the stated learner age without being patronizing?
- Does each theory block support the current mission?
- Are assumptions and fictional data clearly marked?
- Are the expected result and success criteria observable?
- Are English terms introduced only when useful?
- Are dangerous or medically significant claims absent or properly qualified?
- Does the story move forward because of the learner's code?
