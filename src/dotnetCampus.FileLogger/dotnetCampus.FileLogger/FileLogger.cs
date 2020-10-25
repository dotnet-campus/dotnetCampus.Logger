using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using dotnetCampus.Threading;

namespace dotnetCampus.FileLogger
{
    public class FileLogger : IAsyncDisposable
    {
        public FileLogger()
        {
            DoubleBufferTask = new DoubleBufferLazyInitializeTask<string>(WriteFile);
        }

        public FileLogger(FileInfo logFile) : this()
        {
            SetLogFile(logFile);
        }

        public void SetLogFile(FileInfo logFile)
        {
            if (LogFile != null)
            {
                throw new InvalidOperationException($"重复多次设置日志文件");
            }

            LogFile = logFile;
            DoubleBufferTask.OnInitialized();

            IsInitialized = true;
        }

        public bool IsInitialized { private set; get; } = false;

        public FileInfo LogFile { private set; get; } = null!;

        private DoubleBufferLazyInitializeTask<string> DoubleBufferTask { get; }

        public async ValueTask DisposeAsync()
        {
            DoubleBufferTask.Finish();
            await DoubleBufferTask.WaitAllTaskFinish();
        }

        public void WriteLog(string logMessage)
        {
            DoubleBufferTask.AddTask(logMessage);
        }

        private Task WriteFile(List<string> logList)
        {
            // 最多尝试写10次日志
            for (var i = 0; i < 10; i++)
            {
                try
                {
                    return File.AppendAllLinesAsync(LogFile.FullName, logList);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("[FileLogger] {0}", e);
                }

                Debug.WriteLine("[FileLogger] Retry count {0}", i);
            }

            return Task.CompletedTask;
        }
    }
}