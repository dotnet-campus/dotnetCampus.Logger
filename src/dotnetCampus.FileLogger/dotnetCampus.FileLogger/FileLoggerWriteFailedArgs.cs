using System;
using System.Collections.Generic;

namespace dotnetCampus.FileLogger
{
    public class FileLoggerWriteFailedArgs : EventArgs
    {
        public FileLoggerWriteFailedArgs(FileLogger fileLogger, FileLoggerConfiguration fileLoggerConfiguration,
            IReadOnlyList<string> logList)
        {
            FileLoggerConfiguration = fileLoggerConfiguration;
            FileLogger = fileLogger;
            LogList = logList;
        }

        public FileLoggerConfiguration FileLoggerConfiguration { get; }

        public FileLogger FileLogger { get; }

        public IReadOnlyList<string> LogList { get; }
    }
}