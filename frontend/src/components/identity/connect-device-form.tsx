"use client";

import { useState, useSyncExternalStore } from "react";
import { z } from "zod";
import styles from "./identity.module.css";

const schema = z.object({
  code: z.string().trim().min(8).max(9),
  deviceName: z.string().trim().min(1).max(80),
});

const subscribeToHydration = (): (() => void) => () => {};

export function ConnectDeviceForm(): React.ReactElement {
  const ready = useSyncExternalStore(
    subscribeToHydration,
    () => true,
    () => false,
  );
  const [pending, setPending] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const submit = async (
    event: React.FormEvent<HTMLFormElement>,
  ): Promise<void> => {
    event.preventDefault();
    setError(null);
    const form = new FormData(event.currentTarget);
    const parsed = schema.safeParse({
      code: form.get("code"),
      deviceName: form.get("deviceName"),
    });
    if (!parsed.success) {
      setError("Проверьте код и название устройства.");
      return;
    }

    setPending(true);
    try {
      const response = await fetch("/api/child/pair", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(parsed.data),
      });
      if (!response.ok) {
        setError(
          response.status === 400
            ? "Код не подходит. Возможно, он уже использован или его время истекло."
            : "Не удалось подключить устройство. Попробуйте ещё раз.",
        );
        return;
      }
      window.location.assign("/connect");
    } finally {
      setPending(false);
    }
  };

  return (
    <form className={styles.form} onSubmit={submit}>
      <label htmlFor="pairing-code">Код от взрослого</label>
      <input
        id="pairing-code"
        name="code"
        className={styles.codeInput}
        inputMode="text"
        autoComplete="one-time-code"
        placeholder="ABCD-EFGH"
        maxLength={9}
        required
      />
      <label htmlFor="device-name">Название этого устройства</label>
      <input
        id="device-name"
        name="deviceName"
        type="text"
        defaultValue="Мой компьютер"
        maxLength={80}
        required
      />
      <button type="submit" disabled={pending || !ready}>
        {!ready
          ? "Готовим подключение…"
          : pending
            ? "Подключаем…"
            : "Подключить устройство"}
      </button>
      {error ? (
        <p className={styles.error} role="alert">
          {error}
        </p>
      ) : null}
    </form>
  );
}
