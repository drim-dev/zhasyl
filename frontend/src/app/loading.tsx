import styles from "./page.module.css";

export default function Loading(): React.ReactElement {
  return (
    <main className={styles.statePage}>
      <section className={styles.statePanel} role="status" aria-live="polite">
        <p className={styles.eyebrow}>Канал связи S-01</p>
        <h1>Получаем данные станции…</h1>
        <div className={styles.loadingTrack} aria-hidden="true">
          <span />
        </div>
      </section>
    </main>
  );
}
