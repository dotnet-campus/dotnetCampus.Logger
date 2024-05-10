namespace dotnetCampus.Logging.Bridges;

/// <summary>
/// 表示一个日志桥对接器。
/// </summary>
public interface ILoggerBridgeLinker
{
    /// <summary>
    /// 将已指派给此日志桥连接器的所有日志桥对接到 <paramref name="logger"/> 日志记录器上。
    /// </summary>
    /// <param name="logger">要对接的日志记录器。</param>
    /// <exception cref="global::System.InvalidOperationException">
    /// 如果已经对接过日志记录器，则会抛出此异常。
    /// 如果希望针对不同的库对接不同的日志记录器，请编写多个聚合日志桥并分别导入各自的日志桥。
    /// </exception>
    void Link(ILogger logger);
}
