import Link from "next/link";
import { ConnectDeviceForm } from "@/components/identity/connect-device-form";
import styles from "@/components/identity/identity.module.css";
import { getChildSession } from "@/lib/child-session";

export const dynamic = "force-dynamic";

export default async function ConnectPage(): Promise<React.ReactElement> {
  const child = await getChildSession();

  return (
    <main className={styles.page}>
      <div className={`${styles.panel} ${styles.centerPanel}`}>
        <p className={styles.eyebrow}>Подключение к станции</p>
        {child ? (
          <>
            <h1>Устройство подключено</h1>
            <p>
              Профиль: <strong>{child.displayName}</strong>. Теперь работа
              сможет сохраняться на станции.
            </p>
            <Link className={styles.quietLink} href="/">
              Перейти к лабораториям
            </Link>
          </>
        ) : (
          <>
            <h1>Введите код подключения</h1>
            <p>
              Попросите взрослого открыть панель профиля и создать одноразовый
              код.
            </p>
            <ConnectDeviceForm />
            <Link className={styles.quietLink} href="/adult">
              Войти взрослому
            </Link>
            <Link className={styles.quietLink} href="/">
              Вернуться на станцию
            </Link>
          </>
        )}
      </div>
    </main>
  );
}
