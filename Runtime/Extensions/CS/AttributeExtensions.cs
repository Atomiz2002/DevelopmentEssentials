using System;
using System.Reflection;

namespace DevelopmentEssentials.Extensions.CS {

    public static class AttributeExtensions {

        // @formatter:off
        public static TAttribute Get<TAttribute>(this Type classType)              where TAttribute : Attribute => (TAttribute) Attribute.GetCustomAttribute(classType,     typeof(TAttribute));
        public static TAttribute Get<TAttribute>(this MethodBase methodBase)       where TAttribute : Attribute => (TAttribute) Attribute.GetCustomAttribute(methodBase,    typeof(TAttribute));
        public static TAttribute Get<TAttribute>(this MethodInfo methodInfo)       where TAttribute : Attribute => (TAttribute) Attribute.GetCustomAttribute(methodInfo,    typeof(TAttribute));
        public static TAttribute Get<TAttribute>(this MemberInfo memberInfo)       where TAttribute : Attribute => (TAttribute) Attribute.GetCustomAttribute(memberInfo,    typeof(TAttribute));
        public static TAttribute Get<TAttribute>(this Assembly assembly)           where TAttribute : Attribute => (TAttribute) Attribute.GetCustomAttribute(assembly,      typeof(TAttribute));
        public static TAttribute Get<TAttribute>(this Module module)               where TAttribute : Attribute => (TAttribute) Attribute.GetCustomAttribute(module,        typeof(TAttribute));
        public static TAttribute Get<TAttribute>(this ParameterInfo parameterInfo) where TAttribute : Attribute => (TAttribute) Attribute.GetCustomAttribute(parameterInfo, typeof(TAttribute));

        public static bool TryGet<TAttribute>(this Type classType, out TAttribute attribute)              where TAttribute : Attribute => (attribute = Attribute.IsDefined(classType,     typeof(TAttribute)) ? classType.Get<TAttribute>() : null) != null;
        public static bool TryGet<TAttribute>(this MethodBase methodBase, out TAttribute attribute)       where TAttribute : Attribute => (attribute = Attribute.IsDefined(methodBase,    typeof(TAttribute)) ? methodBase.Get<TAttribute>() : null) != null;
        public static bool TryGet<TAttribute>(this MethodInfo methodInfo, out TAttribute attribute)       where TAttribute : Attribute => (attribute = Attribute.IsDefined(methodInfo,    typeof(TAttribute)) ? methodInfo.Get<TAttribute>() : null) != null;
        public static bool TryGet<TAttribute>(this MemberInfo memberInfo, out TAttribute attribute)       where TAttribute : Attribute => (attribute = Attribute.IsDefined(memberInfo,    typeof(TAttribute)) ? memberInfo.Get<TAttribute>() : null) != null;
        public static bool TryGet<TAttribute>(this Assembly assembly, out TAttribute attribute)           where TAttribute : Attribute => (attribute = Attribute.IsDefined(assembly,      typeof(TAttribute)) ? assembly.Get<TAttribute>() : null) != null;
        public static bool TryGet<TAttribute>(this Module module, out TAttribute attribute)               where TAttribute : Attribute => (attribute = Attribute.IsDefined(module,        typeof(TAttribute)) ? module.Get<TAttribute>() : null) != null;
        public static bool TryGet<TAttribute>(this ParameterInfo parameterInfo, out TAttribute attribute) where TAttribute : Attribute => (attribute = Attribute.IsDefined(parameterInfo, typeof(TAttribute)) ? parameterInfo.Get<TAttribute>() : null) != null;
        // @formatter:on

    }

}