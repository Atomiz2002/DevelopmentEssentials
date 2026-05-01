using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace DevelopmentEssentials.Extensions.CS {

    public static class EnumerableExtensions {

        [Pure]
        public static IEnumerable<T> Distinct<T, T2>(this IEnumerable<T> source, Func<T, T2> keySelector) {
            if (source == null) return Enumerable.Empty<T>();

            HashSet<T2> seenKeys = new();
            return source.Where(item => seenKeys.Add(keySelector(item)));
        }

        [Pure]
        public static int Count<T>(this IEnumerable<T> source, T element) => source.Count(x => EqualityComparer<T>.Default.Equals(x, element));

        [Pure]
        public static bool HasDuplicates<T>(this IEnumerable<T> source, T element) => source.Count(element) > 1;

        /// <returns>true if empty or all elements are null</returns>
        [Pure]
        public static bool IsEmpty<T>(this IEnumerable<T> collection) =>
            collection == null || collection.All(e => e == null);

        /// <returns>true if empty or any element is null</returns>
        [Pure]
        public static bool HasEmpty<T>(this IEnumerable<T> collection) =>
            collection == null || collection.Any(e => e == null);

        [Pure]
        public static IEnumerable<T> Except<T>(this IEnumerable<T> collection, T element) =>
            collection.Where(e => !EqualityComparer<T>.Default.Equals(e, element));

        public static IEnumerable<T> OrderByName<T>(this IEnumerable<T> collection) where T : Object => collection.OrderBy(o => o.name);

        /// <returns>The collection with merged duplicate elements</returns>
        [Pure]
        public static IEnumerable<T> MergeDuplicates<T>(this IEnumerable<T> input) {
            using IEnumerator<T> enumerator = input.GetEnumerator();

            if (!enumerator.MoveNext())
                yield break; // Handle empty collection

            T previous = enumerator.Current;
            yield return previous; // Yield the first element

            while (enumerator.MoveNext()) {
                if (EqualityComparer<T>.Default.Equals(previous, enumerator.Current))
                    continue;

                previous = enumerator.Current;
                yield return previous;
            }
        }

        /// Filters out the null <see cref="UnityEngine.Object"/>s
        [Pure]
        public static IEnumerable<T> Existing<T>(this IEnumerable<T> collection) where T : Object => collection.Where(x => x);

        /// Filters out the null <see cref="object"/>s
        [Pure]
        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> collection) => collection.Where(x => x != null);

        [Pure]
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> pairs) =>
            pairs.ToDictionary(pair => pair.Key, pair => pair.Value);

        [Pure]
        public static IEnumerable<T> Enumerate<T>(this int count, Func<int, T> func) => Enumerable.Range(0, count).Select(func);

        /// <seealso cref="ClearUnityNulls"/>
        public static void ClearNulls<T>(this List<T> list) => list.RemoveAll(x => x.Equals(null));

        /// <seealso cref="ClearNulls"/>
        public static List<T> ClearUnityNulls<T>(this List<T> list) where T : Object {
            list.RemoveAll(x => !x);
            return list;
        }

        public static List<T> Distinctify<T>(this List<T> list) {
            if (list.Count <= 1)
                return list;

            list.Sort();

            for (int i = list.Count - 1; i > 0; i--)
                if (list[i].Equals(list[i - 1]))
                    list.RemoveAt(i);

            return list;
        }

        [Pure]
        [StringFormatMethod("format")]
        public static IEnumerable<string> ToStrings<T>(this IEnumerable<T> source, string nullValue = "\"null\"", [NotNull] string format = "{0}") =>
            source?.Select(e => string.Format(format, e.EnsureString(nullValue)));

        [Pure]
        public static T ElementAtOrDefault<T>(this IEnumerable<T> source, Index index, T @default = default) {
            if (source == null)
                return @default;

            T[] iEnumerable = source as T[] ?? source.ToArray();
            int i           = index.GetOffset(iEnumerable.Length);
            T   element     = Enumerable.ElementAtOrDefault(iEnumerable, i);

            if (element == null || element.Equals(@default))
                return @default;

            return element;
        }

        public static T SafeMin<T>(this IEnumerable<T> source, T @default = default) => source.Prepend(@default).Min();

        public static float SafeMin<T1>(this IEnumerable<T1> source, Func<T1, float> selector, float @default = 0) {
            try {
                return source.Min(selector);
            }
            catch (Exception) {
                return @default;
            }
        }

        [Pure]
        [StringFormatMethod("format")]
        public static string Join<T>([NotNull] this IEnumerable<T> collection, string separator = ", ", string defaultValue = "\"null\"", [NotNull] string format = "{0}") =>
            string.Join(separator, collection.ToStrings(defaultValue, format));

        [Pure]
        [StringFormatMethod("format")]
        public static string Join<T, TResult>([NotNull] this IEnumerable<T> collection, Func<T, TResult> selector, string separator = ", ", string defaultValue = "\"null\"", [NotNull] string format = "{0}") =>
            collection.Select(selector).Join(separator, defaultValue, format);

        [Pure]
        public static string JoinSmart(this IEnumerable collection, string separator = "", string defaultValue = "\"null\"", string prefix = "", string suffix = "") {
            IList<string> list = collection.ToStringList(defaultValue);

            int count = list.Count;

            switch (count) {
                case 0: return defaultValue; // Early exit for empty list
                case 1: return prefix + list[0] + suffix; // Early exit for single item (no separator logic needed)
            }

            int  preLen      = prefix.Length;
            int  sufLen      = suffix.Length;
            int  sepLen      = separator.Length;
            long totalLength = 0; // Use long to prevent overflow on massive logs

            // Exact size calculation
            for (int i = 0; i < count; i++) {
                totalLength += preLen + (list[i]?.Length ?? 0) + sufLen;
                if (i < count - 1)
                    totalLength += sepLen;
            }

            // Single-allocation build
            StringBuilder sb = new((int) totalLength);

            for (int i = 0; i < count; i++) {
                sb.Append(prefix);
                sb.Append(list[i]);
                sb.Append(suffix);

                if (i < count - 1)
                    sb.Append(separator);
            }

            return sb.ToString();
        }

        private static IList<string> ToStringList([NotNull] this IEnumerable collection, string nullValue = "\"null\"") {
            switch (collection) {
                case IList<string> list:             return list;
                case IEnumerable<string> enumerable: return new List<string>(enumerable);
                default: {
                    // High-performance manual conversion
                    IEnumerator  enumerator = collection.GetEnumerator();
                    List<string> newList    = new();

                    try {
                        while (enumerator.MoveNext())
                            newList.Add(enumerator.Current?.EnsureString(nullValue) ?? string.Empty);
                    }
                    finally {
                        (enumerator as IDisposable)?.Dispose();
                    }

                    return newList;
                }
            }
        }

        [Pure]
        public static T2 ElementAtOrDefaultValue<T1, T2>(this IEnumerable<T1> collection, Index index, Func<T1, T2> getter, T2 @default = default) =>
            getter(collection.ElementAtOrDefault(index)) ?? @default;

        #region Index

        [Pure]
        public static int IndexOf<T>(this IEnumerable<T> collection, T element, int @default = -1) {
            int index = collection.ToList().IndexOf(element);
            return index == -1 ? @default : index;
        }

        [Pure]
        public static int IndexOfOr([NotNull] this string source, char search, int defaultIndex = 0) =>
            source.Contains(search) ? source.IndexOf(search) : defaultIndex;

        [Pure]
        public static int IndexOfOr([NotNull] this string source, string search, int defaultIndex = 0) =>
            source.Contains(search) ? source.IndexOf(search) : defaultIndex;

        [Pure]
        public static int LastIndexOfOr([NotNull] this string source, string search, int defaultIndex = 0) =>
            source.Contains(search) ? source.LastIndexOf(search) : defaultIndex;

        [Pure]
        public static bool IsIndexWithin<T>(this IEnumerable<T> enumerable, int index) => index >= 0 && index < enumerable.Count();

        #endregion

        #region Random

        public static int RandomIndex<T>(this IEnumerable<T> source) => new Random().Next(source.Count());

        /// <param name="percentage">The percentage of times to return a random element instead of the default value</param>
        /// <example>if percentage = 90<br/>returns a random element 90% of the time<br/>returns the default value 10% of the time</example>
        public static T Random<T>(this IEnumerable<T> source, T @default = default, int percentage = 100) {
            if (new Random().Next(100) >= percentage)
                return @default;

            IEnumerable<T> enumerable = source as T[] ?? source.ToArray();
            return enumerable.ElementAtOrDefault(enumerable.RandomIndex(), @default);
        }

        #endregion

        #region OrDefault

        [Pure]
        public static T ElementAtOrDefault<T>(this IEnumerable<T> source, int index, T @default) {
            if (source == null) return @default;

            T element = source.ElementAtOrDefault(index);

            if (element == null || element.Equals(@default))
                return @default;

            return element;
        }

        [Pure]
        public static T ElementAtOrDefaultValue<T>(this IEnumerable<T> collection, int index, T @default = default) =>
            collection.ElementAtOrDefault(index) ?? @default;

        [Pure]
        public static T2 ElementAtOrDefaultValue<T1, T2>(this IEnumerable<T1> collection, int index, Func<T1, T2> getter, T2 @default = default) =>
            getter(collection.ElementAtOrDefault(index)) ?? @default;

        #endregion

        #region Safe

        [Pure]
        public static IEnumerable<T> Safe<T>(this IEnumerable<T> source) => source ?? Enumerable.Empty<T>();

        [Pure]
        public static T SafeMax<T>(this IEnumerable<T> source, T @default = default) => source.Prepend(@default).Max();

        [Pure]
        public static float SafeMax<T1>(this IEnumerable<T1> source, Func<T1, float> selector, float @default = 0f) {
            try {
                return source.Max(selector);
            }
            catch (Exception) {
                return @default;
            }
        }

        #endregion

        #region Replace

        [Pure]
        public static IEnumerable<T> Replace<T>(this IEnumerable<T> collection, Func<T, T> replacement) => collection.Select(replacement);

        [Pure]
        public static IEnumerable<T> Replace<T>(this IEnumerable<T> collection, bool condition, Func<T, T> replacement) =>
            collection.Select(e => condition ? replacement(e) : e);

        [Pure]
        public static IEnumerable<T> Replace<T>(this IEnumerable<T> collection, Func<T, bool> condition, Func<T, T> replacement) =>
            collection.Select(e => condition(e) ? replacement(e) : e);

        [Pure]
        public static IEnumerable<T> Replace<T>(this IEnumerable<T> collection, bool condition, T replacement) =>
            collection.Select(e => condition ? replacement : e);

        [Pure]
        public static IEnumerable<T> Replace<T>(this IEnumerable<T> collection, Func<T, bool> condition, T replacement) =>
            collection.Select(e => condition(e) ? replacement : e);

        #endregion

        #region IDictionary

        public static IDictionary<TKey, TValue> RemoveKeys<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<TKey, bool> predicate) =>
            dictionary.RemoveKeys((k, _) => predicate.Invoke(k)); // InvokeSafe?

        public static IDictionary<TKey, TValue> RemoveKeys<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<TKey, TValue, bool> predicate) {
            List<TKey> keysToRemove = new();

            foreach ((TKey key, TValue value) in dictionary)
                if (predicate.Invoke(key, value)) // InvokeSafe?
                    keysToRemove.Add(key);

            foreach (TKey key in keysToRemove)
                dictionary.Remove(key);

            return dictionary;
        }

        [Pure]
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue @default = default) =>
            dictionary.TryGetValue(key, out TValue value) ? value : @default;

        [Pure]
        public static Dictionary<TKey, TValue> NonNullKeys<TKey, TValue>(this IDictionary<TKey, TValue> collection) => collection.Where(x => x.Key != null).ToDictionary();

        [Pure]
        public static IDictionary<TKey, TValue> NonNullValues<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) => dictionary.Where(x => x.Value != null).ToDictionary();

        /// Filters out the null <see cref="UnityEngine.Object"/>s
        [Pure]
        public static Dictionary<TKey, TValue> ExistingKeys<TKey, TValue>(this IDictionary<TKey, TValue> collection) where TKey : Object =>
            collection.Where(x => x.Key).ToDictionary();

        /// Filters out the null <see cref="UnityEngine.Object"/>s
        [Pure]
        public static IDictionary<TKey, TValue> ExistingValues<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) where TValue : Object => dictionary.Where(x => x.Value).ToDictionary();

        public static IDictionary<TKey, TValue> RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<TKey, TValue, bool> predicate) {
            foreach ((TKey key, TValue value) in dictionary)
                if (predicate(key, value))
                    dictionary.Remove(key);

            return dictionary;
        }

        [Pure]
        public static IDictionary<TKey, TValue> Except<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) =>
            dictionary.Where(e => !EqualityComparer<TKey>.Default.Equals(e.Key, key)).ToDictionary();

        [Pure]
        public static IDictionary<TKey, TValue> OrderByKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) =>
            dictionary.OrderBy(kvp => kvp.Key).ToDictionary();

        [Pure]
        public static IDictionary<TKey, TValue> OrderByKeyDescending<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) =>
            dictionary.OrderByDescending(kvp => kvp.Key).ToDictionary();

        #endregion

        #region Tuples

        [Pure]
        public static object[] ToArray(this ITuple tuple) {
            if (tuple == null)
                return null;

            object[] array = new object[tuple.Length];

            for (int i = 0; i < tuple.Length; i++)
                array[i] = tuple[i];

            return array;
        }

        [Pure]
        public static T[] ToArray<T>(this ITuple tuple) {
            T[] array = new T[tuple.Length];

            for (int i = 0; i < tuple.Length; i++)
                array[i] = (T) tuple[i];

            return array;
        }

        [Pure]
        public static (T, T) Tuple2<T>(this IEnumerable<T> source) {
            source = source.ToArray();
            return (source.ElementAtOrDefault(0), source.ElementAtOrDefault(1));
        }

        [Pure]
        public static (T, T, T) Tuple3<T>(this IEnumerable<T> source) {
            source = source.ToArray();

            return (source.ElementAtOrDefault(0), source.ElementAtOrDefault(1),
                source.ElementAtOrDefault(2));
        }

        [Pure]
        public static (T, T, T, T) Tuple4<T>(this IEnumerable<T> source) {
            source = source.ToArray();

            return (source.ElementAtOrDefault(0), source.ElementAtOrDefault(1),
                source.ElementAtOrDefault(2), source.ElementAtOrDefault(3));
        }

        #endregion

    }

}