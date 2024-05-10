using dotnetCampus.Logging.Attributes;
using dotnetCampus.Logging.Writers;

namespace LoggerSample.MainApp;

internal class Program
{
    public static void Main(string[] args)
    {
        // 这里是 Main 方法入口。

        // 以下初始化代码可能会较晚执行。
        new LoggerBuilder()
            .UseLevel(LogLevel.Information)
            .AddWriter(new ConsoleLogger())
            .AddBridge(LoggerBridge.Default)
            .BuildIntoStaticLog();
    }
}

[ImportLoggerBridge<global::LoggerSample.LoggerIndependentLibrary.Logging.ILoggerBridge>]
internal partial class LoggerBridge;
