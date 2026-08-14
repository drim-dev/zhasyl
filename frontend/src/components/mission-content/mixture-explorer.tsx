"use client";

import { useState } from "react";
import styles from "./mission-content.module.css";

const formulas = [
  { id: "17-А", flexBase: 50, fiber: 30, dustShield: 20 },
  { id: "17-Б", flexBase: 45, fiber: 35, dustShield: 25 },
  { id: "17-В", flexBase: 60, fiber: 20, dustShield: 20 },
] as const;

export function MixtureExplorer(): React.ReactElement {
  const [selectedId, setSelectedId] = useState("17-А");
  const formula =
    formulas.find((item) => item.id === selectedId) ?? formulas[0];
  const total = formula.flexBase + formula.fiber + formula.dustShield;

  return (
    <div className={styles.mixtureExplorer}>
      <div className={styles.formulaTabs} aria-label="Выбор цифровой формулы">
        {formulas.map((item) => (
          <button
            type="button"
            key={item.id}
            aria-pressed={selectedId === item.id}
            onClick={() => setSelectedId(item.id)}
          >
            Формула {item.id}
          </button>
        ))}
      </div>

      <div
        className={styles.mixtureBar}
        role="img"
        aria-label={`Формула ${formula.id}: гибкая основа ${formula.flexBase} процентов, армирующее волокно ${formula.fiber} процентов, защита от пыли ${formula.dustShield} процентов`}
      >
        <span
          className={styles.flexSegment}
          style={{ flexGrow: formula.flexBase }}
          title={`Гибкая основа: ${formula.flexBase}%`}
        />
        <span
          className={styles.fiberSegment}
          style={{ flexGrow: formula.fiber }}
          title={`Армирующее волокно: ${formula.fiber}%`}
        />
        <span
          className={styles.dustSegment}
          style={{ flexGrow: formula.dustShield }}
          title={`Защита от пыли: ${formula.dustShield}%`}
        />
      </div>

      <ul className={styles.mixtureLegend}>
        <li>
          <span className={styles.flexKey} />
          Гибкая основа · {formula.flexBase}%
        </li>
        <li>
          <span className={styles.fiberKey} />
          Армирующее волокно · {formula.fiber}%
        </li>
        <li>
          <span className={styles.dustKey} />
          Защита от пыли · {formula.dustShield}%
        </li>
      </ul>

      <p
        className={total === 100 ? styles.formulaValid : styles.formulaInvalid}
        role="status"
        aria-live="polite"
      >
        Сумма: <strong>{total}%</strong> ·{" "}
        {total === 100 ? "запись сбалансирована" : "дозатор остановлен"}
      </p>

      <details className={styles.dataAlternative}>
        <summary>Показать все формулы таблицей</summary>
        <table>
          <thead>
            <tr>
              <th>Формула</th>
              <th>Основа</th>
              <th>Волокно</th>
              <th>Защита</th>
              <th>Сумма</th>
            </tr>
          </thead>
          <tbody>
            {formulas.map((item) => {
              const itemTotal = item.flexBase + item.fiber + item.dustShield;
              return (
                <tr key={item.id}>
                  <td>{item.id}</td>
                  <td>{item.flexBase}%</td>
                  <td>{item.fiber}%</td>
                  <td>{item.dustShield}%</td>
                  <td>{itemTotal}%</td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </details>
    </div>
  );
}
