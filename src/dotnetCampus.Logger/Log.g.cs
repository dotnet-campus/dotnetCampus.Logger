#nullable enable

using global::System;
using global::System.Diagnostics.CodeAnalysis;

namespace dotnetCampus.Logging;

public static class Log
{
    private static ILogger _current;

    static Log()
    {
        Current = new NullLogger();
    }


    public static ILogger Current
    {
        get => _current;
        [MemberNotNull(nameof(_current), nameof(Debug), nameof(Trace))]
        private set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (Equals(_current, value))
            {
                Debug ??= new DebugLogger(value);
                Trace ??= new TraceLogger(value);
                return;
            }

            _current = value;
            Debug = new DebugLogger(value);
            Trace = new TraceLogger(value);
        }
    }

    public static DebugLogger Debug { get; private set; }

    public static TraceLogger Trace { get; private set; }

    /// <summary>
    /// 正常记录日志。
    /// </summary>
    /// <param name="message">要记录的消息。</param>
    public static void Info(string message)
    {
        Current.Log(LogLevel.Information, default, message, null, (s, ex) => message);
    }

    /// <summary>
    /// 记录警告日志。
    /// </summary>
    /// <param name="message">要记录的消息，形如 [tag] message</param>
    /// <param name="exception">相关的异常信息。</param>
    public static void Warn(string message, Exception? exception = null)
    {
        Current.Log(LogLevel.Warning, default, message, exception, (s, ex) => message);
    }

    /// <summary>
    /// 记录错误日志。
    /// </summary>
    /// <param name="message">要记录的消息，形如 [tag] message</param>
    /// <param name="exception">相关的异常信息。</param>
    public static void Error(string message, Exception? exception = null)
    {
        Current.Log(LogLevel.Error, default, message, exception, (s, ex) => message);
    }
}
