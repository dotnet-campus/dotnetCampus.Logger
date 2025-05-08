using System;
using System.IO;
using System.Text;
using DotNetCampus.Logging.Writers.ConsoleLoggerHelpers;
using DotNetCampus.Logging.Writers.Helpers;
using C = DotNetCampus.Logging.Writers.ConsoleLoggerHelpers.ConsoleColors;
using B = DotNetCampus.Logging.Writers.ConsoleLoggerHelpers.ConsoleColors.Background;
using D = DotNetCampus.Logging.Writers.ConsoleLoggerHelpers.ConsoleColors.Decoration;
using F = DotNetCampus.Logging.Writers.ConsoleLoggerHelpers.ConsoleColors.Foreground;

namespace DotNetCampus.Logging.Writers;

/// <summary>
/// 在控制台输出日志的日志记录器。
/// </summary>
public class ConsoleLogger : ILogger
{
    /// <summary>
    /// 控制台光标控制是否启用。
    /// </summary>
    private readonly bool _isCursorMovementEnabled;
    private readonly RepeatLoggerDetector _repeat;
    private static bool _isConsoleOutput;
    private static readonly TextWriter Out = GetStandardOutputWriter();

    /// <summary>
    /// 创建一个 <see cref="ConsoleLogger"/> 的新实例。
    /// </summary>
    /// <param name="threadMode">指定控制台日志的线程安全模式。</param>
    /// <param name="mainArgs">Main 方法的参数。</param>
    public ConsoleLogger(LogWritingThreadMode threadMode = LogWritingThreadMode.NotThreadSafe, string[]? mainArgs = null)
        : this(threadMode.CreateCoreLogWriter(SafeWriteLine), TagFilterManager.FromCommandLineArgs(mainArgs ?? []))
    {
    }

    internal ConsoleLogger(ICoreLogWriter coreWriter, TagFilterManager? tagManager)
    {
        _repeat = new RepeatLoggerDetector(ClearAndMoveToLastLine);
        CoreWriter = coreWriter;
        TagManager = tagManager;
        _isConsoleOutput = Out == Console.Out;
        var success = ConsoleInitializer.Initialize();
        // 如果输出流是自己创建的，则不支持光标移动。
        _isCursorMovementEnabled = _isConsoleOutput && success;
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

    public bool IsEnabled(LogLevel logLevel)
    {
        if (logLevel < Level)
        {
            return false;
        }

        return true;
    }

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

        LogCore(logLevel, exception, message, logLevel switch
        {
            LogLevel.Trace => $"{TraceTag} {TraceText}",
            LogLevel.Debug => $"{DebugTag} {DebugText}",
            LogLevel.Information => $"{InformationTag} {InformationText}",
            LogLevel.Warning => $"{WarningTag} {WarningText}",
            LogLevel.Error => $"{ErrorTag} {ErrorText}",
            LogLevel.Critical => $"{CriticalTag}{CriticalText} ",
            _ => $"{EmptyTag} ",
        }, logLevel switch
        {
            LogLevel.Trace => $"{TraceEmptyTag} {TraceText}",
            LogLevel.Debug => $"{DebugEmptyTag} {DebugText}",
            LogLevel.Information => $"{InformationEmptyTag} {InformationText}",
            LogLevel.Warning => $"{WarningEmptyTag} {WarningText}",
            LogLevel.Error => $"{ErrorEmptyTag} {ErrorText}",
            LogLevel.Critical => $"{CriticalEmptyTag}{CriticalText} ",
            _ => $"{EmptyTag} ",
        });
    }

