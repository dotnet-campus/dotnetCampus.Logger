#nullable enable

using EditorBrowsable = global::System.ComponentModel.EditorBrowsableAttribute;
using EditorBrowsableState = global::System.ComponentModel.EditorBrowsableState;
using EventId = global::dotnetCampus.Logging.EventId;
using Exception = global::System.Exception;
using ILogger = global::dotnetCampus.Logging.ILogger;
using LazyThreadSafetyMode = global::System.Threading.LazyThreadSafetyMode;
using LogLevel = global::dotnetCampus.Logging.LogLevel;

namespace dotnetCampus.Logger.Assets.Templates;

partial class Program
{
    /// <summary>
    /// 用于在 <see cref="Program"/> 类的内部记录日志。
    /// </summary>
    /// <remarks>
    /// 由于此代码是源生成器生成的代码，所以可以在日志模块初始化之前记录日志且提前生效。<br/>
    /// 🤩 你甚至能在 Main 方法的第一行就使用它记录日志！
    /// </remarks>
    private static ILogger Log { get; }
}
