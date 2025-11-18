// #define EXTENDED_LOGGER_REQUIRES_ATTRIBUTE
// #define CUSTOM_LOGGER_PRINT_CALLER_ON_EMPTY_LOG
// #define CUSTOM_LOGGER_PRINT_CALLER
// #define UNFORMATTED_LOGS

#if UNITY_EDITOR && !SIMULATE_BUILD || ENABLE_LOGS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

#endif
    public static class ExtendedLogger {

#if UNITY_EDITOR && !SIMULATE_BUILD || ENABLE_LOGS

        private static readonly Dictionary<string, List<object>> logQueue = new();

// TODO STACKABLE CONDITIONAL LOGS, USE .LogLater(index, ...) AND .LogNow(index) INDEPENDENT OF LINE NUMBER
// TODO MAYBE USE DATETIME.NOW.MILLISECONDS TO DISTINGUISH METHOD CALLS, OTHERWISE CLEAR LogQueue CALL IS REQUIRED
// TODO EXAMPLE:
// TODO EXAMPLE: var a = field.Method().Method2(x => x.Method3().LogLater(0)); // stack logs
// TODO EXAMPLE:
// TODO EXAMPLE: if (a.Method4())
// TODO EXAMPLE:     throw new("NotFound".LogNow(0)); // log them if necessary
// TODO EXAMPLE:

        /// Print instantly and return the value for further use.
        /// <param name="formattable">Used as a prefix unless "{0}" is present in which case <paramref name="t"/> is formatted into it.</param>
#if EXTENDED_LOGGER_REQUIRES_ATTRIBUTE
    /// <br/>Requires <see cref="EnableLogsAttribute"/> on the containing class.
#endif
        [StringFormatMethod("formattable")]
        [HideInCallstack]
        public static T Log<T>(this T t, object formattable = null, LogType logType = LogType.Log) =>
            t.LogInternal(logType, true, formattable);

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType,string)"/>
        [StringFormatMethod("formattable")]
        [HideInCallstack]
        public static T LogErr<T>(this T t, object formattable = null) =>
            t.LogInternal(LogType.Error, true, formattable);

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType,string)"/>
        [StringFormatMethod("formattable")]
        [HideInCallstack]
        public static T LogEx<T>(this T t, object formattable = null) =>
            t.LogInternal(LogType.Exception, true, formattable);

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType,string)"/>
        [HideInCallstack]
        public static T1 Log<T1, T2>(this T1 t, Func<T1, T2> func, object formattable = null, LogType logType = LogType.Log) {
            if (t == null)
                return t.Log(formattable, logType);

            func.SafeInvoke(t).LogInternal(logType, true, formattable);
            return t;
        }

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType,string)"/>
        [HideInCallstack]
        public static bool LogIf(this bool t, object formattable = null, string ifCalledThrough = "", LogType logType = LogType.Log) =>
            t.LogIf(formattable, _ => t, ifCalledThrough, logType);

        // /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType,string)"/>
        // [HideInCallstack]
        // public static T LogIf<T>(this T t, bool condition, string ifCalledThrough = "", LogType logType = LogType.Log) =>
        //     t.LogIf(null, _ => condition, ifCalledThrough, logType);

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType,string)"/>
        [HideInCallstack]
        public static T LogIf<T>(this T t, object formattable, bool condition, string ifCalledThrough = "", LogType logType = LogType.Log) =>
            t.LogIf(formattable, _ => condition, ifCalledThrough, logType);

        // /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType,string)"/>
        // [HideInCallstack]
        // public static T LogIf<T>(this T t, bool condition, object formattable = null, string ifCalledThrough = "", LogType logType = LogType.Log) =>
        //     t.LogIf(formattable, _ => condition, ifCalledThrough, logType);

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType,string)"/>
        [HideInCallstack]
        public static T LogIf<T>(this T t, Func<T, bool> condition, object formattable = null, string ifCalledThrough = "", LogType logType = LogType.Log) =>
            t.LogIf(formattable, condition, ifCalledThrough, logType);

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType,string)"/>
        [HideInCallstack]
        public static T LogIf<T>(this T t, object formattable, Func<T, bool> condition, string ifCalledThrough = "", LogType logType = LogType.Log) {
            condition ??= _ => t is not bool b || b;
            GetStackFrames(out StackFrame[] frames);

            if (condition.SafeInvoke(t) && (ifCalledThrough.IsNullOrEmpty() || frames.Any(f => (f.GetMethod().Name + f.GetMethod().DeclaringType?.Name).Contains(ifCalledThrough))))
                t.LogInternal(logType, true, formattable);

            return t;
        }

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType,string)"/>
        [HideInCallstack]
        public static T1 LogIfAs<T1, T2>(this T1 t, Func<T1, bool> condition, Func<T1, T2> func, LogType logType = LogType.Log) {
            if (t == null)
                return t.Log(null, logType);

            if (condition(t))
                func.SafeInvoke(t).LogInternal(logType, true, null);

            return t;
        }

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType,string)"/>
        [HideInCallstack]
        public static T1 LogIfAs<T1, T2>(this T1 t, object formattable, Func<T1, bool> condition, Func<T1, T2> func, LogType logType = LogType.Log) {
            if (t == null)
                return t.Log(formattable, logType);

            if (condition(t))
                func.SafeInvoke(t).LogInternal(logType, true, formattable);

            return t;
        }

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType,string)"/>
        [HideInCallstack]
        public static IEnumerable<T> LogAll<T>(this IEnumerable<T> t, object formattable = null, LogType logType = LogType.Log) =>
            t.LogAll<T, string>(null, formattable, logType);

        /// <inheritdoc cref="Log{T}(T,object,UnityEngine.LogType,string)"/>
        [HideInCallstack]
        public static IEnumerable<T1> LogAll<T1, T2>(this IEnumerable<T1> t, Func<T1, T2> func = null, object formattable = null, LogType logType = LogType.Log) {
            func ??= element => (T2) (object) element.SafeString();
            t    =   t.ToArray();

            t.Select(e => func.SafeInvoke(e)).LogInternal(logType, true, formattable);

            return t;
        }

        /// Store this log and return the value for further use.
        /// <br/>Use <see cref="LogNow{T}"/> at the end of the same line to log the queue.
        /// <param name="index">allows differentiating log calls from the same line to selectively <see cref="LogNow"/> later</param>
