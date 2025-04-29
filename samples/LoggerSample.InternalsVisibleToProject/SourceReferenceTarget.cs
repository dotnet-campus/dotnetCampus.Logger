using dotnetCampus.Logging;

namespace LoggerSample.InternalsVisibleToProject;

public class SourceReferenceTarget
{
    public static void CollectLogs()
    {
        Log.TraceLogger.Trace("[SourceReference] Log.Trace.Trace");
        Log.TraceLogger.Debug("[SourceReference] Log.Trace.Debug");
        Log.TraceLogger.Info("[SourceReference] Log.Trace.Info");
        Log.TraceLogger.Warn("[SourceReference] Log.Trace.Warn");
        Log.TraceLogger.Error("[SourceReference] Log.Trace.Error");
        Log.TraceLogger.Fatal("[SourceReference] Log.Trace.Fatal");

        Log.DebugLogger.Trace("[SourceReference] Log.Debug.Trace");
        Log.DebugLogger.Debug("[SourceReference] Log.Debug.Debug");
        Log.DebugLogger.Info("[SourceReference] Log.Debug.Info");
        Log.DebugLogger.Warn("[SourceReference] Log.Debug.Warn");
        Log.DebugLogger.Error("[SourceReference] Log.Debug.Error");
        Log.DebugLogger.Fatal("[SourceReference] Log.Debug.Fatal");

        Log.Trace("[SourceReference] Log.Trace");
        Log.Debug("[SourceReference] Log.Debug");
        Log.Info("[SourceReference] Log.Info");
        Log.Warn("[SourceReference] Log.Warn");
        Log.Error("[SourceReference] Log.Error");
        Log.Fatal("[SourceReference] Log.Fatal");

        Log.Current.Trace("[SourceReference] Log.Current.Trace");
        Log.Current.Debug("[SourceReference] Log.Current.Debug");
        Log.Current.Info("[SourceReference] Log.Current.Info");
        Log.Current.Warn("[SourceReference] Log.Current.Warn");
        Log.Current.Error("[SourceReference] Log.Current.Error");
        Log.Current.Fatal("[SourceReference] Log.Current.Fatal");
    }
}
