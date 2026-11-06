// Cloudflare Worker that serves the Unity WebGL build for the Discord Activity.
//
// Static files live in ./dist (Workers static assets). This Worker only handles /Build/*:
//  - *.unityweb files are gzip; serve them with Content-Encoding: gzip so the browser
//    decompresses natively (Unity's JS fallback still covers proxies that strip it).
//  - Workers static assets cap each file at 25 MiB, so scripts/prepare.mjs splits big
//    files into *.partNNN and lists them in Build/parts.json; they are stitched back here.
//  - File names are content-hashed, so Build/* is cached as immutable.

const CONTENT_TYPES = {
  ".wasm": "application/wasm",
  ".js": "application/javascript; charset=utf-8",
  ".data": "application/octet-stream",
};

function contentTypeFor(name) {
  const base = name.replace(/\.unityweb$/, "");
  for (const [ext, type] of Object.entries(CONTENT_TYPES)) {
    if (base.endsWith(ext)) return type;
  }
  return "application/octet-stream";
}

async function loadParts(env, origin) {
  const res = await env.ASSETS.fetch(new URL("/Build/parts.json", origin));
  return res.ok ? res.json() : {};
}

export default {
  async fetch(request, env, ctx) {
    const url = new URL(request.url);
    if (request.method !== "GET" && request.method !== "HEAD") {
      return new Response("Method Not Allowed", { status: 405 });
    }
    if (!url.pathname.startsWith("/Build/") || url.pathname.endsWith("/parts.json")) {
      return env.ASSETS.fetch(request);
    }

    const name = url.pathname.slice("/Build/".length);
    const parts = await loadParts(env, url.origin);
    const headers = new Headers({
      "Content-Type": contentTypeFor(name),
      "Cache-Control": "public, max-age=31536000, immutable",
    });
    // Only advertise gzip to clients that accept it. Otherwise send the raw gzip bytes
    // unlabelled and let Unity's decompression fallback inflate them in JS (letting the
    // edge decode a stitched stream for such clients truncated the body).
    const acceptsGzip = /\bgzip\b/i.test(request.headers.get("Accept-Encoding") || "");
    if (name.endsWith(".unityweb") && acceptsGzip) headers.set("Content-Encoding", "gzip");
    headers.set("Vary", "Accept-Encoding");

    let body;
    const entry = parts[name];
    if (entry) {
      headers.set("Content-Length", String(entry.size));
      if (request.method === "HEAD") return new Response(null, { headers, encodeBody: "manual" });

      // Stream the parts back-to-back without buffering the whole file.
      const { readable, writable } = new TransformStream();
      const pump = (async () => {
        for (let i = 0; i < entry.count; i++) {
          const partUrl = new URL(`/Build/${name}.part${String(i).padStart(3, "0")}`, url.origin);
          const res = await env.ASSETS.fetch(partUrl);
          if (!res.ok || !res.body) throw new Error(`missing part ${i} of ${name}`);
          await res.body.pipeTo(writable, { preventClose: true });
        }
        await writable.close();
      })().catch((err) => writable.abort(err));
      ctx.waitUntil(pump);
      body = readable;
    } else {
      const res = await env.ASSETS.fetch(request);
      if (!res.ok) return res;
      const len = res.headers.get("Content-Length");
      if (len) headers.set("Content-Length", len);
      body = request.method === "HEAD" ? null : res.body;
    }

    // encodeBody: "manual" = the body is already gzip; don't let the runtime re-encode it.
    return new Response(body, { headers, encodeBody: "manual" });
  },
};
