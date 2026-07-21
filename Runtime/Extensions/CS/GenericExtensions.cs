using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace DevelopmentEssentials.Extensions.CS {

    public static class GenericExtensions {

        /// <returns>The <paramref name="replacement"/> value if the <paramref name="original"/> equals the <paramref name="comparableValue"/></returns>
        public static T EnsureRef<T>(this ref T original, T replacement, T comparableValue = default) where T : struct, IEquatable<T> =>
            original.Equals(comparableValue) ? replacement : original;

        /// <returns>The <paramref name="replacement"/> value if the <paramref name="original"/> equals the <paramref name="comparableValue"/></returns>
        [Pure]
        public static T Replace<T>(this T original, T replacement, T comparableValue = default) where T : IEquatable<T> =>
            original.Equals(comparableValue) ? replacement : original;

        /// <returns>The <paramref name="replacement"/> value if the <paramref name="original"/> equals the <paramref name="comparableValue"/></returns>
        [Pure]
        public static T Replace<T>(this T original, T replacement, [NotNull] Func<T, bool> condition) where T : IEquatable<T> =>
            condition(original)
                ? replacement
                : original;

        public static T2 Debug<T1, T2>(this T1 t, Func<T1, T2> action) => action(t);

        public static T Debug<T>(this T t, Action<T> action) {
            action(t);
            return t;
        }

        public static T Out<T>(this T t, out T var) => t.Var(out var);

        public static T Out<T, V>(this T t, Func<T, V> getter, out V var) => t.Var(getter, out var);

        public static T Var<T>(this T t, out T var) => var = t;

        public static T Var<T, V>(this T t, Func<T, V> getter, out V var) {
            var = getter(t);
            return t;
        }

        [Pure]
        public static T[] Array<T>(this T t, params T[] ts) {
            T[] result = new T[ts.Length + 1];
            result[0] = t;
            System.Array.Copy(ts, 0, result, 1, ts.Length);
            return result;
        }

        [Pure]
        public static List<T> List<T>(this T t, params T[] ts) {
            List<T> list = new(ts.Length + 1) { t };
            list.AddRange(ts);
            return list;
        }

    }

}