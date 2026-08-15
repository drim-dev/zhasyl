import Link from "next/link";
import { redirect } from "next/navigation";
import { auth } from "@/auth";
import { signOutAdult, revokeDeviceAction } from "@/app/adult/actions";
import {
  CreateChildForm,
  PairingCodeForm,
} from "@/components/identity/adult-controls";
import styles from "@/components/identity/identity.module.css";
import { listChildProfiles } from "@/lib/adult-api";

const dateFormatter = new Intl.DateTimeFormat("ru", {
  day: "numeric",
  month: "long",
  year: "numeric",
});

export const dynamic = "force-dynamic";

export default async function AdultPage(): Promise<React.ReactElement> {
  const session = await auth();
  if (!session) {
    redirect("/adult/sign-in");
  }
  const { children } = await listChildProfiles(session);

  return (
    <main className={styles.page}>
      <div className={styles.shell}>
        <header className={styles.topbar}>
          <Link className={styles.stationLink} href="/">
            Станция «Жасыл-1»
          </Link>
          <form action={signOutAdult}>
            <button className={styles.signOut} type="submit">
              Выйти
            </button>
          </form>
        </header>
        <section className={styles.hero}>
          <p className={styles.eyebrow}>Панель взрослого</p>
          <h1>Доступ детей к станции</h1>
          <p>
            Создайте профиль, получите одноразовый код и введите его на
            компьютере ребёнка. Подключение можно отозвать в любой момент.
          </p>
        </section>
        <div className={styles.grid}>
          <section className={styles.panel}>
            <h2>Новый профиль</h2>
            <p>
              Сохраняем только имя для интерфейса и выбранный язык обучения.
            </p>
            <CreateChildForm />
          </section>
          <section className={styles.children} aria-label="Профили детей">
            {children.length === 0 ? (
              <div className={styles.panel}>
                <h2>Профилей пока нет</h2>
                <p>Создайте первый профиль в форме слева.</p>
              </div>
            ) : (
              children.map((child) => (
                <article className={styles.childCard} key={child.childId}>
                  <div className={styles.childHeader}>
                    <div>
                      <h2>{child.displayName}</h2>
                      <p className={styles.childMeta}>Язык обучения: русский</p>
                    </div>
                  </div>
                  <PairingCodeForm
                    childId={child.childId}
                    displayName={child.displayName}
                  />
                  <div className={styles.deviceList}>
                    <h3>Подключённые устройства</h3>
                    {child.devices.filter((device) => !device.isRevoked)
                      .length === 0 ? (
                      <p className={styles.childMeta}>
                        Активных устройств пока нет.
                      </p>
                    ) : (
                      <ul>
                        {child.devices
                          .filter((device) => !device.isRevoked)
                          .map((device) => (
                            <li className={styles.device} key={device.deviceId}>
                              <span>
                                {device.deviceName}
                                <small>
                                  Подключено{" "}
                                  {dateFormatter.format(
                                    new Date(device.createdAt),
                                  )}
                                </small>
                              </span>
                              <form action={revokeDeviceAction}>
                                <input
                                  type="hidden"
                                  name="childId"
                                  value={child.childId}
                                />
                                <input
                                  type="hidden"
                                  name="deviceId"
                                  value={device.deviceId}
                                />
                                <button type="submit">Отключить</button>
                              </form>
                            </li>
                          ))}
                      </ul>
                    )}
                  </div>
                </article>
              ))
            )}
          </section>
        </div>
      </div>
    </main>
  );
}
