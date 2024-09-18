#nullable enable

using global::System;

namespace dotnetCampus.Logging.Writers;

/// <summary>
/// 不记录任何日志的日志记录器。
/// </summary>
internal class NullLogger : ILogger
{
    public bool IsEnabled(LogLevel logLevel) => false;

    /// <inheritdoc />
    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
    }
}
