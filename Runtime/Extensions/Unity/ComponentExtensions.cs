using UnityEditor;
using UnityEngine;

namespace DevelopmentEssentials.Extensions.Unity {

    public static class ComponentExtensions {

        public static bool FindUIElement<T>(out T component) where T : MonoBehaviour {
            return component = Object.FindFirstObjectByType<Canvas>().GetComponentInChildren<T>(true);
        }

#if DEVELOPMENT_ESSENTIALS_COMPONENT_NAMES
        public static bool TryAddComponent<T>(this Component c, out T component, string name = null) where T : Component => c.gameObject.TryAddComponent(out component, name);
#else
        public static bool TryAddComponent<T>(this Component c, out T component) where T : Component => c.gameObject.TryAddComponent(out component);
#endif

        public static T FindSprite<T>(this Component c) where T : Object =>
            c.gameObject.LinkSprite<T>();

        public static T FindAsset<T>(this Component c, string extension = ".asset") where T : Object =>
            c.gameObject.LinkAsset<T>(extension);

        public static T GetComponentInParent<T>(this Component c, out T parent) where T : Component =>
            c.gameObject.GetComponentInParent(out parent);

        public static T[] GetComponentsInChildren<T>(this Component c, out T[] parent) where T : Component =>
            c.gameObject.GetComponentsInChildren(out parent);

        public static void GetOrInstantiateSibling<T>(this Component c, string name, out T sibling) where T : Component =>
            c.gameObject.GetOrInstantiateSibling(name, out sibling);

        public static T GetOrInstantiateSibling<T>(this Component c, Transform child) where T : Component =>
            c.gameObject.GetOrInstantiateSibling<T>(child);

        public static GameObject GetOrInstantiateSibling(this Component c, string name) =>
            c.gameObject.GetOrInstantiateSibling(name);

        public static bool GetOrInstantiateChild<T>(this Component c, string parentName, out T child) where T : Component =>
            c.gameObject.GetOrInstantiateChild(parentName, out child);

        public static T GetOrInstantiateChild<T>(this Component c, string parentName) where T : Component =>
            c.gameObject.GetOrInstantiateChild<T>(parentName);

        public static T GetOrInstantiateChild<T>(this Component c, Transform parent) where T : Component =>
            c.gameObject.GetOrInstantiateChild<T>(parent);

        public static GameObject GetOrInstantiateChild(this Component c, string parentName) =>
            c.gameObject.GetOrInstantiateChild(parentName);

        public static void SetActive(this Component component, bool value) {
            if (!component)
                return;

            if (component.gameObject.activeSelf == value)
                return;

            component.gameObject.SetActive(value);
        }

        public static void Select(this Component component) {
            if (!component)
                return;

            component.gameObject.Select();
        }

        public static void SelectAndBreak(this Component component) {
            if (!component)
                return;

            component.Select();
            Debug.Break();
        }

        public static void AlignView(this Component component) {
            if (!component)
                return;

#if UNITY_EDITOR
            SceneView.lastActiveSceneView.AlignViewToObject(component.transform);
#endif
        }

    }

}