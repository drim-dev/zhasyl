import { NextResponse } from "next/server";
import { z } from "zod";
import { childSessionCookie } from "@/lib/child-session";

const requestSchema = z.object({
  code: z.string().trim().min(8).max(9),
  deviceName: z.string().trim().min(1).max(80),
});

interface PairingResponse {
  sessionToken: string;
  expiresAt: string;
  childId: string;
  displayName: string;
  learningLocale: string;
}

export async function POST(request: Request): Promise<Response> {
  const parsed = requestSchema.safeParse(await request.json());
  if (!parsed.success) {
    return NextResponse.json({ error: "invalid_request" }, { status: 400 });
  }

  const apiBaseUrl = process.env.API_BASE_URL;
  if (!apiBaseUrl) {
    return NextResponse.json({ error: "service_unavailable" }, { status: 503 });
  }
  const backendResponse = await fetch(new URL("/api/child/pair", apiBaseUrl), {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "X-Zhasyl-Client-Address": clientAddress(request),
    },
    body: JSON.stringify(parsed.data),
  });
  if (!backendResponse.ok) {
    return new Response(await backendResponse.text(), {
      status: backendResponse.status,
      headers: { "Content-Type": "application/problem+json" },
    });
  }

  const pairing = (await backendResponse.json()) as PairingResponse;
  const response = NextResponse.json({
    childId: pairing.childId,
    displayName: pairing.displayName,
    learningLocale: pairing.learningLocale,
  });
  response.cookies.set(childSessionCookie, pairing.sessionToken, {
    httpOnly: true,
    sameSite: "lax",
    secure: process.env.NODE_ENV === "production",
    path: "/",
    expires: new Date(pairing.expiresAt),
  });
  return response;
}

function clientAddress(request: Request): string {
  const address =
    request.headers.get("cf-connecting-ip") ??
    request.headers.get("x-forwarded-for")?.split(",")[0]?.trim() ??
    "local";
  return address.slice(0, 128);
}
