using System;
using DevelopmentEssentials.Extensions.CS;
using Object = UnityEngine.Object;

namespace DevelopmentEssentials.Extensions.Unity {

    public static class UnityNull {

        /// Safe replacement for <c>unityObj is X x &amp;&amp; x...</c>
        public static T As<T>(this Object unityObj, out T t) where T : class => t = (unityObj as T).n();

        /// Safe replacement for <c>unityObj is X</c>
        public static bool Is<T>(this Object unityObj) where T : Object => (unityObj as T).n();

        /// Safe <b>conditional access</b>.<br/>Safe <c>unityObj?.</c> call
        public static TResult n<TValue, TResult>(this TValue unityObj, Func<TValue, TResult> func) where TValue : Object => unityObj ? func.SafeInvoke(unityObj) : default;

        // /// Safe <b>null coalesce</b>.<br/><c>unityObj?.</c> and <c>unityObj ??</c> is unsafe.<br/><c>unityObj.Nc()?.</c> and <c>unityObj.Nc() ??</c> is safe.
        // public static T n<T>(this T unityObj) where T : Object => unityObj ? unityObj : null;

        public static T n<T>(this T obj) where T : class
            => obj is Object o
                ? o
                    ? obj
                    : null
                : obj;

    }

}