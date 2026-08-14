import { loadPyodide } from "/pyodide/pyodide.mjs";

let runtimePromise;

function getRuntime() {
  runtimePromise ??= loadPyodide({
    indexURL: new URL("/pyodide/", self.location.origin).href,
  });
  return runtimePromise;
}

self.onmessage = async (event) => {
  const { id, code } = event.data;
  const output = [];

  try {
    const runtime = await getRuntime();
    runtime.setStdout({ batched: (line) => output.push(line) });
    runtime.setStderr({ batched: (line) => output.push(line) });
    await runtime.runPythonAsync(code);
    self.postMessage({ id, ok: true, output: output.join("\n") });
  } catch (error) {
    const message = error instanceof Error ? error.message : String(error);
    self.postMessage({
      id,
      ok: false,
      output: [...output, message].filter(Boolean).join("\n"),
    });
  }
};
