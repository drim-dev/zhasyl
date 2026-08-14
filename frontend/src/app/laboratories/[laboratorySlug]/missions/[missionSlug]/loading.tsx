import { StationHeader } from "@/components/station-header";
import styles from "@/components/reader-state.module.css";

export default function ReaderLoading(): React.ReactElement {
  return (
    <div
      className={styles.page}
      aria-busy="true"
      aria-label="Загружаем материалы станции"
    >
      <StationHeader />
      <main className={styles.loading}>
        <div className={styles.shortLine} />
        <div className={styles.titleLine} />
        <div className={styles.textLine} />
        <div className={styles.contentBlock} />
        <span className="srOnly">Загружаем материалы станции…</span>
      </main>
    </div>
  );
}
