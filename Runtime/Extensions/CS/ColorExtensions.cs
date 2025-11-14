using JetBrains.Annotations;
using Color = System.Drawing.Color;

namespace DevelopmentEssentials.Extensions.CS {

    public static class ColorExtensions {

        [Pure]
        [StringFormatMethod("format")]
        public static string ToHex(this Color color) => $"#{color.R:X2}{color.G:X2}{color.B:X2}";

        [Pure] public static Color SetAlpha(this Color color, int alpha) => Color.FromArgb(alpha, color);
        [Pure] public static Color SetRed(this Color color, int red)     => Color.FromArgb(red, color.G, color.B);
        [Pure] public static Color SetGreen(this Color color, int green) => Color.FromArgb(color.R, green, color.B);
        [Pure] public static Color SetBlue(this Color color, int blue)   => Color.FromArgb(color.R, color.G, blue);

    }

}