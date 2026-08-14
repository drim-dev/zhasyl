"use client";

import { useState } from "react";
import styles from "./mission-content.module.css";

const sequence = "ACGTTGCA?CTAGGCA";
const allowedBases = new Set(["A", "C", "G", "T", "N"]);

export function SequenceInspector(): React.ReactElement {
  const [selectedIndex, setSelectedIndex] = useState(8);
  const selectedSymbol = sequence[selectedIndex];
  const isValid = allowedBases.has(selectedSymbol);

  return (
    <div className={styles.sequenceInspector}>
      <div className={styles.sequenceHeader}>
        <span>sample_leaf_03.fasta</span>
        <span className={isValid ? styles.validBadge : styles.invalidBadge}>
          {isValid ? "Допустимый символ" : "Требует проверки"}
        </span>
      </div>

      <ol
        className={styles.sequenceStrip}
        aria-label="Последовательность из 16 символов"
      >
        {Array.from(sequence).map((symbol, index) => {
          const symbolIsValid = allowedBases.has(symbol);

          return (
            <li key={`${symbol}-${index}`}>
              <button
                type="button"
                aria-label={`Позиция ${index + 1}: символ ${symbol}`}
                aria-pressed={selectedIndex === index}
                className={
                  symbolIsValid ? styles.baseButton : styles.invalidBaseButton
                }
                onClick={() => setSelectedIndex(index)}
              >
                <span>{symbol}</span>
                <small>{index + 1}</small>
              </button>
            </li>
          );
        })}
      </ol>

      <p className={styles.visualFinding} role="status" aria-live="polite">
        Позиция {selectedIndex + 1}: <strong>{selectedSymbol}</strong> —{" "}
        {isValid
          ? "есть в алфавите сканера."
          : "нет в алфавите сканера; файл нужно проверить."}
      </p>

      <details className={styles.dataAlternative}>
        <summary>Показать данные таблицей</summary>
        <table>
          <thead>
            <tr>
              <th>Позиция</th>
              <th>Символ</th>
              <th>Результат</th>
            </tr>
          </thead>
          <tbody>
            {Array.from(sequence).map((symbol, index) => (
              <tr key={`row-${symbol}-${index}`}>
                <td>{index + 1}</td>
                <td>{symbol}</td>
                <td>{allowedBases.has(symbol) ? "допустим" : "ошибка"}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </details>
    </div>
  );
}
