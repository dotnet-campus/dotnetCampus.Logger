#nullable enable

using EditorBrowsable = global::System.ComponentModel.EditorBrowsableAttribute;
using EditorBrowsableState = global::System.ComponentModel.EditorBrowsableState;
using EventId = global::dotnetCampus.Logging.EventId;
using Exception = global::System.Exception;
using ILogger = global::dotnetCampus.Logging.ILogger;
using LazyThreadSafetyMode = global::System.Threading.LazyThreadSafetyMode;
using LogLevel = global::dotnetCampus.Logging.LogLevel;

namespace dotnetCampus.Logger.Assets.Templates;

partial class Program
{
    /// <summary>
    /// 用于在 <see cref="Program"/> 类的内部记录日志。
    /// </summary>
    /// <remarks>
    /// 由于此代码是源生成器生成的代码，所以可以在日志模块初始化之前记录日志且提前生效。<br/>
    /// 🤩 你甚至能在 Main 方法的第一行就使用它记录日志！
    /// </remarks>
    private static GeneratedMemoryCacheLogger Log => GeneratedMemoryCacheLogger.Instance.Value;

    [EditorBrowsable(EditorBrowsableState.Never)]
    private sealed class GeneratedMemoryCacheLogger : ILogger
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static readonly global::System.Lazy<GeneratedMemoryCacheLogger> Instance = new(
            () => new GeneratedMemoryCacheLogger(),
            LazyThreadSafetyMode.None);

        private ILogger? _realLogger;

        private readonly global::System.Collections.Concurrent.ConcurrentQueue<CachedLogItem> _queue = [];

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            global::System.Func<TState, Exception?, string> formatter)
        {
            if (_realLogger is { } logger)
            {
                logger.Log(logLevel, eventId, state, exception, (s, e) => formatter((TState)s, e));
            }
            _queue.Enqueue(new(logLevel, eventId, state!, exception, (s, e) => formatter((TState)s, e)));
        }

        /// <summary>
        /// 将 <see cref="GeneratedMemoryCacheLogger"/> 隐式转换为可传递到 LogBuilder.UseMemoryCache 方法的委托。
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static implicit operator global::System.Action<ILogger>(GeneratedMemoryCacheLogger logger)
        {
            return logger.Flush;
        }

        private void Flush(ILogger logger)
        {
            _realLogger = logger;
            while (_queue.TryDequeue(out var context))
            {
                logger.Log(context.logLevel, context.eventId, context.state, context.exception, context.formatter);
            }
        }

        /// <summary>
        /// 辅助缓存日志条目。
        /// </summary>
        /// <param name="logLevel">日志级别。</param>
        /// <param name="eventId">事件 Id。</param>
        /// <param name="state">日志内容。</param>
        /// <param name="exception">异常信息。</param>
        /// <param name="formatter">格式化器。</param>
        private readonly record struct CachedLogItem(
            LogLevel logLevel,
            EventId eventId,
            object state,
            Exception? exception,
            global::System.Func<object, Exception?, string> formatter
        );
    }
}
