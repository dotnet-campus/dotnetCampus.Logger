using System;
using System.Collections.Generic;
using dotnetCampus.Logging.Bridges;
using dotnetCampus.Logging.Configurations;
using dotnetCampus.Logging.Writers;

namespace dotnetCampus.Logging;

/// <summary>
/// 辅助创建日志记录器的构建器。
/// </summary>
public class LoggerBuilder
{
    private LogOptions? _options;
    private readonly List<ILogger> _writers = [];
    private readonly List<ILoggerBridgeLinker> _linkers = [];

    /// <summary>
    /// 调用此方法以便在日志模块初始化完成前先对所有记录的日志进行缓存，以便在日志模块初始化完成后再将缓存的日志写入到日志文件中。
    /// </summary>
    /// <param name="flusher">
    /// 在日志模块初始化完成后，将缓存的日志写入到日志文件中。
    /// 如果在 Program.cs 类中，请直接传入源生成器生成的 Log 属性作为此参数。
    /// </param>
    /// <remarks>
    /// 此方法不会在运行时起任何作用，仅决定编译时在 Program.cs 类中所生成的日志记录器。生成后，你可以在 Program.cs 类中使用 Log.Info 等方法进行日志记录。
    /// </remarks>
    public LoggerBuilder UseMemoryCache(Action<ILogger> flusher)
    {
        return this;
    }

    public LoggerBuilder WithLevel(LogLevel level)
    {
        _options ??= new LogOptions();
        _options.LogLevel = level;
        return this;
    }

    public LoggerBuilder WithOptions(LogOptions options)
    {
        _options = options;
        return this;
    }

    public LoggerBuilder AddWriter(ILogger writer)
    {
        _writers.Add(writer);
        return this;
    }

    public LoggerBuilder AddBridge(ILoggerBridgeLinker linker)
    {
        _linkers.Add(linker);
        return this;
    }

    public LoggerBuilder<CompositeLogger> Build()
    {
        var logger = new CompositeLogger(_options ?? new LogOptions())
        {
            Writers = [.._writers],
        };
        foreach (var linker in _linkers)
        {
            linker.Link(logger);
        }
        return new LoggerBuilder<CompositeLogger>(logger);
    }
}

/// <summary>
/// 包含已经创建完成的日志记录器，日志记录器初始状态已不可再更改，但可以继续构建以设置日志记录器的其他行为。
/// </summary>
/// <param name="logger">已经创建完成的日志记录器。</param>
/// <typeparam name="T">日志记录器的类型。</typeparam>
public sealed class LoggerBuilder<T>(T logger) where T : ILogger
{
    /// <summary>
    /// 获取创建好的日志记录器。
    /// </summary>
    public T Logger => logger;

    /// <summary>
    /// 将创建好的日志记录器设置为全局日志记录器。
    /// </summary>
    /// <returns></returns>
    public T IntoGlobalStaticLog()
    {
        Log.SetLogger(logger);
        return logger;
    }

    /// <summary>
    /// 隐式将创建好的日志记录器转换为日志记录器实例。
    /// </summary>
    /// <param name="builder">要转换的日志记录器构建器。</param>
    /// <returns>已经创建好的日志记录器。</returns>
    public static implicit operator T(LoggerBuilder<T> builder) => builder.Logger;
}

partial class Log
{
    /// <summary>
    /// 仅供内部使用，将指定的日志记录器设置为全局日志记录器。
    /// </summary>
    /// <param name="logger">要设置为全局的日志记录器。</param>
    internal static void SetLogger(ILogger logger)
    {
        var oldLogger = Current;
        if (oldLogger is MemoryCacheLogger mcl)
        {
            mcl.Flush(logger);
        }
        Current = logger;
    }
}
