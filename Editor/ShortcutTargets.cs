using System.Diagnostics;
using DevelopmentEssentials.Extensions.CS;
using DevelopmentEssentials.Extensions.Unity;
using DevelopmentEssentials.Extensions.Unity.ExtendedLogger;
using UnityEditor;

namespace DevelopmentEssentials.Editor {

    [InitializeOnLoad]
    public class ShortcutTargets {

        static ShortcutTargets() => Selection.selectionChanged += OnSelectionChanged;

        private static void OnSelectionChanged() {
            if (!Selection.activeObject)
                return;

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (!path!.EndsWith(".lnk"))
                return;

            string shortcutTarget = GetShortcutTarget(path);
            if (shortcutTarget != null)
                Selection.activeObject.name.Link(shortcutTarget).LOG();
        }

        private static string GetShortcutTarget(string lnkPath) {
            Process proc = new() {
                StartInfo = new() {
                    FileName               = "powershell",
                    Arguments              = $"-NoProfile -Command \"(New-Object -ComObject WScript.Shell).CreateShortcut('{lnkPath}').TargetPath\"",
                    UseShellExecute        = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow         = true
                }
            };

            proc.Start();
            string target = proc.StandardOutput.ReadToEnd().Trim();
            proc.WaitForExit();
            return target.RelativePath();
        }

    }

}