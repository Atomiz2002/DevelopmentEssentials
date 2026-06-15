#if UNITY_EDITOR
using System.IO;
using System.Linq;
using DevelopmentEssentials.Extensions.CS;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DevelopmentEssentials {

    public static class EditorHelper {

        public static T InstanceSO<T>(ref T i, string dir = null, string name = null) where T : ScriptableObject {
            if (i)
                return i;

            name ??= typeof(T).Name;
            dir  ??= Path.Combine(Application.dataPath, typeof(T).Name);

            i = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets($"t:{typeof(T).Name}").FirstOrDefault()));
            if (i)
                return i;

            Directory.CreateDirectory(dir);
            AssetDatabase.CreateAsset(i = ScriptableObject.CreateInstance<T>(), Path.Combine(dir, name + ".asset").RelativePath());

            return i;
        }

        public static void Properties(this Object o) => EditorUtility.OpenPropertyEditor(o);

    }

}
#endif