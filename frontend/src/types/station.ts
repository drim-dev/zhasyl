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

export interface AssignmentSummary {
  assignmentId: string;
  revisionId: string;
  version: number;
  order: number;
  name: string;
  objective: string;
  estimatedMinutes: number;
}

export interface MissionContent {
  laboratoryId: string;
  laboratoryName: string;
  missionId: string;
  revisionId: string;
  version: number;
  locale: string;
  name: string;
  problem: string;
  status: string;
  bodyMdx: string;
  assignments: AssignmentSummary[];
}

export interface AssignmentContent {
  laboratoryId: string;
  laboratoryName: string;
  missionId: string;
  missionName: string;
  assignmentId: string;
  revisionId: string;
  version: number;
  order: number;
  locale: string;
  name: string;
  objective: string;
  estimatedMinutes: number;
  bodyMdx: string;
}