#if EXTENDED_LOGGER_REQUIRES_ATTRIBUTE
        /// <br/>Use <see cref="EnableLogsAttribute"/> on the containing class to set a color.
#endif
        [HideInCallstack]
        public static T LogLater<T>(this T t, int index = 0, object formattable = null, Func<T, bool> condition = null, LogType logType = LogType.Log) =>
            condition?.SafeInvoke(t) ?? true
                ? t.LogInternal(logType, false, formattable, new[] { index })
                : t;

        /// Log the queue separated by the <paramref name="separator"/> and return the value for further use.
        /// <br/>Should be called after <see cref="LogLater{T}"/> on the same line.
#if EXTENDED_LOGGER_REQUIRES_ATTRIBUTE
        /// <br/>Use <see cref="EnableLogsAttribute"/> on the containing class to set a color.
#endif
        [HideInCallstack]
        public static T LogNow<T>(this T t, params int[] indexes) =>
            t.LogInternal(LogType.Log, false, null, indexes, "    |    ");

        /// Log the queue separated by the <paramref name="separator"/> and return the value for further use.
        /// <br/>Should be called after <see cref="LogLater{T}"/> on the same line.
#if EXTENDED_LOGGER_REQUIRES_ATTRIBUTE
        /// <br/>Use <see cref="EnableLogsAttribute"/> on the containing class to set a color.
#endif
        [HideInCallstack]
        public static T LogNow<T>(this T t, int index, string separator = "    |    ", LogType logType = LogType.Log) =>
            t.LogInternal(logType, false, null, new[] { index }, separator);

        /// Log the queue separated by the <paramref name="separator"/> and return the value for further use.
        /// <br/>Should be called after <see cref="LogLater{T}"/> on the same line number.
        /// <param name="indexes">null results in logging all queued logs on this line</param>
#if EXTENDED_LOGGER_REQUIRES_ATTRIBUTE
        /// <br/>Use <see cref="EnableLogsAttribute"/> on the containing class to set a color.
