using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Builds the WebGL player for the Discord Activity into discord/dist.
/// Menu: Build > Discord Activity (WebGL)
/// CLI : Unity -batchmode -quit -projectPath . -buildTarget WebGL -executeMethod DiscordBuild.Build
/// Player settings touched here are restored afterwards so other builds are unaffected.
/// </summary>
public static class DiscordBuild
{
    [MenuItem("Build/Discord Activity (WebGL)")]
    public static void Build()
    {
        var outDir = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "discord", "dist"));
        if (Directory.Exists(outDir)) Directory.Delete(outDir, true);

        var prevTemplate = PlayerSettings.WebGL.template;
        var prevCompression = PlayerSettings.WebGL.compressionFormat;
        var prevFallback = PlayerSettings.WebGL.decompressionFallback;
        var prevHashes = PlayerSettings.WebGL.nameFilesAsHashes;

        BuildReport report;
        try
        {
            PlayerSettings.WebGL.template = "PROJECT:Discord";
            // Gzip files are served with Content-Encoding by the Worker; the fallback lets
            // the loader decompress in JS if a proxy ever strips the header.
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
            PlayerSettings.WebGL.decompressionFallback = true;
            // Content-hashed file names so the Worker can cache Build/* forever.
            PlayerSettings.WebGL.nameFilesAsHashes = true;

            var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
            report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outDir,
                target = BuildTarget.WebGL,
                options = BuildOptions.None,
            });
        }
        finally
        {
            PlayerSettings.WebGL.template = prevTemplate;
            PlayerSettings.WebGL.compressionFormat = prevCompression;
            PlayerSettings.WebGL.decompressionFallback = prevFallback;
            PlayerSettings.WebGL.nameFilesAsHashes = prevHashes;
            AssetDatabase.SaveAssets();
        }

        var summary = report.summary;
        Debug.Log($"[DiscordBuild] result={summary.result} size={summary.totalSize / 1048576f:F1}MB time={summary.totalTime} errors={summary.totalErrors} out={outDir}");

        // Largest assets in the build, to see what to shrink if the download gets too big.
        var largest = report.packedAssets
            .SelectMany(p => p.contents)
            .GroupBy(c => c.sourceAssetPath)
            .Select(g => (path: g.Key, size: g.Sum(c => (long)c.packedSize)))
            .OrderByDescending(x => x.size)
            .Take(20);
        foreach (var (path, size) in largest)
            Debug.Log($"[DiscordBuild] {size / 1048576f,7:F2}MB  {path}");

        if (Application.isBatchMode)
            EditorApplication.Exit(summary.result == BuildResult.Succeeded ? 0 : 1);
    }
}
