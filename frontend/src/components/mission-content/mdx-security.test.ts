import { restrictMdxToKnownComponents } from "./mdx-security";

describe("restrictMdxToKnownComponents", () => {
  it("accepts registered visual and learning components", () => {
    const validate = restrictMdxToKnownComponents();

    expect(() =>
      validate({
        type: "root",
        children: [
          {
            type: "mdxJsxFlowElement",
            name: "Figure",
            attributes: [
              { type: "mdxJsxAttribute", name: "caption", value: "Схема" },
            ],
            children: [
              {
                type: "mdxJsxFlowElement",
                name: "SequenceInspector",
                attributes: [],
              },
            ],
          },
        ],
      }),
    ).not.toThrow();
  });

  it("rejects executable expressions", () => {
    const validate = restrictMdxToKnownComponents();

    expect(() =>
      validate({ type: "root", children: [{ type: "mdxFlowExpression" }] }),
    ).toThrow("not allowed");
  });

  it("rejects unknown components", () => {
    const validate = restrictMdxToKnownComponents();

    expect(() =>
      validate({
        type: "root",
        children: [{ type: "mdxJsxFlowElement", name: "UnsafeWidget" }],
      }),
    ).toThrow("UnsafeWidget");
  });
});
