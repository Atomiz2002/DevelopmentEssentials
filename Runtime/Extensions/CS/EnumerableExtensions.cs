using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace DevelopmentEssentials.Extensions.CS {

    public static class EnumerableExtensions {

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

        [Pure]
        public static IEnumerable<string> ToStrings<T>(this IEnumerable<T> source, string nullValue = "\"null\"", [NotNull] string format = "{0}") =>
            source?.Select(e => string.Format(format, e.EnsureString(nullValue)));

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

        [Pure]
        [StringFormatMethod("format")]
        public static string Join<T>([NotNull] this IEnumerable<T> collection, string separator = ", ", string defaultValue = "\"null\"",
            [NotNull] string format = "{0}") =>
            string.Join(separator, collection.ToStrings(defaultValue, format));

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

        #region IDictionary

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

        [Pure]
        public static IEnumerable<T> Enumerate<T>(this int count, Func<int, T> func) => Enumerable.Range(0, count).Select(func);

        #region Tuples

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

        public static void ClearNulls<T>(this List<T> list) => list.RemoveAll(x => x.Equals(null));

    }

}