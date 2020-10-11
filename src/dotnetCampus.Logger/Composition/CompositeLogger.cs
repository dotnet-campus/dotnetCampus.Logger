using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace dotnetCampus.Logging.Composition
{
    /// <summary>
    /// 组合日志。提供各种不同输出的日志合集共同输出。
    /// </summary>
    public class CompositeLogger : ILogger, IEnumerable<ILogger>
    {
        private readonly Dictionary<ILogger, ILogger> _loggers;

        /// <summary>
        /// 创建组合日志的新实例。
        /// </summary>
        public CompositeLogger(params ILogger[] initialLoggers)
        {
            if (initialLoggers is null)
            {
                throw new ArgumentNullException(nameof(initialLoggers));
            }

            _loggers = new Dictionary<ILogger, ILogger>(initialLoggers.ToDictionary(x => x, x => x));
        }

        /// <summary>
        /// 获取 <see cref="_loggers"/> 操作专用的锁。
        /// </summary>
        private object LoggersLocker => _loggers;

        /// <summary>
        /// 向组合日志中添加日志实例。如果添加的日志已存在，会忽略而不会重复添加也不会出现异常。
        /// </summary>
        /// <param name="logger">要添加的日志实例。</param>
        public void Add(ILogger logger)
        {
            lock (LoggersLocker)
            {
                _loggers[logger] = logger ?? throw new ArgumentNullException(nameof(logger));
            }
        }

        /// <summary>
        /// 从组合日志中移除日志实例。
        /// </summary>
        /// <param name="logger">要移除的日志实例。</param>
        /// <returns>如果要移除的日志不存在，则返回 false；否则返回 true。</returns>
        public bool Remove(ILogger logger)
        {
            lock (LoggersLocker)
            {
                return _loggers.Remove(logger ?? throw new ArgumentNullException(nameof(logger)));
            }
        }

        /// <inheritdoc />
        IEnumerator<ILogger> IEnumerable<ILogger>.GetEnumerator()
        {
            lock (LoggersLocker)
            {
                return _loggers.Select(x => x.Value).ToList().GetEnumerator();
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<ILogger>)this).GetEnumerator();

        /// <summary>
        /// 转发日志到所有的子日志系统。
        /// </summary>
        /// <param name="context">当条日志上下文。</param>

        public void Log(in LogContext context)
        {
            lock (LoggersLocker)
            {
                foreach (var logger in _loggers)
                {
                    logger.Value.Log(context);
                }
            }
        }
    }
}
