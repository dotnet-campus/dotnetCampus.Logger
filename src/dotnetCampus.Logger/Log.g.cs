#nullable enable

using global::System;
using global::System.Diagnostics.CodeAnalysis;

namespace dotnetCampus.Logging;

/// <summary>
/// 提供静态的日志记录方法。
/// </summary>
public static partial class Log
{
    private static ILogger _current;

    static Log()
    {
        // 在全局日志中，默认日志记录器是 NullLogger。
        // 然而在源生成器为单独库生成的代码中，默认日志记录器是 BridgeLogger。
        Current = new NullLogger();
    }

    /// <summary>
    /// 获取此静态日志记录器当前所用的日志记录器实例。
    /// </summary>
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

    /// <summary>
    /// 获取仅在 debug 配置下才会记录的日志记录器。
    /// </summary>
    /// <remarks>
    /// 请注意，所有通过此记录器记录的日志仅在以 debug 配置编译时才会被记录，并且在非 debug 编译后对此记录器的调用都会被编译器优化掉。
    /// </remarks>
    public static DebugLogger Debug { get; private set; }

    /// <summary>
    /// 获取仅在 TRACE 条件编译符被定义时才会记录的日志记录器。
    /// 通常情况下，debug 和 release 配置下都会定义 TRACE 条件编译符。
    /// </summary>
    /// <remarks>
    /// 请注意，所有通过此记录器记录的日志仅在定义了 TRACE 条件编译符时才会被记录，并且在未定义 TRACE 条件编译符后对此记录器的调用都会被编译器优化掉。
    /// </remarks>
    public static TraceLogger Trace { get; private set; }

    /// <summary>
    /// 记录信息日志。
    /// </summary>
    /// <param name="message">要记录的消息。</param>
    public static void Info(string message)
    {
        Current.Log(LogLevel.Information, default, message, null, (s, ex) => message);
    }

    /// <summary>
    /// 记录警告日志。
    /// </summary>
    /// <param name="message">要记录的消息。</param>
    /// <param name="exception">如果有异常信息，可以传入此参数。</param>
    public static void Warn(string message, Exception? exception = null)
    {
        Current.Log(LogLevel.Warning, default, message, exception, (s, ex) => message);
    }

    /// <summary>
    /// 记录错误日志。
    /// </summary>
    /// <param name="message">要记录的消息。</param>
    /// <param name="exception">如果有异常信息，可以传入此参数。</param>
    public static void Error(string message, Exception? exception = null)
    {
        Current.Log(LogLevel.Error, default, message, exception, (s, ex) => message);
    }

    /// <summary>
    /// 记录崩溃日志。
    /// </summary>
    /// <param name="message">要记录的消息。</param>
    /// <param name="exception">如果有异常信息，可以传入此参数。</param>
    public static void Fatal(string message, Exception? exception = null)
    {
        Current.Log(LogLevel.Critical, default, message, null, (s, ex) => message);
    }
}
