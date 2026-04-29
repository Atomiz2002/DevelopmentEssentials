using System;
using System.Linq;
using DevelopmentEssentials.Extensions.CS;
using DevelopmentEssentials.Extensions.Unity;
using UnityEditor;
using UnityEngine;
using Color = System.Drawing.Color;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
#if ONLY_EXCEPTIONS
using System.Diagnostics;
#endif

#if UNITY_EDITOR
namespace DevelopmentEssentials {

    [InitializeOnLoad]
#endif
    public static class FilteredConsoleLogger {

        private static bool        usingOriginalLogger;
        private static ILogHandler originalLogHandler;
        private static ILogHandler filteredLogHandler;

        static FilteredConsoleLogger() => Initialize();

        [HideInCallstack]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize() {
            usingOriginalLogger = true;
            originalLogHandler  = Debug.unityLogger.logHandler;
            filteredLogHandler  = new FilteredLogger(Debug.unityLogger.logHandler);
#if ENABLE_LOGS
            ToggleLogs(true, "ENABLE_LOGS");
#elif UNITY_EDITOR
#if SIMULATE_BUILD
        ToggleLogs(false, "!ENABLE_LOGS && SIMULATE_BUILD");
#else
        ToggleLogs(true, "UNITY_EDITOR");
#endif
#else
        ToggleLogs(false, "SIMULATE_BUILD");
#endif
            ToggleLogger(false);
        }

        /// <inheritdoc cref="ToggleLogs(bool, out bool, string)"/>
        [HideInCallstack]
        public static void ToggleLogs(bool enabled, string reason = "") => ToggleLogs(enabled, out _, reason);

        /// <param name="reason">If not empty - logs "Logs enabled/disabled ({reason})"</param>
        [HideInCallstack]
        public static void ToggleLogs(bool enabled, out bool wasEnabled, string reason = "") {
            wasEnabled = Debug.unityLogger.logEnabled;
            bool notify = !reason.IsNullOrWhiteSpace();

            if (enabled == wasEnabled && !notify)
                return;

            if (enabled) {
                Debug.unityLogger.logEnabled = true;
                if (notify)
                    Debug.Log($"Logs enabled ({reason})".Colored(Color.GreenYellow));
            }
            else {
                if (notify)
                    Debug.Log($"Logs disabled ({reason})".Colored(Color.Red));

                Debug.unityLogger.logEnabled = false;
            }
        }

        /// <inheritdoc cref="ToggleLogs(bool, out bool, string)"/>
        [HideInCallstack]
        public static void ToggleLogger(bool useOriginal, string reason = "") => ToggleLogger(useOriginal, out _, reason);

        /// <inheritdoc cref="ToggleLogs(bool, out bool, string)"/>
        [HideInCallstack]
        public static void ToggleLogger(bool useOriginal, out bool wasOriginal, string reason = "") {
            wasOriginal = usingOriginalLogger;
            bool notify = !reason.IsNullOrWhiteSpace();

            if (useOriginal == wasOriginal && !notify)
                return;

            if (useOriginal) {
                Debug.unityLogger.logHandler = originalLogHandler;
                if (notify)
                    Debug.Log($"Using original logger ({reason})".Colored(Color.GreenYellow));
            }
            else {
                Debug.unityLogger.logHandler = filteredLogHandler;

                if (notify)
#if ONLY_EXCEPTIONS
                Debug.Log($"Using filtered logger [ONLY_EXCEPTIONS] ({reason})".Colored(Color.Red));
#else
                    Debug.Log($"Using filtered logger ({reason})".Colored(Color.DarkOrange));
#endif
            }
        }

        private class FilteredLogger : ILogHandler {

#if UNITY_EDITOR && !SIMULATE_BUILD
            private EditorWindow consoleWindow;
#endif
            private readonly ILogHandler originalLogHandler;

