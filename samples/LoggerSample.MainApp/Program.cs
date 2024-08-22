using System;
using System.Threading.Tasks;
using dotnetCampus.Logging.Attributes;
using dotnetCampus.Logging.Configurations;
using dotnetCampus.Logging.Writers;

namespace LoggerSample.MainApp;

internal class Program
{
    public static void Main(string[] args)
    {
        // 这里是 Main 方法入口。
        Log.Info($"[App] App started with args: {string.Join(", ", args)}");

        // 以下初始化代码可能会较晚执行。
        new LoggerBuilder()
            .WithMemoryCache(new MemoryCacheOptions
            {
            })
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

        Run();
    }

    private static void Run()
    {
        Parallel.For(0, 0x00010000, i =>
        {
            Log.Debug($"[TEST] {DateTime.Now:HH:mm:ss}");
        });
    }
}

[ImportLoggerBridge<global::LoggerSample.LoggerIndependentLibrary.Logging.ILoggerBridge>]
[ImportLoggerBridge<global::LoggerSample.LoggerIndependentProject.Logging.ILoggerBridge>]
internal partial class LoggerBridgeLinker;
