"use client";

import { useSyncExternalStore } from "react";
import { useTheme } from "next-themes";
import styles from "@/app/page.module.css";

const subscribe = (): (() => void) => () => undefined;

export function ThemeToggle(): React.ReactElement {
  const { resolvedTheme, setTheme } = useTheme();
  const mounted = useSyncExternalStore(
    subscribe,
    () => true,
    () => false,
  );
  const isDark = mounted && resolvedTheme === "dark";
  const title = !mounted
    ? "Сменить цветовую тему"
    : isDark
      ? "Включить светлую тему"
      : "Включить тёмную тему";

  const toggleTheme = (): void => {
    const activeTheme =
      resolvedTheme ?? document.documentElement.getAttribute("data-theme");

    setTheme(activeTheme === "dark" ? "light" : "dark");
  };

  return (
    <button
      className={styles.themeToggle}
      type="button"
      aria-label="Переключить цветовую тему"
      title={title}
      onClick={toggleTheme}
    >
      <svg className={styles.themeIcon} viewBox="0 0 24 24" aria-hidden="true">
        {isDark ? (
          <path d="M12 3v2m0 14v2M3 12h2m14 0h2M5.64 5.64l1.42 1.42m9.88 9.88 1.42 1.42m0-12.72-1.42 1.42M7.06 16.94l-1.42 1.42M16 12a4 4 0 1 1-8 0 4 4 0 0 1 8 0Z" />
        ) : (
          <path d="M20.2 15.1A8.5 8.5 0 0 1 8.9 3.8 8.5 8.5 0 1 0 20.2 15.1Z" />
        )}
      </svg>
    </button>
  );
}
