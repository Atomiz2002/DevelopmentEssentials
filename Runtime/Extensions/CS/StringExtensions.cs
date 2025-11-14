using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace DevelopmentEssentials.Extensions.CS {

    public static class StringExtensions {

        /// <returns>Repeats the string <paramref name="n"/> times</returns>
        [Pure]
        public static string Repeat(this string str, int n = 100) => Enumerable.Repeat(str, n).Join(string.Empty);

        [Pure]
        public static string Flatten(this string str, string separator = " | ") => str.Replace("\n", separator);

        [Pure]
        public static string Remove(this string str, string value) => str.Replace(value, string.Empty);

        #region Starts/Ends|With

        [Pure]
        public static string PrependIf(this string source, string defaultValue, Func<string, bool> condition) => condition(source) ? defaultValue + source : source;

        public static string Inlined(this string str, string separator = "") => str.Replace("\n", separator);

        public static string Concise(this string str) => str.Replace("  ", " ");

        #endregion

        /// <param name="nullStringsIncluded">Whether to include literal "null" strings in the check</param>
        /// <param name="richTagsIncluded">Whether to include rich text tags in the check like color, b etc.</param>
        [Pure]
        public static bool IsNullOrEmpty(this string str, bool nullStringsIncluded = false, bool richTagsIncluded = true) {
            if (richTagsIncluded)
                str = Regex.Replace(str.OtherIfNull(), "<.*?>", string.Empty);

            if (string.IsNullOrEmpty(str))
                return true;

            if (nullStringsIncluded)
                return str == "null";

            if (richTagsIncluded)
                return str.StartsWith("<") && str.EndsWith(">");

            return false;
        }

        /// <param name="nullStringsIncluded">Whether to include <c>null</c> and <c>"null"</c> strings in the check</param>
        /// <param name="richTagsIncluded">Whether to include rich text tags in the check like color, b etc.</param>
        [Pure]
        public static bool IsNullOrWhiteSpace(this string str, bool nullStringsIncluded = false, bool richTagsIncluded = true) {
            if (richTagsIncluded)
                str = Regex.Replace(str.OtherIfNull(), "<.*?>", string.Empty);

            if (string.IsNullOrWhiteSpace(str))
                return true;

            if (nullStringsIncluded)
                return str is "null" or "\"null\"";

            if (richTagsIncluded)
                return str.StartsWith("<") && str.EndsWith(">");

            return false;
        }

        [Pure]
        public static string SafeString(this object source, string defaultValue = "\"null\"") => source?.ToString() ?? defaultValue;

        [Pure] [NotNull]
        public static string OtherIfNull([CanBeNull] this string source, [NotNull] string other = "") => source ?? other;

        /// Converts enumerables into strings by calling ToString on each element<br/>
        /// <see cref="SafeString"/> does not call ToString on the elements
        [Pure]
        public static string EnsureString<T>(this T source, string separator = ", ", string defaultValue = "\"null\"") {
            if (source == null)
                return defaultValue;

            return source is IEnumerable enumerable and not string
                ? enumerable.Cast<object>().Join(separator, defaultValue)
                : source.SafeString(defaultValue);
        }

        #region Starts/Ends|With

        /// <example>
        /// <code>
        /// string example = "C:/UnityProject/Assets/TextureAssets/Player.png.meta"
        /// example.StartAt()                         //   "Assets/Player.png.meta"
        /// example.StartAt("Assets", false)          //         "/Player.png.meta"
        /// example.StartAt("Assets", true,  "-")      // "-Assets/Player.png.meta"
        /// example.StartAt("Assets", false, "-")           //  "-/Player.png.meta"
        /// </code>
        /// </example>
        /// <param name="inclusive">Whether to include the specified <paramref name="search"/> in the result</param>
        /// <param name="prepend">The string to prepend if <paramref name="search"/> is found</param>
        /// <returns>Everything after the last index of <paramref name="search"/> if found, otherwise <paramref name="source"/></returns>
        /// <seealso cref="StartAtLast"/>
        /// <seealso cref="EndAt"/>
        /// <seealso cref="EndAtLast"/>
        [Pure]
        public static string StartAt([NotNull] this string source, string search = "", bool inclusive = true, string prepend = "") {
            search ??= string.Empty;
            int startIndex = source.IndexOf(search);

            if (startIndex == -1)
                return source;

            if (!inclusive)
                startIndex += search.Length;

            return prepend + source[startIndex..];
        }

        /// <example>
        /// <code>
        /// string example = "C:/UnityProject/Assets/TextureAssets/Player.png.meta"
        /// example.StartAtLast()                      //  "Assets/Player.png.meta"
        /// example.StartAtLast("Assets", false)       //        "/Player.png.meta"
        /// example.StartAtLast("Assets", true,  "-")  // "-Assets/Player.png.meta"
        /// example.StartAtLast("Assets", false, "-")  //       "-/Player.png.meta"
        /// </code>
        /// </example>
        /// <param name="inclusive">Whether to include the specified <paramref name="search"/> in the result</param>
        /// <param name="prepend">The string to prepend if <paramref name="search"/> is found</param>
        /// <returns>Everything after the last index of <paramref name="search"/> if found, otherwise <paramref name="source"/></returns>
        /// <seealso cref="StartAt"/>
        /// <seealso cref="EndAt"/>
        /// <seealso cref="EndAtLast"/>
        [Pure]
        public static string StartAtLast([NotNull] this string source, string search = "", bool inclusive = true, string prepend = "") {
            search ??= string.Empty;
            int startIndex = source.LastIndexOf(search);

            if (startIndex == -1)
                return source;

            if (!inclusive)
                startIndex += search.Length;

            return prepend + source[startIndex..];
        }

        /// <example>
        /// <code>
        /// string example =                       "C:/UnityProject/Assets/TextureAssets/Player.png.meta"
        /// example.EndAt()                     // "C:/UnityProject/Assets"
        /// example.EndAt("Assets", false)      // "C:/UnityProject/"
        /// example.EndAt("Assets", true,  "-") // "C:/UnityProject/Assets-"
        /// example.EndAt("Assets", false, "-") // "C:/UnityProject/-"
        /// </code>
        /// </example>
        /// <param name="inclusive">Whether to include the specified <paramref name="search"/> in the result</param>
        /// <param name="append">The string to append if <paramref name="search"/> is found</param>
        /// <returns>Everything before the last index of <paramref name="search"/> if found, otherwise <paramref name="source"/></returns>
        /// <seealso cref="StartAt"/>
        /// <seealso cref="StartAtLast"/>
        /// <seealso cref="EndAtLast"/>
        [Pure]
        public static string EndAt([NotNull] this string source, string search = "", bool inclusive = true, string append = "") {
            search ??= string.Empty;
            int endIndex = source.IndexOf(search);

            if (endIndex == -1)
                return source;

            if (inclusive)
                endIndex += search.Length;

            return source[..endIndex] + append;
        }

        /// <example>
        /// <code>
        /// string example =                           "C:/UnityProject/Assets/TextureAssets/Player.png.meta"
        /// example.EndAtLast()                     // "C:/UnityProject/Assets/TextureAssets"
        /// example.EndAtLast("Assets", false)      // "C:/UnityProject/Assets/Texture"
        /// example.EndAtLast("Assets", true,  "-") // "C:/UnityProject/Assets/TextureAssets-"
        /// example.EndAtLast("Assets", false, "-") // "C:/UnityProject/Assets/Texture-"
        /// </code>
        /// </example>
        /// <param name="inclusive">Whether to include the specified <paramref name="search"/> in the result</param>
        /// <param name="append">The string to append if <paramref name="search"/> is found</param>
        /// <returns>Everything before the last index of <paramref name="search"/> if found, otherwise <paramref name="source"/></returns>
        /// <seealso cref="StartAt"/>
        /// <seealso cref="StartAtLast"/>
        /// <seealso cref="EndAt"/>
        [Pure]
        public static string EndAtLast([NotNull] this string source, string search = "", bool inclusive = true, string append = "") {
            search ??= string.Empty;
            int endIndex = source.LastIndexOf(search);

            if (endIndex == -1)
                return source;

            if (inclusive)
                endIndex += search.Length;

            return source[..endIndex] + append;
        }

        #endregion

    }

}