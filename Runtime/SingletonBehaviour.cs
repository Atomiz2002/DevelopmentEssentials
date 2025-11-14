using System;
using System.Collections.Generic;
using DevelopmentEssentials.Extensions.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace DevelopmentEssentials {

    [DisallowMultipleComponent]
    public abstract class SingletonBehaviour<T> : GeneralBehaviour where T : SingletonBehaviour<T> {

        /// Dont destroy on load new scene
        protected virtual bool Global => true;

        private static T instance;
        public static  T Instance => instance ? instance : instance = FindFirstObjectByType<T>();

        protected readonly UnityEvent OnInitializedEvent = new();

#if UNITY_EDITOR
        protected override void Reference() {
            base.Reference();
            SingletonUtils.EnsureSingleInstances();
        }
#endif

        protected virtual void OnInitialized() {}

        private void OnDestroy() {
            instance = null;
            OnInitializedEvent.RemoveAllListeners();
        }

        public static class SingletonUtils {

            // internal static void EnsureSingleInstance<T>() where T : SingletonBehaviour<T> {
            //     Dictionary<Type, string> existing = new();
            //
            //     foreach (T gameBehaviour in MonoBehaviour.FindObjectsOfType<T>())
            //         if (!existing.TryAdd(typeof(T), gameBehaviour.name))
            //             Debug.LogError($"Found duplicates for component {typeof(T).Name} in {existing[typeof(T)]} and {gameBehaviour.name} game objects");
            // }

#if UNITY_EDITOR
            internal static void EnsureSingleInstances() {
                Dictionary<Type, GameObject> existing = new();

                foreach (MonoBehaviour behaviour in FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                    if (IsGenericBehaviour(behaviour, typeof(SingletonBehaviour<>), out Type type))
                        if (!existing.TryAdd(type, behaviour.gameObject))
                            Debug.LogError("Found duplicates for component "
                                           + $"<color={Color.cyan}>{type.Name}</color> in "
                                           + $"{existing[type].PathInHierarchy().Colored(Color.red)} and "
                                           + $"{behaviour.gameObject.PathInHierarchy().Colored(Color.red)} game objects");
            }

            private static bool IsGenericBehaviour(MonoBehaviour behaviour, Type genericType, out Type type) {
                return (type = GetGenericBaseType()?.GetGenericArguments()[0]) != null;

                Type GetGenericBaseType() {
                    Type baseType = behaviour.GetType();

                    while (baseType != null && (!baseType.IsGenericType || baseType.GetGenericTypeDefinition() != genericType))
                        baseType = baseType.BaseType;

                    return baseType;
                }
            }
#endif

        }

    }

}