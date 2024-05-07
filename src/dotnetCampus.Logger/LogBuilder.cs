using System;
using System.Collections.Generic;
using dotnetCampus.Logging.Configurations;

namespace dotnetCampus.Logging;

public class LoggerBuilder
{
    private LogOptions? _options;
    private readonly List<ILogger> _writers = [];
    private Action<ILogger>? _flusher;

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
        _flusher = flusher;
        return this;
    }

    public LoggerBuilder UseLevel(LogLevel level)
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

    public CompositeLogger Build()
    {
        var logger = new CompositeLogger(_options ?? new LogOptions())
        {
            Writers = [.._writers],
        };
        if (_flusher is { } flusher)
        {
            flusher(logger);
        }
        return logger;
    }
}
