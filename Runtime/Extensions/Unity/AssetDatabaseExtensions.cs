using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DevelopmentEssentials.Extensions.CS;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DevelopmentEssentials.Extensions.Unity {

    public static class AssetDatabaseExtensions {

#if UNITY_EDITOR

        #region Assets

        public static StackFrame OpenAsset([CanBeNull] this StackFrame frame, string path = null, int line = 0, int column = 0) {
            if (frame != null) {
                path   = frame.GetFileName();
                line   = frame.GetFileLineNumber();
                column = frame.GetFileColumnNumber();
            }

            if (path!.Contains("Assets")) {
                AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(path.StartAt("Assets")), line, column);
            }
            else {
                string quantumFilePath    = Path.Join(new DirectoryInfo(Application.dataPath).Parent!.Parent!.SafeString(), path.StartAt("quantum_code"));
                string quantumProjectPath = quantumFilePath.EndAt("quantum.code", false, "quantum_code.sln");
                Process.Start(EditorPrefs.GetString("kScriptsDefaultApp"), $"--project \"{quantumProjectPath}\" --line {line} --column {column - 1} -- \"{quantumFilePath}\"");
            }

            return frame;
        }

        [Pure]
        public static GUID PathToGUID(this string path) => AssetDatabase.GUIDFromAssetPath(path);

        [Pure]
        [CanBeNull]
        public static string GUIDToPath(this string guid) => AssetDatabase.GUIDToAssetPath(guid);

        [Pure]
        [CanBeNull]
        public static string GUIDToPath(this GUID guid) => AssetDatabase.GUIDToAssetPath(guid);

        [Pure]
        [CanBeNull]
        public static T LoadAssetByGUID<T>(this string guid) where T : Object => AssetDatabase.LoadAssetAtPath<T>(guid.GUIDToPath());

        [Pure]
        [CanBeNull]
        public static T LoadAssetByGUID<T>(this GUID guid) where T : Object => AssetDatabase.LoadAssetAtPath<T>(guid.GUIDToPath());

        [Pure]
        [CanBeNull]
        public static Object LoadAssetByGUID(this string guid) => AssetDatabase.LoadMainAssetAtPath(guid.GUIDToPath());

        [Pure]
        [CanBeNull]
        public static Object LoadAssetByGUID(this GUID guid) => AssetDatabase.LoadMainAssetAtPath(guid.GUIDToPath());

        [Pure]
        [CanBeNull]
        public static Object LoadAsset(this string path) => AssetDatabase.LoadMainAssetAtPath(path);

        [Pure]
        public static void LoadAsset<T>(this string assetPath, [CanBeNull] out T asset) where T : Object => asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

        [Pure]
        [CanBeNull]
        public static T LoadAsset<T>(this Object mainAsset) where T : Object => AssetDatabase.LoadAssetAtPath<T>(mainAsset.GetAssetPath());

        [Pure]
        [CanBeNull]
        public static IEnumerable<T> LoadSubAssets<T>(this Object mainAsset) where T : Object => AssetDatabase.LoadAllAssetRepresentationsAtPath(mainAsset.GetAssetPath()).Cast<T>();

        [Pure]
        [CanBeNull]
        public static IEnumerable<Object> LoadAllAssets(this Object mainAsset) => AssetDatabase.LoadAllAssetsAtPath(mainAsset.GetAssetPath());

        [Pure]
        [CanBeNull]
        public static T LoadAsset<T>(this string path) where T : Object => AssetDatabase.LoadAssetAtPath<T>(path);

        [Pure]
        public static bool TryLoadAsset<T>(this string path, out T asset) where T : Object => asset = AssetDatabase.LoadAssetAtPath<T>(path);

        [Pure]
        public static IEnumerable<T> LoadAssets<T>(this string path) where T : Object => AssetDatabase.LoadAllAssetsAtPath(path).OfType<T>();

        [Pure]
        [CanBeNull]
        public static string GetAssetPath(this Object asset) => AssetDatabase.GetAssetPath(asset);

        [Pure]
        public static GUID GetAssetGUID(this Object asset) => asset.GetAssetPath().PathToGUID();

        [Pure]
        [CanBeNull]
        public static T FindAsset<T>(params string[] folders) where T : Object => $"t:{typeof(T).Name}".FindAsset<T>(folders);

        [Pure]
        [CanBeNull]
        public static T FindAsset<T>(this string filter, params string[] folders) where T : Object => filter.FindAssetsGUIDs(folders).FirstOrDefault().LoadAssetByGUID<T>();

        [Pure]
        public static IEnumerable<T> FindAssets<T>(this Type type, params string[] folders) where T : Object =>
            $"t:{type.Name}".FindAssetsGUIDs(folders).Select(guid => guid.LoadAssetByGUID<T>());

        [Pure]
        public static IEnumerable<T> FindAssets<T>(params string[] folders) where T : Object =>
            $"t:{typeof(T).Name}".FindAssetsGUIDs(folders).Select(guid => guid.LoadAssetByGUID<T>());

        [Pure]
        public static IEnumerable<Object> FindAssets(this Type type, params string[] folders) =>
            $"t:{type.Name}".FindAssetsGUIDs(folders).Select(guid => guid.LoadAsset());

        [Pure]
        public static IEnumerable<T> FindAssets<T>(this string filter, params string[] folders) where T : Object =>
            filter.FindAssetsGUIDs(folders).Select(guid => guid.LoadAssetByGUID<T>());

        [Pure]
        public static string[] FindAssetsGUIDs(this string filter, params string[] folders) => AssetDatabase.FindAssets(filter, folders);

        [Pure]
        public static T FindPrefab<T>() where T : Object => FindPrefabs<T>().FirstOrDefault();

        [Pure]
        public static IEnumerable<T> FindPrefabs<T>() where T : Object {
            foreach (string path in AssetDatabase.GetAllAssetPaths().Where(path => path.StartsWith("Assets") && path.EndsWith(".prefab")))
                if (path.TryLoadAsset(out T asset))
                    yield return asset;
        }

        #endregion

        public static void Destroy(this Object obj, bool delayed = false) {
            if (delayed)
                EditorApplication.delayCall += Destroy;
            else
                Destroy();

            return;

            void Destroy() {
                if (Application.isPlaying)
                    Object.Destroy(obj);
                else
                    Object.DestroyImmediate(obj);
            }
        }
#else
        public static void Destroy(this Object self) => Object.Destroy(self);
#endif

        [Pure]
        public static IEnumerable<TResult> ToNew<TResult>(this IEnumerable collection) where TResult : class =>
            collection.Cast<object>().Select(source => Activator.CreateInstance(typeof(TResult), source) as TResult);

    }

}