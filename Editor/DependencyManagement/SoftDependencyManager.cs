using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DevelopmentEssentials.Extensions.CS;
using DevelopmentEssentials.Extensions.Unity.ExtendedLogger;
using UnityEditor;
using UnityEngine;

namespace DevelopmentEssentials.Editor.DependencyManagement {

    public class SoftDependencyManager : AssetPostprocessor {

        private static readonly Dictionary<string, Dictionary<string, List<SoftDependency>>> allAsmdefSoftDependencies = new();

        public static void RegisterAsmdefSoftDependencies(string prefix, Dictionary<string, List<SoftDependency>> asmdefDependencies) {
            if (allAsmdefSoftDependencies.ContainsKey(prefix)) {
                $"Assembly with prefix {prefix} is already registered. Please use a different prefix for safety.".LogEx();
                return;
            }

            foreach (List<SoftDependency> softDependencies in asmdefDependencies.Values)
            foreach (SoftDependency softDependency in softDependencies)
                softDependency.PrefixDefine(prefix);

            allAsmdefSoftDependencies[prefix] = asmdefDependencies;
        }

        [InitializeOnLoadMethod]
        private static void Initialize() {
            RegisterAsmdefSoftDependencies("DEVELOPMENT_ESSENTIALS_",
                new() {
                    {
                        "DevelopmentEssentials.asmdef",
                        new() {
                            new("COMPONENT_NAMES", "ComponentNames", "ComponentNames.Editor"),
                            // precompiled
                            new("ODIN_INSPECTOR", "Sirenix.OdinInspector.Attributes.dll", "Sirenix.Serialization.dll", "Sirenix.Utilities.Editor.dll")
                        }
                    }
                });
        }

        // TODO works every other time
        // TODO safeguard sirenix dependent code with #if
        // TODO if missing is set, it doesnt get unset automatically when the dependency is later added
        private static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFromAssetPaths) {
            bool modified = false;
            AssetDatabase.StartAssetEditing();

            try {
                foreach ((string prefix, Dictionary<string, List<SoftDependency>> asmdefSoftDependencies) in allAsmdefSoftDependencies)
                foreach ((string asmdef, List<SoftDependency> softDependencies) in asmdefSoftDependencies)
                    ReferenceSoftDependenciesForAssembly(asmdef, ref modified, prefix, softDependencies);
            }
            finally {
                AssetDatabase.StopAssetEditing();
            }

            if (modified)
                AssetDatabase.Refresh();
        }

        private static void ReferenceSoftDependenciesForAssembly(string packageAsmdef, ref bool modified, string definesPrefix, List<SoftDependency> softDependencies) {
            string packageAsmdefPath = AssetDatabase.FindAssets($"t:AssemblyDefinitionAsset {Path.GetFileNameWithoutExtension(packageAsmdef)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .FirstOrDefault(p => Path.GetFileName(p) == packageAsmdef); // kind of an unnecessary check?

            if (string.IsNullOrEmpty(packageAsmdefPath))
                throw new("Failed to find packageAsmdef");

            AsmdefData asmdefData   = JsonUtility.FromJson<AsmdefData>(File.ReadAllText(packageAsmdefPath, Encoding.UTF8));
            bool       modifiedThis = false;

            foreach (SoftDependency softDependency in softDependencies)
                ReferenceSoftDependencies(asmdefData, ref modifiedThis, softDependency);

            if (modifiedThis) {
                File.WriteAllText(packageAsmdefPath, JsonUtility.ToJson(asmdefData, true), Encoding.UTF8);
                modified = true;
            }
        }

        private static void ReferenceSoftDependencies(AsmdefData asmdefData, ref bool modified, SoftDependency softDependency) {
            const string GUID_Prefix = "GUID:";

            bool foundAllDependencies = true;

            foreach ((string dependency, bool located) in softDependency.Dependencies) {
                AsmdefData.VersionDefine missing = AsmdefData.VersionDefine.Missing(softDependency.Define, dependency);

                if (located) {
                    asmdefData.versionDefines.RemoveAll(vd => vd.define == missing.define);

                    List<string> references = dependency.EndsWith(".dll")
                        ? asmdefData.precompiledReferences
                        : asmdefData.references;

                    if (references
                        .Select(reference => reference.StartsWith(GUID_Prefix)
                            ? JsonUtility.FromJson<AsmdefData>(File.ReadAllText(AssetDatabase.GUIDToAssetPath(reference[GUID_Prefix.Length..]), Encoding.UTF8)).name
                            : reference)
                        .Contains(dependency))
                        continue;

                    references.Add(dependency);
                }
                else {
                    foundAllDependencies = false;

                    if (asmdefData.versionDefines.RemoveAll(vd => vd.define == softDependency.Define) > 0)
                        modified = true;

                    if (asmdefData.versionDefines.Any(vd => vd.define == missing.define))
                        continue;

                    asmdefData.versionDefines.Insert(0, missing);
                }

                modified = true;
            }

            if (foundAllDependencies && !asmdefData.versionDefines.Select(vd => vd.define).Contains(softDependency.Define))
                asmdefData.versionDefines.Add(AsmdefData.VersionDefine.Located(softDependency.Define));
        }

    }

}