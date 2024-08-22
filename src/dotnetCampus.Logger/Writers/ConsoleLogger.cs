using System;
using System.IO;
using dotnetCampus.Logging.Writers.Helpers;
using C = dotnetCampus.Logging.Writers.ConsoleLoggerHelpers.ConsoleColors;
using B = dotnetCampus.Logging.Writers.ConsoleLoggerHelpers.ConsoleColors.Background;
using D = dotnetCampus.Logging.Writers.ConsoleLoggerHelpers.ConsoleColors.Decoration;
using F = dotnetCampus.Logging.Writers.ConsoleLoggerHelpers.ConsoleColors.Foreground;

namespace dotnetCampus.Logging.Writers;

/// <summary>
/// 在控制台输出日志的日志记录器。
/// </summary>
public class ConsoleLogger : ILogger
{
    /// <summary>
    /// 控制台光标控制是否启用。目前可容纳的错误次数为 3 次，当降低到 0 次时，将不再尝试移动光标。
    /// </summary>
    private int _isCursorMovementEnabled = 3;

    private readonly RepeatLoggerDetector _repeat;

    /// <summary>
    /// 创建一个 <see cref="ConsoleLogger"/> 的新实例。
    /// </summary>
    /// <param name="threadMode">指定控制台日志的线程安全模式。</param>
    /// <param name="mainArgs">Main 方法的参数。</param>
    public ConsoleLogger(LogWritingThreadMode threadMode = LogWritingThreadMode.NotThreadSafe, string[]? mainArgs = null)
        : this(threadMode.CreateCoreLogWriter(), TagFilterManager.FromCommandLineArgs(mainArgs ?? []))
    {
    }

    internal ConsoleLogger(ICoreLogWriter coreWriter, TagFilterManager? tagManager)
    {
        _repeat = new RepeatLoggerDetector(ClearAndMoveToLastLine);
        CoreWriter = coreWriter;
        TagManager = tagManager;
    }

    /// <summary>
    /// 高于或等于此级别的日志才会被记录。
    /// </summary>
    public LogLevel Level { get; init; }

    /// <summary>
    /// 最终日志写入器。
    /// </summary>
    private ICoreLogWriter CoreWriter { get; }

    /// <summary>
    /// 管理控制台日志的标签过滤。
    /// </summary>
    private TagFilterManager? TagManager { get; }

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (logLevel < Level)
        {
            return;
        }

        var message = formatter(state, exception);
        if (TagManager?.IsTagEnabled(message) is false)
        {
            return;
        }

        var traceTag = TraceTag;
        var debugTag = DebugTag;
        var informationTag = InformationTag;
        var warningTag = WarningTag;
        var errorTag = ErrorTag;
        var criticalTag = CriticalTag;
        LogCore(logLevel, exception, message, m => logLevel switch
        {
            LogLevel.Trace => $"{traceTag} {TraceText}{m}{Reset}",
            LogLevel.Debug => $"{debugTag} {DebugText}{m}{Reset}",
            LogLevel.Information => $"{informationTag} {InformationText}{m}{Reset}",
            LogLevel.Warning => $"{warningTag} {WarningText}{m}{Reset}",
            LogLevel.Error => $"{errorTag} {ErrorText}{m}{Reset}",
            LogLevel.Critical => $"{criticalTag} {CriticalText}{m}{Reset}",
            _ => null,
        });
    }

    /// <summary>
    /// 记录日志。在必要的情况下会保证线程安全。
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="exception"></param>
    /// <param name="message"></param>
    /// <param name="formatter"></param>
    private void LogCore(LogLevel logLevel, Exception? exception, string message, Func<string, string?> formatter) => CoreWriter.Do(() =>
    {
        if (_repeat.RepeatOrResetLastLog(logLevel, message, exception) is var count and > 1)
        {
            ConsoleMultilineMessage($"上述日志已重复 {count} 次", formatter, true);
        }
        else if (exception is null)
        {
            ConsoleMultilineMessage(message, formatter);
        }
        else
        {
            var tag = logLevel switch
            {
                LogLevel.Warning => WarningExceptionTag,
                LogLevel.Error => ErrorExceptionTag,
                LogLevel.Critical => CriticalExceptionTag,
                _ => "",
            };
            ConsoleMultilineMessage($"""
                {message}
                {tag}{exception}
                """, formatter);
        }
    });

    /// <summary>
    /// 记录多行日志。
    /// </summary>
    /// <param name="message"></param>
    /// <param name="formatter"></param>
    /// <param name="forceSingleLine"></param>
    private void ConsoleMultilineMessage(string message, Func<string, string?> formatter, bool forceSingleLine = false)
    {
        if (forceSingleLine || !message.Contains('\n'))
        {
            Console.WriteLine(formatter(message));
        }
        else
        {
            using var reader = new StringReader(message);
            while (reader.ReadLine() is { } line)
            {
                Console.WriteLine(formatter(line));
            }
        }
    }

    /// <summary>
    /// 清空当前行并移动光标到上一行。
    /// </summary>
    /// <param name="repeatCount">此移动光标，是因为日志已重复第几次。</param>
    private void ClearAndMoveToLastLine(int repeatCount)
    {
        if (_isCursorMovementEnabled <= 0 || repeatCount <= 2)
        {
            // 如果光标控制不可用，或者还没有重复次数，则不尝试移动光标。
            return;
        }

        try
        {
            var desiredY = Console.CursorTop - 1;
            var y = Math.Clamp(desiredY, 0, Console.WindowHeight - 1);
            Console.SetCursorPosition(0, y);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, y);
        }
        catch (IOException)
        {
            // 日志记录时，如果无法移动光标，说明可能当前输出位置不在缓冲区内。
            // 如果多次尝试失败，则认为当前控制台缓冲区不支持光标移动，遂放弃。
            _isCursorMovementEnabled--;
        }
        catch (ArgumentException)
        {
            // 日志记录时，有可能已经移动到头了，就不要移动了。
        }
    }

    private const string Reset = C.Reset;
    private const string TraceText = F.Magenta;
    private const string DebugText = F.White;
    private const string InformationText = F.Green + D.Bold;
    private const string WarningText = F.Yellow;
    private const string ErrorText = F.BrightRed;
    private const string CriticalText = F.Red;

    private static string TraceTag => $"{B.Magenta}{F.White}[{DateTime.Now:HH:mm:ss.fff}]{Reset}";
    private static string DebugTag => $"{B.BrightBlack}{F.White}[{DateTime.Now:HH:mm:ss.fff}]{Reset}";
    private static string InformationTag => $"{B.Green}{F.Black}[{DateTime.Now:HH:mm:ss.fff}]{Reset}";
    private static string WarningTag => $"{B.Yellow}{F.Black}[{DateTime.Now:HH:mm:ss.fff}]{Reset}";
    private static string ErrorTag => $"{B.BrightRed}{F.Black}[{DateTime.Now:HH:mm:ss.fff}]{Reset}";
    private static string CriticalTag => $"{B.Red}{F.Black}[{DateTime.Now:HH:mm:ss.fff}]{Reset}";

    private static string WarningExceptionTag => $"{B.Yellow}{F.Black} ! {Reset}{WarningText} ";
    private static string ErrorExceptionTag => $"{B.BrightRed}{F.Black} X {Reset}{ErrorText} ";
    private static string CriticalExceptionTag => $"{B.Red}{F.Black} 💥 {Reset}{CriticalText} ";
}
