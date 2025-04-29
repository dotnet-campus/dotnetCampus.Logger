#nullable enable

using System;
using System.ComponentModel;

namespace DotNetCampus.Logging.Bridges;

/// <summary>
/// DotNetCampus.Logging.Bridges 的桥接日志记录器。
/// </summary>
internal class BridgeLogger : ILogger
{
    public bool IsEnabled(LogLevel logLevel)
    {
        return
#if NETCOREAPP3_0_OR_GREATER
        ILoggerBridge
#else
        LoggerBridgeLinker
#endif
            .Bridge?.IsEnabled((int)logLevel) ?? false;
    }

    /// <inheritdoc />
    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
#if NETCOREAPP3_0_OR_GREATER
        ILoggerBridge
#else
        LoggerBridgeLinker
#endif
            .Bridge?.Log((int)logLevel, eventId.Id, eventId.Name, state, exception, formatter);
    }
}
