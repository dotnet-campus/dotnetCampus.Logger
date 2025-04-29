using DotNetCampus.Logging;

namespace LoggerSample.LoggerDependentLibrary;

public class DllReferenceTarget
{
    public static void CollectLogs()
    {
        Log.TraceLogger.Trace("[DllReference] Log.Trace.Trace");
        Log.TraceLogger.Debug("[DllReference] Log.Trace.Debug");
        Log.TraceLogger.Info("[DllReference] Log.Trace.Info");
        Log.TraceLogger.Warn("[DllReference] Log.Trace.Warn");
        Log.TraceLogger.Error("[DllReference] Log.Trace.Error");
        Log.TraceLogger.Fatal("[DllReference] Log.Trace.Fatal");

        Log.DebugLogger.Trace("[DllReference] Log.Debug.Trace");
        Log.DebugLogger.Debug("[DllReference] Log.Debug.Debug");
        Log.DebugLogger.Info("[DllReference] Log.Debug.Info");
        Log.DebugLogger.Warn("[DllReference] Log.Debug.Warn");
        Log.DebugLogger.Error("[DllReference] Log.Debug.Error");
        Log.DebugLogger.Fatal("[DllReference] Log.Debug.Fatal");

        Log.Trace("[DllReference] Log.Trace");
        Log.Debug("[DllReference] Log.Debug");
        Log.Info("[DllReference] Log.Info");
        Log.Warn("[DllReference] Log.Warn");
        Log.Error("[DllReference] Log.Error");
        Log.Fatal("[DllReference] Log.Fatal");

        Log.Current.Trace("[DllReference] Log.Current.Trace");
        Log.Current.Debug("[DllReference] Log.Current.Debug");
        Log.Current.Info("[DllReference] Log.Current.Info");
        Log.Current.Warn("[DllReference] Log.Current.Warn");
        Log.Current.Error("[DllReference] Log.Current.Error");
        Log.Current.Fatal("[DllReference] Log.Current.Fatal");
    }
}