    /// <summary>
    /// 记录日志。在必要的情况下会保证线程安全。
    /// </summary>
    /// <param name="logLevel">日志严重程度。</param>
    /// <param name="exception">如果有异常，则记录异常。</param>
    /// <param name="message">日志的消息内容。</param>
    /// <param name="firstLineStyle">日志的首行样式（如果只有一行，也使用此样式）。</param>
    /// <param name="otherLineStyle">日志的其他行样式。</param>
    private void LogCore(LogLevel logLevel, Exception? exception, string message, string firstLineStyle, string otherLineStyle) => CoreWriter.Do(() =>
    {
        if (_repeat.RepeatOrResetLastLog(logLevel, message, exception) is var count and > 1)
        {
            if (_isConsoleOutput)
            {
                string SingleLineFormatter(string m, int i) => $"{firstLineStyle}{m}{Reset}";
                ConsoleMultilineMessage($"{Reset}{F.BrightBlack}上述日志已重复 {count} 次{Reset}", SingleLineFormatter, forceSingleLine: true);
            }
            else
            {
                string AppendFirstLineFormatter(string m, int i) => $"{firstLineStyle}{m} {Reset}{F.BrightBlack}(重复 {count} 次){Reset}";

                string OtherLineFormatter(string m, int i) => logLevel is LogLevel.Critical
                    ? $"{otherLineStyle}{m.PadRight(SafeGetSpaceCount(" ", EmptyTag))}{Reset}"
                    : $"{otherLineStyle}{m}{Reset}";

                ConsoleMultilineMessage(message, AppendFirstLineFormatter, OtherLineFormatter);
            }
        }
        else if (exception is null)
        {
            string FirstLineFormatter(string m, int i) => logLevel is LogLevel.Critical
                ? $"{firstLineStyle}{m.PadRight(SafeGetSpaceCount(" ", EmptyTag))}{Reset}"
                : $"{firstLineStyle}{m}{Reset}";

            string OtherLineFormatter(string m, int i) => logLevel is LogLevel.Critical
                ? $"{otherLineStyle}{m.PadRight(SafeGetSpaceCount(" ", EmptyTag))}{Reset}"
                : $"{otherLineStyle}{m}{Reset}";

            ConsoleMultilineMessage(message, FirstLineFormatter, OtherLineFormatter);
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

            string FirstLineFormatter(string m, int i) => logLevel is LogLevel.Critical
                ? $"{firstLineStyle}{m.PadRight(SafeGetSpaceCount(" ", EmptyTag))}{Reset}"
                : $"{firstLineStyle}{m}{Reset}";

            string OtherLineFormatter(string m, int i) => (logLevel, i) switch
            {
                (LogLevel.Critical, 1) => $"{otherLineStyle}{tag}{m.PadRight(SafeGetSpaceCount(" ", EmptyTag, EmptyExceptionTag))}{Reset}",
                (LogLevel.Critical, _) => $"{otherLineStyle}{m.PadRight(SafeGetSpaceCount(" ", EmptyTag))}{Reset}",
                _ => $"{otherLineStyle}{m}{Reset}",
            };

            ConsoleMultilineMessage($"""
                {message}
                {exception}
                """, FirstLineFormatter, OtherLineFormatter);
        }
    });

    /// <summary>
    /// 清空当前行并移动光标到上一行。
    /// </summary>
    /// <param name="repeatCount">此移动光标，是因为日志已重复第几次。</param>
    private void ClearAndMoveToLastLine(int repeatCount)
    {
        if (!_isCursorMovementEnabled)
        {
            // 如果光标控制不可用，或者还没有重复次数，则不尝试移动光标。
            return;
        }

        var width = SafeGetBufferWidth();
        const string cursorPreviousLine = "\e[1F";
        const string cursorHorizontalAbsolute0 = "\e[0G";
#if NETCOREAPP3_1_OR_GREATER
        Span<char> builder = stackalloc char[cursorPreviousLine.Length + width + cursorHorizontalAbsolute0.Length];
        cursorPreviousLine.CopyTo(builder);
        for (var i = cursorPreviousLine.Length; i < cursorPreviousLine.Length + width; i++)
        {
            builder[cursorPreviousLine.Length + i] = ' ';
        }
        cursorHorizontalAbsolute0.CopyTo(builder.Slice(cursorPreviousLine.Length + width));
        Out.Write(builder.ToString());
#else
        Out.Write($"{cursorPreviousLine}{new string(' ', width)}{cursorHorizontalAbsolute0}");
#endif
    }

    /// <summary>
    /// 记录多行日志。
    /// </summary>
    /// <param name="message">多行日志消息内容。</param>
    /// <param name="firstLineFormatter">首行格式化。</param>
    /// <param name="otherLineFormatter">其他行格式化（如果设置为 <see langword="null"/> 则与首行相同。</param>
    /// <param name="forceSingleLine">将多行消息内容视为单行内容记录。</param>
    private void ConsoleMultilineMessage(string message,
        Func<string, int, string?> firstLineFormatter, Func<string, int, string?>? otherLineFormatter = null, bool forceSingleLine = false)
    {
        if (forceSingleLine || !message.Contains('\n'))
        {
            SafeWriteLine(firstLineFormatter(message, 0));
        }
        else
        {
            var lineIndex = 0;
            using var reader = new StringReader(message);
            while (reader.ReadLine() is { } line)
            {
                var formatter = lineIndex is 0
                    ? firstLineFormatter
                    : otherLineFormatter ?? firstLineFormatter;
                SafeWriteLine(formatter(line, lineIndex));
                lineIndex++;
            }
        }
    }

