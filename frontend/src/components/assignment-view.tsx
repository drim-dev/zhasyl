import Link from "next/link";
import type { ReactNode } from "react";
import { StationHeader } from "@/components/station-header";
import type { AssignmentContent } from "@/types/station";
import styles from "./reader-layout.module.css";

interface AssignmentViewProps {
  assignment: AssignmentContent;
  body: ReactNode;
}

export function AssignmentView({
  assignment,
  body,
}: AssignmentViewProps): React.ReactElement {
  const missionHref = `/laboratories/${assignment.laboratoryId}/missions/${assignment.missionId}`;

  return (
    <div className={styles.page}>
      <StationHeader />
      <main>
        <nav className={styles.breadcrumbs} aria-label="Навигация">
          <Link href="/">Лаборатории</Link>
          <span aria-hidden="true">/</span>
          <Link href={missionHref}>{assignment.missionName}</Link>
          <span aria-hidden="true">/</span>
          <span>Задание {assignment.order}</span>
        </nav>

        <header className={styles.assignmentHero}>
          <p className={styles.eyebrow}>
            Задание станции · {String(assignment.order).padStart(2, "0")}
          </p>
          <h1>{assignment.name}</h1>
          <p className={styles.lead}>{assignment.objective}</p>
          <div className={styles.assignmentMeta}>
            <span>{assignment.estimatedMinutes} минут</span>
            <span>Python в браузере</span>
            <span>Черновик сохраняется локально</span>
          </div>
        </header>

        <article className={styles.assignmentBody}>{body}</article>

        <nav className={styles.assignmentFooter} aria-label="После задания">
          <div>
            <p className={styles.eyebrow}>Миссия продолжается</p>
            <strong>{assignment.missionName}</strong>
          </div>
          <Link href={missionHref}>
            Вернуться к миссии <span aria-hidden="true">→</span>
          </Link>
        </nav>
      </main>
      <footer className={styles.footer}>
        Станция «Жасыл-1» · {assignment.locale.toUpperCase()}
      </footer>
    </div>
  );
}
