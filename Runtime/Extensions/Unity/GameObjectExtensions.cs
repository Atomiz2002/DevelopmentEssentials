using System.IO;
using System.Linq;
using DevelopmentEssentials.Extensions.CS;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
#if DEVELOPMENT_ESSENTIALS_COMPONENT_NAMES
using ComponentNames;
#endif

namespace DevelopmentEssentials.Extensions.Unity {

    public static class GameObjectExtensions {

        // TODO: ExcludeChildren: test and implement this
        // public static bool IsDescendantOf(this Transform descendant, Transform parent) =>
        //     descendant.parent == parent
        //     || descendant.parent.IsDescendantOf(parent) && descendant.parent != descendant.root;
        //
        // public static bool IsParentOf(this Transform potentialParent, Transform potentialChild) =>
        //     potentialParent.Cast<Transform>().Any(child => child == potentialChild);

        public static T New<T>(string name = null) where T : SingletonBehaviour<T> => new GameObject(name ?? typeof(T).Name).AddComponent<T>();

        public static string PathInHierarchy(this GameObject go) {
            return go.transform.parent
                ? $"{go.transform.parent.gameObject.PathInHierarchy()}.{go.name}"
                : go.name;
        }

#if DEVELOPMENT_ESSENTIALS_COMPONENT_NAMES
        public static bool TryGetComponentAbove<T>(this GameObject go, ref T component, string name = null, bool @override = false) where T : Component {
#else
        public static bool TryGetComponentAbove<T>(this GameObject go, ref T component, bool @override = false) where T : Component {
#endif
            if (component && !@override) return true;

            Transform parent = go.transform;

            while (!component && parent) {
                parent = parent.parent;

                if (parent.gameObject.TryGetComponent(ref component))
#if DEVELOPMENT_ESSENTIALS_COMPONENT_NAMES
                    if (name == null || component.GetName() == name)
#endif
                        return true;
            }

            return false;
        }

#if DEVELOPMENT_ESSENTIALS_COMPONENT_NAMES
        public static bool TryGetComponentBelow<T>(this GameObject go, ref T component, string name = null, bool @override = false) where T : Component {
#else
        public static bool TryGetComponentBelow<T>(this GameObject go, ref T component, bool @override = false) where T : Component {
#endif
            if (component && !@override) return true;
#if DEVELOPMENT_ESSENTIALS_COMPONENT_NAMES
            return component = go.GetComponentsInChildren<T>().FirstOrDefault(c => name == null || c.GetName() == name);
#else
            return component = go.GetComponentInChildren<T>();
#endif
        }

#if DEVELOPMENT_ESSENTIALS_COMPONENT_NAMES
        public static bool TryGetComponent<T>(this GameObject go, ref T component, string name = null, bool @override = false) where T : Component {
#else
        public static bool TryGetComponent<T>(this GameObject go, ref T component, bool @override = false) where T : Component {
#endif
            if (component && !@override) return true;
#if DEVELOPMENT_ESSENTIALS_COMPONENT_NAMES
            return component = go.GetComponents<T>().FirstOrDefault(c => name == null || c.GetName() == name);
#else
            return component = go.GetComponent<T>();
#endif
        }

        public static T TryAddComponent<T>(this GameObject go) where T : Component => go.TryGetComponent(out T component) ? component : go.AddComponent<T>();

        /// <returns>true if added new component</returns>
#if DEVELOPMENT_ESSENTIALS_COMPONENT_NAMES
        public static bool TryAddComponent<T>(this GameObject go, out T component, string name = null) where T : Component {
            component = go.GetComponents<T>().FirstOrDefault(c => name == null || c.GetName() == name);

            if (component)
                return false;

            component = go.AddComponent<T>();
            component.SetName(name);
            return true;
        }
#else
        public static bool TryAddComponent<T>(this GameObject go, out T component) where T : Component {
            if (go.TryGetComponent(out component))
                return false;

            component = go.AddComponent<T>();
            return true;
        }
#endif

        public static T GetComponentInParent<T>(this GameObject go, out T parent) where T : Component => parent = go.GetComponentInParent<T>();

        public static T[] GetComponentsInChildren<T>(this GameObject go, out T[] parent) where T : Component => parent = go.GetComponentsInChildren<T>();

        public static void GetOrInstantiateSibling<T>(this GameObject go, string name, out T sibling) where T : Component =>
            go.GetOrInstantiateChild(name, out sibling);

        public static T GetOrInstantiateSibling<T>(this GameObject go, Transform child) where T : Component =>
            go.GetOrInstantiateChild<T>(child.parent);

        public static GameObject GetOrInstantiateSibling(this GameObject go, string name) =>
            go.GetOrInstantiateChild(name);

        public static bool GetOrInstantiateChild<T>(this GameObject go, string parentName, out T child) where T : Component =>
            child = go.GetOrInstantiateChild<T>(parentName);

        public static T GetOrInstantiateChild<T>(this GameObject go, string parentName) where T : Component {
            Transform uniqueChildWithComponent = go.transform.Find(parentName);

            return uniqueChildWithComponent
                ? uniqueChildWithComponent.GetComponent<T>()
                : go.GetOrInstantiateChild(parentName).AddComponent<T>();
        }

        public static T GetOrInstantiateChild<T>(this GameObject go, Transform parent) where T : Component {
            T uniqueChildWithComponent = parent.GetComponentInChildren<T>();

            return uniqueChildWithComponent
                ? uniqueChildWithComponent
                : go.GetOrInstantiateChild(typeof(T).Name).AddComponent<T>();
        }

        public static GameObject GetOrInstantiateChild(this GameObject go, string parentName) {
            Transform parent        = go.transform;
            Transform existingChild = parent.Find(parentName);

            return existingChild
                ? existingChild.gameObject
                : new(parentName) {
                    name = parentName,
                    transform = {
                        position = parent.position,
                        parent   = parent
                    }
                };
        }

        public static GameObject[] GetChildren(this GameObject go) =>
            go.transform
                .Cast<Transform>()
                .Select(t => t.gameObject)
                .ToHashSet()
                .ToArray();

        public static T LinkSprite<T>(this GameObject go) where T : Object {
#if UNITY_EDITOR
            return go.LinkAsset<T>(".png");
#else
            return null;
#endif
        }

        /// Gets the asset at the appropriate path.
        public static T LinkAsset<T>(this GameObject go, string extension = ".asset") where T : Object {
#if UNITY_EDITOR
            string[] scriptPath = go.GetType().FullName?.Replace("Scripts", "Textures").Split('.')
                                  ?? throw new($"{go.GetType().Name}{typeof(T).Name} has no Namespace");

            string assetType = extension == ".asset"
                ? typeof(T).Name
                : string.Empty;

            string spritePath = $"{Path.Combine("Assets", Path.Combine(scriptPath))}{assetType}{extension}";

            T asset = AssetDatabase.LoadAssetAtPath<T>(spritePath);

            if (!asset)
                throw new("Required asset not located at the appropriate path");

            return asset;
#else
            return null;
#endif
        }

        public static T Select<T>(this T t) where T : Object {
#if UNITY_EDITOR
            if (t is MonoBehaviour mono)
                Selection.activeObject = mono.gameObject;
            else
                Selection.activeObject = t;
#endif
            return t;
        }

        public static T Ping<T>(this T t) where T : Object {
#if UNITY_EDITOR
            if (t is MonoBehaviour mono)
                EditorGUIUtility.PingObject(mono.gameObject);
            else
                EditorGUIUtility.PingObject(t);
#endif
            return t;
        }

        public static T SelectAndPing<T>(this T t) where T : Object => t.Select().Ping();

        #region Instantiate

        // Unity

        public static T Instantiate<T>(this T original) where T : Object {
            T instance = Object.Instantiate(original);
#if UNITY_EDITOR && !SIMULATE_BUILD
            if (instance is GameObject i && original is GameObject o)
                i.TryAddComponent<PrefabSource>().SourcePrefab = o;
#endif
            return instance;
        }

        public static T Instantiate<T>(this T original, Transform parent) where T : Object {
            T instance = Object.Instantiate(original, parent, false);
#if UNITY_EDITOR && !SIMULATE_BUILD
            if (instance is GameObject i && original is GameObject o)
                i.TryAddComponent<PrefabSource>().SourcePrefab = o;
#endif
            return instance;
        }

        public static T Instantiate<T>(this T original, Transform parent, bool worldPositionStays) where T : Object {
            T instance = Object.Instantiate(original, parent, worldPositionStays);
#if UNITY_EDITOR && !SIMULATE_BUILD
            if (instance is GameObject i && original is GameObject o)
                i.TryAddComponent<PrefabSource>().SourcePrefab = o;
#endif
            return instance;
        }

        public static T Instantiate<T>(this T original, Vector3 position, Quaternion rotation = default) where T : Object {
            T instance = Object.Instantiate(original, position, rotation);
#if UNITY_EDITOR && !SIMULATE_BUILD
            if (instance is GameObject i && original is GameObject o)
                i.TryAddComponent<PrefabSource>().SourcePrefab = o;
#endif
            return instance;
        }

        public static T Instantiate<T>(this T original, Vector3 position, Quaternion rotation, Transform parent) where T : Object {
            T instance = Object.Instantiate(original, position, rotation, parent);
#if UNITY_EDITOR && !SIMULATE_BUILD
            if (instance is GameObject i && original is GameObject o)
                i.TryAddComponent<PrefabSource>().SourcePrefab = o;
#endif
            return instance;
        }

        public static T InstantiateLocal<T>(this T original, Transform parent, Vector3 localPosition, Quaternion localRotation) where T : Object {
            T instance = Object.Instantiate(original, localPosition, localRotation, parent);

            if (instance is GameObject go) {
                go.transform.localPosition = localPosition;
                go.transform.localRotation = localRotation;
#if UNITY_EDITOR && !SIMULATE_BUILD
                if (original is GameObject o)
                    go.TryAddComponent<PrefabSource>().SourcePrefab = o;
#endif
            }

            return instance;
        }

        public static T Instantiate<T>(this T original, InstantiateParameters parameters) where T : Object {
            T instance = Object.Instantiate(original, parameters);
#if UNITY_EDITOR && !SIMULATE_BUILD
            if (instance is GameObject i && original is GameObject o)
                i.TryAddComponent<PrefabSource>().SourcePrefab = o;
#endif
            return instance;
        }

        public static T Instantiate<T>(this T original, Vector3 position, Quaternion rotation, InstantiateParameters parameters) where T : Object {
            T instance = Object.Instantiate(original, position, rotation, parameters);
#if UNITY_EDITOR && !SIMULATE_BUILD
            if (instance is GameObject i && original is GameObject o)
                i.TryAddComponent<PrefabSource>().SourcePrefab = o;
#endif
            return instance;
        }

        // Additional

        public static GameObject Instantiate(this GameObject prefab) => Object.Instantiate(prefab);

        public static T Instantiate<T>(this GameObject prefab) where T : Component => prefab.Instantiate().GetComponent<T>();

        public static GameObject Instantiate(this GameObject prefab, out GameObject instance) => instance = prefab.Instantiate();

        public static GameObject Instantiate(this GameObject prefab, Transform parent, Vector3 localPosition, Vector3 localRotation, float localScale = 1f) =>
            prefab.Instantiate<Component>(parent, localPosition, localRotation, out _, localScale);

        public static GameObject Instantiate(this GameObject prefab, Transform parent, Vector3 localPosition, Quaternion localRotation, float localScale = 1f) =>
            prefab.Instantiate<Component>(parent, localPosition, localRotation, out _, localScale);

        public static GameObject Instantiate<T>(this GameObject prefab, out T component) where T : Component =>
            prefab.Instantiate(null, Vector3.zero, Quaternion.identity, out component);

        public static GameObject Instantiate<T>(this GameObject prefab, Transform parent, out T component) where T : Component =>
            prefab.Instantiate(parent, Vector3.zero, Quaternion.identity, out component);

        public static GameObject Instantiate<T>(this GameObject prefab, Transform parent, Vector3 localPosition, Vector3 localRotation, out T component, float localScale = 1f) where T : Component =>
            prefab.Instantiate(parent, localPosition, Quaternion.Euler(localRotation), out component, localScale);

        public static GameObject Instantiate<T>(this GameObject prefab, Transform parent, Vector3 localPosition, Quaternion localRotation, out T component, float localScale = 1f) where T : Component {
            GameObject gameObject = prefab.InstantiateLocal(parent, localPosition, localRotation);
            gameObject.TryGetComponent(out component);
            gameObject.transform.localScale *= localScale;
            return gameObject;
        }

#if UNITY_EDITOR && !SIMULATE_BUILD

        public class PrefabSource : MonoBehaviour {

            [ReadOnly] public GameObject SourcePrefab;

        }

        public static void DestroySmart(this Object obj, bool allowDestroyingAssets = false, bool delayCall = false, bool destroyGO = true) {
            if (!obj)
                return;

            Object destroy = obj;

            if (destroyGO && obj is Component component)
                destroy = component.gameObject;

            if (delayCall)
                EditorApplication.delayCall += Destroy;
            else
                Destroy();

            return;

            void Destroy() {
                if (Application.isPlaying)
                    Object.Destroy(destroy);
                else
                    Object.DestroyImmediate(destroy, allowDestroyingAssets);
            }
        }
#else
        public static void DestroySmart(this Object self, bool _ = false, bool __ = false, bool ___ = true) => Object.Destroy(self);
#endif

        #endregion

    }

}