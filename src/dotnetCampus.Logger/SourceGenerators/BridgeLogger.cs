using System;

namespace dotnetCampus.Logging.SourceGenerators;

internal class BridgeLogger : ILogger
{
    private ILoggerBridge? _bridge;

    private ILoggerBridge? Bridge => _bridge ??= LogFactory.TryGetBridge();

    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _bridge?.Log((int)logLevel, eventId.Id, eventId.Name, state, exception, formatter);
    }
}
