using System;
using System.Collections.Generic;
using dotnetCampus.Logging.Configurations;

namespace dotnetCampus.Logging;

public class CompositeLogger : ILogger
{
    private InheritedConfiguration<LogOptions> Configuration { get; } = new();

    public List<ILogger> Writers { get; }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        throw new NotImplementedException();
    }
}
