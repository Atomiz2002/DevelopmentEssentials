using System.IO;
using System.Linq;
using UnityEditor;

namespace DevelopmentEssentials.Editor.DependencyManagement {

    public class SoftDependencyManager : AssetPostprocessor {

        [MenuItem("Tools/Atomiz/Reset Soft Dependencies")]
        private static void Reset() {
            string[] asmdefs = {
                "DevelopmentEssentials",
                "DevelopmentEssentials.Editor",
                "DevelopmentTools",
                "DevelopmentTools.Editor"
            };

            foreach (string path in AssetDatabase.FindAssets("t:AssemblyDefinitionAsset")
                         .Select(AssetDatabase.GUIDToAssetPath)
                         .Where(p => asmdefs.Contains(Path.GetFileNameWithoutExtension(p)))) {
                AsmdefData asmdef = new(path);
                asmdef.ClearReferences();
                asmdef.WriteToFile(path);
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Atomiz/Distinct Soft Dependencies Version Defines")]
        private static void Distinct() {
            string[] asmdefs = {
                "DevelopmentEssentials",
                "DevelopmentEssentials.Editor",
                "DevelopmentTools",
                "DevelopmentTools.Editor"
            };

            foreach (string path in AssetDatabase.FindAssets("t:AssemblyDefinitionAsset")
                         .Select(AssetDatabase.GUIDToAssetPath)
                         .Where(p => asmdefs.Contains(Path.GetFileNameWithoutExtension(p)))) {
                AsmdefData asmdef = new(path);
                asmdef.WriteToFile(path);
            }

            AssetDatabase.Refresh();
        }

    }

}