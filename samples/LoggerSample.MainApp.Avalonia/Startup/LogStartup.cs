using System;
using Avalonia;
using dotnetCampus.Logging;

namespace dotnetCampus.LoggerSample.Startup;

public static class LogStartup
{
    public static AppBuilder UseLogger(this AppBuilder appBuilder, Action<LoggerBuilder> builder)
    {
        var b = new LoggerBuilder();
        builder(b);
        return appBuilder.LogToTrace();
    }
}
