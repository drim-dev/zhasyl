import type { Session } from "next-auth";
import { requireOk } from "@/lib/problem-details";

export interface ChildDevice {
  deviceId: string;
  deviceName: string;
  createdAt: string;
  expiresAt: string;
  isRevoked: boolean;
}

export interface ChildProfile {
  childId: string;
  displayName: string;
  learningLocale: string;
  devices: ChildDevice[];
}

export interface ChildProfilesResponse {
  children: ChildProfile[];
}

export interface PairingCodeResponse {
  code: string;
  expiresAt: string;
}

function adultHeaders(session: Session): HeadersInit {
  return {
    Accept: "application/json",
    "X-Adult-Id": session.user.id,
    "X-Adult-Email": session.user.email ?? "",
  };
}

function backendUrl(path: string): URL {
  const baseUrl = process.env.API_BASE_URL;
  if (!baseUrl) {
    throw new Error("API_BASE_URL is not configured.");
  }
  return new URL(path, baseUrl);
}

export async function listChildProfiles(
  session: Session,
): Promise<ChildProfilesResponse> {
  const response = await requireOk(
    await fetch(backendUrl("/api/adult/children"), {
      cache: "no-store",
      headers: adultHeaders(session),
    }),
  );
  return (await response.json()) as ChildProfilesResponse;
}

export async function createChildProfile(
  session: Session,
  displayName: string,
): Promise<void> {
  await requireOk(
    await fetch(backendUrl("/api/adult/children"), {
      method: "POST",
      headers: {
        ...adultHeaders(session),
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ displayName, learningLocale: "ru" }),
    }),
  );
}

export async function createPairingCode(
  session: Session,
  childId: string,
): Promise<PairingCodeResponse> {
  const response = await requireOk(
    await fetch(backendUrl(`/api/adult/children/${childId}/pairing-codes`), {
      method: "POST",
      headers: adultHeaders(session),
    }),
  );
  return (await response.json()) as PairingCodeResponse;
}

export async function revokeChildDevice(
  session: Session,
  childId: string,
  deviceId: string,
): Promise<void> {
  await requireOk(
    await fetch(
      backendUrl(`/api/adult/children/${childId}/devices/${deviceId}`),
      {
        method: "DELETE",
        headers: adultHeaders(session),
      },
    ),
  );
}
