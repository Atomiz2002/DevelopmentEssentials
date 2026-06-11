#pragma warning disable CS0168 // Variable is declared but never used
using System;
using System.Globalization;
using System.Reflection;

namespace DevelopmentEssentials.Extensions.CS {

    public static class MethodInfoExtensions {

        public static void SafeInvoke(this MethodInfo methodInfo, object obj, object[] parameters) {
            try {
                methodInfo?.Invoke(obj, parameters);
            }
            catch (Exception e) {
#if ENABLE_LOGS
                e.LogEx();
#endif
            }
        }

        public static void SafeInvoke(this MethodInfo methodInfo, object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) {
            try {
                methodInfo?.Invoke(obj, invokeAttr, binder, parameters, culture);
            }
            catch (Exception e) {
#if ENABLE_LOGS
                e.LogEx();
#endif
            }
        }

    }

}