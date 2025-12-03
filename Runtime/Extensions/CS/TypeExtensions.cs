using System;
using System.Drawing;
using System.Linq;
using DevelopmentEssentials.Extensions.Unity;
using JetBrains.Annotations;

namespace DevelopmentEssentials.Extensions.CS {

    public static class TypeExtensions {

        [Pure]
        public static string BetterName(this Type type, Color? color = null) =>
            type.IsGenericType
                ? $"{type.Name[..type.Name.IndexOfOr('`', type.Name.Length)]}<{type.GetGenericArguments().Select(x => x.BetterName(color)).Join().Colored(color)}>"
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