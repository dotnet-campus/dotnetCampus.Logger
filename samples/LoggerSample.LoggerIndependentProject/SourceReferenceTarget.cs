using LoggerSample.LoggerIndependentProject.Logging;

namespace LoggerSample.LoggerIndependentProject;

public class SourceReferenceTarget
{
    public static void CollectLogs()
    {
        Log.Trace.Trace("[SourceReference] Log.Trace.Trace");
        Log.Trace.Debug("[SourceReference] Log.Trace.Debug");
        Log.Trace.Info("[SourceReference] Log.Trace.Info");
        Log.Trace.Warn("[SourceReference] Log.Trace.Warn");
        Log.Trace.Error("[SourceReference] Log.Trace.Error");
        Log.Trace.Fatal("[SourceReference] Log.Trace.Fatal");

        Log.Debug.Trace("[SourceReference] Log.Debug.Trace");
        Log.Debug.Debug("[SourceReference] Log.Debug.Debug");
        Log.Debug.Info("[SourceReference] Log.Debug.Info");
        Log.Debug.Warn("[SourceReference] Log.Debug.Warn");
        Log.Debug.Error("[SourceReference] Log.Debug.Error");
        Log.Debug.Fatal("[SourceReference] Log.Debug.Fatal");

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
