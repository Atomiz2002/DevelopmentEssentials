#if DEVELOPMENT_ESSENTIALS_RUNTIME_ODIN_INSPECTOR
using System;

namespace DevelopmentEssentials.Extensions.Attributes {

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TimestampAttribute : Attribute {

        public readonly Format format;

        public TimestampAttribute(Format format) => this.format = format;

        public enum Format {

            Seconds,
            Milliseconds,
            Ticks

        }

    }

}
#endif