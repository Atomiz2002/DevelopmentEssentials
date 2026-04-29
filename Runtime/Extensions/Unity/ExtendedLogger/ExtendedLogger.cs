// #define EXTENDED_LOGGER_REQUIRES_ATTRIBUTE
// #define CUSTOM_LOGGER_PRINT_CALLER_ON_EMPTY_LOG
// #define CUSTOM_LOGGER_PRINT_CALLER
// #define UNFORMATTED_LOGS

#if UNITY_EDITOR && !SIMULATE_BUILD || ENABLE_LOGS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DevelopmentEssentials.Extensions.CS;
using JetBrains.Annotations;
using UnityEngine;
using Color = System.Drawing.Color;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace DevelopmentEssentials.Extensions.Unity.ExtendedLogger {

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
#endif

    public static class ExtendedLogger {

#if UNITY_EDITOR && !SIMULATE_BUILD || ENABLE_LOGS
        private static readonly Dictionary<string, List<object>> logQueue = new();

        #region Log Info, Error, Exception

        /// Print instantly and return the value for further use.
        /// <param name="formattable">Used as a prefix unless "{0}" is present in which case <paramref name="t"/> is formatted into it.</param>
#if EXTENDED_LOGGER_REQUIRES_ATTRIBUTE
    /// <br/>Requires <see cref="EnableLogsAttribute"/> on the containing class.
#endif
        [StringFormatMethod("formattable")]
        [HideInCallstack]
        public static T Log<T>(this T t, object formattable = null, LogType logType = LogType.Log) => t.LogInternal(logType, true, formattable);

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType)"/>
        [StringFormatMethod("formattable")]
        [HideInCallstack]
        public static T LogError<T>(this T t, object formattable = null) => t.LogInternal(LogType.Error, true, formattable);

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType)"/>
        [StringFormatMethod("formattable")]
        [HideInCallstack]
        public static T LogErr<T>(this T t, object formattable = null) => t.LogInternal(LogType.Error, true, formattable);

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType)"/>
        [StringFormatMethod("formattable")]
        [HideInCallstack]
        public static T LogException<T>(this T t, object formattable = null) => t.LogInternal(LogType.Exception, true, formattable);

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType)"/>
        [StringFormatMethod("formattable")]
        [HideInCallstack]
        public static T LogEx<T>(this T t, object formattable = null) => t.LogInternal(LogType.Exception, true, formattable);

        #endregion

        #region Log Priority

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType)"/>
        /// <br/>Prints ignoring the enabled state of the logger.
        [StringFormatMethod("formattable")]
        [HideInCallstack]
        public static T LOG<T>(this T t, object formattable = null) {
            FilteredConsoleLogger.ToggleLogs(true, out bool wasEnabled);
            t.LogInternal(LogType.Log, true, formattable); // Do we need exception variant?
            FilteredConsoleLogger.ToggleLogs(wasEnabled);
            return t;
        }

        // private static readonly HashSet<string> logOnce = new();
        //
        // /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType)"/>
        // /// <br/>Prints ignoring the enabled state of the logger.
        // [StringFormatMethod("formattable")]
        // [HideInCallstack]
        // public static T LOG_ONCE<T>(this T t, object formattable = null) {
        //     if (!logOnce.Add(GetStackFrames()[^1].EnsureString().LOG()))
        //         return t;
        //
        //     FilteredConsoleLogger.ToggleLogs(true, out bool wasEnabled, false);
        //     t.LogInternal(LogType.Log, true, formattable); // Do we need exception variant?
        //     FilteredConsoleLogger.ToggleLogs(wasEnabled, false);
        //     return t;
        // }

        #endregion

        #region Log Modifiers

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType)"/>
        [HideInCallstack]
        public static T1 LogAs<T1, T2>(this T1 t, Func<T1, T2> func, object formattable = null, LogType logType = LogType.Log) {
            if (t == null)
                return t.LogInternal(logType, true, formattable);

            func(t).LogInternal(logType, true, formattable);
            return t;
        }

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType)"/>
        [HideInCallstack]
        public static bool LogIf(this bool t, object formattable = null, string ifCalledThrough = "", LogType logType = LogType.Log) =>
            t.LogIf(formattable, _ => t, ifCalledThrough, logType);

        // /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType,string)"/>
        // [HideInCallstack]
        // public static T LogIf<T>(this T t, bool condition, string ifCalledThrough = "", LogType logType = LogType.Log) =>
        //     t.LogIf(null, _ => condition, ifCalledThrough, logType);

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType)"/>
        [HideInCallstack]
        public static T LogIf<T>(this T t, object formattable, bool condition, string ifCalledThrough = "", LogType logType = LogType.Log) =>
            t.LogIf(formattable, _ => condition, ifCalledThrough, logType);

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType,string)"/>
        [HideInCallstack]
        public static T LogIf<T>(this T t, bool condition, object formattable = null, string ifCalledThrough = "", LogType logType = LogType.Log) =>
            t.LogIf(formattable, _ => condition, ifCalledThrough, logType);

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType)"/>
        [HideInCallstack]
        public static T LogIf<T>(this T t, Func<T, bool> condition, object formattable = null, string ifCalledThrough = "", LogType logType = LogType.Log) =>
            t.LogIf(formattable, condition, ifCalledThrough, logType);

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType)"/>
        [HideInCallstack]
        public static T LogIf<T>(this T t, object formattable, Func<T, bool> condition, string ifCalledThrough = "", LogType logType = LogType.Log) {
            condition ??= _ => t is not bool b || b;
            StackFrame[] frames = GetStackFrames();

            if (condition(t) && (ifCalledThrough.IsNullOrEmpty() || frames.Any(f => (f.GetMethod().Name + f.GetMethod().DeclaringType?.Name).Contains(ifCalledThrough))))
                t.LogInternal(logType, true, formattable);

            return t;
        }

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType)"/>
        [HideInCallstack]
        public static T1 LogIfAs<T1, T2>(this T1 t, Func<T1, bool> condition, Func<T1, T2> func, LogType logType = LogType.Log) {
            if (t == null)
                return t.Log(null, logType);

            if (condition(t))
                func(t).LogInternal(logType, true, null);

            return t;
        }

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType)"/>
        [HideInCallstack]
        public static T1 LogIfAs<T1, T2>(this T1 t, object formattable, Func<T1, bool> condition, Func<T1, T2> func, LogType logType = LogType.Log) {
            if (t == null)
                return t.Log(formattable, logType);

            if (condition(t))
                func(t).LogInternal(logType, true, formattable);

            return t;
        }

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType)"/>
        [HideInCallstack]
        public static IEnumerable<T> LogAll<T>(this IEnumerable<T> t, object formattable = null, LogType logType = LogType.Log) =>
            t.LogAll<T, string>(null, formattable, logType);

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType)"/>
        [HideInCallstack]
        public static IEnumerable<T1> LogAll<T1, T2>(this IEnumerable<T1> t, Func<T1, T2> func = null, object formattable = null,
            LogType logType = LogType.Log) {

            func ??= element => (T2) (object) element.ToString();
            t    =   t.ToArray();

            t.Select(func).LogInternal(logType, true, formattable);

            return t;
        }

        /// Store this log and return the value for further use.
        /// <br/>Use <see cref="LogNow{T}"/> at the end of the same line to log the queue.
