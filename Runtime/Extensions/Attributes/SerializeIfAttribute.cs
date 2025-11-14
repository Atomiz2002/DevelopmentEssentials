using System;
using UnityEngine;

namespace DevelopmentEssentials.Extensions.Attributes {

    public abstract class SerializeIfAttributes {

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
        public class SerializeIfBoolAttribute : PropertyAttribute {

            public string ConditionFieldName { get; }
            public bool   Value              { get; }

            public SerializeIfBoolAttribute(string conditionFieldName, bool value = true) {
                ConditionFieldName = conditionFieldName;
                Value              = value;
            }

        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
        public class SerializeIfEnumAttribute : PropertyAttribute {

            public string ConditionFieldName { get; }
            public int    Value              { get; }

            public SerializeIfEnumAttribute(string conditionFieldName, int value) {
                ConditionFieldName = conditionFieldName;
                Value              = value;
            }

        }

    }

}