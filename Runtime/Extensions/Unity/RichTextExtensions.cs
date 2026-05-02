using System.IO;
using System.Text.RegularExpressions;
using DevelopmentEssentials.Extensions.CS;
using JetBrains.Annotations;
using UnityEngine;

namespace DevelopmentEssentials.Extensions.Unity {

    public static class RichTextExtensions {

        /// <returns>&lt;color=<see cref="ToHex"/>&gt;str&lt;/color&gt;</returns>
        [Pure]
        public static string Colored(this string source, Color color, bool condition = true) => condition ? source.Tag("color", $"={color.ToHex()}") : source;

        [Pure]
        public static string Colored(this string source, System.Drawing.Color color, bool condition = true) => condition ? source.Tag("color", $"={color.ToHex()}") : source;

        /// <returns>&lt;color=<see cref="ToHex"/>&gt;str&lt;/color&gt;</returns>
        [Pure]
        public static string Colored(this string source, Color? color) => color.HasValue ? source.Tag("color", $"={color.Value.ToHex()}") : source;

        [Pure]
        public static string Colored(this string source, System.Drawing.Color? color) => color.HasValue ? source.Tag("color", $"={color.Value.ToHex()}") : source;

        [Pure]
        public static string Recolored(this string source, Color color) {
#if UNITY_EDITOR // why?
            return source.Decolored().Colored(color);
#else
            return source;
#endif
        }

        [Pure]
        public static string Recolored(this string source, System.Drawing.Color color) {
#if UNITY_EDITOR // why?
            return source.Decolored().Colored(color);
#else
            return source;
#endif
        }

        [Pure]
        public static string Link([NotNull] this string source, string path, string line) =>
            source.Link(path, int.TryParse(line, out int ln) ? ln : 0);

        [Pure]
        public static string Link([NotNull] this string source, string path = null, int line = 0) =>
            path != null
                ? source.Tag("a", $" href=\"{path}\" line=\"{line}\"")
                : source.Link(source, line);

        [Pure]
        public static string Size(this string source, int size) => source.Tag("size", $"={size}");

        [Pure]
        public static string Bold(this string source, bool condition = true) => condition ? source.Tag("b") : source;

        /// <returns>&lt;<paramref name="tag"/><paramref name="setting"/>&gt;str&lt;/<paramref name="tag"/>&gt;</returns>
        [Pure]
        private static string Tag(this string source, string tag, string setting = null) {
#if UNITY_EDITOR
            return source.IsNullOrWhiteSpace()
                ? source
                : $"<{tag}{setting.OtherIfNull()}>{source.Replace("\n", $"</{tag}>\n<{tag}{setting.OtherIfNull()}>")}</{tag}>";
#else
            return source;
#endif
        }

        [Pure]
        public static string Decolored(this string source) {
#if UNITY_EDITOR
            return Regex.Replace(Regex.Replace(source, "<color=.*?>", string.Empty), "</color>", string.Empty);
#else
            return source;
#endif
        }

        [Pure]
        public static string Unformatted(this string source) {
#if UNITY_EDITOR
            return Regex.Replace(source, "<.*?>", string.Empty);
#else
        return source;
#endif
        }

        /// Formats the <paramref name="source"/> by replacing all locally accessible paths with hyperlinks
        [Pure]
        public static string LinkPaths(this string source) {
            if (string.IsNullOrEmpty(source))
                return source;

            foreach (Match m in Regex.Matches(source, @" in (file:line:column )?(\w.*?):(\d+)(:\d+)?")) {
                try {
                    string fullMatch = m.Value;
                    string path      = m.Groups[2].Value.Trim();
                    string name      = Path.GetFileName(path); // can throw for invalid chars
                    string line      = m.Groups[3].Value;
                    string link      = name.Link(path, line);
                    string replaced  = fullMatch.Replace(name, link);
                    source = source.Replace(fullMatch, replaced);
                }
                catch {}
            }

            return source;
        }

    }

}