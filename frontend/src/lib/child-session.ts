import { cookies } from "next/headers";
import { requireOk } from "@/lib/problem-details";

export const childSessionCookie = "zhasyl.child-session";

export interface ChildSession {
  childId: string;
  displayName: string;
  learningLocale: string;
}

function backendUrl(path: string): URL {
  const baseUrl = process.env.API_BASE_URL;
  if (!baseUrl) {
    throw new Error("API_BASE_URL is not configured.");
  }
  return new URL(path, baseUrl);
}

export async function getChildSession(): Promise<ChildSession | null> {
  const token = (await cookies()).get(childSessionCookie)?.value;
  if (!token) {
    return null;
  }

  const response = await fetch(backendUrl("/api/child/session"), {
    cache: "no-store",
    headers: { "X-Child-Session": token },
  });
  if (response.status === 401) {
    return null;
  }
  await requireOk(response);
  return (await response.json()) as ChildSession;
}