            private static readonly string[] blacklist = {
                "GPGSUpgrader",
                "Disposing UnityDB",
                "Compiling...",
                "Compilation complete.",
                "Manifest aliases:",
                "Flattened manifest aliases:",
                "Add manifests to package 'External Dependency Manager':",
                "Add manifests to package 'GooglePlayGamesPlugin':",
                "ResourceManager initialized with ",
                @"Parsing manifest 'Assets\ExternalDependencyManager\Editor\external-dependency-manager_version-",
                "'External Dependency Manager' Manifest:",
                @"Parsing manifest 'Assets\GooglePlayGames\com.google.play.games\Editor\GooglePlayGamesPlugin_v",
                "'GooglePlayGamesPlugin' Manifest:",
                "[Apple.Core Plug-In] Initializing API Availability Checking",
                "Auto baking Assets/",
                "Platform Info: Architecture: x",
                "Verifying Memory Integrity Using: QuantumUnityMemoryLayoutVerifierPlatform",
                "Memory Integrity Verified",
                "Local Players: ",
                "[FMOD] Please add an 'FMOD Studio Listener' component to your camera in the scene for correct 3D positioning of sounds.",
                "Max Tweens reached: capacity has automatically been increased from",
            };

            private static readonly string[] blacklistError = {
                "Unable to perform online search",
                "apkanalyzer failed to estimate the apk size",
                "Some objects were not cleaned up when closing the scene. (Did you spawn new GameObjects from OnDestroy?)",
                "Sentry: (Error) sentry-cli: error: Project not found. Ensure that you configured the correct project and organization.",
                "Missing shader. PostProcessing render passes will not execute. Check for missing reference in the renderer resources.",
            };

            private static readonly string[] blacklistException = {
                "AppleCoreNativeMac assembly",
                "The given key 'Steam' was not present in the dictionary.",
                "Unity Remote requirements check failed",
                "(0,0): Burst error BC1091: External and internal calls are not allowed inside static constructors: Interop.BCrypt.BCryptGenRandom(System.IntPtr hAlgorithm, byte* pbBuffer, int cbBuffer, int dwFlags)",
                "UnityException: get_realtimeSinceStartup is not allowed to be called during serialization, call it from OnEnable instead. Called from ScriptableObject 'DebugLogger'.",
            };

            public FilteredLogger(ILogHandler original) => originalLogHandler = original;

            [HideInCallstack]
            public void LogFormat(LogType logType, Object context, string format, params object[] args) {
                string message = string.Format(format, args);

#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
            Sentry.SentrySdk.AddBreadcrumb(message.Unformatted(), level: logType switch {
                LogType.Error   => Sentry.BreadcrumbLevel.Error,
                LogType.Assert  => Sentry.BreadcrumbLevel.Debug,
                LogType.Warning => Sentry.BreadcrumbLevel.Warning,
                _               => Sentry.BreadcrumbLevel.Info
            });
#endif

#if ONLY_EXCEPTIONS
            string stackTrace = new StackTrace().SafeString();

            if (logType != LogType.Error && !stackTrace.Contains(nameof(ExtendedLogger)) && !stackTrace.Contains(".P[T] (") && !stackTrace.Contains($"{nameof(ExtendedLogger)}.L"))
                return;
#endif

                switch (logType) {
                    case LogType.Log when blacklist.Any(message.Contains):
                    case LogType.Error when blacklistError.Any(message.Contains):
                        return;
                    default:
                        originalLogHandler.LogFormat(logType, context, format, args);
                        break;
                }
            }

            [HideInCallstack]
            public void LogException(Exception exception, Object context) {
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD // TODO test added sentry tags
            exception.Data["username"] = StateSystem.I.stateRemote.profile.username;
            exception.Data["email"] = StateSystem.I.stateLocal.email;
            Sentry.SentrySdk.CaptureException(exception);
#endif

#if !ENABLE_LOGS && !(UNITY_EDITOR && !SIMULATE_BUILD)
            return;
#endif

                if (blacklistException.Any(str => exception.Message.Contains(str)))
                    return;

#if UNITY_EDITOR && !SIMULATE_BUILD
                // if (Application.isPlaying && EngineSettings.FocusExceptions) {
                //     if (!consoleWindow)
                //         consoleWindow = EngineSettings.TryFocusWindow("Console");
                //
                //     if (!consoleWindow)
                //         EditorApplication.ExecuteMenuItem("Window/General/Console");
                // }
#endif

                // string message = exception.Message.Unformatted();
                // originalLogHandler.LogException(new(message, exception), context);

                originalLogHandler.LogException(exception, context);

#if UNITY_EDITOR && !SIMULATE_BUILD
                // Exception ex = exception;

                // while (ex.InnerException != null) {
                //     string message = $"  ↳{ex.Message.Unformatted()}";
                //     originalLogHandler.LogException(new _(message), context);
                //     ex = ex.InnerException;
                // }
#endif
            }

            private class _ : Exception {

                public _(string message) : base(message) {}

            }

        }

    }

}