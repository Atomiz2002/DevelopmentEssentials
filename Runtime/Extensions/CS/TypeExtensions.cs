using System;
using System.Linq;
using JetBrains.Annotations;

namespace DevelopmentEssentials.Extensions.CS {

    public static class TypeExtensions {

        [Pure]
        public static string Name(this Type type) =>
            type.IsGenericType
                ? $"{type.Name[..type.Name.IndexOf('`')]}<{type.GetGenericArguments().Select(Name).Join()}>"
                : type.Name;

        public static bool Inherits<TChild, TParent>(this TChild childClass, out TParent parentClass) where TParent : class {
            if (typeof(TParent).IsAssignableFrom(childClass.GetType())) {
                parentClass = childClass as TParent;
                return true;
            }

            parentClass = null;
            return false;
        }

    }

}