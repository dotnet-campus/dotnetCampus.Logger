using System;
using dotnetCampus.Logging.Configurations;

namespace dotnetCampus.Logging.Writers;

public class ConsoleLogger : ILogger
{
    internal InheritedConfiguration<ConsoleLogOptions> Configuration { get; } = new();

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {

    }

}

public class ConsoleLogOptions : LogOptions
{
}
