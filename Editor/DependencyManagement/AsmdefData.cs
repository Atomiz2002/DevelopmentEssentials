using System;
using System.Collections.Generic;

namespace DevelopmentEssentials.Editor.DependencyManagement {

    [Serializable]
    internal class AsmdefData {

        public string name;

        // General Options
        public bool   allowUnsafeCode;
        public bool   autoReferenced;
        public bool   noEngineReferences;
        public bool   overrideReferences;
        public string rootNamespace;

        /// ASMDEFs
        public List<string> references;

        /// DLLs
        public List<string> precompiledReferences;

        // Platforms
        public List<string> includePlatforms;
        public List<string> excludePlatforms;

        public List<string>        defineConstraints;
        public List<VersionDefine> versionDefines = new();

        [Serializable]
        public class VersionDefine {

            public string name;
            public string expression;
            public string define;

            private VersionDefine(string name, string expression, string define) {
                this.name       = name;
                this.expression = expression;
                this.define     = define;
            }

            public static VersionDefine Located(string define)                   => new("Unity", string.Empty, define);
            public static VersionDefine Missing(string define, string reference) => new($"MISSING {reference}", string.Empty, define);

            public override string ToString() => $"{define} | {name} | {expression}";

        }

    }

}