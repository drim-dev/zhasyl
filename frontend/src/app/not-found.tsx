import Link from "next/link";
import styles from "./page.module.css";

export default function NotFound(): React.ReactElement {
  return (
    <main className={styles.statePage}>
      <section className={styles.statePanel} aria-labelledby="not-found-title">
        <p className={styles.eyebrow}>Модуль не найден</p>
        <h1 id="not-found-title">Такого отсека пока нет</h1>
        <p>Вернитесь к обзору станции и выберите доступную лабораторию.</p>
        <Link className={styles.primaryAction} href="/">
          К обзору станции
        </Link>
      </section>
    </main>
  );
}
