using System;
using UnityEngine;

namespace DevelopmentEssentials.Extensions.Attributes {

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ColoredAttribute : PropertyAttribute {

        public Color Color;

        public ColoredAttribute(float r, float g, float b) => Color = new(r, g, b);

    }

}