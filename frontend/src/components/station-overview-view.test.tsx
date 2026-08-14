import { render, screen } from "@testing-library/react";
import { StationOverviewView } from "./station-overview-view";
import type { StationOverview } from "@/types/station";

const overview: StationOverview = {
  stationId: "zhasyl-1",
  stationName: "Станция «Жасыл-1»",
  locale: "ru",
  location: "Равнина Аркадия · Марс · 2035 год",
  briefing: "Станция готовится к прибытию поселенцев.",
  laboratories: [
    {
      id: "bioinformatics",
      name: "Лаборатория биоинформатики",
      purpose: "Исследует живые системы.",
      specialist: "Лариса Ким",
      firstMission: {
        id: "bioscout",
        name: "BioScout",
        problem: "Неизвестная болезнь растений.",
        status: "Подготовка первого задания",
      },
    },
    {
      id: "materials",
      name: "Лаборатория материалов",
      purpose: "Проектирует материалы.",
      specialist: "Зарема Дадаева",
      firstMission: {
        id: "sealant-17",
        name: "Герметик № 17",
        problem: "Нужен состав для жилого модуля.",
        status: "Подготовка первого задания",
      },
    },
  ],
};

describe("StationOverviewView", () => {
  it("presents the station briefing and available laboratories", () => {
    render(<StationOverviewView overview={overview} />);

    expect(
      screen.getByRole("heading", {
        name: "Выбери задачу, которая важна для жизни на Марсе",
      }),
    ).toBeInTheDocument();
    expect(
      screen.getByRole("link", { name: /Посмотреть лаборатории/ }),
    ).toHaveAttribute("href", "#laboratories");
    expect(
      screen.getByRole("heading", { name: "Лаборатория биоинформатики" }),
    ).toBeInTheDocument();
    expect(
      screen.getByRole("heading", { name: "Лаборатория материалов" }),
    ).toBeInTheDocument();
    expect(
      screen.getByText("Лариса Ким", { exact: false }),
    ).toBeInTheDocument();
    expect(
      screen.getByText("Зарема Дадаева", { exact: false }),
    ).toBeInTheDocument();
  });
});
