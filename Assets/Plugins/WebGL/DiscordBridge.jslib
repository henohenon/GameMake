// Unity <-> discord-bridge.js (window.discordBridge, see discord/src/bridge.js).
// Every function is a no-op when the page wasn't built from the Discord WebGL template.
mergeInto(LibraryManager.library, {
  DiscordBridge_ShareSeed: function (seed) {
    if (window.discordBridge) window.discordBridge.shareSeed(seed >>> 0);
  },

  DiscordBridge_TakeLaunchSeed: function () {
    return window.discordBridge ? window.discordBridge.takeLaunchSeed() : -1;
  },
});
