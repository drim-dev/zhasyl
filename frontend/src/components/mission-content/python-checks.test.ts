import { workbenches } from "./python-checks";

describe("deterministic Python checks", () => {
  it("accepts a working BioScout sequence check", () => {
    const code = `
allowed_bases = {"A", "C", "G", "T", "N"}
for index, symbol in enumerate("ACGTTGCA?CTAGGCA"):
    if symbol not in allowed_bases:
        print(index + 1)
`;
    const checks = workbenches["bioscout-check"].checks(code, {
      ok: true,
      output: "Найдены ошибки в позициях: [9]",
    });

    expect(checks.every((check) => check.passed)).toBe(true);
  });

  it("keeps the sealant check incomplete while starter placeholders remain", () => {
    const definition = workbenches["sealant-balance"];
    const checks = definition.checks(definition.starterCode, {
      ok: true,
      output: "Сумма: 0 %\nПроверь пропорции",
    });

    expect(checks.find((check) => check.code === "python-ran")?.passed).toBe(
      true,
    );
    expect(
      checks.find((check) => check.code === "calculated-total")?.passed,
    ).toBe(false);
    expect(checks.find((check) => check.code === "condition")?.passed).toBe(
      false,
    );
  });
});
