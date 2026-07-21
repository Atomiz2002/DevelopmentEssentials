using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
#if UNITY_EDITOR && !SIMULATE_BUILD
using System.Reflection;
#endif

namespace DevelopmentEssentials.Extensions.CS {

    public static class StringExtensions {

        private static readonly HashSet<string> ReservedKeywords = new() {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
            "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
            "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
            "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
            "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
            "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed",
            "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw",
            "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using",
            "virtual", "void", "volatile", "while"
        };

        /// <returns>Repeats the string <paramref name="n"/> times</returns>
        [Pure]
        public static string Repeat(this string str, int n = 100) => Enumerable.Repeat(str, n).Join(string.Empty);

        [Pure]
        public static string Flatten(this string str, string separator = " | ") => str.Replace("\n", separator);

        [Pure]
        public static string Remove(this string str, string value) => str.Replace(value, string.Empty);

        /// <returns>Repeats the string <paramref name="n"/> times</returns>
        [Pure]
        public static string Regex([NotNull] this string str, [RegexPattern] string pattern, string replacement = "") =>
            System.Text.RegularExpressions.Regex.Replace(str, pattern, replacement);

        [Pure]
        public static string Prepend(this string source, string prefix) => prefix + source;

        [Pure]
        public static string PrependIf(this string source, string defaultValue, Func<string, bool> condition) => source.PrependIf(defaultValue, condition.InvokeSafe(source));

        [Pure]
        public static string PrependIf(this string source, string defaultValue, bool condition) => condition ? defaultValue + source : source;

        [Pure]
        public static string Inlined(this string str, string separator = "") => str.Replace("\n", separator);

        [Pure]
        public static string Concise(this string str) {
            while (str.Contains("  "))
                str = str.Replace("  ", " ");

            return str;
        }

        [Pure]
        public static string FullString(this object obj, string separator = "\n") {
#if UNITY_EDITOR && !SIMULATE_BUILD
            if (obj == null)
                return "null";

            Type        type   = obj.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            string      result = $"{type.Name}:{(separator.Contains("\n") ? "\n" : " ")}"; // name: a, b, c | name:↵a↵b↵c

            for (int i = 0; i < fields.Length; i++) {
                FieldInfo f = fields[i];
                result += $"{f.Name}: {f.GetValue(obj).SafeString()}";

                if (i < fields.Length - 1)
                    result += separator;
            }

            return result;
#else
            return obj?.ToString() ?? "null";
#endif
        }

        [Pure]
        public static string Display(this object obj, string objName, string separator = ": ") => objName + separator + obj.SafeString();

        /// <param name="nullStringsIncluded">Whether to include literal "null" strings in the check</param>
        /// <param name="richTagsIncluded">Whether to include rich text tags in the check like color, b etc.</param>
        [Pure]
        [ContractAnnotation("str:null => true")]
        public static bool IsNullOrEmpty([CanBeNull] this string str, bool nullStringsIncluded = false, bool richTagsIncluded = true) {
            if (richTagsIncluded)
                str = str?.Regex("<.*?>");

            if (string.IsNullOrEmpty(str))
                return true;

            if (nullStringsIncluded)
                return str == "null";

            if (richTagsIncluded)
                return str.StartsWith("<") && str.EndsWith(">");

            return false;
        }

        /// <param name="includingFakeNulls">Whether to include <c>null</c> and <c>"null"</c> strings in the check</param>
        /// <param name="richTagsIncluded">Whether to include rich text tags in the check like color, b etc.</param>
        [Pure]
        [ContractAnnotation("str:null => true")]
        public static bool IsNullOrWhiteSpace([CanBeNull] this string str, bool includingFakeNulls = false, bool richTagsIncluded = true) {
            if (richTagsIncluded)
                str = str?.Regex("<.*?>");

            if (string.IsNullOrWhiteSpace(str))
                return true;

            if (includingFakeNulls)
                return str is "null" or "\"null\"";

            if (richTagsIncluded)
                return str.StartsWith("<") && str.EndsWith(">");

            return false;
        }

        [Pure]
        public static string SafeString(this object source, string defaultValue = "\"null\"") => source?.ToString() ?? defaultValue;

        [Pure] [NotNull]
        public static string OtherIfNull([CanBeNull] this string source, [NotNull] string other = "") => source ?? other;

        /// Stringifies enumerables elements
        [Pure] [ContractAnnotation("source:null,defaultValue:null => true")]
        public static string EnsureString<T>(this T source, string separator = ", ", string defaultValue = "\"null\"") {
            if (source == null)
                return defaultValue;

            return source is IEnumerable enumerable and not string
                ? enumerable.JoinSmart(separator, defaultValue)
                : source.SafeString(defaultValue);
        }

        [Pure]
        public static string RemoveWhitespace(this string source) => new(source.Where(c => !char.IsWhiteSpace(c)).ToArray());

        [Pure]
        public static bool IsValidClassName(this string name) {
            if (name.IsNullOrWhiteSpace())
                return false;

            // Must start with a letter or underscore
            if (!name[0].IsLetter() && name[0] != '_')
                return false;

            // Remaining must be letter, digit, or underscore
            if (name.Any(c => !c.IsLetterOrDigit() && c != '_'))
                return false;

            return !ReservedKeywords.Contains(name);
        }

        [Pure]
        public static string ReplaceSlashes(this string path) => path.Replace("\\", "/");

        [Pure]
        public static string RelativePath(this string fullPath) => fullPath.ReplaceSlashes().Replace(Application.dataPath, "Assets");

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
        public static string StartAt(this string source, string search = "", bool inclusive = true, string prepend = "") {
            if (search == null)
                return source;

            source ??= string.Empty;

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
        public static string StartAtLast(this string source, string search = null, bool inclusive = true, string prepend = "") {
            if (search == null)
                return source;

            source ??= string.Empty;

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
        public static string EndAt(this string source, string search = null, bool inclusive = true, string append = "") {
            if (search == null)
                return source;

            source ??= string.Empty;

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
        public static string EndAtLast(this string source, string search = "", bool inclusive = true, string append = "") {
            if (search == null)
                return source;

            source ??= string.Empty;

            int endIndex = source.LastIndexOf(search);

            if (endIndex == -1)
                return source;

            if (inclusive)
                endIndex += search.Length;

            return source[..endIndex] + append;
        }

        #endregion

        #region char

        [Pure]
        public static bool IsLetter(this char c) => char.IsLetter(c);

        [Pure]
        public static bool IsLetterOrDigit(this char c) => char.IsLetterOrDigit(c);

        #endregion

    }

}