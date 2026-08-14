import Link from "next/link";
import type { ReactNode } from "react";
import { StationHeader } from "@/components/station-header";
import type { MissionContent } from "@/types/station";
import styles from "./reader-layout.module.css";

interface MissionViewProps {
  mission: MissionContent;
  body: ReactNode;
}

export function MissionView({
  mission,
  body,
}: MissionViewProps): React.ReactElement {
  const firstAssignment = mission.assignments[0];

  return (
    <div className={styles.page}>
      <StationHeader />
      <main>
        <nav className={styles.breadcrumbs} aria-label="Навигация">
          <Link href="/">Лаборатории</Link>
          <span aria-hidden="true">/</span>
          <span>{mission.laboratoryName}</span>
        </nav>

        <header className={styles.missionHero}>
          <div>
            <p className={styles.eyebrow}>Миссия · {mission.laboratoryName}</p>
            <h1>{mission.name}</h1>
            <p className={styles.lead}>{mission.problem}</p>
            {firstAssignment ? (
              <Link
                className={styles.primaryAction}
                href={`/laboratories/${mission.laboratoryId}/missions/${mission.missionId}/assignments/${firstAssignment.assignmentId}`}
              >
                Начать первое задание
                <span aria-hidden="true">→</span>
              </Link>
            ) : null}
          </div>
          <dl className={styles.missionFacts}>
            <div>
              <dt>Статус</dt>
              <dd>{mission.status}</dd>
            </div>
            <div>
              <dt>Заданий доступно</dt>
              <dd>{mission.assignments.length}</dd>
            </div>
            <div>
              <dt>Версия материалов</dt>
              <dd>{mission.version}</dd>
            </div>
          </dl>
        </header>

        <div className={styles.missionGrid}>
          <article className={styles.missionBody}>{body}</article>
          <aside
            className={styles.assignmentRail}
            aria-labelledby="assignment-list-title"
          >
            <p className={styles.eyebrow}>Путь миссии</p>
            <h2 id="assignment-list-title">Задания станции</h2>
            {mission.assignments.length ? (
              <ol>
                {mission.assignments.map((assignment) => (
                  <li key={assignment.assignmentId}>
                    <span className={styles.assignmentNumber}>
                      {String(assignment.order).padStart(2, "0")}
                    </span>
                    <div>
                      <h3>{assignment.name}</h3>
                      <p>{assignment.objective}</p>
                      <span>{assignment.estimatedMinutes} минут</span>
                      <Link
                        href={`/laboratories/${mission.laboratoryId}/missions/${mission.missionId}/assignments/${assignment.assignmentId}`}
                      >
                        Открыть задание <span aria-hidden="true">→</span>
                      </Link>
                    </div>
                  </li>
                ))}
              </ol>
            ) : (
              <p className={styles.emptyState}>Первое задание готовится.</p>
            )}
          </aside>
        </div>
      </main>
      <footer className={styles.footer}>
        Станция «Жасыл-1» · {mission.locale.toUpperCase()}
      </footer>
    </div>
  );
}
