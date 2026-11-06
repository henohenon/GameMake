// Discord Activity bridge for the Unity WebGL build.
// Bundled to Assets/WebGLTemplates/Discord/TemplateData/discord-bridge.js by `npm run build`.
//
// - Inside Discord (the frame is opened with ?frame_id=...), performs the
//   Embedded App SDK handshake and routes window.open (used by the tweet button)
//   through Discord's "open external link" prompt, since popups are blocked in the frame.
// - Outside Discord (plain browser), does nothing, so the same build still runs anywhere.
import { DiscordSDK } from "@discord/embedded-app-sdk";

const params = new URLSearchParams(window.location.search);
const inDiscord = params.has("frame_id");

// The Activity is served from https://<client_id>.discordsays.com, so the
// client ID can be read from the hostname; no per-app build config needed.
function resolveClientId() {
  const fromQuery = params.get("client_id");
  if (fromQuery) return fromQuery;
  const m = window.location.hostname.match(/^(\d+)\.discordsays\.com$/);
  return m ? m[1] : null;
}

const bridge = {
  inDiscord,
  sdk: null,
  ready: Promise.resolve(null),
};
window.discordBridge = bridge;

if (inDiscord) {
  const clientId = resolveClientId();
  if (!clientId) {
    console.error("[discord-bridge] could not determine client ID");
  } else {
    const sdk = new DiscordSDK(clientId);
    bridge.sdk = sdk;
    bridge.ready = sdk
      .ready()
      .then(() => {
        console.log("[discord-bridge] ready");
        return sdk;
      })
      .catch((err) => {
        console.error("[discord-bridge] ready failed", err);
        return null;
      });

    window.open = function (url) {
      if (url) {
        bridge.ready
          .then((s) => s && s.commands.openExternalLink({ url: String(url) }))
          .catch((err) => console.error("[discord-bridge] openExternalLink failed", err));
      }
      return null;
    };
  }
}
