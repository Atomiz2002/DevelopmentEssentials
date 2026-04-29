using System;
using UnityEngine;

namespace DevelopmentEssentials.Attributes {

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class RangeSlider : PropertyAttribute {}

}