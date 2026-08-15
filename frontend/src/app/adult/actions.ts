"use server";

import { revalidatePath } from "next/cache";
import { z } from "zod";
import { auth, signIn, signOut } from "@/auth";
import {
  createChildProfile,
  createPairingCode,
  revokeChildDevice,
} from "@/lib/adult-api";
import { ApiError } from "@/lib/problem-details";

export interface AdultActionState {
  status: "idle" | "success" | "error";
  message?: string;
  code?: string;
  expiresAt?: string;
}

const childSchema = z.object({
  displayName: z.string().trim().min(1).max(60),
});

export async function signInWithProvider(formData: FormData): Promise<void> {
  const provider = z
    .enum(["google", "github", "gitlab"])
    .parse(formData.get("provider"));
  await signIn(provider, { redirectTo: "/adult" });
}

export async function signInForDevelopment(): Promise<void> {
  if (process.env.NODE_ENV === "production") {
    return;
  }
  await signIn("development", {
    email: "parent@local.zhasyl",
    redirectTo: "/adult",
  });
}

export async function signOutAdult(): Promise<void> {
  await signOut({ redirectTo: "/" });
}

export async function createChildAction(
  _previous: AdultActionState,
  formData: FormData,
): Promise<AdultActionState> {
  const parsed = childSchema.safeParse({
    displayName: formData.get("displayName"),
  });
  if (!parsed.success) {
    return { status: "error", message: "Введите имя длиной до 60 символов." };
  }

  const session = await auth();
  if (!session) {
    return { status: "error", message: "Сессия завершилась. Войдите ещё раз." };
  }

  try {
    await createChildProfile(session, parsed.data.displayName);
    revalidatePath("/adult");
    return { status: "success", message: "Профиль создан." };
  } catch (error) {
    return adultError(error);
  }
}

export async function createPairingCodeAction(
  childId: string,
  _previous: AdultActionState,
): Promise<AdultActionState> {
  void _previous;
  const session = await auth();
  if (!session) {
    return { status: "error", message: "Сессия завершилась. Войдите ещё раз." };
  }

  try {
    const code = await createPairingCode(session, childId);
    return {
      status: "success",
      message:
        "Код действует 10 минут и подходит только для одного подключения.",
      code: code.code,
      expiresAt: code.expiresAt,
    };
  } catch (error) {
    return adultError(error);
  }
}

export async function revokeDeviceAction(formData: FormData): Promise<void> {
  const values = z
    .object({ childId: z.string().uuid(), deviceId: z.string().uuid() })
    .parse({
      childId: formData.get("childId"),
      deviceId: formData.get("deviceId"),
    });
  const session = await auth();
  if (!session) {
    return;
  }

  await revokeChildDevice(session, values.childId, values.deviceId);
  revalidatePath("/adult");
}

function adultError(error: unknown): AdultActionState {
  if (error instanceof ApiError && error.status === 409) {
    return {
      status: "error",
      message: "Профиль с таким именем уже существует.",
    };
  }
  return {
    status: "error",
    message: "Не удалось выполнить действие. Попробуйте ещё раз.",
  };
}
