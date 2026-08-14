import { expect, test } from "@playwright/test";

const bioAssignment =
  "/laboratories/bioinformatics/missions/bioscout/assignments/check-sequence";
const sealantAssignment =
  "/laboratories/materials/missions/sealant-17/assignments/balance-formula";

test("opens both mission readers and their first assignments", async ({
  page,
}, testInfo) => {
  await page.emulateMedia({ colorScheme: "light" });
  await page.addInitScript(() => localStorage.removeItem("theme"));
  await page.goto("/");
  await page.getByRole("link", { name: "Открыть миссию" }).first().click();
  await expect(
    page.getByRole("heading", {
      name: "BioScout: код Красной планеты",
      level: 1,
    }),
  ).toBeVisible();
  await page.getByRole("link", { name: /Начать первое задание/ }).click();
  await expect(
    page.getByRole("heading", {
      name: "Проверь сигнал из агрокомплекса",
      level: 1,
    }),
  ).toBeVisible();
  await expect(
    page.getByRole("button", { name: "Позиция 9: символ ?" }),
  ).toBeVisible();
  await expect(
    page.getByRole("textbox", { name: "Редактор Python-кода" }),
  ).toBeVisible();

  await page.goto(sealantAssignment);
  await expect(
    page.getByRole("heading", { name: "Сбалансируй формулу № 17", level: 1 }),
  ).toBeVisible();
  await page.getByRole("button", { name: "Формула 17-Б" }).click();
  await expect(
    page.getByRole("status").filter({ hasText: "105%" }),
  ).toBeVisible();

  await page.screenshot({
    path: testInfo.outputPath("sealant-assignment.png"),
    fullPage: true,
  });

  await page.getByRole("button", { name: "Переключить цветовую тему" }).click();
  await expect(page.locator("html")).toHaveAttribute("data-theme", "dark");

  await page.screenshot({
    path: testInfo.outputPath("sealant-assignment-dark.png"),
    fullPage: true,
  });
});

test("runs and checks the BioScout Python solution", async ({
  page,
}, testInfo) => {
  test.skip(
    testInfo.project.name !== "desktop",
    "Python runtime is exercised once.",
  );
  test.setTimeout(90_000);
  await page.goto(bioAssignment);

  const editor = page.getByRole("textbox", { name: "Редактор Python-кода" });
  await editor.fill(`sequence = "ACGTTGCA?CTAGGCA"
allowed_bases = {"A", "C", "G", "T", "N"}
invalid_positions = []

for index, symbol in enumerate(sequence):
    if symbol not in allowed_bases:
        invalid_positions.append(index + 1)

if invalid_positions:
    print("Найдены ошибки в позициях:", invalid_positions)
else:
    print("Последовательность прошла проверку")
`);
  await page.getByRole("button", { name: "Запустить и проверить" }).click();

  await expect(
    page.getByText("Найдены ошибки в позициях: [9]", { exact: false }),
  ).toBeVisible({ timeout: 45_000 });
  await expect(page.getByText("4 из 4")).toBeVisible();
  await page.reload();
  await expect(editor).toHaveValue(/invalid_positions\.append\(index \+ 1\)/);
});

test("runs and checks the sealant Python solution", async ({
  page,
}, testInfo) => {
  test.skip(
    testInfo.project.name !== "desktop",
    "Python runtime is exercised once.",
  );
  test.setTimeout(90_000);
  await page.goto(sealantAssignment);

  const editor = page.getByRole("textbox", { name: "Редактор Python-кода" });
  await editor.fill(`flex_base = 50
reinforcing_fiber = 30
dust_shield = 20

total = flex_base + reinforcing_fiber + dust_shield
print("Сумма:", total, "%")

if total == 100:
    print("Формула сбалансирована")
else:
    print("Проверь пропорции")
`);
  await page.getByRole("button", { name: "Запустить и проверить" }).click();

  const workbench = page.getByRole("region", {
    name: "Калькулятор формулы № 17",
  });
  await expect(workbench.locator("pre")).toContainText(
    "Формула сбалансирована",
    {
      timeout: 45_000,
    },
  );
  await expect(page.getByText("4 из 4")).toBeVisible();
});
