using System;
using DevelopmentEssentials.Extensions.Unity.ExtendedLogger;

namespace DevelopmentEssentials.Extensions.CS {

    public delegate void Out<T>(out T value);

    public static class DelegateExtensions {

        public static void InvokeSafe<T>(this Out<T> d, out T value) {
            try {
                d.Invoke(out value);
            }
            catch (Exception e) {
                value = default;
                e.LogEx();
            }
        }

    }

}