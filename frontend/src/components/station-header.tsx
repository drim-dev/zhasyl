import Link from "next/link";
import { ThemeToggle } from "@/components/theme-toggle";
import styles from "./reader-layout.module.css";

export function StationHeader(): React.ReactElement {
  return (
    <header className={styles.topbar}>
      <Link className={styles.stationIdentity} href="/">
        <span className={styles.stationMark} aria-hidden="true">
          Ж1
        </span>
        <span>
          <strong>Станция «Жасыл-1»</strong>
          <small>Система подготовки поселения</small>
        </span>
      </Link>
      <div className={styles.headerControls}>
        <Link className={styles.accessLink} href="/connect">
          Доступ
        </Link>
        <div className={styles.connectionStatus} role="status">
          <span aria-hidden="true" />
          Связь установлена
        </div>
        <ThemeToggle />
      </div>
    </header>
  );
}
