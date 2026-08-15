import Link from "next/link";
import { redirect } from "next/navigation";
import { auth, configuredSocialProviders } from "@/auth";
import { signInForDevelopment, signInWithProvider } from "@/app/adult/actions";
import styles from "@/components/identity/identity.module.css";

const providerNames: Record<string, string> = {
  google: "Войти через Google",
  github: "Войти через GitHub",
  gitlab: "Войти через GitLab",
};

export default async function AdultSignInPage(): Promise<React.ReactElement> {
  if (await auth()) {
    redirect("/adult");
  }

  return (
    <main className={styles.page}>
      <div className={`${styles.panel} ${styles.centerPanel}`}>
        <p className={styles.eyebrow}>Доступ взрослого</p>
        <h1>Подготовьте профиль ребёнка</h1>
        <p>
          Взрослый управляет подключёнными устройствами и сохранённой работой.
          Ребёнку отдельный аккаунт и email не понадобятся.
        </p>
        <div className={styles.providerList}>
          {configuredSocialProviders.map((provider) => (
            <form action={signInWithProvider} key={provider}>
              <input type="hidden" name="provider" value={provider} />
              <button type="submit">{providerNames[provider]}</button>
            </form>
          ))}
          {process.env.NODE_ENV !== "production" ? (
            <form action={signInForDevelopment}>
              <button type="submit">Войти локально для разработки</button>
            </form>
          ) : null}
        </div>
        <Link className={styles.quietLink} href="/">
          Вернуться на станцию
        </Link>
      </div>
    </main>
  );
}
