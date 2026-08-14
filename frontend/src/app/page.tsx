import { StationOverviewView } from "@/components/station-overview-view";
import { getStationOverview } from "@/lib/station-api";

export const dynamic = "force-dynamic";

export default async function Home(): Promise<React.ReactElement> {
  const overview = await getStationOverview("ru");

  return <StationOverviewView overview={overview} />;
}
