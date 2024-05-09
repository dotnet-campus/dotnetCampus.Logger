#nullable enable

using GEventId = global::dotnetCampus.Logging.EventId;
using GException = global::System.Exception;
using GILogger = global::dotnetCampus.Logging.ILogger;
using GLog = global::dotnetCampus.Logging.Log;
using GLogLevel = global::dotnetCampus.Logging.LogLevel;

namespace dotnetCampus.Logger.Assets.Templates;

/// <summary>
/// 聚合各个来源的日志桥。调用其 <see cref="Link"/> 方法可以将
/// </summary>
partial class AggregateLoggerBridge
{
    private GILogger? _logger;

    /// <summary>
    /// 将所有已指派给此聚合日志桥的所有日志桥对接到 <paramref name="logger"/> 日志记录器上。
    /// </summary>
    /// <param name="logger">要对接的日志记录器。如果不指定，则默认使用全局的 <see cref="GLog"/>.<see cref="GLog.Current"/>。</param>
    /// <exception cref="global::System.InvalidOperationException">
    /// 如果已经对接过日志记录器，则会抛出此异常。
    /// 如果希望针对不同的库对接不同的日志记录器，请编写多个聚合日志桥并分别导入各自的日志桥。
    /// </exception>
    public void Link(GILogger? logger = null)
    {
        if (_logger != null)
        {
            throw new global::System.InvalidOperationException("The logger has been linked. If you want to link different logger instance, please create a new AggregateLoggerBridge instance.");
        }

        _logger = logger;

        // 链接来自各个源的日志桥：
        // <FLAG>
        //
        // 源生成器请在此处添加代码...
        //
        // 对于 .NET 运行时：
        // global::Xxx.Logging.ILoggerBridge.Link(this);
        //
        // 对于 .NET Framework 运行时：
        // global::Xxx.Logging.LoggerBridgeLinker.Link(this);
        //
        // </FLAG>
    }

    /// <summary>
    /// 写入日志条目。
    /// </summary>
    /// <param name="logLevel">将在此级别上写入条目。</param>
    /// <param name="eventId">事件的 Id。</param>
    /// <param name="eventName">事件的名称。</param>
    /// <param name="state">要写入的条目。也可以是一个对象。</param>
    /// <param name="exception">与此条目相关的异常。</param>
    /// <param name="formatter">创建一条字符串消息以记录 <paramref name="state" /> 和 <paramref name="exception" />。</param>
    /// <typeparam name="TState">要写入的对象的类型。</typeparam>
    public void Log<TState>(
        int logLevel,
        int eventId,
        string? eventName,
        TState state,
        GException? exception,
        global::System.Func<TState, GException?, string> formatter)
    {
        var logger = _logger ?? GLog.Current;
        logger.Log(
            (GLogLevel)logLevel,
            new GEventId(eventId, eventName),
            state,
            exception,
            formatter);
    }
}
