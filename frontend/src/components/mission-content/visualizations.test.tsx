import { fireEvent, render, screen } from "@testing-library/react";
import { MixtureExplorer } from "./mixture-explorer";
import { SequenceInspector } from "./sequence-inspector";

describe("mission visualizations", () => {
  it("lets a learner inspect individual sequence positions", () => {
    render(<SequenceInspector />);

    expect(screen.getByRole("status")).toHaveTextContent("Позиция 9");
    fireEvent.click(
      screen.getByRole("button", { name: "Позиция 1: символ A" }),
    );
    expect(screen.getByRole("status")).toHaveTextContent("есть в алфавите");
  });

  it("shows why formula 17-Б stops the dispenser", () => {
    render(<MixtureExplorer />);

    fireEvent.click(screen.getByRole("button", { name: "Формула 17-Б" }));
    expect(screen.getByRole("status")).toHaveTextContent("105%");
    expect(screen.getByRole("status")).toHaveTextContent("дозатор остановлен");
  });
});
