import { notFound } from "next/navigation";
import { MissionMdx } from "@/components/mission-content/mission-mdx";
import { MissionView } from "@/components/mission-view";
import { getMissionContent } from "@/lib/station-api";

export const dynamic = "force-dynamic";

interface MissionPageProps {
  params: Promise<{
    laboratorySlug: string;
    missionSlug: string;
  }>;
}

export default async function MissionPage({
  params,
}: MissionPageProps): Promise<React.ReactElement> {
  const { laboratorySlug, missionSlug } = await params;
  const mission = await getMissionContent(laboratorySlug, missionSlug, "ru");

  if (!mission) {
    notFound();
  }

  return (
    <MissionView
      mission={mission}
      body={<MissionMdx source={mission.bodyMdx} />}
    />
  );
}
