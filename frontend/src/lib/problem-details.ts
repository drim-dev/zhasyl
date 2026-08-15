export interface ProblemDetails {
  status: number;
  title: string;
  detail: string;
  errorCode?: string;
  errorCodes?: string[];
}

export class ApiError extends Error {
  constructor(
    public readonly status: number,
    public readonly problem: ProblemDetails,
  ) {
    super(problem.detail);
  }
}

export async function requireOk(response: Response): Promise<Response> {
  if (response.ok) {
    return response;
  }

  let payload: unknown;
  try {
    payload = await response.json();
  } catch {
    payload = null;
  }

  const problem = toProblemDetails(payload, response.status);
  throw new ApiError(response.status, problem);
}

function toProblemDetails(value: unknown, status: number): ProblemDetails {
  if (typeof value !== "object" || value === null) {
    return {
      status,
      title: "Request failed",
      detail: "The request could not be completed.",
    };
  }

  const record = value as Record<string, unknown>;
  return {
    status,
    title: typeof record.title === "string" ? record.title : "Request failed",
    detail:
      typeof record.detail === "string"
        ? record.detail
        : "The request could not be completed.",
    errorCode:
      typeof record.errorCode === "string" ? record.errorCode : undefined,
    errorCodes: Array.isArray(record.errorCodes)
      ? record.errorCodes.filter(
          (code): code is string => typeof code === "string",
        )
      : undefined,
  };
}
