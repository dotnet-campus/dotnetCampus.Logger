using System;
using System.IO;
using System.Threading.Tasks;

namespace dotnetCampus.FileLogger
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var logFile = new FileInfo("log.txt");

            if (logFile.Exists)
            {
                File.Delete(logFile.FullName);
            }

            var fileLogger = new FileLogger();

            for (int i = 0; i < 100; i++)
            {
                fileLogger.WriteLog(i.ToString());
            }

            fileLogger.SetLogFile(logFile);

            await fileLogger.DisposeAsync();

            if (File.Exists(logFile.FullName))
            {

            }
        }
    }
}
