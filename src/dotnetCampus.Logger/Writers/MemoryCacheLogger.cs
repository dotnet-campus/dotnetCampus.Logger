using System;

namespace dotnetCampus.Logging.Writers;

/// <summary>
/// 用于在日志模块初始化完成前先对所有记录的日志进行缓存，以便在日志模块初始化完成后再将缓存的日志写入到日志文件中。
/// </summary>
internal class MemoryCacheLogger : ILogger
{
    private ILogger? _realLogger;

    private readonly System.Collections.Concurrent.ConcurrentQueue<CachedLogItem> _queue = [];

    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (_realLogger is { } logger)
        {
            logger.Log(logLevel, eventId, state, exception, (s, e) => formatter((TState)s, e));
        }
        _queue.Enqueue(new(logLevel, eventId, state!, exception, (s, e) => formatter((TState)s, e)));
    }

    /// <summary>
    /// 将所有缓存的日志写入到真正的日志记录器中。
    /// </summary>
    /// <param name="logger">真正的日志记录器。</param>
    internal void Flush(ILogger logger)
    {
        _realLogger = logger;
        while (_queue.TryDequeue(out var context))
        {
            logger.Log(context.LogLevel, context.EventId, context.State, context.Exception, context.Formatter);
        }
    }

    /// <summary>
    /// 辅助缓存日志条目。
    /// </summary>
    /// <param name="LogLevel">日志级别。</param>
    /// <param name="EventId">事件 Id。</param>
    /// <param name="State">日志内容。</param>
    /// <param name="Exception">异常信息。</param>
    /// <param name="Formatter">格式化器。</param>
    private readonly record struct CachedLogItem(
        LogLevel LogLevel,
        EventId EventId,
        object State,
        Exception? Exception,
        Func<object, Exception?, string> Formatter
    );
}
