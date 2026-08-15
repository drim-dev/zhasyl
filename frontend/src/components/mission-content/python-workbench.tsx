"use client";

import { useEffect, useRef, useState } from "react";
import {
  workbenches,
  type CheckResult,
  type RunResult,
  type WorkbenchId,
} from "./python-checks";
import {
  loadWorkspace,
  saveWorkspace,
  WorkspaceConflictError,
} from "@/lib/workspace-client";
import styles from "./python-workbench.module.css";

export interface PythonWorkbenchProps {
  assignment: WorkbenchId;
  assignmentRevisionId: string;
}

interface WorkerResponse extends RunResult {
  id: string;
}

interface LocalSyncState {
  serverVersion: number;
  dirty: boolean;
}

const runTimeoutMilliseconds = 45_000;

export function PythonWorkbench({
  assignment,
  assignmentRevisionId,
}: PythonWorkbenchProps): React.ReactElement {
  const definition = workbenches[assignment];
  const storageKey = `zhasyl:workspace:${assignment}:v1`;
  const syncStorageKey = `${storageKey}:sync`;
  const [code, setCode] = useState(definition.starterCode);
  const [output, setOutput] = useState(
    "Нажми «Запустить и проверить», когда код будет готов.",
  );
  const [checks, setChecks] = useState<CheckResult[] | null>(null);
  const [runState, setRunState] = useState<"idle" | "running">("idle");
  const [saveState, setSaveState] = useState("Проверяем сохранённую работу…");
  const [workspaceReady, setWorkspaceReady] = useState(false);
  const [hasConflict, setHasConflict] = useState(false);
  const workerRef = useRef<Worker | null>(null);
  const timeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const saveTimerRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const versionRef = useRef(0);
  const pairedRef = useRef(false);
  const savingRef = useRef(false);
  const pendingCodeRef = useRef<string | null>(null);
  const syncBlockedRef = useRef(false);
  const mountedRef = useRef(true);

  useEffect(() => {
    mountedRef.current = true;
    const localCode = window.localStorage.getItem(storageKey);
    const localSync = readLocalSyncState(syncStorageKey);

    async function restore(): Promise<void> {
      try {
        const remote = await loadWorkspace(assignmentRevisionId);
        if (!mountedRef.current) return;
        if (!remote) {
          if (localCode) setCode(localCode);
          setSaveState(
            localCode
              ? "Восстановлен черновик из этого браузера"
              : "Черновик сохраняется в этом браузере",
          );
          setWorkspaceReady(true);
          return;
        }

        pairedRef.current = true;
        versionRef.current = remote.version;
        if (remote.code !== null) {
          if (localCode && localSync.dirty && localCode !== remote.code) {
            setCode(localCode);
            if (localSync.serverVersion === remote.version) {
              setSaveState("Отправляем несохранённый черновик на станцию…");
              const saved = await saveWorkspace(
                assignmentRevisionId,
                remote.version,
                localCode,
              );
              if (!mountedRef.current) return;
              versionRef.current = saved.version;
              writeLocalSyncState(syncStorageKey, saved.version, false);
              setSaveState(`Сохранено на станции · версия ${saved.version}`);
            } else {
              syncBlockedRef.current = true;
              setHasConflict(true);
              setSaveState(
                "На станции есть другая версия · локальный черновик сохранён",
              );
            }
          } else {
            setCode(remote.code);
            window.localStorage.setItem(storageKey, remote.code);
            writeLocalSyncState(syncStorageKey, remote.version, false);
            setSaveState(`Восстановлено со станции · версия ${remote.version}`);
          }
        } else if (localCode) {
          setCode(localCode);
          setSaveState("Переносим локальный черновик на станцию…");
          const saved = await saveWorkspace(
            assignmentRevisionId,
            remote.version,
            localCode,
          );
          if (!mountedRef.current) return;
          versionRef.current = saved.version;
          writeLocalSyncState(syncStorageKey, saved.version, false);
          setSaveState(`Сохранено на станции · версия ${saved.version}`);
        } else {
          setSaveState("Устройство подключено · изменений пока нет");
        }
      } catch {
        if (localCode) setCode(localCode);
        setSaveState("Нет связи со станцией · черновик сохранится в браузере");
      } finally {
        if (mountedRef.current) setWorkspaceReady(true);
      }
    }
    void restore();

    return () => {
      mountedRef.current = false;
      workerRef.current?.terminate();
      if (timeoutRef.current) {
        clearTimeout(timeoutRef.current);
      }
      if (saveTimerRef.current) {
        clearTimeout(saveTimerRef.current);
      }
    };
  }, [assignmentRevisionId, storageKey, syncStorageKey]);

  async function flushServerSave(): Promise<void> {
    if (savingRef.current || syncBlockedRef.current) return;
    savingRef.current = true;
    while (pendingCodeRef.current !== null && !syncBlockedRef.current) {
      const content = pendingCodeRef.current;
      pendingCodeRef.current = null;
      try {
        const saved = await saveWorkspace(
          assignmentRevisionId,
          versionRef.current,
          content,
        );
        versionRef.current = saved.version;
        writeLocalSyncState(
          syncStorageKey,
          saved.version,
          window.localStorage.getItem(storageKey) !== content,
        );
        if (mountedRef.current) {
          setSaveState(`Сохранено на станции · версия ${saved.version}`);
        }
      } catch (error) {
        if (error instanceof WorkspaceConflictError) {
          syncBlockedRef.current = true;
          if (mountedRef.current) {
            setHasConflict(true);
            setSaveState(
              "На другом устройстве есть новая версия · выберите версию станции",
            );
          }
        } else if (mountedRef.current) {
          setSaveState("Нет связи со станцией · черновик сохранён в браузере");
        }
      }
    }
    savingRef.current = false;
  }

  function saveCode(nextCode: string): void {
    setCode(nextCode);
    window.localStorage.setItem(storageKey, nextCode);
    writeLocalSyncState(syncStorageKey, versionRef.current, true);
    if (!pairedRef.current) {
      setSaveState("Сохранено в этом браузере");
      return;
    }
    pendingCodeRef.current = nextCode;
    setSaveState("Сохраняем на станции…");
    if (saveTimerRef.current) clearTimeout(saveTimerRef.current);
    saveTimerRef.current = setTimeout(() => void flushServerSave(), 800);
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
        "Выполнение остановлено через 45 секунд. Проверь, нет ли бесконечного цикла.",
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

  function downloadCode(): void {
    const url = URL.createObjectURL(
      new Blob([code], { type: "text/x-python;charset=utf-8" }),
    );
    const link = document.createElement("a");
    link.href = url;
    link.download = definition.fileName;
    link.click();
    URL.revokeObjectURL(url);
  }

  async function restoreStationVersion(): Promise<void> {
    if (
      !window.confirm(
        "Загрузить версию со станции? Локальный черновик будет заменён.",
      )
    ) {
      return;
    }
    setSaveState("Загружаем версию со станции…");
    try {
      const remote = await loadWorkspace(assignmentRevisionId);
      if (!remote || remote.code === null) {
        throw new Error("No remote workspace.");
      }
      versionRef.current = remote.version;
      syncBlockedRef.current = false;
      setHasConflict(false);
      setCode(remote.code);
      window.localStorage.setItem(storageKey, remote.code);
      writeLocalSyncState(syncStorageKey, remote.version, false);
      setSaveState(`Восстановлено со станции · версия ${remote.version}`);
    } catch {
      setSaveState("Не удалось загрузить версию станции · черновик сохранён");
    }
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
            disabled={!workspaceReady}
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
          disabled={!workspaceReady || runState === "running"}
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
        <button
          type="button"
          className={styles.secondaryButton}
          onClick={downloadCode}
          disabled={!workspaceReady}
        >
          Скачать файл
        </button>
        {hasConflict ? (
          <button
            type="button"
            className={styles.secondaryButton}
            onClick={() => void restoreStationVersion()}
          >
            Загрузить версию станции
          </button>
        ) : null}
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

function readLocalSyncState(key: string): LocalSyncState {
  try {
    const value: unknown = JSON.parse(
      window.localStorage.getItem(key) ?? "null",
    );
    if (
      typeof value === "object" &&
      value !== null &&
      "serverVersion" in value &&
      typeof value.serverVersion === "number" &&
      "dirty" in value &&
      typeof value.dirty === "boolean"
    ) {
      return { serverVersion: value.serverVersion, dirty: value.dirty };
    }
  } catch {
    // A damaged cache marker must not prevent access to the local source draft.
  }
  return { serverVersion: 0, dirty: true };
}

function writeLocalSyncState(
  key: string,
  serverVersion: number,
  dirty: boolean,
): void {
  window.localStorage.setItem(key, JSON.stringify({ serverVersion, dirty }));
}
