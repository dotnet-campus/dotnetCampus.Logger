#nullable enable

using global::System;

namespace dotnetCampus.Logging;

public static class LoggerExtensions
{
    /// <summary>
    /// 在开启了追踪的情况下输出日志。（默认开启）
    /// </summary>
    /// <param name="logger">记录日志所使用的记录器。</param>
    /// <param name="message">要记录的消息，形如 [tag] message</param>
    public static void Trace(this ILogger logger, string message)
    {
        logger.Log(LogLevel.Trace, default, message, null, (s, ex) => message);
    }

    /// <summary>
    /// 记录 debug 级别的日志。
    /// </summary>
    /// <param name="logger">记录日志所使用的记录器。</param>
    /// <param name="message">要记录的消息。</param>
    public static void Debug(this ILogger logger, string message)
    {
        logger.Log(LogLevel.Debug, default, message, null, (s, ex) => message);
    }

    /// <summary>
    /// 正常记录日志。
    /// </summary>
    /// <param name="logger">记录日志所使用的记录器。</param>
    /// <param name="message">要记录的消息。</param>
    public static void Info(this ILogger logger, string message)
    {
        logger.Log(LogLevel.Information, default, message, null, (s, ex) => message);
    }

    /// <summary>
    /// 记录警告日志。
    /// </summary>
    /// <param name="logger">记录日志所使用的记录器。</param>
    /// <param name="message">要记录的消息，形如 [tag] message</param>
    /// <param name="exception">相关的异常信息。</param>
    public static void Warn(this ILogger logger, string message, Exception? exception = null)
    {
        logger.Log(LogLevel.Warning, default, message, null, (s, ex) => message);
    }

    /// <summary>
    /// 记录错误日志。
    /// </summary>
    /// <param name="logger">记录日志所使用的记录器。</param>
    /// <param name="message">要记录的消息，形如 [tag] message</param>
    /// <param name="exception">相关的异常信息。</param>
    public static void Error(this ILogger logger, string message, Exception? exception = null)
    {
        logger.Log(LogLevel.Warning, default, message, null, (s, ex) => message);
    }

    /// <summary>
    /// 记录崩溃日志。
    /// </summary>
    /// <param name="logger">记录日志所使用的记录器。</param>
    /// <param name="message">要记录的消息，形如 [tag] message</param>
    /// <param name="exception">相关的异常信息。</param>
    public static void Fatal(this ILogger logger, string message, Exception? exception = null)
    {
        logger.Log(LogLevel.Critical, default, message, null, (s, ex) => message);
    }
}
