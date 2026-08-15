export interface WorkspaceSnapshot {
  assignmentRevisionId: string;
  version: number;
  code: string | null;
  savedAt: string | null;
}

export class WorkspaceConflictError extends Error {}

interface WorkspaceSaveResult {
  assignmentRevisionId: string;
  version: number;
  savedAt: string;
}

export async function loadWorkspace(
  assignmentRevisionId: string,
): Promise<WorkspaceSnapshot | null> {
  const response = await fetch(
    `/api/child/workspaces/${assignmentRevisionId}`,
    { cache: "no-store" },
  );
  if (response.status === 401) {
    return null;
  }
  if (!response.ok) {
    throw new Error("Workspace could not be loaded.");
  }
  return (await response.json()) as WorkspaceSnapshot;
}

export async function saveWorkspace(
  assignmentRevisionId: string,
  expectedVersion: number,
  code: string,
): Promise<WorkspaceSaveResult> {
  const response = await fetch(
    `/api/child/workspaces/${assignmentRevisionId}`,
    {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ expectedVersion, code }),
    },
  );
  if (response.status === 409) {
    throw new WorkspaceConflictError("Workspace has a newer version.");
  }
  if (!response.ok) {
    throw new Error("Workspace could not be saved.");
  }
  return (await response.json()) as WorkspaceSaveResult;
}
