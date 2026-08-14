"use client";

import { useEffect } from "react";
import styles from "./page.module.css";

interface ErrorPageProps {
  error: Error & { digest?: string };
  reset: () => void;
}

export default function ErrorPage({
  error,
  reset,
}: ErrorPageProps): React.ReactElement {
  useEffect(() => {
    console.error(error);
  }, [error]);

  return (
    <main className={styles.statePage}>
      <section className={styles.statePanel} aria-labelledby="load-error-title">
        <p className={styles.eyebrow}>Связь со станцией прервана</p>
        <h1 id="load-error-title">Не удалось загрузить данные</h1>
        <p>
          Проверьте соединение и попробуйте ещё раз. Выполненная работа при этом
          не изменится.
        </p>
        <button className={styles.primaryAction} onClick={reset} type="button">
          Повторить попытку
        </button>
      </section>
    </main>
  );
}
