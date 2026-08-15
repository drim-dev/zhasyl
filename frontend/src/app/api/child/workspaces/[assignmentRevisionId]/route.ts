import { cookies } from "next/headers";
import { NextResponse } from "next/server";
import { z } from "zod";
import { childSessionCookie } from "@/lib/child-session";

const parametersSchema = z.object({ assignmentRevisionId: z.string().uuid() });
const saveSchema = z.object({
  expectedVersion: z.number().int().nonnegative(),
  code: z.string().max(200_000),
});

interface RouteContext {
  params: Promise<{ assignmentRevisionId: string }>;
}

export async function GET(
  _request: Request,
  context: RouteContext,
): Promise<Response> {
  return proxyWorkspace(context, undefined);
}

export async function PUT(
  request: Request,
  context: RouteContext,
): Promise<Response> {
  const body: unknown = await request.json().catch(() => null);
  const parsedBody = saveSchema.safeParse(body);
  if (!parsedBody.success) {
    return NextResponse.json({ error: "invalid_request" }, { status: 400 });
  }
  return proxyWorkspace(context, parsedBody.data);
}

async function proxyWorkspace(
  context: RouteContext,
  body: z.infer<typeof saveSchema> | undefined,
): Promise<Response> {
  const parameters = parametersSchema.safeParse(await context.params);
  const token = (await cookies()).get(childSessionCookie)?.value;
  if (!parameters.success || !token) {
    return NextResponse.json(
      { error: "child_session_required" },
      { status: 401 },
    );
  }

  const apiBaseUrl = process.env.API_BASE_URL;
  if (!apiBaseUrl) {
    return NextResponse.json({ error: "service_unavailable" }, { status: 503 });
  }
  const backendResponse = await fetch(
    new URL(
      `/api/child/workspaces/${parameters.data.assignmentRevisionId}`,
      apiBaseUrl,
    ),
    {
      method: body ? "PUT" : "GET",
      headers: {
        "X-Child-Session": token,
        ...(body ? { "Content-Type": "application/json" } : {}),
      },
      body: body ? JSON.stringify(body) : undefined,
      cache: "no-store",
    },
  );
  return new Response(await backendResponse.text(), {
    status: backendResponse.status,
    headers: {
      "Content-Type":
        backendResponse.headers.get("Content-Type") ?? "application/json",
    },
  });
}