#if EXTENDED_LOGGER_REQUIRES_ATTRIBUTE
        /// <br/>Use <see cref="EnableLogsAttribute"/> on the containing class to set a color.
#endif
        [HideInCallstack]
        public static T LogLater<T>(this T t, object formattable = null, Func<T, bool> condition = null, LogType logType = LogType.Log) =>
            condition?.Invoke(t) ?? true
                ? t.LogInternal(logType, false, formattable)
                : t;

        /// Log the queue separated by the <paramref name="separator"/> and return the value for further use.
        /// <br/>Should be called after <see cref="LogLater{T}"/> on the same line.
#if EXTENDED_LOGGER_REQUIRES_ATTRIBUTE
        /// <br/>Use <see cref="EnableLogsAttribute"/> on the containing class to set a color.
#endif
        [HideInCallstack]
        public static T LogNow<T>(this T t, string separator = "    |    ", LogType logType = LogType.Log) =>
            t.LogInternal(logType, false, null, separator);

        #endregion

        #region Internal

        [HideInCallstack]
        private static T LogInternal<T>(this T t, LogType logType, bool instant, object formattable, [CanBeNull] string separator = null) {
            StackFrame[] stackFrames = GetStackFrames();

            if (!t.EnabledLogs(stackFrames[0], out Color logColor))
                return t;

            string message = formattable.SafeString(string.Empty);

#if CUSTOM_LOGGER_PRINT_CALLER
        foreach (stackFrame f in stackFrames) {
            if (f.GetMethod().Name == caller || f.GetMethod().DeclaringType?.Name == caller) {
                MethodBase method = f.GetMethod();
                string     className = method.DeclaringType?.FullName;
                string     methodName = method.Name;

                caller = $"[{string.Join(".", className?.Colored(Color.Violet), methodName.Colored(Color.Orchid))}]";
                break;
            }
        }

        message = string.Join(": ", caller, message.Trim());
#elif CUSTOM_LOGGER_PRINT_CALLER_ON_EMPTY_LOG
        message = message.IsNullOrEmpty() ? $"[{caller}]: ".Colored(Color.Violet) : message.Trim();
#else
            message = message.Trim();

            if (!message.IsNullOrEmpty())
                message += ": ";
#endif

            message = message.Replace("{", "{{").Replace("}", "}}").Replace("{{0}}", "{0}");

            if (!message.Contains("{0}"))
                message += "{0}";

            switch (t) {
                case Color c:             message = "█ ".Colored(c) + message; break;
                case UnityEngine.Color c: message = "█ ".Colored(c) + message; break;
            }

            if (t is IEnumerable enumerable and not string) {
                object[] collection            = enumerable.Cast<object>().ToArray();
                string   coloredTypeWithLength = (t.GetType().Name().Replace("[]") + $"[{collection.Length}]").Colored(Color.Orange);

                ConsoleLog(logType, string.Format(message, coloredTypeWithLength), logColor, t, formattable, stackFrames);

                foreach (object o in collection)
                    ConsoleLog(logType, $"   - {o.SafeString()}", logColor, o, formattable, stackFrames);

                return t;
            }

            message = string.Format(message, t.SafeString());

            if (instant) {
                ConsoleLog(logType, message, logColor, t, formattable, stackFrames);
                return t;
            }

            GetCaller(stackFrames[0], out string caller);
            QueueLog(caller, message);
            TryLog(logType, caller, separator, logColor, t, formattable, stackFrames);

            return t;
        }

        private static StackFrame[] GetStackFrames() =>
            new StackTrace(3, true).GetFrames()?.SkipWhile(f => f.GetFileName()?.EndsWith($"{nameof(ExtendedLogger)}.cs") ?? false).ToArray();

        [Pure]
        private static bool EnabledLogs<T>(this T t, StackFrame stackFrame, out Color color) {
#if EXTENDED_LOGGER_REQUIRES_ATTRIBUTE
        MethodBase method = stackFrame.GetMethod();
        Type       classType = method.DeclaringType!;

        bool methodHasAttribute = Attribute.IsDefined(method,    typeof(EnableCustomLogsAttribute));
        bool classHasAttribute = Attribute.IsDefined(classType, typeof(EnableCustomLogsAttribute));

        color = EnableCustomLogsAttribute.DefaultColor;

        if (methodHasAttribute || classHasAttribute)
            color = ((EnableCustomLogsAttribute) Attribute.GetCustomAttribute(classHasAttribute ? classType : method, typeof(EnableCustomLogsAttribute))).Color;
        else
            return false;
#else
            color = t switch {
                Color c              => c,
                UnityEngine.Color uc => uc.ToCsColor(),
                _                    => ColoredLogsAttribute.DefaultColor
            };

            return true;
#endif
        }

        private static void GetCaller(StackFrame stackFrame, out string caller) =>
            caller = $"{stackFrame.GetFileName()}:{stackFrame.GetMethod().Name}";

        private static void QueueLog(string caller, object log) {
            logQueue.TryAdd(caller, new());
            logQueue[caller].Add(log);
        }

        private static void TryLog(LogType logType, string path, [CanBeNull] string separator, Color color, object context, object formattable, StackFrame[] stackFrames) {
            if (separator == null) // not called from LogNow
                return;

            ConsoleLog(logType, string.Join(separator.Colored(Color.Red), logQueue[path]), color, context, formattable, stackFrames);
            logQueue.Remove(path);
        }

        [HideInCallstack]
        private static void ConsoleLog(LogType logType, string message, Color color, object context, object formattable, StackFrame[] stackFrames) {
#if UNFORMATTED_LOGS
        message = message.Unformatted();
#endif

            // message = message.Decolored();
            // TODO capture timestamps and add smart (XX:XX:XX.xx XX/XX/XXXX)

#if !UNITY_EDITOR || SIMULATE_BUILD
        switch (logType) {
            default:
            case LogType.Log:
                Debug.Log(message);
                break;
            case LogType.Warning:
                Debug.LogWarning(message);
                break;
            case LogType.Error:
                Debug.LogError(message);
                break;
            case LogType.Exception:
#if UNITY_EDITOR
                Debug.LogException(new(message.Colored(Color.Firebrick)));
#else
                Debug.LogException(new(message));
#endif
                break;
            case LogType.Assert:
                Debug.LogAssertion(message);
                break;
        }
#else
            // AppendFilteredStackTrace(ref message, stackFrames);
            ReplacePathsWithLinks(ref message);

            Object ctx = context switch {
                Component c   => c.gameObject,
                GameObject go => go,
                _             => (context as Object).n() ?? formattable as Object
            };

            switch (logType) {
                default:
                case LogType.Log:
                    Debug.Log(message.Colored(color), ctx);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message.Colored(Color.Yellow), ctx);
                    break;
                case LogType.Error:
                    Debug.LogError(message.Colored(Color.Red), ctx);
                    break;
                case LogType.Exception:
                    Debug.LogException(new(message.Colored(Color.Firebrick).Bold()), ctx);
                    break;
                case LogType.Assert:
                    Debug.LogAssertion(message.Colored(Color.Aqua), ctx);
                    break;
            }
