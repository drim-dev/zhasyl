import type { AnchorHTMLAttributes, ReactNode } from "react";
import styles from "./mission-content.module.css";

interface ContentBlockProps {
  title?: string;
  children: ReactNode;
}

interface StationMessageProps extends ContentBlockProps {
  author: string;
  role: string;
}

interface PredictionProps {
  prompt: string;
}

interface HintProps extends ContentBlockProps {
  level: string;
}

export function StationMessage({
  author,
  role,
  children,
}: StationMessageProps): React.ReactElement {
  return (
    <aside
      className={styles.stationMessage}
      aria-label={`Сообщение от ${author}`}
    >
      <div className={styles.messageIdentity} aria-hidden="true">
        {author
          .split(" ")
          .map((part) => part[0])
          .join("")}
      </div>
      <div>
        <p className={styles.messageMeta}>Входящее сообщение · {role}</p>
        <h2>{author}</h2>
        <div>{children}</div>
      </div>
    </aside>
  );
}

export function ResearcherNote({
  title = "Заметка исследователя",
  children,
}: ContentBlockProps): React.ReactElement {
  return (
    <aside className={styles.researcherNote}>
      <p className={styles.blockLabel}>Граница знания</p>
      <h3>{title}</h3>
      <div>{children}</div>
    </aside>
  );
}

export function Prediction({ prompt }: PredictionProps): React.ReactElement {
  return (
    <aside className={styles.prediction}>
      <div className={styles.blockNumber} aria-hidden="true">
        ?
      </div>
      <div>
        <p className={styles.blockLabel}>Сначала прогноз</p>
        <p>{prompt}</p>
      </div>
    </aside>
  );
}

export function Hint({
  level,
  title = "Подсказка",
  children,
}: HintProps): React.ReactElement {
  return (
    <details className={styles.hint}>
      <summary>
        <span>Подсказка {level}</span>
        <strong>{title}</strong>
      </summary>
      <div>{children}</div>
    </details>
  );
}

export function SystemCriteria({
  title = "Проверка системы",
  children,
}: ContentBlockProps): React.ReactElement {
  return (
    <section className={styles.systemCriteria} aria-label={title}>
      <p className={styles.blockLabel}>Критерии готовности</p>
      <h3>{title}</h3>
      <div>{children}</div>
    </section>
  );
}

export function JournalPrompt({
  title = "Научный журнал",
  children,
}: ContentBlockProps): React.ReactElement {
  return (
    <section className={styles.journalPrompt} aria-label={title}>
      <div className={styles.journalIcon} aria-hidden="true">
        Ж
      </div>
      <div>
        <p className={styles.blockLabel}>Научный журнал</p>
        <h3>{title}</h3>
        <div>{children}</div>
      </div>
    </section>
  );
}

export function SafeLink({
  href = "",
  children,
  ...props
}: AnchorHTMLAttributes<HTMLAnchorElement>): React.ReactElement {
  const isExternal = href.startsWith("https://") || href.startsWith("http://");
  const isInternal = href.startsWith("/") || href.startsWith("#");

  if (!isExternal && !isInternal) {
    return <span>{children}</span>;
  }

  return (
    <a
      {...props}
      href={href}
      target={isExternal ? "_blank" : undefined}
      rel={isExternal ? "noreferrer" : undefined}
    >
      {children}
    </a>
  );
}
