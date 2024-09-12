#nullable enable

namespace dotnetCampus.Logging;

/// <summary>
/// 定义日志的严重程度。
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// 包含最详细消息的日志。这些消息可能包含敏感的应用程序数据。默认情况下禁用这些消息，不应在生产环境中启用。
    /// </summary>
    Trace = 0,

    /// <summary>
    /// 用于开发过程中的交互式调查的日志。这些日志应主要包含有用于调试的信息，没有长期价值。
    /// </summary>
    Debug = 1,

    /// <summary>
    /// 跟踪应用程序的一般流程的日志。这些日志应具有长期价值。
    /// </summary>
    Information = 2,

    /// <summary>
    /// 强调应用程序流程中的异常或意外事件的日志，但不会导致应用程序执行停止。
    /// </summary>
    Warning = 3,

    /// <summary>
    /// 强调当前执行流程由于失败而停止的日志。这些日志应指示当前活动的失败，而不是应用程序范围的失败。
    /// </summary>
    Error = 4,

    /// <summary>
    /// 描述不可恢复的应用程序或系统崩溃，或需要立即处理的灾难性失败的日志。
    /// </summary>
    Critical = 5,

    /// <summary>
    /// 此严重程度不用于写入日志消息，配置成此级别仅表示不会写入任何日志。
    /// </summary>
    None = 6,
}

/// <summary>
/// 辅助将字符串解析为日志级别。
/// </summary>
public static class LogLevelParser
{
    /// <summary>
    /// 尝试解析字符串为日志级别，支持常用的日志级别别名，大小写不敏感。
    /// </summary>
    /// <param name="text">要解析的字符串。</param>
    /// <returns>日志级别。</returns>
    /// <remarks>
    /// 目前已支持的别名有：
    /// <list type="bullet">
    /// <item><description>追踪级：0, trace, tracing</description></item>
    /// <item><description>调试级：1, debug, debugging</description></item>
    /// <item><description>一般级：2, info, information</description></item>
    /// <item><description>警告级：3, warn, warning</description></item>
    /// <item><description>错误级：4, err, error</description></item>
    /// <item><description>崩溃级：5, critical, fatal</description></item>
    /// <item><description>无日志：6, no, none</description></item>
    /// </list>
    /// 其他所有字符串均返回 <see langword="null"/>。
    /// </remarks>
    public static LogLevel? Parse(string text) => text.ToLowerInvariant() switch
    {
        "trace" => LogLevel.Trace,
        "tracing" => LogLevel.Trace,
        "debug" => LogLevel.Debug,
        "debugging" => LogLevel.Debug,
        "info" => LogLevel.Information,
        "information" => LogLevel.Information,
        "warn" => LogLevel.Warning,
        "warning" => LogLevel.Warning,
        "err" => LogLevel.Error,
        "error" => LogLevel.Error,
        "critical" => LogLevel.Critical,
        "fatal" => LogLevel.Critical,
        "no" => LogLevel.None,
        "none" => LogLevel.None,
        "0" => LogLevel.Trace,
        "1" => LogLevel.Debug,
        "2" => LogLevel.Information,
        "3" => LogLevel.Warning,
        "4" => LogLevel.Error,
        "5" => LogLevel.Critical,
        "6" => LogLevel.None,
        _ => null,
    };
}
