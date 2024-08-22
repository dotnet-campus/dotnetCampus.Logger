using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace dotnetCampus.Logging.Writers.Helpers;

/// <summary>
/// 提供各种不同线程安全方式的最终日志写入功能。
/// </summary>
internal interface ICoreLogWriter
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
internal sealed class NotThreadSafeLogWriter : ICoreLogWriter
{
    /// <inheritdoc />
    public void Write(string? message)
    {
        if (message is not null)
        {
            Console.WriteLine(message);
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
internal sealed class LockLogWriter : ICoreLogWriter
{
    private readonly object _lock = new();

    /// <inheritdoc />
    public void Write(string? message)
    {
        if (message is not null)
        {
            lock (_lock)
            {
                Console.WriteLine(message);
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
    private readonly BlockingCollection<object> _queue = new();

    /// <summary>
    /// 创建 <see cref="ProducerConsumerLogWriter"/> 的新实例，并启动消费线程。
    /// </summary>
    public ProducerConsumerLogWriter()
    {
        new Task(Consume, TaskCreationOptions.LongRunning).Start();
    }

    /// <inheritdoc />
    public void Write(string? message)
    {
        if (message is not null)
        {
            _queue.Add(message);
        }
    }

    /// <inheritdoc />
    public void Do(Action action)
    {
        _queue.Add(action);
    }

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
                    Console.WriteLine(message);
                    break;
                case Action action:
                    action();
                    break;
            }
        }
    }
}
