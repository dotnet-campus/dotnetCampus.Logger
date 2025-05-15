using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DotNetCampus.Logging;
using DotNetCampus.Logging.Attributes;
using DotNetCampus.Logging.Configurations;
using DotNetCampus.Logging.Writers;

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
                LogLevel = LogLevel.Trace,
            })
            .AddConsoleLogger(b => b
                .WithThreadSafe(LogWritingThreadMode.ProducerConsumer)
                .FilterConsoleTagsFromCommandLineArgs(args))
            .AddBridge(LoggerBridgeLinker.Default)
            .Build()
            .IntoGlobalStaticLog();

        Log.Trace("Trace log");
        Log.Debug("Debug log");
        Log.Info("Info log");
        Log.Warn("Warn log");
        Log.Error("Error log");
        Log.Fatal("Fatal log");

        Log.Trace("""
            This is a very very long trace log
            written in multiline.
            """);
        Log.Debug("""
            This is a very very long debug log
            written in multiline.
            """);
        Log.Info("""
            This is a very very long information log
            written in multiline.
            """);
        Log.Warn("""
            This is a very very long warning log
            written in multiline.
            """);
        Log.Error("""
            This is a very very long error log
            written in multiline.
            """);
        Log.Fatal("""
            This is a very very long critical log
            written in multiline.
            """);
        Log.Fatal("""
            This is a very very long critical log
            written in multiline.
            """);

        var exception = GetException();
        Log.Warn("Ah..., exception!", exception);
        Log.Error("Ah..., exception!", exception);
        Log.Fatal("Ah..., exception!", exception);
        Log.Fatal("Ah..., exception!", exception);

        Run();
        Thread.Sleep(5000);
    }

    private static void Run()
    {
        var stopwatch = Stopwatch.StartNew();
        Log.Info($"[TEST] 开始 {stopwatch.ElapsedMilliseconds}ms");
        Parallel.For(0, 0x00001000, i =>
        {
            Thread.Sleep(1);
            Log.Info($"[TEST] {DateTime.Now:HH:mm:ss}");
        });
        Log.Info($"[TEST] 完成 {stopwatch.ElapsedMilliseconds}ms");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Exception GetException()
    {
        try
        {
            return ThrowException();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Exception ThrowException()
    {
        throw new InvalidOperationException();
    }
}

[ImportLoggerBridge<global::LoggerSample.LoggerIndependentLibrary.Logging.ILoggerBridge>]
[ImportLoggerBridge<global::LoggerSample.LoggerIndependentProject.Logging.ILoggerBridge>]
internal partial class LoggerBridgeLinker;
