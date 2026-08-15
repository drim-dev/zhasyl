import { expect, test } from "@playwright/test";

const bioAssignment =
  "/laboratories/bioinformatics/missions/bioscout/assignments/check-sequence";

test("shows a focused device connection journey", async ({ page }) => {
  await page.goto("/connect");

  await expect(
    page.getByRole("heading", { name: "Введите код подключения" }),
  ).toBeVisible();
  await expect(page.getByLabel("Код от взрослого")).toBeVisible();
  await expect(page.getByLabel("Название этого устройства")).toBeVisible();
  await expect(
    page.getByRole("button", { name: "Подключить устройство" }),
  ).toBeVisible();
  await expect(
    page.getByRole("link", { name: "Войти взрослому" }),
  ).toBeVisible();
});

test("lets an adult pair and revoke a child browser", async ({
  page,
}, testInfo) => {
  test.skip(
    testInfo.project.name !== "desktop",
    "The complete identity mutation journey runs once; responsive entry is covered separately.",
  );

  const displayName = `Лиза ${Date.now()}`;
  await page.goto("/adult/sign-in");
  await page
    .getByRole("button", { name: "Войти локально для разработки" })
    .click();
  await expect(
    page.getByRole("heading", { name: "Доступ детей к станции" }),
  ).toBeVisible();

  await page.getByLabel("Как показывать имя ребёнка").fill(displayName);
  await page.getByRole("button", { name: "Создать профиль" }).click();
  await expect(page.getByRole("heading", { name: displayName })).toBeVisible();

  await page
    .getByRole("button", { name: `Подключить устройство для ${displayName}` })
    .click();
  const code = (
    await page.locator("strong").filter({ hasText: "-" }).last().textContent()
  )?.trim();
  expect(code).toMatch(/^[A-Z0-9]{4}-[A-Z0-9]{4}$/);

  await page.getByRole("link", { name: "Открыть экран ввода кода" }).click();
  await page.getByLabel("Код от взрослого").fill(code!);
  await page.getByLabel("Название этого устройства").fill("Учебный ноутбук");
  const pairingResponse = page.waitForResponse(
    (response) =>
      response.url().endsWith("/api/child/pair") &&
      response.request().method() === "POST",
  );
  await page.getByRole("button", { name: "Подключить устройство" }).click();
  expect((await pairingResponse).status()).toBe(200);
  await expect(
    page.getByRole("heading", { name: "Устройство подключено" }),
  ).toBeVisible({ timeout: 15_000 });
  await expect(page.getByText(`Профиль: ${displayName}`)).toBeVisible();

  await page.goto("/adult");
  const pairedChild = page
    .getByRole("article")
    .filter({ has: page.getByRole("heading", { name: displayName }) });
  const pairedDevice = pairedChild
    .getByRole("listitem")
    .filter({ hasText: "Учебный ноутбук" });
  await expect(pairedDevice).toBeVisible();
  await pairedDevice.getByRole("button", { name: "Отключить" }).click();
  await expect(pairedDevice).not.toBeVisible();

  await page.goto("/connect");
  await expect(
    page.getByRole("heading", { name: "Введите код подключения" }),
  ).toBeVisible();
});