    /// <summary>
    /// 安全地写入一行日志，避免出现 IO 异常。
    /// </summary>
    /// <param name="message"></param>
    internal static void SafeWriteLine(string? message)
    {
        try
        {
            Out.WriteLine(message);
        }
        catch (IOException)
        {
            // 如果写入日志时发生 IO 异常，已经没有办法救活了。
            // 这是日志系统的内部实现，所以日志也记不了了，汗……
        }
    }

    /// <summary>
    /// 安全地获取控制台的宽度，避免出现 IO 异常。
    /// </summary>
    /// <returns>控制台的宽度。如果不支持获取宽度，则返回 0。</returns>
    private int SafeGetBufferWidth()
    {
        try
        {
            return _isCursorMovementEnabled ? Console.WindowWidth : 0;
        }
        catch (IOException)
        {
            return 0;
        }
    }

    /// <summary>
    /// 安全地获取填充控制台行所需的剩余空白字符数量，避免出现 IO 异常或超出控制台宽度的情况。<br/>
    /// </summary>
    /// <param name="messages">填充剩余字符数量前，该行会输出的字符串。</param>
    /// <returns>填充控制台行所需的剩余字符数量。</returns>
    private int SafeGetSpaceCount(params
#if NET6_0_OR_GREATER
        ReadOnlySpan<string>
#else
        string[]
#endif
        messages)
    {
        var width = SafeGetBufferWidth();
        if (width == 0)
        {
            return 0;
        }

        var paddingWidth = width;

        for (var i = 0; i < messages.Length; i++)
        {
            paddingWidth -= messages[i].Length;
        }

        if (paddingWidth < 0)
        {
            return 0;
        }

        return paddingWidth;
    }

    /// <summary>
    /// 获取标准输出的写入流。<br/>
    /// 如果当前在控制台中输出，则使用控制台的输出流；否则创建一个 UTF-8 编码的标准输出流。
    /// </summary>
    /// <returns>文本写入流。</returns>
    private static TextWriter GetStandardOutputWriter()
    {
        if (Console.OutputEncoding.CodePage is not 0)
        {
            return Console.Out;
        }

        var standardOutput = Console.OpenStandardOutput();
        var writer = new StreamWriter(standardOutput, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false))
        {
            AutoFlush = true,
        };
        return writer;
    }

    private const string Reset = C.Reset;
    private const string TraceText = F.BrightBlack;
    private const string DebugText = F.Magenta;
    private const string InformationText = F.White;
    private const string WarningText = F.Yellow;
    private const string ErrorText = F.Red;
    private const string CriticalText = $"{B.Red}{F.Black}";

    private static string TraceTag => $"{F.BrightBlack}[{DateTime.Now:HH:mm:ss.fff}]{Reset}";
    private static string TraceEmptyTag => $"{F.BrightBlack}              {Reset}";
    private static string DebugTag => $"{F.Magenta}[{DateTime.Now:HH:mm:ss.fff}]{Reset}";
    private static string DebugEmptyTag => $"{F.Magenta}              {Reset}";
    private static string InformationTag => $"{B.Green}{F.Black}[{DateTime.Now:HH:mm:ss.fff}]{Reset}";
    private static string InformationEmptyTag => $"{B.Green}{F.Black}              {Reset}";
    private static string WarningTag => $"{B.Yellow}{F.Black}[{DateTime.Now:HH:mm:ss.fff}]{Reset}";
    private static string WarningEmptyTag => $"{B.Yellow}{F.Black}              {Reset}";
    private static string ErrorTag => $"{B.Red}{F.Black}[{DateTime.Now:HH:mm:ss.fff}]{Reset}";
    private static string ErrorEmptyTag => $"{B.Red}{F.Black}              {Reset}";
    private static string CriticalTag => $"{B.Black}{D.Bold}{F.Red}[{DateTime.Now:HH:mm:ss.fff}]{Reset}";
    private static string CriticalEmptyTag => $"{B.Black}{D.Bold}{F.Red}              {Reset}";
    private static string EmptyTag => "              ";

    private static string WarningExceptionTag => $"{B.Yellow}{F.Black} ! {Reset}{WarningText} ";
    private static string ErrorExceptionTag => $"{B.Red}{F.Black} X {Reset}{ErrorText} ";
    private static string CriticalExceptionTag => $"{B.Black}{D.Bold}{F.Red} X {Reset}{CriticalText} ";
    private static string EmptyExceptionTag => "    ";
}
