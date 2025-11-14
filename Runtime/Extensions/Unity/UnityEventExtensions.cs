using System;
using DevelopmentEssentials.Extensions.Unity.ExtendedLogger;
using UnityEngine.Events;

namespace DevelopmentEssentials.Extensions.Unity {

    public static class UnityEventExtensions {

        public static void SafeInvoke(this UnityEvent action) {
            try {
                action?.Invoke();
            }
            catch (Exception e) {
                e.LogEx();
            }
        }

        public static void SafeInvoke<T>(this UnityEvent<T> action, T arg) {
            try {
                action?.Invoke(arg);
            }
            catch (Exception e) {
                e.LogEx();
            }
        }

        public static void SafeInvoke<T1, T2>(this UnityEvent<T1, T2> action, T1 arg1, T2 arg2) {
            try {
                action?.Invoke(arg1, arg2);
            }
            catch (Exception e) {
                e.LogEx();
            }
        }

        public static void SafeInvoke<T1, T2, T3>(this UnityEvent<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3) {
            try {
                action?.Invoke(arg1, arg2, arg3);
            }
            catch (Exception e) {
                e.LogEx();
            }
        }

        public static void SafeInvoke<T1, T2, T3, T4>(this UnityEvent<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4) {
            try {
                action?.Invoke(arg1, arg2, arg3, arg4);
            }
            catch (Exception e) {
                e.LogEx();
            }
        }

    }

}