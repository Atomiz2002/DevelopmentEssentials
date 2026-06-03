#if UNITY_EDITOR && !SIMULATE_BUILD
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
#if DEVELOPMENT_ESSENTIALS_RUNTIME_NEWTONSOFT_JSON
using Newtonsoft.Json;
#endif

namespace DevelopmentEssentials.Extensions.CS {

    public static class CopyPasteExtensions {

        public static T CopyObjToClipboard<T>(this T t) {
            if (t is Texture2D t2d)
                t2d.CopyTextureToClipboard();
            else {
#if DEVELOPMENT_ESSENTIALS_RUNTIME_NEWTONSOFT_JSON
                EditorGUIUtility.systemCopyBuffer = t is Object obj
                    ? JsonUtility.ToJson(obj, true)
                    : JsonConvert.SerializeObject(t, Formatting.Indented);
#else
                EditorGUIUtility.systemCopyBuffer = JsonUtility.ToJson(t, true);
#endif
            }

            if (t is ICopyable copyable)
                copyable.OnCopy(EditorGUIUtility.systemCopyBuffer);

            return t;
        }

        public static T PasteObjFromClipboard<T>(this T t) {
#if DEVELOPMENT_ESSENTIALS_RUNTIME_NEWTONSOFT_JSON
            if (t is Object obj)
                JsonUtility.FromJsonOverwrite(EditorGUIUtility.systemCopyBuffer, obj);
            else
                JsonConvert.PopulateObject(EditorGUIUtility.systemCopyBuffer, t);
#else
            JsonUtility.FromJsonOverwrite(EditorGUIUtility.systemCopyBuffer, t);
#endif

            if (t is ICopyable copyable)
                copyable.OnPaste();

            return t;
        }

    }

}
#endif