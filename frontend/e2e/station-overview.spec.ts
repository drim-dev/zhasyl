import { expect, test } from "@playwright/test";
import type { Locator, Page } from "@playwright/test";

async function tabTo(page: Page, target: Locator, limit = 6): Promise<void> {
  for (let index = 0; index < limit; index += 1) {
    await page.keyboard.press("Tab");
    if (
      await target.evaluate((element) => element === document.activeElement)
    ) {
      return;
    }
  }

  await expect(target).toBeFocused();
}

test.beforeEach(async ({ page }) => {
  await page.goto("/");
});

test("loads the API-backed station overview", async ({ page }, testInfo) => {
  await expect(
    page.getByRole("heading", {
      name: "Выбери задачу, которая важна для жизни на Марсе",
    }),
  ).toBeVisible();
  await expect(
    page.getByRole("heading", { name: "Лаборатория биоинформатики" }),
  ).toBeVisible();
  await expect(
    page.getByRole("heading", { name: "Лаборатория материалов" }),
  ).toBeVisible();

  await page.screenshot({
    path: testInfo.outputPath("station-overview.png"),
    fullPage: true,
  });
});

test("keeps the primary action keyboard accessible", async ({ page }) => {
  const stationLink = page.getByRole("link", { name: /Станция «Жасыл-1»/ });
  await expect(stationLink).toBeVisible();

  await page.keyboard.press("Tab");
  await expect(stationLink).toBeFocused();

  const themeToggle = page.getByRole("button", {
    name: "Переключить цветовую тему",
  });
  await tabTo(page, themeToggle);
  await expect(themeToggle).toBeFocused();

  const primaryAction = page.getByRole("link", {
    name: /Посмотреть лаборатории/,
  });
  await tabTo(page, primaryAction);
  await expect(primaryAction).toBeFocused();
  await expect(primaryAction).toHaveCSS("outline-style", "solid");
});

test("switches and remembers the colour theme", async ({ page }, testInfo) => {
  await page.emulateMedia({ colorScheme: "light" });
  await page.evaluate(() => localStorage.removeItem("theme"));
  await page.reload();

  const root = page.locator("html");
  await expect(root).toHaveAttribute("data-theme", "light");

  await page.getByRole("button", { name: "Переключить цветовую тему" }).click();
  await expect(root).toHaveAttribute("data-theme", "dark");
  await expect(
    page.getByRole("button", { name: "Переключить цветовую тему" }),
  ).toHaveAttribute("title", "Включить светлую тему");

  await page.screenshot({
    path: testInfo.outputPath("station-overview-dark.png"),
    fullPage: true,
  });

  await page.reload();
  await expect(root).toHaveAttribute("data-theme", "dark");
});
