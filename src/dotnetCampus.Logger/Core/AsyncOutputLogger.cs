
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace dotnetCampus.Logging.Core
{
    /// <summary>
    /// 为异步的日志记录提供公共基类。
    /// </summary>
    public abstract partial class AsyncOutputLogger : ILogger
    {
        private readonly AsyncQueue<LogContext> _queue;
        private bool _isInitialized;
        private CancellationTokenSource _waitForEmptyCancellationTokenSource = new CancellationTokenSource();
        private TaskCompletionSource<object?>? _waitForEmptyTaskCompletionSource;
        private object _waitForEmptyLocker = new object();

        /// <summary>
        /// 创建 Markdown 格式的日志记录实例。
        /// </summary>
        protected AsyncOutputLogger()
        {
            _queue = new AsyncQueue<LogContext>();
            StartLogging();
        }

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

            _queue.Enqueue(context);
        }

        /// <summary>
        /// 开始异步输出日志。
        /// </summary>
        private async void StartLogging()
        {
            while (true)
            {
                LogContext context;

                try
                {
                    context = await _queue.DequeueAsync(_waitForEmptyCancellationTokenSource.Token).ConfigureAwait(false);
                    await Write(context).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    while (_queue.Count > 0)
                    {
                        context = await _queue.DequeueAsync().ConfigureAwait(false);
                        await Write(context).ConfigureAwait(false);
                    }
                }

                if (_queue.Count == 0)
                {
                    _waitForEmptyCancellationTokenSource.Dispose();
                    _waitForEmptyCancellationTokenSource = new CancellationTokenSource();
                    _waitForEmptyTaskCompletionSource?.SetResult(null);
                }
            }

            async Task Write(LogContext context)
            {
                if (!_isInitialized)
                {
                    _isInitialized = true;
                    await OnInitializedAsync().ConfigureAwait(false);
                }
                OnLogReceived(context);
            }
        }

        /// <summary>
        /// 派生类重写此方法时，可以在收到第一条日志的时候执行一些初始化操作。
        /// </summary>
        protected abstract Task OnInitializedAsync();

        /// <summary>
        /// 派生类重写此方法时，将日志输出。
        /// </summary>
        /// <param name="context">包含一条日志的所有上下文信息。</param>
        protected abstract void OnLogReceived(in LogContext context);

        /// <summary>
        /// 如果派生类需要等待当前尚未完成日志输出的日志全部完成输出，则调用此方法。
        /// 但请注意：因为并发问题，如果等待期间还有新写入的日志，那么也会一并等待。
        /// </summary>
        /// <returns>可等待对象。</returns>
        protected async Task WaitFlushingAsync()
        {
            if (_waitForEmptyTaskCompletionSource is null)
            {
                lock (_waitForEmptyLocker)
                {
                    if (_waitForEmptyTaskCompletionSource is null)
                    {
                        _waitForEmptyTaskCompletionSource = new TaskCompletionSource<object?>();
                        _waitForEmptyCancellationTokenSource.Cancel();
                    }
                    else if (_waitForEmptyTaskCompletionSource.Task.IsCompleted)
                    {
                        return;
                    }
                }
            }

            await _waitForEmptyTaskCompletionSource.Task.ConfigureAwait(false);

            lock (_waitForEmptyLocker)
            {
                _waitForEmptyTaskCompletionSource = null;
            }
        }
    }
}
