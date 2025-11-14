using System.Text.RegularExpressions;
using DevelopmentEssentials.Extensions.CS;
using JetBrains.Annotations;
using UnityEngine;

namespace DevelopmentEssentials.Extensions.Unity {

    public static class RichTextExtensions {

        /// <returns>&lt;color=<see cref="ToHex"/>&gt;str&lt;/color&gt;</returns>
        [Pure]
        public static string Colored(this string source, Color color) => source.Tag("color", $"={color.ToHex()}");

        [Pure]
        public static string Colored(this string source, System.Drawing.Color color) => source.Tag("color", $"={color.ToHex()}");

        [Pure]
        public static string Recolored(this string source, Color color) {
#if UNITY_EDITOR
            return source.Decolored().Colored(color);
#endif
            return source;
        }

        [Pure]
        public static string Recolored(this string source, System.Drawing.Color color) {
#if UNITY_EDITOR
            return source.Decolored().Colored(color);
#endif
            return source;
        }

        [Pure]
        public static string Link([NotNull] this string source, string path = null, string line = "0") =>
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
#endif
            return source;
        }

        [Pure]
        public static string Decolored(this string source) {
#if UNITY_EDITOR
            return Regex.Replace(Regex.Replace(source, "<color=.*?>", string.Empty), "</color>", string.Empty);
#endif
            return source;
        }

        [Pure]
        public static string Unformatted(this string source) {
#if UNITY_EDITOR
            return Regex.Replace(source, "<.*?>", string.Empty);
#else
        return source;
#endif
        }

    }

}