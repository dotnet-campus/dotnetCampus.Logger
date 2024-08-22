using System;
using dotnetCampus.Logging.Writers.Helpers;

namespace dotnetCampus.Logging.Writers;

/// <summary>
/// 表示如何管理日志的写入线程。
/// </summary>
public enum LogWritingThreadMode
{
    /// <summary>
    /// 在哪个线程调用日志，就在哪个线程写入日志。不处理线程安全问题。
    /// </summary>
    NotThreadSafe,

    /// <summary>
    /// 使用锁来保证线程安全。
    /// </summary>
    Lock,

    /// <summary>
    /// 使用生产者消费者模式，将日志写入到队列中，由后台线程消费。
    /// </summary>
    /// <remarks>
    /// 截至目前，使用此方法难以保证在程序退出时，所有日志都能被写入。
    /// </remarks>
    ProducerConsumer,
}

/// <summary>
/// 包含 <see cref="LogWritingThreadMode"/> 的扩展方法。
/// </summary>
internal static class LogWritingThreadModeExtensions
{
    /// <summary>
    /// 根据 <see cref="LogWritingThreadMode"/> 创建对应的 <see cref="ICoreLogWriter"/> 实例。
    /// </summary>
    /// <param name="threadMode">线程安全模式。</param>
    /// <returns>最终日志写入器。</returns>
    /// <exception cref="ArgumentOutOfRangeException">当线程安全模式不支持时抛出。</exception>
    public static ICoreLogWriter CreateCoreLogWriter(this LogWritingThreadMode threadMode) => threadMode switch
    {
        LogWritingThreadMode.NotThreadSafe => new NotThreadSafeLogWriter(),
        LogWritingThreadMode.Lock => new LockLogWriter(),
        LogWritingThreadMode.ProducerConsumer => new ProducerConsumerLogWriter(),
        _ => throw new ArgumentOutOfRangeException(nameof(threadMode)),
    };
}
