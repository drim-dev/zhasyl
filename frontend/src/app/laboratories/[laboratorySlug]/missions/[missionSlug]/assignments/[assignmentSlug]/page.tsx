import { notFound } from "next/navigation";
import { AssignmentView } from "@/components/assignment-view";
import { MissionMdx } from "@/components/mission-content/mission-mdx";
import { getAssignmentContent } from "@/lib/station-api";

export const dynamic = "force-dynamic";

interface AssignmentPageProps {
  params: Promise<{
    laboratorySlug: string;
    missionSlug: string;
    assignmentSlug: string;
  }>;
}

export default async function AssignmentPage({
  params,
}: AssignmentPageProps): Promise<React.ReactElement> {
  const { laboratorySlug, missionSlug, assignmentSlug } = await params;
  const assignment = await getAssignmentContent(
    laboratorySlug,
    missionSlug,
    assignmentSlug,
    "ru",
  );

  if (!assignment) {
    notFound();
  }

  return (
    <AssignmentView
      assignment={assignment}
      body={<MissionMdx source={assignment.bodyMdx} />}
    />
  );
}
