using System;

namespace dotnetCampus.Logging.SourceGenerators;

/// <summary>
/// 仅由基本类型构成的日志源。用于源生成器将无依赖库中的日志重定向到应用程序聚合日志系统中。
/// </summary>
public interface ILoggerBridge
{
    /// <summary>Writes a log entry.</summary>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="eventId">Id of the event.</param>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="state">The entry to be written. Can be also an object.</param>
    /// <param name="exception">The exception related to this entry.</param>
    /// <param name="formatter">Function to create a <see cref="T:System.String" /> message of the <paramref name="state" /> and <paramref name="exception" />.</param>
    /// <typeparam name="TState">The type of the object to be written.</typeparam>
    void Log<TState>(
        int logLevel,
        int? eventId,
        string? eventName,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter);
}
