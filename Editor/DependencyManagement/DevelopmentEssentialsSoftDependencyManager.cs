using UnityEditor;

namespace DevelopmentEssentials.Editor.DependencyManagement {

    public class DevelopmentEssentialsSoftDependencyManager : SoftDependencyManager {

        private static readonly AsmdefDependencies runtimeDependencies = new AsmdefDependencies("DevelopmentEssentials.asmdef", "DEVELOPMENT_ESSENTIALS_RUNTIME_")
            .SetSoftDependencies(
                new("COMPONENT_NAMES",
                    "ComponentNames"));

        private static readonly AsmdefDependencies editorDependencies = new AsmdefDependencies("DevelopmentEssentials.Editor.asmdef", "DEVELOPMENT_ESSENTIALS_EDITOR_")
            .SetHardDependencies(
                new("DevelopmentEssentials"))
            .SetSoftDependencies(
                new("COMPONENT_NAMES",
                    "ComponentNames.Editor"),
                new AsmdefDependencies.SoftAsmdefDependency("ODIN_INSPECTOR",
                    "Sirenix.OdinInspector.Attributes.dll",
                    "Sirenix.Serialization.dll",
                    "Sirenix.Utilities.Editor.dll"));

        // TODO works every other time
        // TODO safeguard sirenix dependent code with #if
        private static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFromAssetPaths) {
            runtimeDependencies.ReferenceDependencies();
            editorDependencies.ReferenceDependencies();

            AssetDatabase.Refresh();
        }

    }

}