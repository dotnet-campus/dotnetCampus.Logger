using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace dotnetCampus.Logging
{
    /// <summary>
    /// 提供记录日志的方法。
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 当实现 <see cref="ILogger"/> 接口时，请在此方法中实现写入日志。
        /// </summary>
        /// <param name="context"></param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        void Log(in LogContext context);
    }
}
