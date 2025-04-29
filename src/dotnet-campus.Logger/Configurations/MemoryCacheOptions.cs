namespace dotnetCampus.Logging.Configurations;

/// <summary>
/// 当日志系统尚未初始化，但有模块已经开始记录日志时，此选项指定这些日志应如何缓存。
/// </summary>
public record MemoryCacheOptions
{
    /// <summary>
    /// 缓存的日志数量上限。超过此数量的日志将被丢弃。
    /// </summary>
    public int MaxCachedLogCount { get; init; } = 1024;

    /// <summary>
    /// 在日志系统初始化完成前，如果应用程序崩溃退出，则会将崩溃日志写入到此文件中。
    /// </summary>
    public string? EmergencyCrashLoggerFile { get; init; }
}
