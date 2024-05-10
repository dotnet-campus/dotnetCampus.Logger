using dotnetCampus.Logging.Attributes;
using dotnetCampus.Logging.Configurations;
using dotnetCampus.Logging.Writers;

namespace LoggerSample.MainApp;

internal class Program
{
    public static void Main(string[] args)
    {
        // 这里是 Main 方法入口。

        // 以下初始化代码可能会较晚执行。
        new LoggerBuilder()
            .WithLevel(LogLevel.Information)
            .WithOptions(new LogOptions
            {
                LogLevel = LogLevel.Debug,
            })
            .AddWriter(new ConsoleLogger
            {
                // Options = new ConsoleLoggerOptions
                // {
                //     IncludeScopes = true,
                // },
            })
            .AddBridge(LoggerBridge.Default)
            .Build()
            .IntoGlobalStaticLog();
    }
}

[ImportLoggerBridge<global::LoggerSample.LoggerIndependentLibrary.Logging.ILoggerBridge>]
internal partial class LoggerBridge;
