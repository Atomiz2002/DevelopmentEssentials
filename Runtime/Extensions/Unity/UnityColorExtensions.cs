using JetBrains.Annotations;
using UnityEngine;

namespace DevelopmentEssentials.Extensions.Unity {

    public static class UnityColorExtensions {

        // TODO: ExcludeChildren: test and implement this
        // public static bool IsDescendantOf(this Transform descendant, Transform parent) =>
        //     descendant.parent == parent
        //     || descendant.parent.IsDescendantOf(parent) && descendant.parent != descendant.root;
        //
        // public static bool IsParentOf(this Transform potentialParent, Transform potentialChild) =>
        //     potentialParent.Cast<Transform>().Any(child => child == potentialChild);

        /// <returns>#RRGGBB</returns>
        [Pure]
        public static string ToHex(this Color color) => $"#{ColorUtility.ToHtmlStringRGB(color)}";

        /// <returns><see cref="System.Drawing.Color"/></returns>
        [Pure]
        public static System.Drawing.Color ToCsColor(this Color color) =>
            System.Drawing.Color.FromArgb((int) (color.a * 255), (int) (color.r * 255), (int) (color.g * 255), (int) (color.b * 255));

        /// <returns><see cref="System.Drawing.Color"/></returns>
        [Pure]
        public static Color ToUnityColor(this System.Drawing.Color color) => new(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);

        [Pure]
        public static Color A(this Color color, float alpha) => new(color.r, color.g, color.b, alpha);

        [Pure]
        public static Color A(this System.Drawing.Color color, float alpha) => new(color.R, color.G, color.B, alpha);

        [Pure]
        public static Texture ToTexture(this Color color) {
            Texture2D texture = new(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

    }

}