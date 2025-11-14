using System.Diagnostics;
using DevelopmentEssentials.Extensions.Unity;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DevelopmentEssentials {

    [HideMonoScript]
    public abstract class GeneralBehaviour : MonoBehaviour {

        #region Constant Names

        protected const string NAME_BOUNDING_BOX    = "Bounding Box";
        protected const string NAME_HITBOX          = "Hitbox";
        protected const string NAME_SPRITE          = "Sprite";
        protected const string NAME_ITEM_HOLDER     = "ItemHolder";
        protected const string NAME_HEALTH_BAR_BG   = "HealthBarBg";
        protected const string NAME_HEALTH_BAR_FILL = "HealthBarFill";

        #endregion

        /// IDK, for separation’s sake but if its confusing just remove it
        [Conditional("UNITY_EDITOR")]
        protected virtual void Reference() {}

        [OnInspectorInit]
        protected virtual void OnValidate() {
            Reference();
        }

        protected virtual void Reset() {
            Reference();
        }

#if DEVELOPMENT_ESSENTIALS_COMPONENT_NAMES
        protected bool TryGetComponentAbove<T>(ref T component, string name = null, bool @override = false) where T : Component => gameObject.TryGetComponentAbove(ref component, name, @override);
        protected bool TryGetComponentBelow<T>(ref T component, string name = null, bool @override = false) where T : Component => gameObject.TryGetComponentBelow(ref component, name, @override);
        protected bool TryGetComponent<T>(ref T component, string name = null, bool @override = false) where T : Component      => gameObject.TryGetComponent(ref component, name, @override);
        protected bool TryAddComponent<T>(out T component, string name = null) where T : Component                              => gameObject.TryAddComponent(out component, name);
#else
        protected bool TryGetComponentAbove<T>(ref T component, string unused = null, bool @override = false) where T : Component => gameObject.TryGetComponentAbove(ref component, @override);
        protected bool TryGetComponentBelow<T>(ref T component, string unused = null, bool @override = false) where T : Component => gameObject.TryGetComponentBelow(ref component, @override);
        protected bool TryGetComponent<T>(ref T component, string unused = null, bool @override = false) where T : Component      => gameObject.TryGetComponent(ref component, @override);
        protected bool TryAddComponent<T>(out T component, string unused = null) where T : Component                              => gameObject.TryAddComponent(out component);
#endif
        protected T            LinkSprite<T>() where T : Object                                           => gameObject.LinkSprite<T>();
        protected T            LinkAsset<T>(string extension = ".asset") where T : Object                 => gameObject.LinkAsset<T>(extension);
        protected T            GetComponentInParent<T>(out T parent) where T : Component                  => gameObject.GetComponentInParent(out parent);
        protected T[]          GetComponentsInChildren<T>(out T[] parent) where T : Component             => gameObject.GetComponentsInChildren(out parent);
        protected void         GetOrInstantiateSibling<T>(string name, out T sibling) where T : Component => gameObject.GetOrInstantiateSibling(name, out sibling);
        protected T            GetOrInstantiateSibling<T>(Transform child) where T : Component            => gameObject.GetOrInstantiateSibling<T>(child);
        protected GameObject   GetOrInstantiateSibling(string name)                                       => gameObject.GetOrInstantiateSibling(name);
        protected bool         GetOrInstantiateChild<T>(string name, out T child) where T : Component     => gameObject.GetOrInstantiateChild(name, out child);
        protected T            GetOrInstantiateChild<T>(string name) where T : Component                  => gameObject.GetOrInstantiateChild<T>(name);
        protected T            GetOrInstantiateChild<T>(Transform parent) where T : Component             => gameObject.GetOrInstantiateChild<T>(parent);
        protected GameObject   GetOrInstantiateChild(string name)                                         => gameObject.GetOrInstantiateChild(name);
        protected GameObject[] GetChildren()                                                              => gameObject.GetChildren();

    }

}