import { evaluate } from "@mdx-js/mdx";
import type { MDXComponents } from "mdx/types";
import * as runtime from "react/jsx-runtime";
import remarkGfm from "remark-gfm";
import { Figure } from "./figure";
import {
  Hint,
  JournalPrompt,
  Prediction,
  ResearcherNote,
  SafeLink,
  StationMessage,
  SystemCriteria,
} from "./mdx-primitives";
import { MixtureExplorer } from "./mixture-explorer";
import { restrictMdxToKnownComponents } from "./mdx-security";
import { PythonWorkbench } from "./python-workbench";
import { SequenceInspector } from "./sequence-inspector";
import styles from "./mission-content.module.css";

const baseComponents: MDXComponents = {
  a: SafeLink,
  Figure,
  Hint,
  JournalPrompt,
  MixtureExplorer,
  Prediction,
  PythonWorkbench,
  ResearcherNote,
  SequenceInspector,
  StationMessage,
  SystemCriteria,
};

interface MissionMdxProps {
  source: string;
  assignmentRevisionId?: string;
}

export async function MissionMdx({
  source,
  assignmentRevisionId,
}: MissionMdxProps): Promise<React.ReactElement> {
  const evaluated = await evaluate(source, {
    ...runtime,
    remarkPlugins: [remarkGfm, restrictMdxToKnownComponents],
  });
  const Content = evaluated.default;
  const components: MDXComponents = assignmentRevisionId
    ? {
        ...baseComponents,
        PythonWorkbench: (props) => (
          <PythonWorkbench
            assignment={props.assignment}
            assignmentRevisionId={assignmentRevisionId}
          />
        ),
      }
    : baseComponents;

  return (
    <div className={styles.prose}>
      <Content components={components} />
    </div>
  );
}
