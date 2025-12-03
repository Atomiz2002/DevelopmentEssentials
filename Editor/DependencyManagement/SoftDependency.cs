using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace DevelopmentEssentials.Editor.DependencyManagement {

    public class SoftDependency {

        public string                   Define       { get; private set; }
        public Dictionary<string, bool> Dependencies { get; }

        public SoftDependency(string define, params string[] dependencies) {
            Define = define;

            Dependencies = dependencies.Select(dependency =>
                    (dependency, located: !string.IsNullOrEmpty(dependency.EndsWith(".dll")
                        ? AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(dependency))
                            .FirstOrDefault(guid => Path.GetFileName(AssetDatabase.GUIDToAssetPath(guid)) == dependency)
                        : AssetDatabase.FindAssets($"t:AssemblyDefinitionAsset {dependency}")
                            .FirstOrDefault(guid => Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(guid)) == dependency))))
                .ToDictionary(guid => guid.dependency, guid => guid.located);
        }

        internal void PrefixDefine(string prefix) => Define = prefix + Define;

        public override string ToString() => $"{Define}: {string.Join(", ", Dependencies.Select(d => $"{d.Key}: {d.Value}"))}";

    }

}