using System;
using System.Drawing;

namespace DevelopmentEssentials.Extensions.Unity.ExtendedLogger {

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ColoredLogsAttribute : Attribute {

        public static readonly Color DefaultColor = Color.White;

        public readonly Color Color;

        public ColoredLogsAttribute() => Color = Color.White;
        public ColoredLogsAttribute(byte r, byte g, byte b) => Color = Color.FromArgb(255, r, g, b);
        public ColoredLogsAttribute(string name) => Color = Color.FromName(name);
        public ColoredLogsAttribute(Color color) => Color = color;

    }

}