using System;
using System.Runtime.CompilerServices;

namespace dotnetCampus.Logging.Core
{
    /// <summary>
    /// 为同步的日志记录提供公共基类。
    /// </summary>
    public abstract class OutputLogger : ILogger
    {
        private readonly object _locker = new object();
        private bool _isInitialized;

        /// <summary>
        /// 获取或设置日志的记录等级。
        /// 你可以在日志记录的过程当中随时修改日志等级，修改后会立刻生效。
        /// 默认是所有调用日志记录的方法都全部记录。
        /// </summary>
        public virtual LogLevel Level { get; set; } = LogLevel.Message;

        /// <summary>
        /// 使用底层的日志记录方法来异步记录日志。
        /// </summary>
        /// <param name="context">当条日志上下文。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Log(in LogContext context)
        {
            if (string.IsNullOrWhiteSpace(context.CallerMemberName))
            {
                throw new ArgumentException("不允许显式将 CallerMemberName 指定成 null 或空字符串。", nameof(LogContext.CallerMemberName));
            }

            if (Level < context.CurrentLevel)
            {
                return;
            }

            if (!_isInitialized)
            {
                lock (_locker)
                {
                    if (!_isInitialized)
                    {
                        _isInitialized = true;
                        OnInitialized();
                    }
                }
            }

            lock (_locker)
            {
                OnLogReceived(context);
            }
        }

        /// <summary>
        /// 派生类重写此方法时，可以在收到第一条日志的时候执行一些初始化操作。
        /// </summary>
        protected abstract void OnInitialized();

        /// <summary>
        /// 派生类重写此方法时，将日志输出。
        /// </summary>
        /// <param name="context">包含一条日志的所有上下文信息。</param>
        protected abstract void OnLogReceived(in LogContext context);
    }
}
