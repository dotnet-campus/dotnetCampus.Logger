using System;
using System.Threading;

namespace DotNetCampus.Logging.Writers.Helpers;

internal class RepeatLoggerDetector(Action<int> whenRepeated)
{
    private static volatile int _lastSameItemCount;
    private static LogItem? _lastItem;

    internal int RepeatOrResetLastLog(LogLevel level, string message, Exception? exception)
    {
        var (lastLevel, lastMessage, lastException) = _lastItem ?? new(default(LogLevel), null!, null);
        if (level == lastLevel && message == lastMessage && exception == lastException)
        {
            // 相同日志，标记重复。
            var count = Interlocked.Increment(ref _lastSameItemCount);
            whenRepeated(count);
            return count;
        }

        // 不同日志，设置新重复状态。
        _lastSameItemCount = 1;
        _lastItem = new(level, message, exception);
        return 1;
    }

    private readonly record struct LogItem(LogLevel Level, string Message, Exception? Exception);
}
