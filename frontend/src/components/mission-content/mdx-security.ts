const allowedComponents = new Set([
  "Figure",
  "Hint",
  "JournalPrompt",
  "MixtureExplorer",
  "Prediction",
  "PythonWorkbench",
  "ResearcherNote",
  "SequenceInspector",
  "StationMessage",
  "SystemCriteria",
]);

interface MdxAttribute {
  type?: string;
  name?: string;
  value?: unknown;
}

interface MdxNode {
  type?: string;
  name?: string | null;
  attributes?: MdxAttribute[];
  children?: MdxNode[];
}

const forbiddenNodeTypes = new Set([
  "html",
  "mdxFlowExpression",
  "mdxTextExpression",
  "mdxjsEsm",
]);

export function restrictMdxToKnownComponents(): (tree: unknown) => void {
  return (tree: unknown): void => inspectNode(tree as MdxNode);
}

function inspectNode(node: MdxNode): void {
  if (node.type && forbiddenNodeTypes.has(node.type)) {
    throw new Error(`MDX node type '${node.type}' is not allowed.`);
  }

  if (node.type === "mdxJsxFlowElement" || node.type === "mdxJsxTextElement") {
    if (!node.name || !allowedComponents.has(node.name)) {
      throw new Error(
        `MDX component '${node.name ?? "fragment"}' is not allowed.`,
      );
    }

    for (const attribute of node.attributes ?? []) {
      if (
        attribute.type !== "mdxJsxAttribute" ||
        (attribute.value !== null &&
          attribute.value !== undefined &&
          typeof attribute.value !== "string")
      ) {
        throw new Error(
          `MDX component '${node.name}' contains an executable attribute.`,
        );
      }
    }
  }

  for (const child of node.children ?? []) {
    inspectNode(child);
  }
}
