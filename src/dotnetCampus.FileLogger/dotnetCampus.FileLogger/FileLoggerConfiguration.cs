using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCampus.FileLogger
{
    public class FileLoggerConfiguration
    {
        public uint MaxWriteLogFileRetryCount { set; get; } = DefaultMaxWriteLogFileRetryCount;

        public const uint DefaultMaxWriteLogFileRetryCount = 10;

        public TimeSpan RetryDelayTime { set; get; } = DefaultRetryDelayTime;

        public static readonly TimeSpan DefaultRetryDelayTime = TimeSpan.FromMilliseconds(100);

        public FileInfo LogFile { set; get; } = null!;

        public uint NotifyMinWriteTextCount { set; get; } = DefaultNotifyMinWriteTextCount;

        public const uint DefaultNotifyMinWriteTextCount = 5 * 1024 * 1024;// 大约是 5-10M 左右

        public FileLoggerConfiguration Clone()
        {
            var fileLoggerConfiguration = (FileLoggerConfiguration)MemberwiseClone();
            fileLoggerConfiguration.LimitTextCountFilterList = LimitTextCountFilterList.ToList();
            return fileLoggerConfiguration;
        }

        public List<ILimitTextCountFilter> LimitTextCountFilterList { private set;get; } = new List<ILimitTextCountFilter>();
    }

    public interface ILimitTextCountFilter
    {
        Task FilterLogFile(FileInfo logFile);
    }
}