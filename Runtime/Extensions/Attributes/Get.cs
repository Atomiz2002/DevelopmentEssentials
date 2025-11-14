using System;
using System.Reflection;
using UnityEngine;
using static System.Attribute;

namespace DevelopmentEssentials.Extensions.Attributes {

    public static class Get {

        public static Color Color(object type, string name) => FieldAttribute<ColoredAttribute>(type, name).Color;

        public static T FieldAttribute<T>(object type, string name) where T : Attribute {
            MemberInfo member = type.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);

            if (member == null || GetCustomAttribute(member, typeof(T)) is not T attribute)
                return null;

            return attribute;
        }

        public static T PropertyAttribute<T>(object type, string name) where T : Attribute {
            MemberInfo member = type.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic);

            if (member == null || GetCustomAttribute(member, typeof(T)) is not T attribute)
                return null;

            return attribute;
        }

    }

}