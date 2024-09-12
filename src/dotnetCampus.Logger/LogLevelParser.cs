namespace dotnetCampus.Logging;

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
