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

        public FileLogger(FileLoggerConfiguration configuration) : this()
        {
            SetConfiguration(configuration);
        }

        public void SetConfiguration(FileLoggerConfiguration configuration)
        {
            if (FileLoggerConfiguration != null)
            {
                throw new InvalidOperationException($"重复多次设置日志文件");
            }

            FileLoggerConfiguration = configuration.Clone();

            DoubleBufferTask.OnInitialized();

            IsInitialized = true;
        }

        private FileLoggerConfiguration FileLoggerConfiguration { set; get; } = null!;

        public bool IsInitialized { private set; get; } = false;

        public FileInfo LogFile => FileLoggerConfiguration.LogFile;

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

        private uint CurrentWriteTextCount { set; get; } = 0;

        private async Task WriteFile(List<string> logList)
        {
            // 最多尝试写10次日志
            var maxWriteLogFileRetryCount = FileLoggerConfiguration.MaxWriteLogFileRetryCount;
            for (var i = 0; i < maxWriteLogFileRetryCount; i++)
            {
                try
                {
                    await File.AppendAllLinesAsync(LogFile.FullName, logList);

                    // 当前写入的数据量
                    foreach (var logText in logList)
                    {
                        CurrentWriteTextCount += (uint)logText.Length;
                    }

                    if (CurrentWriteTextCount > FileLoggerConfiguration.NotifyMinWriteTextCount)
                    {
                        foreach (var limitTextCountFilter in FileLoggerConfiguration.LimitTextCountFilterList)
                        {
                            await limitTextCountFilter.FilterLogFile(LogFile);
                        }
                    }

                    return;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("[FileLogger] {0}", e);
                }

                Debug.WriteLine("[FileLogger] Retry count {0}", i);
                await Task.Delay(FileLoggerConfiguration.RetryDelayTime);
            }

            FileLoggerWriteFailed?.Invoke(this, new FileLoggerWriteFailedArgs(this, FileLoggerConfiguration, logList));
            // 如果超过次数依然写入失败，那就忽略失败了
        }

        public event EventHandler<FileLoggerWriteFailedArgs>? FileLoggerWriteFailed;
    }
}