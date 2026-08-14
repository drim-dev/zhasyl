import Link from "next/link";
import { ThemeToggle } from "@/components/theme-toggle";
import type { StationOverview } from "@/types/station";
import styles from "@/app/page.module.css";

interface StationOverviewViewProps {
  overview: StationOverview;
}

export function StationOverviewView({
  overview,
}: StationOverviewViewProps): React.ReactElement {
  return (
    <main className={styles.page}>
      <header className={styles.topbar}>
        <a className={styles.stationIdentity} href="#station-briefing">
          <span className={styles.stationMark} aria-hidden="true">
            Ж1
          </span>
          <span>
            <strong>{overview.stationName}</strong>
            <small>Система подготовки поселения</small>
          </span>
        </a>

        <div className={styles.headerControls}>
          <div className={styles.connectionStatus} role="status">
            <span className={styles.statusDot} aria-hidden="true" />
            Связь установлена
          </div>
          <ThemeToggle />
        </div>
      </header>

      <section className={styles.hero} id="station-briefing">
        <div className={styles.heroContent}>
          <p className={styles.eyebrow}>{overview.location}</p>
          <h1>Выбери задачу, которая важна для жизни на Марсе</h1>
          <p className={styles.lead}>{overview.briefing}</p>
          <a className={styles.primaryAction} href="#laboratories">
            Посмотреть лаборатории <span aria-hidden="true">↓</span>
          </a>
        </div>

        <aside
          className={styles.stationPriority}
          aria-labelledby="station-priority-title"
        >
          <div className={styles.priorityHeader}>
            <span>Приоритет станции</span>
            <span className={styles.priorityCode}>S-01</span>
          </div>
          <h2 id="station-priority-title">Подготовить станцию к расширению</h2>
          <p>
            Научные группы проверяют системы, от которых будет зависеть большая
            команда поселенцев.
          </p>
          <dl className={styles.stationFacts}>
            <div>
              <dt>Среда</dt>
              <dd>Марс</dd>
            </div>
            <div>
              <dt>Горизонт</dt>
              <dd>2035</dd>
            </div>
            <div>
              <dt>Статус</dt>
              <dd>Подготовка</dd>
            </div>
          </dl>
        </aside>
      </section>

      <section
        className={styles.laboratories}
        id="laboratories"
        aria-labelledby="laboratories-title"
      >
        <div className={styles.sectionHeading}>
          <div>
            <p className={styles.eyebrow}>Доступные направления</p>
            <h2 id="laboratories-title">Лаборатории станции</h2>
          </div>
          <p>
            Лаборатории независимы: можно начать с той задачи, которая
            интереснее именно сейчас.
          </p>
        </div>

        <div className={styles.laboratoryGrid}>
          {overview.laboratories.map((laboratory, index) => {
            const titleId = `laboratory-${laboratory.id}-title`;
            const missionHref = `/laboratories/${laboratory.id}/missions/${laboratory.firstMission.id}`;

            return (
              <article
                className={styles.laboratoryCard}
                key={laboratory.id}
                aria-labelledby={titleId}
              >
                <div className={styles.cardIndex} aria-hidden="true">
                  {String(index + 1).padStart(2, "0")}
                </div>
                <div className={styles.cardBody}>
                  <p className={styles.specialist}>
                    Специалист · {laboratory.specialist}
                  </p>
                  <h3 id={titleId}>{laboratory.name}</h3>
                  <p className={styles.purpose}>{laboratory.purpose}</p>

                  <div className={styles.missionPanel}>
                    <div className={styles.missionLabel}>
                      <span>Первая миссия</span>
                      <span className={styles.missionStatus}>
                        {laboratory.firstMission.status}
                      </span>
                    </div>
                    <h4>{laboratory.firstMission.name}</h4>
                    <p>{laboratory.firstMission.problem}</p>
                    <Link className={styles.missionAction} href={missionHref}>
                      Открыть миссию <span aria-hidden="true">→</span>
                    </Link>
                  </div>
                </div>
              </article>
            );
          })}
        </div>
      </section>

      <footer className={styles.footer}>
        <span>{overview.stationName}</span>
        <span>Данные станции · {overview.locale.toUpperCase()}</span>
      </footer>
    </main>
  );
}
