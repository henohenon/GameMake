using System.Runtime.InteropServices;

/// <summary>
/// Discord Activity との連携（WebGL ビルドでのみ有効。エディタや他プラットフォームでは何もしない）。
/// 実体は Assets/Plugins/WebGL/DiscordBridge.jslib → discord/src/bridge.js。
/// </summary>
public static class DiscordBridge
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] private static extern void DiscordBridge_ShareSeed(uint seed);
    [DllImport("__Internal")] private static extern int DiscordBridge_TakeLaunchSeed();
#else
    private static void DiscordBridge_ShareSeed(uint seed) { }
    private static int DiscordBridge_TakeLaunchSeed() => -1;
#endif

    /// <summary>
    /// Commander シーンへ渡すシード。シード付きの Activity リンクから起動したときに
    /// タイトルで受け取り、Commander シーンが一度だけ使う。
    /// </summary>
    public static uint? PendingCommanderSeed { get; set; }

    /// <summary>兵士のシードを、指揮官画面を開く Activity リンクとして共有する画面を出す。</summary>
    public static void ShareSeed(uint seed) => DiscordBridge_ShareSeed(seed);

    /// <summary>シード付きのリンクから起動していればそのシードを返す（一度だけ）。</summary>
    public static uint? TakeLaunchSeed()
    {
        var seed = DiscordBridge_TakeLaunchSeed();
        return seed >= 0 ? (uint)seed : null;
    }
}
