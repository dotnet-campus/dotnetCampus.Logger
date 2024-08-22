using System;
using System.Threading;
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
            .AddConsoleLogger(b => b
                .WithThreadSafe(LogWritingThreadMode.ProducerConsumer)
                .FilterConsoleTagsFromCommandLineArgs(args))
            .AddBridge(LoggerBridgeLinker.Default)
            .Build()
            .IntoGlobalStaticLog();

        Run();
        Thread.Sleep(5000);
    }

    private static void Run()
    {
        Log.Debug("[TEST] 开始");
        Parallel.For(0, 0x00004000, i =>
        {
            Thread.Sleep(1);
            Log.Debug($"[TEST] {DateTime.Now:HH:mm:ss}");
        });
        Log.Debug("[TEST] 完成");
    }
}

[ImportLoggerBridge<global::LoggerSample.LoggerIndependentLibrary.Logging.ILoggerBridge>]
[ImportLoggerBridge<global::LoggerSample.LoggerIndependentProject.Logging.ILoggerBridge>]
internal partial class LoggerBridgeLinker;
