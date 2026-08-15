"use client";

import { useActionState } from "react";
import {
  createChildAction,
  createPairingCodeAction,
  type AdultActionState,
} from "@/app/adult/actions";
import styles from "./identity.module.css";

const initialState: AdultActionState = { status: "idle" };

export function CreateChildForm(): React.ReactElement {
  const [state, action, pending] = useActionState(
    createChildAction,
    initialState,
  );

  return (
    <form className={styles.form} action={action}>
      <label htmlFor="child-display-name">Как показывать имя ребёнка</label>
      <div className={styles.formRow}>
        <input
          id="child-display-name"
          name="displayName"
          type="text"
          maxLength={60}
          autoComplete="off"
          required
        />
        <button type="submit" disabled={pending}>
          {pending ? "Создаём…" : "Создать профиль"}
        </button>
      </div>
      <p className={styles.formHint}>Email и пароль ребёнку не нужны.</p>
      {state.message ? (
        <p
          className={state.status === "error" ? styles.error : styles.success}
          role="status"
        >
          {state.message}
        </p>
      ) : null}
    </form>
  );
}

export function PairingCodeForm({
  childId,
  displayName,
}: {
  childId: string;
  displayName: string;
}): React.ReactElement {
  const pairingAction = createPairingCodeAction.bind(null, childId);
  const [state, action, pending] = useActionState(pairingAction, initialState);

  return (
    <form className={styles.pairingForm} action={action}>
      <button type="submit" disabled={pending}>
        {pending ? "Создаём код…" : `Подключить устройство для ${displayName}`}
      </button>
      {state.code ? (
        <div className={styles.codePanel} role="status">
          <span>Код подключения</span>
          <strong>{state.code}</strong>
          <p>{state.message}</p>
          <a href="/connect">Открыть экран ввода кода</a>
        </div>
      ) : state.message ? (
        <p className={styles.error} role="status">
          {state.message}
        </p>
      ) : null}
    </form>
  );
}
