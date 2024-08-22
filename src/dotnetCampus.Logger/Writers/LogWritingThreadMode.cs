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
