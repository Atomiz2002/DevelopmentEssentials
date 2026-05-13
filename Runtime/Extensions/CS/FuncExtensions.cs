#pragma warning disable CS0168

using System;
using UnityEngine;
#if ENABLE_LOGS
using DevelopmentEssentials.Extensions.Unity.ExtendedLogger;
#endif

// TODO: Build a window to track all invokes
namespace DevelopmentEssentials.Extensions.CS {

    public static class FuncExtensions {

        [HideInCallstack]
        public static T InvokeSafe<T>(this Func<T> func) {
            try {
                return func.Invoke();
            }
            catch (Exception e) {
#if ENABLE_LOGS
                e.LogEx();
#endif
            }

            return default;
        }

        [HideInCallstack]
        public static T2 InvokeSafe<T1, T2>(this Func<T1, T2> func, T1 arg1) {
            try {
                return func.Invoke(arg1);
            }
            catch (Exception e) {
#if ENABLE_LOGS
                e.LogEx();
#endif
            }

            return default;
        }

        [HideInCallstack]
        public static T3 InvokeSafe<T1, T2, T3>(this Func<T1, T2, T3> func, T1 arg1, T2 arg2) {
            try {
                return func.Invoke(arg1, arg2);
            }
            catch (Exception e) {
#if ENABLE_LOGS
                e.LogEx();
#endif
            }

            return default;
        }

        [HideInCallstack]
        public static T4 InvokeSafe<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> func, T1 arg1, T2 arg2, T3 arg3) {
            try {
                return func.Invoke(arg1, arg2, arg3);
            }
            catch (Exception e) {
#if ENABLE_LOGS
                e.LogEx();
#endif
            }

            return default;
        }

        [HideInCallstack]
        public static T5 InvokeSafe<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4) {
            try {
                return func.Invoke(arg1, arg2, arg3, arg4);
            }
            catch (Exception e) {
#if ENABLE_LOGS
                e.LogEx();
#endif
            }

            return default;
        }

        [HideInCallstack]
        public static T6 InvokeSafe<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) {
            try {
                return func.Invoke(arg1, arg2, arg3, arg4, arg5);
            }
            catch (Exception e) {
#if ENABLE_LOGS
                e.LogEx();
#endif
            }

            return default;
        }

        [HideInCallstack]
        public static T7 InvokeSafe<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) {
            try {
                return func.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
            }
            catch (Exception e) {
#if ENABLE_LOGS
                e.LogEx();
#endif
            }

            return default;
        }

        [HideInCallstack]
        public static T8 InvokeSafe<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) {
            try {
                return func.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            }
            catch (Exception e) {
#if ENABLE_LOGS
                e.LogEx();
#endif
            }

            return default;
        }

        [HideInCallstack]
        public static T9 InvokeSafe<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) {
            try {
                return func.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            }
            catch (Exception e) {
#if ENABLE_LOGS
                e.LogEx();
#endif
            }

            return default;
        }

        [HideInCallstack]
        public static T10 InvokeSafe<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9) {
            try {
                return func.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            }
            catch (Exception e) {
#if ENABLE_LOGS
                e.LogEx();
#endif
            }

            return default;
        }

        [HideInCallstack]
        public static T11 InvokeSafe<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10) {
            try {
                return func.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            }
            catch (Exception e) {
#if ENABLE_LOGS
                e.LogEx();
#endif
            }

            return default;
        }

        [HideInCallstack]
        public static T12 InvokeSafe<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11) {
            try {
                return func.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
            }
            catch (Exception e) {
#if ENABLE_LOGS
                e.LogEx();
#endif
            }

            return default;
        }

        [HideInCallstack]
        public static T13 InvokeSafe<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12) {
            try {
                return func.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
            }
            catch (Exception e) {
#if ENABLE_LOGS
                e.LogEx();
#endif
            }

            return default;
        }

        [HideInCallstack]
        public static T14 InvokeSafe<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13) {
            try {
                return func.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
            }
            catch (Exception e) {
#if ENABLE_LOGS
                e.LogEx();
#endif
            }

            return default;
        }

        [HideInCallstack]
        public static T15 InvokeSafe<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14) {
            try {
                return func.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
            }
            catch (Exception e) {
#if ENABLE_LOGS
                e.LogEx();
#endif
            }

            return default;
        }

        [HideInCallstack]
        public static T16 InvokeSafe<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15) {
            try {
                return func.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
            }
            catch (Exception e) {
#if ENABLE_LOGS
                e.LogEx();
#endif
            }

            return default;
        }

        [HideInCallstack]
        public static T17 InvokeSafe<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16) {
            try {
                return func.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
            }
            catch (Exception e) {
#if ENABLE_LOGS
                e.LogEx();
#endif
            }

            return default;
        }

    }

}