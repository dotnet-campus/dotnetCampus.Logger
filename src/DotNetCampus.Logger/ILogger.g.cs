#nullable enable
using global::System;

namespace DotNetCampus.Logging;

/// <summary>
/// 表示用于执行日志记录的类型。
/// </summary>
/// <remarks>
/// 将大多数日志模式聚合到一个方法中。
/// </remarks>
public interface ILogger
{
    /// <summary>
    /// 检查是否已启用给定的日志级别。
    /// </summary>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    bool IsEnabled(LogLevel logLevel);

    /// <summary>
    /// 写入日志条目。
    /// </summary>
    /// <param name="logLevel">将在此级别上写入条目。</param>
    /// <param name="eventId">事件的 Id。</param>
    /// <param name="state">要写入的条目。也可以是一个对象。</param>
    /// <param name="exception">与此条目相关的异常。</param>
    /// <param name="formatter">创建一条字符串消息以记录 <paramref name="state" /> 和 <paramref name="exception" />。</param>
    /// <typeparam name="TState">要写入的对象的类型。</typeparam>
    void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter);
}
