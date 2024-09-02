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
    private static DebugLogger _debug;
    private static TraceLogger _trace;

    static Log()
    {
        // 在全局日志中，默认日志记录器是 MemoryCacheLogger。
        // 然而在源生成器为单独库生成的代码中，默认日志记录器是 BridgeLogger。
        Current = new MemoryCacheLogger();
    }

    /// <summary>
    /// 获取此静态日志记录器当前所用的日志记录器实例。
    /// </summary>
    public static ILogger Current
    {
        get => _current;
        [MemberNotNull(nameof(_current), nameof(_debug), nameof(_trace))]
        private set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (Equals(_current, value))
            {
                _debug ??= new DebugLogger(value);
                _trace ??= new TraceLogger(value);
                return;
            }

            _current = value;
            _debug = new DebugLogger(value);
            _trace = new TraceLogger(value);
        }
    }

    /// <summary>
    /// 获取仅在 debug 配置下才会记录的日志记录器。
    /// </summary>
    /// <remarks>
    /// 请注意，所有通过此记录器记录的日志仅在以 debug 配置编译时才会被记录，并且在非 debug 编译后对此记录器的调用都会被编译器优化掉。
    /// </remarks>
    public static DebugLogger DebugLogger => _debug;

    /// <summary>
    /// 获取仅在 TRACE 条件编译符被定义时才会记录的日志记录器。
    /// 通常情况下，debug 和 release 配置下都会定义 TRACE 条件编译符。
    /// </summary>
    /// <remarks>
    /// 请注意，所有通过此记录器记录的日志仅在定义了 TRACE 条件编译符时才会被记录，并且在未定义 TRACE 条件编译符后对此记录器的调用都会被编译器优化掉。
    /// </remarks>
    public static TraceLogger TraceLogger => _trace;

    /// <summary>
    /// 记录跟踪日志。
    /// </summary>
    /// <param name="message">要记录的消息。</param>
    /// <remarks>
    /// 如果开启了跟踪级别的日志记录，此方法产生的日志在生产环境中也依然会被记录。<br/>
    /// 如果希望仅在 TRACE 条件编译符被定义时记录日志，请使用 <see cref="TraceLogger"/>。
    /// </remarks>
    public static void Trace(string message)
    {
        Current.Log(LogLevel.Trace, default, message, null, static (s, ex) => s);
    }

    /// <summary>
    /// 记录调试日志。
    /// </summary>
    /// <param name="message">要记录的消息。</param>
    /// <remarks>
    /// 如果开启了调试级别的日志记录，此方法产生的日志在生产环境中也依然会被记录。<br/>
    /// 如果希望仅在 debug 配置下记录日志，请使用 <see cref="DebugLogger"/>。
    /// </remarks>
    public static void Debug(string message)
    {
        Current.Log(LogLevel.Debug, default, message, null, static (s, ex) => s);
    }

    /// <summary>
    /// 记录信息日志。
    /// </summary>
    /// <param name="message">要记录的消息。</param>
    public static void Info(string message)
    {
        Current.Log(LogLevel.Information, default, message, null, static (s, ex) => s);
    }

    /// <summary>
    /// 记录警告日志。
    /// </summary>
    /// <param name="message">要记录的消息。</param>
    /// <param name="exception">如果有异常信息，可以传入此参数。</param>
    public static void Warn(string message, Exception? exception = null)
    {
        Current.Log(LogLevel.Warning, default, message, exception, static (s, ex) => s);
    }

    /// <summary>
    /// 记录错误日志。
    /// </summary>
    /// <param name="message">要记录的消息。</param>
    /// <param name="exception">如果有异常信息，可以传入此参数。</param>
    public static void Error(string message, Exception? exception = null)
    {
        Current.Log(LogLevel.Error, default, message, exception, static (s, ex) => s);
    }

    /// <summary>
    /// 记录崩溃日志。
    /// </summary>
    /// <param name="message">要记录的消息。</param>
    /// <param name="exception">如果有异常信息，可以传入此参数。</param>
    public static void Fatal(string message, Exception? exception = null)
    {
        Current.Log(LogLevel.Critical, default, message, null, static (s, ex) => s);
    }
}
