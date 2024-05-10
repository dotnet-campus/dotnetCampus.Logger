using dotnetCampus.Logging.Attributes;
using dotnetCampus.Logging.Configurations;
using dotnetCampus.Logging.Writers;
using LoggerSample.MainApp.Logging;

namespace LoggerSample.MainApp;

internal partial class Program
{
    public static void Main(string[] args)
    {
        // 这里是 Main 方法入口。
        Log.Info($"[App] App started with args: {string.Join(", ", args)}");

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
            .AddBridge(LoggerBridgeLinker.Default)
            .Build()
            .IntoGlobalStaticLog();
    }
}

[ImportLoggerBridge<global::LoggerSample.LoggerIndependentLibrary.Logging.ILoggerBridge>]
[ImportLoggerBridge<global::LoggerSample.LoggerIndependentProject.Logging.ILoggerBridge>]
[ImportLoggerBridge<global::LoggerSample.MainApp.Logging.ILoggerBridge>]
internal partial class LoggerBridgeLinker;
