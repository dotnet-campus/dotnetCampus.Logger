namespace DotNetCampus.Logging.Configurations;

/// <summary>
/// 用于配置日志记录器的选项。
/// </summary>
public record  LogOptions
{
    /// <summary>
    /// 高于或等于此级别的日志才会被记录。
    /// </summary>
    public LogLevel LogLevel { get; set; }
}
