using dotnetCampus.Logging.Attributes;

namespace LoggerSample.MainApp;

internal partial class Program
{
    public static void Main(string[] args)
    {
    }
}

[ImportLoggerBridge<global::LoggerSample.LoggerIndependentLibrary.Logging.ILoggerBridge>]
internal partial class LoggerBridge;
