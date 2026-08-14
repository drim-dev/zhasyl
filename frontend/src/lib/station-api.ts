import type { LaboratorySummary, StationOverview } from "@/types/station";

function isLaboratory(value: unknown): value is LaboratorySummary {
  if (typeof value !== "object" || value === null) {
    return false;
  }

  const laboratory = value as Record<string, unknown>;
  const mission = laboratory.firstMission;

  return (
    typeof laboratory.id === "string" &&
    typeof laboratory.name === "string" &&
    typeof laboratory.purpose === "string" &&
    typeof laboratory.specialist === "string" &&
    typeof mission === "object" &&
    mission !== null &&
    typeof (mission as Record<string, unknown>).id === "string" &&
    typeof (mission as Record<string, unknown>).name === "string" &&
    typeof (mission as Record<string, unknown>).problem === "string" &&
    typeof (mission as Record<string, unknown>).status === "string"
  );
}

function isStationOverview(value: unknown): value is StationOverview {
  if (typeof value !== "object" || value === null) {
    return false;
  }

  const overview = value as Record<string, unknown>;

  return (
    typeof overview.stationId === "string" &&
    typeof overview.stationName === "string" &&
    typeof overview.locale === "string" &&
    typeof overview.location === "string" &&
    typeof overview.briefing === "string" &&
    Array.isArray(overview.laboratories) &&
    overview.laboratories.every(isLaboratory)
  );
}

export async function getStationOverview(
  locale: string,
): Promise<StationOverview> {
  const apiBaseUrl = process.env.API_BASE_URL;

  if (!apiBaseUrl) {
    throw new Error("API_BASE_URL is not configured.");
  }

  const url = new URL("/api/station/overview", apiBaseUrl);
  url.searchParams.set("locale", locale);

  const response = await fetch(url, {
    cache: "no-store",
    headers: { Accept: "application/json" },
  });

  if (!response.ok) {
    throw new Error(
      `Station overview request failed with status ${response.status}.`,
    );
  }

  const payload: unknown = await response.json();

  if (!isStationOverview(payload)) {
    throw new Error("Station overview response has an unexpected shape.");
  }

  return payload;
}