#endif
        [HideInCallstack]
        // ReSharper disable once MethodOverloadWithOptionalParameter
        public static T LogNow<T>(this T t, int[] indexes, string separator = "    |    ", LogType logType = LogType.Log) =>
            t.LogInternal(logType, false, null, indexes, separator);

        /// <param name="indexes">specified by LogLater or LogNow</param>
        /// <param name="separator">specified by LogNow only</param>
        [HideInCallstack]
        private static T LogInternal<T>(this T t, LogType logType, bool instant, object formattable, int[] indexes = null, string separator = null) {
            GetStackFrames(out StackFrame[] stackFrames);

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

            if (t is IEnumerable enumerable and not string) {
                object[] collection            = enumerable.Cast<object>().ToArray();
                string   coloredTypeWithLength = (t.GetType().Name().Replace("[]") + $"[{collection.Length}]").Colored(Color.Orange);

                ConsoleLog(logType, string.Format(message, coloredTypeWithLength), logColor, t, formattable, stackFrames);

                foreach (object o in collection)
                    ConsoleLog(logType, $"   - {o.SafeString()}", logColor, o, formattable, stackFrames);

                return t;
            }

            message = string.Format(message, t.EnsureString("\"null\""));

            if (instant) {
                ConsoleLog(logType, message, logColor, t, formattable, stackFrames);
                return t;
            }

            foreach (string caller in GetCallers(stackFrames[0], indexes)) {
                QueueLog(caller, message);
                TryLog(logType, caller, separator, logColor, t, formattable, stackFrames);
            }

            return t;
        }

        private static void GetStackFrames(out StackFrame[] stackFrames) =>
            stackFrames = new StackTrace(3, true).GetFrames()?.SkipWhile(f => f.GetFileName()?.EndsWith($"{nameof(ExtendedLogger)}.cs") ?? false).ToArray();

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

        [Pure]
        private static IEnumerable<string> GetCallers(StackFrame stackFrame, [CanBeNull] int[] indexes) {
            if (indexes != null)
                foreach (int index in indexes)
                    yield return $"{stackFrame.GetFileName()}:{stackFrame.GetMethod().Name}{index}";

            yield return $"{stackFrame.GetFileName()}:{stackFrame.GetMethod().Name}";
        }

        private static void QueueLog(string caller, object log) {
            logQueue.TryAdd(caller, new());
            logQueue[caller].Add(log);
        }

        private static void TryLog(LogType logType, string caller, [CanBeNull] string separator, Color color, object context, object formattable, StackFrame[] stackFrames) {
            if (separator == null) // not called from LogNow
                return;

            ConsoleLog(logType, string.Join(separator.Colored(Color.Red), logQueue[caller]), color, context, formattable, stackFrames);
            logQueue.Remove(caller);
        }

        [HideInCallstack]
        private static void ConsoleLog<T>(LogType logType, string message, Color color, T t, object formattable, StackFrame[] stackFrames) {
#if UNFORMATTED_LOGS
        message = message.Unformatted();
#endif

            message = message.Decolored();

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
                Debug.LogException(new(message));
                break;
            case LogType.Assert:
                Debug.LogAssertion(message);
                break;
        }
#else

            if (t is Color clr)
                message += clr.Name + "test";

            ReplacePathsWithLinks(ref message);

            Object ctx;

            if (t is Component c)
                ctx = c.gameObject;
            else if (t is GameObject go)
                ctx = go;
            else
                ctx = t as Object ?? formattable as Object;

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

        public static void ReplacePathsWithLinks(ref string message) {
            if (string.IsNullOrEmpty(message)) return;

            foreach (Match m in Regex.Matches(message, @" in (file:line:column )?(\w.*):(\d+)(:\d+)?")) {
                string fullMatch = m.Value;
                string path      = m.Groups[2].Value.Trim();
                string name      = Path.GetFileName(path);
                string line      = m.Groups[3].Value;
                string link      = name.Link(path, line);
                string replaced  = fullMatch.Replace(name, link);
                message = message.Replace(fullMatch, replaced);
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
                           .Join("\n") /*.Log("BBBBBBBBBBBBBB")*/;
        }

#endif

    }

}