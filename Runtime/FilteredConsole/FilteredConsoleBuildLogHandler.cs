using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace DevelopmentEssentials.FilteredConsole {

    public class FilteredConsoleBuildLogHandler : IPreprocessBuildWithReport, IPostprocessBuildWithReport {

        private bool wasEnabled;

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report) => FilteredConsoleLogger.ToggleLogs(true, out wasEnabled, "Build started");

        public void OnPostprocessBuild(BuildReport report) => FilteredConsoleLogger.ToggleLogs(wasEnabled, "Build ended");

    }

}