export interface MissionSummary {
  id: string;
  name: string;
  problem: string;
  status: string;
}

export interface LaboratorySummary {
  id: string;
  name: string;
  purpose: string;
  specialist: string;
  firstMission: MissionSummary;
}

export interface StationOverview {
  stationId: string;
  stationName: string;
  locale: string;
  location: string;
  briefing: string;
  laboratories: LaboratorySummary[];
}
