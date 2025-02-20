using System;
using System.Threading.Tasks;

#if NET6_0_OR_GREATER
using System.Threading.Channels;
#else
using System.Collections.Concurrent;
#endif

namespace dotnetCampus.Logging.Writers.Helpers;

/// <summary>
/// 提供各种不同线程安全方式的最终日志写入功能。
/// </summary>
public interface ICoreLogWriter
{
    /// <summary>
    /// 写入日志。
    /// </summary>
    /// <param name="message">日志消息。为 <see langword="null"/> 时不写入。</param>
    void Write(string? message);

    /// <summary>
    /// 插入一个动作。
    /// </summary>
    /// <param name="action">动作。</param>
    void Do(Action action);
}

/// <summary>
/// 不处理线程安全问题的日志写入器。
/// </summary>
internal sealed class NotThreadSafeLogWriter(Action<string> logger) : ICoreLogWriter
{
    /// <inheritdoc />
    public void Write(string? message)
    {
        if (message is not null)
        {
            logger(message);
        }
    }

    /// <inheritdoc />
    public void Do(Action action)
    {
        action();
    }
}

/// <summary>
/// 使用锁来保证线程安全的日志写入器。
/// </summary>
internal sealed class LockLogWriter(Action<string> logger) : ICoreLogWriter
{
    private readonly object _lock = new();

    /// <inheritdoc />
    public void Write(string? message)
    {
        if (message is not null)
        {
            lock (_lock)
            {
                logger(message);
            }
        }
    }

    /// <inheritdoc />
    public void Do(Action action)
    {
        lock (_lock)
        {
            action();
        }
    }
}

/// <summary>
/// 使用生产者消费者模式的日志写入器。
/// </summary>
internal sealed class ProducerConsumerLogWriter : ICoreLogWriter
{
    private readonly Action<string> _logger;
#if NET6_0_OR_GREATER
    private readonly Channel<object> _queue = Channel.CreateUnbounded<object>(new UnboundedChannelOptions
    {
        SingleReader = true,
        SingleWriter = false,
        AllowSynchronousContinuations = false,
    });
#else
    private readonly BlockingCollection<object> _queue = new();
#endif

    /// <summary>
    /// 创建 <see cref="ProducerConsumerLogWriter"/> 的新实例，并启动消费线程。
    /// </summary>
    public ProducerConsumerLogWriter(Action<string> logger)
    {
        _logger = logger;
#if NET6_0_OR_GREATER
        _ = Task.Run(Consume);
#else
        new Task(Consume, TaskCreationOptions.LongRunning).Start();
#endif
    }

    /// <inheritdoc />
    public void Write(string? message)
    {
        if (message is not null)
        {
#if NET6_0_OR_GREATER
            _queue.Writer.TryWrite(message);
#else
            _queue.Add(message);
#endif
        }
    }

    /// <inheritdoc />
    public void Do(Action action)
    {
#if NET6_0_OR_GREATER
        _queue.Writer.TryWrite(action);
#else
        _queue.Add(action);
#endif
    }

#if NET6_0_OR_GREATER
    /// <summary>
    /// 消费队列中的元素。
    /// </summary>
    private async Task Consume()
    {
        while (true)
        {
            var success = await _queue.Reader.WaitToReadAsync();
            if (!success)
            {
                break;
            }

            while (_queue.Reader.TryRead(out var item))
            {
                try
                {
                    switch (item)
                    {
                        case string message:
                            _logger(message);
                            break;
                        case Action action:
                            action();
                            break;
                    }
                }
                catch (Exception)
                {
                    // 本次日志发生了异常，已经无法继续写入日志，只能抛弃异常。
                }
            }
        }
    }
#else
    /// <summary>
    /// 消费队列中的元素。
    /// </summary>
    private void Consume()
    {
        foreach (var item in _queue.GetConsumingEnumerable())
        {
            switch (item)
            {
                case string message:
                    _logger(message);
                    break;
                case Action action:
                    action();
                    break;
            }
        }
    }
#endif
}