#endif
        }

        #endregion

        #region Extra

        public static void ReplacePathsWithLinks(ref string message) {
            if (string.IsNullOrEmpty(message))
                return;

            foreach (Match m in Regex.Matches(message, @" in (file:line:column )?(\w.*):(\d+)(:\d+)?")) {
                try {
                    string fullMatch = m.Value;
                    string path      = m.Groups[2].Value.Trim();
                    string name      = Path.GetFileName(path); // can throw for invalid chars
                    string line      = m.Groups[3].Value;
                    string link      = name.Link(path, line);
                    string replaced  = fullMatch.Replace(name, link);
                    message = message.Replace(fullMatch, replaced);
                }
                catch {}
            }
        }

        public static void AppendFilteredStackTrace(ref string message, StackFrame[] stackFrames) { // todo untested
            // message.Log("AAAAAAAAAAAAAA");

            message += "\n\n"
                       + stackFrames
                           .Select(f => f.GetFileName()?
                                            .StartAt("Assets", true, "...".Colored(Color.Gray))
                                            .StartAt("quantum.code", true, "...".Colored(Color.Gray))
                                        + (":" + f.GetFileLineNumber()).Colored(Color.Gray))
                           .Replace(path => !path.StartsWith("...Assets")
                                            && !path.StartsWith("...quantum.code")
                                            || path.Contains("Core\\Core.cs"),
                               "...".Colored(Color.Gray))
                           .MergeDuplicates()
                           .JoinSmart("\n") /*.Log("BBBBBBBBBBBBBB")*/;
        }

        #endregion

#else
    public static void LogEx<T>(this T t, object formattable = null)        => Debug.LogException(new(Formatted(t, formattable)));
    public static void LogException<T>(this T t, object formattable = null) => Debug.LogException(new(Formatted(t, formattable)));

    private static string Formatted<T>(T t, object formattable) {
        string message = formattable.SafeString().Trim();

        if (!message.IsNullOrEmpty())
            message += ": ";

        if (!message.Contains("{0}"))
            message += "{0}";

        return string.Format(message, t.SafeString());
    }

#endif

    }

}