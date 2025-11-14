using System;
using UnityEngine;

namespace DevelopmentEssentials.Extensions.Attributes {

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class RangeSlider : PropertyAttribute {}

}