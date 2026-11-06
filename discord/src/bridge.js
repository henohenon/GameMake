// Discord Activity bridge for the Unity WebGL build.
// Bundled to Assets/WebGLTemplates/Discord/TemplateData/discord-bridge.js by `npm run build`.
//
// - Inside Discord (the frame is opened with ?frame_id=...), performs the
//   Embedded App SDK handshake and routes window.open (used by the tweet button)
//   through Discord's "open external link" prompt, since popups are blocked in the frame.
// - Shares the Soldier's seed as an Activity link (shareSeed) and hands a seed from
//   such a link to the game on launch (takeLaunchSeed). Unity calls these through
//   Assets/Plugins/WebGL/DiscordBridge.jslib.
// - Outside Discord (plain browser), does nothing, so the same build still runs anywhere.
//   (?custom_id=c<seed> still works there, which makes the Commander path easy to test.)
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

// Activity links shared by the Soldier carry custom_id=c<seed>. Discord passes it
// back as ?custom_id=... when someone launches the Activity from that link.
const SEED_PREFIX = "c";
function parseSeed(customId) {
  const m = customId && customId.match(/^c(\d{1,9})$/);
  return m ? Number(m[1]) : -1;
}
let launchSeed = parseSeed(params.get("custom_id"));

const bridge = {
  inDiscord,
  sdk: null,
  ready: Promise.resolve(null),

  // Seed the Activity was launched with, or -1. Returns it only once, so going
  // back to the title later doesn't jump into the Commander screen again.
  takeLaunchSeed() {
    const seed = launchSeed;
    launchSeed = -1;
    return seed;
  },

  // Opens Discord's share dialog with a link that launches the Commander with this seed.
  shareSeed(seed) {
    if (!inDiscord) return;
    bridge.ready
      .then((s) =>
        s &&
        s.commands.shareLink({
          message: `incoming transmission — soldierId: ${seed}`,
          custom_id: `${SEED_PREFIX}${seed}`,
        }),
      )
      .then((res) => res && console.log("[discord-bridge] shareLink", res))
      .catch((err) => console.error("[discord-bridge] shareLink failed", err));
  },
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
