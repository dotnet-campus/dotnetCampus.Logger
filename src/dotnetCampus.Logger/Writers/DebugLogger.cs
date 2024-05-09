#nullable enable

using System;
using System.Diagnostics;

namespace dotnetCampus.Logging.Writers;

/// <summary>
/// 提供仅在 debug 下才会记录的日志。
/// </summary>
public class DebugLogger(ILogger realLogger) : ILogger
{
    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        realLogger.Log(logLevel, eventId, state, exception, formatter);
    }

    /// <summary>
    /// 在开启了追踪的情况下输出日志。（默认开启）
    /// </summary>
    /// <param name="message">要记录的消息，形如 [tag] message</param>
    [Conditional("DEBUG")]
    public void Trace(string message)
    {
        realLogger.Log(LogLevel.Trace, default, message, null, (s, ex) => message);
    }

    /// <summary>
    /// 记录 debug 级别的日志。
    /// </summary>
    /// <param name="message">要记录的消息。</param>
    [Conditional("DEBUG")]
    public void Debug(string message)
    {
        realLogger.Log(LogLevel.Debug, default, message, null, (s, ex) => message);
    }

    /// <summary>
    /// 正常记录日志。
    /// </summary>
    /// <param name="message">要记录的消息。</param>
    [Conditional("DEBUG")]
    public void Info(string message)
    {
        realLogger.Log(LogLevel.Information, default, message, null, (s, ex) => message);
    }

    /// <summary>
    /// 记录警告日志。
    /// </summary>
    /// <param name="message">要记录的消息，形如 [tag] message</param>
    [Conditional("DEBUG")]
    public void Warn(string message)
    {
        realLogger.Log(LogLevel.Warning, default, message, null, (s, ex) => message);
    }
}
