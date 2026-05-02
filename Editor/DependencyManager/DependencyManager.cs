using DependencyManagement;
using UnityEditor;

namespace DevelopmentEssentials.Editor {

    [InitializeOnLoad]
    public class DependencyManager : DependencyManagement.DependencyManager {

        static DependencyManager() {
            AsmdefsDependencies.Add(new AsmdefDependencies("DevelopmentEssentials.asmdef", "DEVELOPMENT_ESSENTIALS_RUNTIME_")
                .SetSoftDependencies(
                    new("UNI_TASK",
                        "UniTask"),
                    new("NEWTONSOFT_JSON",
                        "Newtonsoft.Json.dll"),
                    new("ODIN_INSPECTOR",
                        "Sirenix.OdinInspector.Attributes.dll",
                        "Sirenix.Utilities.dll"),
                    new("COMPONENT_NAMES",
                        "ComponentNames")));

            AsmdefsDependencies.Add(new AsmdefDependencies("DevelopmentEssentials.Editor.asmdef", "DEVELOPMENT_ESSENTIALS_EDITOR_")
                .SetHardDependencies(
                    new("DevelopmentEssentials"))
                .SetSoftDependencies(
                    new("COMPONENT_NAMES",
                        "ComponentNames.Editor"),
                    new AsmdefDependencies.SoftAsmdefDependency("ODIN_INSPECTOR",
                        "Sirenix.OdinInspector.Editor.dll",
                        "Sirenix.OdinInspector.Attributes.dll",
                        "Sirenix.Serialization.dll",
                        "Sirenix.Utilities.Editor.dll")));
        }

    }

}