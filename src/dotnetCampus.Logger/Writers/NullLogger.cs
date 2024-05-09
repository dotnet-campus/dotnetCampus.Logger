#nullable enable

using System;

namespace dotnetCampus.Logging.Writers;

internal class NullLogger : ILogger
{
    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
    }
}
