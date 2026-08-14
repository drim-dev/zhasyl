export type WorkbenchId = "bioscout-check" | "sealant-balance";

export interface RunResult {
  ok: boolean;
  output: string;
}

export interface CheckResult {
  code: string;
  label: string;
  passed: boolean;
}

export interface WorkbenchDefinition {
  title: string;
  fileName: string;
  starterCode: string;
  checks: (code: string, result: RunResult) => CheckResult[];
}

function bioScoutChecks(code: string, result: RunResult): CheckResult[] {
  return [
    {
      code: "python-ran",
      label: "Код выполняется без ошибки Python",
      passed: result.ok,
    },
    {
      code: "invalid-symbol",
      label: "Проверка использует алфавит допустимых символов",
      passed:
        code.includes("allowed_bases") &&
        /symbol\s+not\s+in\s+allowed_bases/.test(code),
    },
    {
      code: "human-position",
      label: "В отчёте найдена позиция 9, посчитанная с единицы",
      passed:
        /index\s*\+\s*1/.test(code) && /(^|\D)9(\D|$)/.test(result.output),
    },
    {
      code: "implemented",
      label: "Программа выводит понятный отчёт о найденной ошибке",
      passed: result.output.includes("Найдены ошибки в позициях"),
    },
  ];
}

function sealantChecks(code: string, result: RunResult): CheckResult[] {
  return [
    {
      code: "python-ran",
      label: "Код выполняется без ошибки Python",
      passed: result.ok,
    },
    {
      code: "calculated-total",
      label: "Сумма вычисляется из трёх переменных",
      passed:
        /total\s*=\s*flex_base\s*\+\s*reinforcing_fiber\s*\+\s*dust_shield/.test(
          code.replaceAll("\n", " "),
        ),
    },
    {
      code: "condition",
      label: "Условие сравнивает сумму со 100",
      passed: /if\s+total\s*==\s*100\s*:/.test(code),
    },
    {
      code: "visible-result",
      label: "В выводе есть результат проверки формулы",
      passed:
        result.output.includes("сбалансирована") ||
        result.output.includes("Проверь пропорции"),
    },
  ];
}

export const workbenches: Record<WorkbenchId, WorkbenchDefinition> = {
  "bioscout-check": {
    title: "Модуль проверки BioScout",
    fileName: "sequence_check.py",
    starterCode: `sequence = "ACGTTGCA?CTAGGCA"
allowed_bases = {"A", "C", "G", "T", "N"}
invalid_positions = []

for index, symbol in enumerate(sequence):
    # Если символа нет в алфавите,
    # добавь его позицию с единицы
    pass

if invalid_positions:
    print("Найдены ошибки в позициях:", invalid_positions)
else:
    print("Последовательность прошла проверку")
`,
    checks: bioScoutChecks,
  },
  "sealant-balance": {
    title: "Калькулятор формулы № 17",
    fileName: "formula_check.py",
    starterCode: `flex_base = 50
reinforcing_fiber = 30
dust_shield = 20

# Сложи три значения вместо готового нуля
total = 0

print("Сумма:", total, "%")

# Замени False проверкой суммы
if False:
    print("Формула сбалансирована")
else:
    print("Проверь пропорции")
`,
    checks: sealantChecks,
  },
};
