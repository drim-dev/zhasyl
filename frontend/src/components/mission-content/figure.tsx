"use client";

import { useEffect, useRef, type ReactNode } from "react";
import styles from "./mission-content.module.css";

interface FigureProps {
  caption: string;
  source?: string;
  children: ReactNode;
}

export function Figure({
  caption,
  source,
  children,
}: FigureProps): React.ReactElement {
  const dialogRef = useRef<HTMLDialogElement>(null);

  useEffect(() => {
    const dialog = dialogRef.current;
    if (!dialog) {
      return;
    }

    const handleClose = (): void => {
      document.body.style.overflow = "";
    };

    dialog.addEventListener("close", handleClose);
    return () => {
      dialog.removeEventListener("close", handleClose);
      document.body.style.overflow = "";
    };
  }, []);

  function openDialog(): void {
    dialogRef.current?.showModal();
    document.body.style.overflow = "hidden";
  }

  function closeDialog(): void {
    dialogRef.current?.close();
  }

  return (
    <figure className={styles.figure}>
      <div className={styles.figureViewport}>{children}</div>
      <div className={styles.figureFooter}>
        <figcaption>
          <span>{caption}</span>
          {source ? <small>Источник: {source}</small> : null}
        </figcaption>
        <button
          className={styles.expandButton}
          type="button"
          onClick={openDialog}
          aria-label="Увеличить визуализацию"
        >
          <span aria-hidden="true">↗</span>
          Увеличить
        </button>
      </div>

      <dialog
        className={styles.figureDialog}
        ref={dialogRef}
        aria-label={caption}
        onClick={(event) => {
          if (event.target === event.currentTarget) {
            closeDialog();
          }
        }}
      >
        <div className={styles.dialogPanel}>
          <div className={styles.dialogHeader}>
            <p>{caption}</p>
            <button type="button" onClick={closeDialog} aria-label="Закрыть">
              ×
            </button>
          </div>
          <div className={styles.dialogVisual}>{children}</div>
        </div>
      </dialog>
    </figure>
  );
}
