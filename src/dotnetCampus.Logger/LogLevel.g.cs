#nullable enable

namespace dotnetCampus.Logging;

/// <summary>
/// 定义日志的严重程度。
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// 包含最详细消息的日志。这些消息可能包含敏感的应用程序数据。默认情况下禁用这些消息，不应在生产环境中启用。
    /// </summary>
    Trace = 0,

    /// <summary>
    /// 用于开发过程中的交互式调查的日志。这些日志应主要包含有用于调试的信息，没有长期价值。
    /// </summary>
    Debug = 1,

    /// <summary>
    /// 跟踪应用程序的一般流程的日志。这些日志应具有长期价值。
    /// </summary>
    Information = 2,

    /// <summary>
    /// 强调应用程序流程中的异常或意外事件的日志，但不会导致应用程序执行停止。
    /// </summary>
    Warning = 3,

    /// <summary>
    /// 强调当前执行流程由于失败而停止的日志。这些日志应指示当前活动的失败，而不是应用程序范围的失败。
    /// </summary>
    Error = 4,

    /// <summary>
    /// 描述不可恢复的应用程序或系统崩溃，或需要立即处理的灾难性失败的日志。
    /// </summary>
    Critical = 5,

    /// <summary>
    /// 此严重程度不用于写入日志消息，配置成此级别仅表示不会写入任何日志。
    /// </summary>
    None = 6,
}
