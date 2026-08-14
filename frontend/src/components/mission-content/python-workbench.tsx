"use client";

import { useEffect, useRef, useState } from "react";
import {
  workbenches,
  type CheckResult,
  type RunResult,
  type WorkbenchId,
} from "./python-checks";
import styles from "./python-workbench.module.css";

interface PythonWorkbenchProps {
  assignment: WorkbenchId;
}

interface WorkerResponse extends RunResult {
  id: string;
}

const runTimeoutMilliseconds = 45_000;

export function PythonWorkbench({
  assignment,
}: PythonWorkbenchProps): React.ReactElement {
  const definition = workbenches[assignment];
  const storageKey = `zhasyl:workspace:${assignment}:v1`;
  const [code, setCode] = useState(definition.starterCode);
  const [output, setOutput] = useState(
    "Нажми «Запустить и проверить», когда код будет готов.",
  );
  const [checks, setChecks] = useState<CheckResult[] | null>(null);
  const [runState, setRunState] = useState<"idle" | "running">("idle");
  const [saveState, setSaveState] = useState(
    "Черновик сохраняется в этом браузере",
  );
  const workerRef = useRef<Worker | null>(null);
  const timeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  useEffect(() => {
    const savedCode = window.localStorage.getItem(storageKey);
    let restoreTimer: ReturnType<typeof setTimeout> | undefined;

    if (savedCode) {
      restoreTimer = setTimeout(() => {
        setCode(savedCode);
        setSaveState("Восстановлен локальный черновик");
      }, 0);
    }

    return () => {
      if (restoreTimer) {
        clearTimeout(restoreTimer);
      }
      workerRef.current?.terminate();
      if (timeoutRef.current) {
        clearTimeout(timeoutRef.current);
      }
    };
  }, [storageKey]);

  function saveCode(nextCode: string): void {
    setCode(nextCode);
    window.localStorage.setItem(storageKey, nextCode);
    setSaveState("Сохранено в этом браузере");
  }

  function getWorker(): Worker {
    workerRef.current ??= new Worker("/workers/python-worker.mjs", {
      type: "module",
    });
    return workerRef.current;
  }

  function stopWorker(message: string): void {
    workerRef.current?.terminate();
    workerRef.current = null;
    setRunState("idle");
    setOutput(message);
    setChecks(null);
  }

  function runCode(): void {
    const worker = getWorker();
    const id = crypto.randomUUID();
    setRunState("running");
    setOutput(
      "Загружаем Python и запускаем код… Первый запуск может занять несколько секунд.",
    );
    setChecks(null);

    worker.onmessage = (event: MessageEvent<WorkerResponse>): void => {
      if (event.data.id !== id) {
        return;
      }

      if (timeoutRef.current) {
        clearTimeout(timeoutRef.current);
      }
      const result = { ok: event.data.ok, output: event.data.output };
      setRunState("idle");
      setOutput(result.output || "Код выполнен. Программа ничего не вывела.");
      setChecks(definition.checks(code, result));
    };

    worker.postMessage({ id, code });
    timeoutRef.current = setTimeout(() => {
      stopWorker(
        "Выполнение остановлено через 10 секунд. Проверь, нет ли бесконечного цикла.",
      );
    }, runTimeoutMilliseconds);
  }

  function restoreStarter(): void {
    if (
      !window.confirm(
        "Вернуть стартовый код? Текущий локальный черновик будет заменён.",
      )
    ) {
      return;
    }

    saveCode(definition.starterCode);
    setOutput("Стартовый код восстановлен.");
    setChecks(null);
  }

  const passedCount = checks?.filter((check) => check.passed).length ?? 0;

  return (
    <section
      className={styles.workbench}
      aria-labelledby={`${assignment}-title`}
    >
      <div className={styles.workbenchHeader}>
        <div>
          <p>Научный журнал · Python</p>
          <h3 id={`${assignment}-title`}>{definition.title}</h3>
        </div>
        <div className={styles.fileStatus}>
          <span>{definition.fileName}</span>
          <small role="status">{saveState}</small>
        </div>
      </div>

      <div className={styles.workspaceGrid}>
        <div className={styles.editorPanel}>
          <div className={styles.panelLabel}>Редактор</div>
          <textarea
            aria-label="Редактор Python-кода"
            value={code}
            onChange={(event) => saveCode(event.target.value)}
            spellCheck={false}
            autoCapitalize="off"
            autoCorrect="off"
          />
        </div>

        <div className={styles.outputPanel}>
          <div className={styles.panelLabel}>Вывод программы</div>
          <pre aria-live="polite">{output}</pre>
        </div>
      </div>

      <div className={styles.workbenchActions}>
        <button
          className={styles.runButton}
          type="button"
          onClick={runCode}
          disabled={runState === "running"}
        >
          <span aria-hidden="true">▶</span>
          {runState === "running"
            ? "Python работает…"
            : "Запустить и проверить"}
        </button>
        {runState === "running" ? (
          <button
            type="button"
            className={styles.secondaryButton}
            onClick={() => stopWorker("Выполнение остановлено.")}
          >
            Остановить
          </button>
        ) : (
          <button
            type="button"
            className={styles.secondaryButton}
            onClick={restoreStarter}
          >
            Вернуть стартовый код
          </button>
        )}
      </div>

      {checks ? (
        <div className={styles.checkPanel} aria-live="polite">
          <div className={styles.checkSummary}>
            <strong>Проверка системы</strong>
            <span>
              {passedCount} из {checks.length}
            </span>
          </div>
          <ul>
            {checks.map((check) => (
              <li key={check.code} data-passed={check.passed}>
                <span aria-hidden="true">{check.passed ? "✓" : "·"}</span>
                {check.label}
              </li>
            ))}
          </ul>
        </div>
      ) : null}
    </section>
  );
}