test("restores a saved workspace in a second paired browser", async ({
  browser,
}, testInfo) => {
  test.skip(
    testInfo.project.name !== "desktop",
    "The cross-device journey runs once.",
  );
  const displayName = `Илья ${Date.now()}`;
  const savedCode = `station_sample = "MARS-${Date.now()}"\nprint(station_sample)\n`;
  const adultContext = await browser.newContext();
  const adultPage = await adultContext.newPage();
  await adultPage.goto("/adult/sign-in");
  await adultPage
    .getByRole("button", { name: "Войти локально для разработки" })
    .click();
  await adultPage.getByLabel("Как показывать имя ребёнка").fill(displayName);
  await adultPage.getByRole("button", { name: "Создать профиль" }).click();

  const childCard = adultPage
    .getByRole("article")
    .filter({ has: adultPage.getByRole("heading", { name: displayName }) });
  await childCard
    .getByRole("button", { name: `Подключить устройство для ${displayName}` })
    .click();
  const firstCode = (await childCard.locator("strong").textContent())?.trim();

  const firstChildContext = await browser.newContext();
  const firstChildPage = await firstChildContext.newPage();
  await firstChildPage.goto("/connect");
  await firstChildPage.getByLabel("Код от взрослого").fill(firstCode!);
  await firstChildPage
    .getByLabel("Название этого устройства")
    .fill("Первый ноутбук");
  const firstPairingResponse = firstChildPage.waitForResponse(
    (response) =>
      response.url().endsWith("/api/child/pair") &&
      response.request().method() === "POST",
  );
  await firstChildPage
    .getByRole("button", { name: "Подключить устройство" })
    .click();
  expect((await firstPairingResponse).status()).toBe(200);
  await expect(
    firstChildPage.getByRole("heading", { name: "Устройство подключено" }),
  ).toBeVisible({ timeout: 15_000 });
  await firstChildPage.goto(bioAssignment);
  const firstWorkbench = firstChildPage.getByRole("region", {
    name: "Модуль проверки BioScout",
  });
  const firstEditor = firstWorkbench.getByRole("textbox", {
    name: "Редактор Python-кода",
  });
  await expect(firstEditor).toBeEnabled();
  await firstEditor.fill(savedCode);
  await expect(firstWorkbench.getByRole("status")).toContainText(
    "Сохранено на станции",
  );

  await adultPage.goto("/adult");
  const refreshedCard = adultPage
    .getByRole("article")
    .filter({ has: adultPage.getByRole("heading", { name: displayName }) });
  await refreshedCard
    .getByRole("button", { name: `Подключить устройство для ${displayName}` })
    .click();
  const secondCode = (
    await refreshedCard.locator("strong").textContent()
  )?.trim();

  const secondChildContext = await browser.newContext();
  const secondChildPage = await secondChildContext.newPage();
  await secondChildPage.goto("/connect");
  await secondChildPage.getByLabel("Код от взрослого").fill(secondCode!);
  await secondChildPage
    .getByLabel("Название этого устройства")
    .fill("Второй ноутбук");
  const secondPairingResponse = secondChildPage.waitForResponse(
    (response) =>
      response.url().endsWith("/api/child/pair") &&
      response.request().method() === "POST",
  );
  await secondChildPage
    .getByRole("button", { name: "Подключить устройство" })
    .click();
  expect((await secondPairingResponse).status()).toBe(200);
  await expect(
    secondChildPage.getByRole("heading", { name: "Устройство подключено" }),
  ).toBeVisible({ timeout: 15_000 });
  await secondChildPage.goto(bioAssignment);
  const secondWorkbench = secondChildPage.getByRole("region", {
    name: "Модуль проверки BioScout",
  });
  await expect(
    secondWorkbench.getByRole("textbox", { name: "Редактор Python-кода" }),
  ).toHaveValue(savedCode);
  await expect(secondWorkbench.getByRole("status")).toContainText(
    "Восстановлено со станции",
  );

  const stationCode = `${savedCode}# версия первого ноутбука\n`;
  await firstEditor.fill(stationCode);
  await expect(firstWorkbench.getByRole("status")).toContainText(
    "Сохранено на станции · версия 2",
  );

  const localConflictCode = `${savedCode}# локальная версия второго ноутбука\n`;
  const secondEditor = secondWorkbench.getByRole("textbox", {
    name: "Редактор Python-кода",
  });
  await secondEditor.fill(localConflictCode);
  await expect(secondWorkbench.getByRole("status")).toContainText(
    "На другом устройстве есть новая версия",
  );
  await expect(secondEditor).toHaveValue(localConflictCode);

  secondChildPage.once("dialog", (dialog) => void dialog.accept());
  await secondWorkbench
    .getByRole("button", { name: "Загрузить версию станции" })
    .click();
  await expect(secondEditor).toHaveValue(stationCode);

  await Promise.all([
    adultContext.close(),
    firstChildContext.close(),
    secondChildContext.close(),
  ]);
});
