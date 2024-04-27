using System;

namespace dotnetCampus.Logging;

/// <summary>Represents a type used to perform logging.</summary>
/// <remarks>Aggregates most logging patterns to a single method.</remarks>
public interface ILogger
{
    /// <summary>Writes a log entry.</summary>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="eventId">Id of the event.</param>
    /// <param name="state">The entry to be written. Can be also an object.</param>
    /// <param name="exception">The exception related to this entry.</param>
    /// <param name="formatter">Function to create a <see cref="T:System.String" /> message of the <paramref name="state" /> and <paramref name="exception" />.</param>
    /// <typeparam name="TState">The type of the object to be written.</typeparam>
    void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter);
}

/// <summary>
/// A generic interface for logging where the category name is derived from the specified
/// <typeparamref name="TCategoryName" /> type name.
/// Generally used to enable activation of a named <see cref="T:Microsoft.Extensions.Logging.ILogger" /> from dependency injection.
/// </summary>
/// <typeparam name="TCategoryName">The type whose name is used for the logger category name.</typeparam>
public interface ILogger<out TCategoryName> : ILogger
{
}
