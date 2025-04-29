#nullable enable

using LILogger = global::DotNetCampus.Logging.ILogger;
using LLog = global::DotNetCampus.Logging.Log;

namespace DotNetCampus.Logger.Assets.Templates;

partial class Program
{
    /// <summary>
    /// 用于在 <see cref="Program"/> 类的内部记录日志。
    /// </summary>
    /// <remarks>
    /// 由于此代码是源生成器生成的代码，所以可以在日志模块初始化之前记录日志且提前生效。<br/>
    /// 🤩 你甚至能在 Main 方法的第一行就使用它记录日志！
    /// </remarks>
    private static LILogger Log => LLog.Current;
}
