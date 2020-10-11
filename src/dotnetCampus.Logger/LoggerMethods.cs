using dotnetCampus.Logging.Core;

using System;
using System.Runtime.CompilerServices;

namespace dotnetCampus.Logging
{
    /// <summary>
    /// 为 <see cref="ILogger"/> 接口的使用提供扩展方法。
    /// </summary>
    public static class LoggerMethods
    {
        /// <summary>
        /// 记录每一个方法的执行分支，确保仅通过日志文件就能还原代码的执行过程。
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> 的实例。</param>
        /// <param name="text">描述当前步骤正准备做什么。如果某个步骤耗时较长或容易出现异常，建议在结束后也记录一次。</param>
        /// <param name="callerMemberName">编译器自动传入。</param>
        public static void Trace(this ILogger logger, string text, [CallerMemberName] string? callerMemberName = null)
            => LogCore(logger, text, LogLevel.Detail, null, callerMemberName);

        /// <summary>
        /// 记录一个关键步骤执行完成之后的摘要，部分耗时的关键步骤也需要在开始之前记录一些摘要。
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> 的实例。</param>
        /// <param name="text">
        /// 描述当前步骤完成之后做了什么关键性的更改，关键的状态变化是什么。
        /// 描述当前步骤开始之前程序是一个什么样的状态，关键的状态是什么。
        /// </param>
        /// <param name="callerMemberName">编译器自动传入。</param>
        public static void Message(this ILogger logger, string text, [CallerMemberName] string? callerMemberName = null)
            => LogCore(logger, text, LogLevel.Message, null, callerMemberName);

        /// <summary>
        /// 如果方法进入了非预期的分支，请调用此方法以便在日志文件中高亮显示。
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> 的实例。</param>
        /// <param name="text">描述当前进入的代码分支。</param>
        /// <param name="callerMemberName">编译器自动传入。</param>
        public static void Warning(this ILogger logger, string text, [CallerMemberName] string? callerMemberName = null)
            => LogCore(logger, text, LogLevel.Warning, null, callerMemberName);

        /// <summary>
        /// 在单独的日志文件中记录异常，并同时在普通的日志文件中插入一段高亮显示的日志。
        /// 请注意，并不是所有的异常都需要调用此方法记录，此方法仅仅记录非预期的异常。
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> 的实例。</param>
        /// <param name="text">对当前异常的文字描述。</param>
        /// <param name="exception">异常实例。</param>
        /// <param name="callerMemberName">编译器自动传入。</param>
        public static void Error(this ILogger logger, string text, Exception? exception = null, [CallerMemberName] string? callerMemberName = null)
            => LogCore(logger, text, LogLevel.Error, exception?.ToString(), callerMemberName);

        /// <summary>
        /// 在单独的日志文件中记录异常，并同时在普通的日志文件中插入一段高亮显示的日志。
        /// 请注意，并不是所有的异常都需要调用此方法记录，此方法仅仅记录非预期的异常。
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> 的实例。</param>
        /// <param name="exception">异常实例。</param>
        /// <param name="text">对当前异常的文字描述。</param>
        /// <param name="callerMemberName">编译器自动传入。</param>
        public static void Error(this ILogger logger, Exception exception, string? text = null, [CallerMemberName] string? callerMemberName = null)
            => LogCore(logger, text, LogLevel.Error, exception.ToString(), callerMemberName);

        /// <summary>
        /// 在单独的日志文件中记录一条导致致命性错误的异常，并同时在普通的日志文件中插入一段高亮显示的致命错误标记。
        /// 请注意，仅在全局区域记录此异常，全局区域如果还能收到异常说明方法内部有未处理的异常。
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> 的实例。</param>
        /// <param name="exception">异常实例。</param>
        /// <param name="text">对当前异常的文字描述。</param>
        /// <param name="callerMemberName">编译器自动传入。</param>
        public static void Fatal(this ILogger logger, Exception exception, string text, [CallerMemberName] string? callerMemberName = null)
            => LogCore(logger, text, LogLevel.Error, exception.ToString(), callerMemberName);

        /// <summary>
        /// 使用底层的日志记录方法来异步记录日志。
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> 的实例。</param>
        /// <param name="text">要记录的日志的文本。</param>
        /// <param name="currentLevel">要记录的当条日志等级。</param>
        /// <param name="extraInfo">如果此条日志包含额外的信息，则在此传入额外的信息。</param>
        /// <param name="callerMemberName">此参数由编译器自动生成，请勿传入。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void LogCore(ILogger logger, string? text, LogLevel currentLevel,
            string? extraInfo, [CallerMemberName] string? callerMemberName = null)
        {
            if (callerMemberName is null)
            {
                throw new ArgumentNullException(nameof(callerMemberName), "不允许显式将 CallerMemberName 指定成 null。");
            }

            if (string.IsNullOrWhiteSpace(callerMemberName))
            {
                throw new ArgumentException("不允许显式将 CallerMemberName 指定成空字符串。", nameof(callerMemberName));
            }

            if (logger is OutputLogger ol && ol.Level < currentLevel
                || logger is AsyncOutputLogger aol && aol.Level < currentLevel)
            {
                return;
            }

            logger.Log(new LogContext(DateTimeOffset.Now, callerMemberName, text ?? "", extraInfo, currentLevel));
        }
    }
}
