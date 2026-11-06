// Post-process the Unity build in ./dist for Workers static assets:
// split any file over the per-file limit into *.partNNN and write Build/parts.json.
import { readdirSync, readFileSync, statSync, openSync, readSync, writeFileSync, closeSync, rmSync, existsSync } from "node:fs";
import { join } from "node:path";

const PART_SIZE = 20 * 1024 * 1024; // stay well under the 25 MiB asset limit
const buildDir = join(import.meta.dirname, "..", "dist", "Build");

if (!existsSync(buildDir)) {
  console.error(`No build found at ${buildDir}. Run the Unity build first (Build > Discord Activity (WebGL)).`);
  process.exit(1);
}

// Keep entries from a previous run (their originals are already deleted), so re-running is safe.
const partsPath = join(buildDir, "parts.json");
const parts = existsSync(partsPath) ? JSON.parse(readFileSync(partsPath, "utf8")) : {};
for (const name of readdirSync(buildDir)) {
  if (/\.part\d{3}$/.test(name) || name === "parts.json") continue;
  const path = join(buildDir, name);
  const size = statSync(path).size;
  if (size <= PART_SIZE) continue;

  const fd = openSync(path, "r");
  const buf = Buffer.alloc(PART_SIZE);
  let count = 0;
  for (let offset = 0; offset < size; offset += PART_SIZE, count++) {
    const n = readSync(fd, buf, 0, PART_SIZE, offset);
    writeFileSync(`${path}.part${String(count).padStart(3, "0")}`, buf.subarray(0, n));
  }
  closeSync(fd);
  rmSync(path);
  parts[name] = { size, count };
  console.log(`split ${name} (${(size / 1048576).toFixed(1)} MB) into ${count} parts`);
}

writeFileSync(partsPath, JSON.stringify(parts, null, 2));

let total = 0;
for (const name of readdirSync(buildDir)) total += statSync(join(buildDir, name)).size;
console.log(`Build/ total ${(total / 1048576).toFixed(1)} MB`);
