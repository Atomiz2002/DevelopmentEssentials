using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace DevelopmentEssentials.Editor.DependencyManagement {

    public class SoftDependencyManager : AssetPostprocessor {

        private static readonly AsmdefDependencies runtimeDependencies = new AsmdefDependencies("DevelopmentEssentials.asmdef", "DEVELOPMENT_ESSENTIALS_RUNTIME_")
            .SetHardDependencies(
                new("DevelopmentEssentials.Editor"),
                new("DevelopmentEssentials.Runtime"))
            .SetSoftDependencies(
                new("COMPONENT_NAMES",
                    "ComponentNames",
                    "ComponentNames.Editor"),
                // precompiled
                new("ODIN_INSPECTOR",
                    "Sirenix.OdinInspector.Attributes.dll",
                    "Sirenix.Serialization.dll",
                    "Sirenix.Utilities.Editor.dll"));

        private static readonly AsmdefDependencies editorDependencies = new AsmdefDependencies("DevelopmentEssentials.Editor.asmdef", "DEVELOPMENT_ESSENTIALS_EDITOR_")
            .SetHardDependencies(
                new AsmdefDependencies.AsmdefDependency("DevelopmentEssentials.Editor"))
            .SetSoftDependencies(
                new AsmdefDependencies.AsmdefDependency("ODIN_INSPECTOR",
                    "Sirenix.OdinInspector.Attributes.dll",
                    "Sirenix.Serialization.dll",
                    "Sirenix.Utilities.Editor.dll"));

        // TODO works every other time
        // TODO safeguard sirenix dependent code with #if
        private static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFromAssetPaths) {
            runtimeDependencies.ReferenceDependencies();
            editorDependencies.ReferenceDependencies();
        }

        public class AsmdefDependencies {

            public readonly string asmdef;
            public readonly string definesPrefix; // PREFIXES THE DEPENDENCIES DEFINES

            public readonly List<AsmdefDependency> hardDependencies = new();
            public readonly List<AsmdefDependency> softDependencies = new();

            public AsmdefDependencies(string asmdef, string definesPrefix) {
                this.asmdef        = asmdef;
                this.definesPrefix = definesPrefix;
            }

            public AsmdefDependencies SetHardDependencies(params AsmdefDependency[] hardDependencies) {
                this.hardDependencies.Clear();
                this.hardDependencies.AddRange(hardDependencies);
                this.hardDependencies.ForEach(d => d.define = definesPrefix + d.define);
                return this;
            }

            public AsmdefDependencies SetSoftDependencies(params AsmdefDependency[] softDependencies) {
                this.softDependencies.Clear();
                this.softDependencies.AddRange(softDependencies);
                this.softDependencies.ForEach(d => d.define = definesPrefix + d.define);
                return this;
            }

            public void ReferenceDependencies() {
                string packageAsmdefPath = AssetDatabase.FindAssets($"t:AssemblyDefinitionAsset {Path.GetFileNameWithoutExtension(asmdef)}")
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .FirstOrDefault(p => Path.GetFileName(p) == asmdef); // kind of an unnecessary check?

                if (string.IsNullOrEmpty(packageAsmdefPath))
                    throw new($"Failed to find package asmdef: {asmdef}");

                AsmdefData asmdefData = JsonUtility.FromJson<AsmdefData>(File.ReadAllText(packageAsmdefPath, Encoding.UTF8));
                bool       modified   = false;

                foreach (AsmdefDependency hardDependency in hardDependencies)
                    ReferenceHardDependencies(asmdefData, ref modified, hardDependency);

                foreach (AsmdefDependency softDependency in softDependencies)
                    ReferenceSoftDependencies(asmdefData, ref modified, softDependency);

                if (modified) {
                    File.WriteAllText(packageAsmdefPath, JsonUtility.ToJson(asmdefData, true), Encoding.UTF8);
                    AssetDatabase.Refresh();
                }
            }

            private static void ReferenceHardDependencies(AsmdefData asmdefData, ref bool modified, AsmdefDependency asmdefDependency) {
                const string GUID_Prefix = "GUID:";

                bool foundAllDependencies = true;

                foreach ((string dependency, bool located) in asmdefDependency.dependencies) {
                    AsmdefData.VersionDefine missing = AsmdefData.VersionDefine.Required(asmdefDependency.define, dependency);

                    if (located) {
                        asmdefData.versionDefines.RemoveAll(vd => vd.define == missing.define);

                        List<string> references = dependency.EndsWith(".dll")
                            ? asmdefData.precompiledReferences
                            : asmdefData.references;

                        if (!references.Select(reference => reference.StartsWith(GUID_Prefix)
                                ? JsonUtility.FromJson<AsmdefData>(File.ReadAllText(AssetDatabase.GUIDToAssetPath(reference[GUID_Prefix.Length..]), Encoding.UTF8)).name
                                : reference).Contains(dependency))
                            references.Add(dependency);
                    }
                    else {
                        foundAllDependencies = false;

                        if (asmdefData.versionDefines.RemoveAll(vd => vd.define == asmdefDependency.define) > 0)
                            modified = true;

                        if (asmdefData.versionDefines.Any(vd => vd.define == missing.define))
                            continue;

                        asmdefData.versionDefines.Insert(0, missing);
                    }

                    modified = true;
                }

                if (foundAllDependencies && !asmdefData.versionDefines.Select(vd => vd.define).Contains(asmdefDependency.define))
                    asmdefData.versionDefines.Add(AsmdefData.VersionDefine.Located(asmdefDependency.define));
            }

            private static void ReferenceSoftDependencies(AsmdefData asmdefData, ref bool modified, AsmdefDependency asmdefDependency) {
                const string GUID_Prefix = "GUID:";

                bool foundAllDependencies = true;

                foreach ((string dependency, bool located) in asmdefDependency.dependencies) {
                    AsmdefData.VersionDefine missing = AsmdefData.VersionDefine.Missing(asmdefDependency.define, dependency);

                    if (located) {
                        asmdefData.versionDefines.RemoveAll(vd => vd.define == missing.define);

                        List<string> references = dependency.EndsWith(".dll")
                            ? asmdefData.precompiledReferences
                            : asmdefData.references;

                        if (!references.Select(reference => reference.StartsWith(GUID_Prefix)
                                ? JsonUtility.FromJson<AsmdefData>(File.ReadAllText(AssetDatabase.GUIDToAssetPath(reference[GUID_Prefix.Length..]), Encoding.UTF8)).name
                                : reference).Contains(dependency))
                            references.Add(dependency);
                    }
                    else {
                        foundAllDependencies = false;

                        if (asmdefData.versionDefines.RemoveAll(vd => vd.define == asmdefDependency.define) > 0)
                            modified = true;

                        if (asmdefData.versionDefines.Any(vd => vd.define == missing.define))
                            continue;

                        asmdefData.versionDefines.Insert(0, missing);
                    }

                    modified = true;
                }

                if (foundAllDependencies && !asmdefData.versionDefines.Select(vd => vd.define).Contains(asmdefDependency.define))
                    asmdefData.versionDefines.Add(AsmdefData.VersionDefine.Located(asmdefDependency.define));
            }

            public class AsmdefDependency {

                public          string                   define;
                public readonly Dictionary<string, bool> dependencies;

                public AsmdefDependency(string define, params string[] dependencies) {
                    this.define = define;

                    // TODO nonexistent still get "located" ...?
                    this.dependencies = dependencies.Select(dependency =>
                            (dependency, located: !string.IsNullOrEmpty(dependency.EndsWith(".dll")
                                ? AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(dependency))
                                    .FirstOrDefault(guid => Path.GetFileName(AssetDatabase.GUIDToAssetPath(guid)) == dependency)
                                : AssetDatabase.FindAssets($"t:AssemblyDefinitionAsset {dependency}")
                                    .FirstOrDefault(guid => Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(guid)) == dependency))))
                        .ToDictionary(guid => guid.dependency, guid => guid.located);
                }

                public override string ToString() => $"{define} {string.Join(", ", dependencies.Select(d => $"{d.Key}: {d.Value}"))}";

            }

        }

        [Serializable]
        private class AsmdefData {

            public string name;

            // General Options
            public bool   allowUnsafeCode;
            public bool   autoReferenced;
            public bool   noEngineReferences;
            public bool   overrideReferences;
            public string rootNamespace;

            /// ASMDEFs
            public List<string> references;

            /// DLLs
            public List<string> precompiledReferences;

            // Platforms
            public List<string> includePlatforms;
            public List<string> excludePlatforms;

            public List<string>        defineConstraints;
            public List<VersionDefine> versionDefines = new();

            [Serializable]
            public class VersionDefine {

                public string name;
                public string expression;
                public string define;

                private VersionDefine(string name, string expression, string define) {
                    this.name       = name;
                    this.expression = expression;
                    this.define     = define;
                }

                public static VersionDefine Required(string package, string define)  => new(package, string.Empty, define);
                public static VersionDefine Located(string define)                   => new("Unity", string.Empty, define);
                public static VersionDefine Missing(string define, string reference) => new("Unity", $"Missing {reference}", define);

                public override string ToString() => $"{define} | {name} | {expression}";

            }

        }

    }

}