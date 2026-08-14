import type {
  AssignmentContent,
  AssignmentSummary,
  LaboratorySummary,
  MissionContent,
  StationOverview,
} from "@/types/station";

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === "object" && value !== null;
}

function isLaboratory(value: unknown): value is LaboratorySummary {
  if (!isRecord(value)) {
    return false;
  }

  const mission = value.firstMission;

  return (
    typeof value.id === "string" &&
    typeof value.name === "string" &&
    typeof value.purpose === "string" &&
    typeof value.specialist === "string" &&
    isRecord(mission) &&
    typeof mission.id === "string" &&
    typeof mission.name === "string" &&
    typeof mission.problem === "string" &&
    typeof mission.status === "string"
  );
}

function isStationOverview(value: unknown): value is StationOverview {
  return (
    isRecord(value) &&
    typeof value.stationId === "string" &&
    typeof value.stationName === "string" &&
    typeof value.locale === "string" &&
    typeof value.location === "string" &&
    typeof value.briefing === "string" &&
    Array.isArray(value.laboratories) &&
    value.laboratories.every(isLaboratory)
  );
}

function isAssignmentSummary(value: unknown): value is AssignmentSummary {
  return (
    isRecord(value) &&
    typeof value.assignmentId === "string" &&
    typeof value.revisionId === "string" &&
    typeof value.version === "number" &&
    typeof value.order === "number" &&
    typeof value.name === "string" &&
    typeof value.objective === "string" &&
    typeof value.estimatedMinutes === "number"
  );
}

function isMissionContent(value: unknown): value is MissionContent {
  return (
    isRecord(value) &&
    typeof value.laboratoryId === "string" &&
    typeof value.laboratoryName === "string" &&
    typeof value.missionId === "string" &&
    typeof value.revisionId === "string" &&
    typeof value.version === "number" &&
    typeof value.locale === "string" &&
    typeof value.name === "string" &&
    typeof value.problem === "string" &&
    typeof value.status === "string" &&
    typeof value.bodyMdx === "string" &&
    Array.isArray(value.assignments) &&
    value.assignments.every(isAssignmentSummary)
  );
}

function isAssignmentContent(value: unknown): value is AssignmentContent {
  return (
    isRecord(value) &&
    typeof value.laboratoryId === "string" &&
    typeof value.laboratoryName === "string" &&
    typeof value.missionId === "string" &&
    typeof value.missionName === "string" &&
    typeof value.assignmentId === "string" &&
    typeof value.revisionId === "string" &&
    typeof value.version === "number" &&
    typeof value.order === "number" &&
    typeof value.locale === "string" &&
    typeof value.name === "string" &&
    typeof value.objective === "string" &&
    typeof value.estimatedMinutes === "number" &&
    typeof value.bodyMdx === "string"
  );
}

function getApiBaseUrl(): string {
  const apiBaseUrl = process.env.API_BASE_URL;

  if (!apiBaseUrl) {
    throw new Error("API_BASE_URL is not configured.");
  }

  return apiBaseUrl;
}

async function getContent<T>(
  path: string,
  locale: string,
  guard: (value: unknown) => value is T,
  label: string,
): Promise<T | null> {
  const url = new URL(path, getApiBaseUrl());
  url.searchParams.set("locale", locale);

  const response = await fetch(url, {
    cache: "no-store",
    headers: { Accept: "application/json" },
  });

  if (response.status === 404) {
    return null;
  }

  if (!response.ok) {
    throw new Error(`${label} request failed with status ${response.status}.`);
  }

  const payload: unknown = await response.json();

  if (!guard(payload)) {
    throw new Error(`${label} response has an unexpected shape.`);
  }

  return payload;
}

export async function getStationOverview(
  locale: string,
): Promise<StationOverview> {
  const overview = await getContent(
    "/api/station/overview",
    locale,
    isStationOverview,
    "Station overview",
  );

  if (!overview) {
    throw new Error("Station overview was not found.");
  }

  return overview;
}

export function getMissionContent(
  laboratorySlug: string,
  missionSlug: string,
  locale: string,
): Promise<MissionContent | null> {
  return getContent(
    `/api/laboratories/${laboratorySlug}/missions/${missionSlug}`,
    locale,
    isMissionContent,
    "Mission content",
  );
}

export function getAssignmentContent(
  laboratorySlug: string,
  missionSlug: string,
  assignmentSlug: string,
  locale: string,
): Promise<AssignmentContent | null> {
  return getContent(
    `/api/laboratories/${laboratorySlug}/missions/${missionSlug}/assignments/${assignmentSlug}`,
    locale,
    isAssignmentContent,
    "Assignment content",
  );
}
