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
            if (count > 2)
            {
                // 仅当重复次数大于 2 时，才会向上移动光标，这是因为：
                //  1. 当本次为第 0 次重复时，尚未输出
                //  2. 当本次为第 1 次重复时，首次输出
                //  3. 当本次为第 2 次重复时，输出“重复日志行”
                //  4. 当本次为第 >2 次重复时，将向上移动光标，顶替掉上次的“重复日志行”
                whenRepeated(count);
                return count;
            }
            else
            {
                return count;
            }
        }

        // 不同日志，设置新重复状态。
        _lastSameItemCount = 1;
        _lastItem = new(level, message, exception);
        return 1;
    }

    private readonly record struct LogItem(LogLevel Level, string Message, Exception? Exception);
}
