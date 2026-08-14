"use client";

import Link from "next/link";
import { StationHeader } from "@/components/station-header";
import styles from "@/components/reader-state.module.css";

export default function ReaderError({
  reset,
}: {
  reset: () => void;
}): React.ReactElement {
  return (
    <div className={styles.page}>
      <StationHeader />
      <main className={styles.errorState}>
        <p>Канал данных временно недоступен</p>
        <h1>Не удалось открыть материалы</h1>
        <span>
          Повтори запрос. Твой локальный черновик кода останется в этом
          браузере.
        </span>
        <div>
          <button type="button" onClick={reset}>
            Повторить
          </button>
          <Link href="/">К лабораториям</Link>
        </div>
      </main>
    </div>
  );
}
