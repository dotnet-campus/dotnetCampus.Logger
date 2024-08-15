using System;
using System.IO;
using System.Text;
using dotnetCampus.Logging.Writers.Helpers;
using C = dotnetCampus.Logging.Writers.ConsoleLoggerHelpers.ConsoleColors;
using B = dotnetCampus.Logging.Writers.ConsoleLoggerHelpers.ConsoleColors.Background;
using D = dotnetCampus.Logging.Writers.ConsoleLoggerHelpers.ConsoleColors.Decoration;
using F = dotnetCampus.Logging.Writers.ConsoleLoggerHelpers.ConsoleColors.Foreground;

namespace dotnetCampus.Logging.Writers;

public class ConsoleLogger : ILogger
{
    /// <summary>
    /// 控制台光标控制是否启用。目前可容纳的错误次数为 3 次，当降低到 0 次时，将不再尝试移动光标。
    /// </summary>
    private int _isCursorMovementEnabled = 3;

    private readonly RepeatLoggerDetector _repeat;
    private TagFilterManager? _tagFilterManager;

    /// <summary>
    /// 高于或等于此级别的日志才会被记录。
    /// </summary>
    public LogLevel Level { get; set; }

    public ConsoleLogger()
    {
        _repeat = new(ClearAndMoveToLastLine);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (logLevel < Level)
        {
            return;
        }

        var message = formatter(state, exception);
        if (_tagFilterManager?.IsTagEnabled(message) is false)
        {
            return;
        }

        LogCore(logLevel, exception, message, m => logLevel switch
        {
            LogLevel.Trace => $"{TraceTag} {TraceText}{m}{Reset}",
            LogLevel.Debug => $"{DebugTag} {DebugText}{m}{Reset}",
            LogLevel.Information => $"{InformationTag} {InformationText}{m}{Reset}",
            LogLevel.Warning => $"{WarningTag} {WarningText}{m}{Reset}",
            LogLevel.Error => $"{ErrorTag} {ErrorText}{m}{Reset}",
            LogLevel.Critical => $"{CriticalTag} {CriticalText}{m}{Reset}",
            _ => null,
        });
    }

    private void LogCore(LogLevel logLevel, Exception? exception, string message, Func<string, string?> formatter)
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
    }

    private static void ConsoleMultilineMessage(string message, Func<string, string?> formatter, bool forceSingleLine = false)
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
    /// 高于或等于此级别的日志才会被记录。
    /// </summary>
    public ConsoleLogger UseLevel(LogLevel level)
    {
        Level = level;
        return this;
    }

    /// <summary>
    /// 从命令行参数中提取过滤标签。
    /// </summary>
    /// <param name="args">命令行参数。</param>
    public ConsoleLogger FilterConsoleTagsFromCommandLineArgs(string[] args)
    {
        _tagFilterManager = TagFilterManager.FromCommandLineArgs(args);
        return this;
    }

    private void ClearAndMoveToLastLine(int repeatCount)
    {
        if (_isCursorMovementEnabled > 0 && repeatCount > 2)
        {
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
    }

    private const string Reset = C.Reset;
    private const string DebugText = F.White;
    private const string TraceText = F.BrightBlack;
    private const string InformationText = F.Green + D.Bold;
    private const string WarningText = F.Yellow;
    private const string ErrorText = F.BrightRed;
    private const string CriticalText = F.Red;

    private static string TraceTag => $"{B.Black}{F.BrightBlack}[{DateTime.Now:HH:mm:ss.fff}]{Reset}";
    private static string DebugTag => $"{B.BrightBlack}{F.White}[{DateTime.Now:HH:mm:ss.fff}]{Reset}";
    private static string InformationTag => $"{B.Green}{F.Black}[{DateTime.Now:HH:mm:ss.fff}]{Reset}";
    private static string WarningTag => $"{B.Yellow}{F.Black}[{DateTime.Now:HH:mm:ss.fff}]{Reset}";
    private static string ErrorTag => $"{B.BrightRed}{F.Black}[{DateTime.Now:HH:mm:ss.fff}]{Reset}";
    private static string CriticalTag => $"{B.Red}{F.Black}[{DateTime.Now:HH:mm:ss.fff}]{Reset}";

    private static string WarningExceptionTag => $"{B.Yellow}{F.Black} ! {Reset}{WarningText} ";
    private static string ErrorExceptionTag => $"{B.BrightRed}{F.Black} X {Reset}{ErrorText} ";
    private static string CriticalExceptionTag => $"{B.Red}{F.Black} 💥 {Reset}{CriticalText} ";
}
