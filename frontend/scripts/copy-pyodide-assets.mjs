import { copyFile, mkdir } from "node:fs/promises";
import { fileURLToPath } from "node:url";
import path from "node:path";

const frontendRoot = path.resolve(
  fileURLToPath(new URL("..", import.meta.url)),
);
const sourceRoot = path.join(frontendRoot, "node_modules", "pyodide");
const destinationRoot = path.join(frontendRoot, "public", "pyodide");
const runtimeFiles = [
  "pyodide-lock.json",
  "pyodide.asm.mjs",
  "pyodide.asm.wasm",
  "pyodide.mjs",
  "python_stdlib.zip",
];

await mkdir(destinationRoot, { recursive: true });
await Promise.all(
  runtimeFiles.map((fileName) =>
    copyFile(
      path.join(sourceRoot, fileName),
      path.join(destinationRoot, fileName),
    ),
  ),
);

console.log(`Prepared Pyodide ${runtimeFiles.length} runtime assets.`);
