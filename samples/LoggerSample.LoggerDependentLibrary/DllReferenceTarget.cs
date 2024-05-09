using dotnetCampus.Logging;

namespace LoggerSample.LoggerDependentLibrary;

public class DllReferenceTarget
{
    public static void CollectLogs()
    {
        Log.Trace.Trace("[DllReference] Log.Trace.Trace");
        Log.Trace.Debug("[DllReference] Log.Trace.Debug");
        Log.Trace.Info("[DllReference] Log.Trace.Info");
        Log.Trace.Warn("[DllReference] Log.Trace.Warn");
        Log.Trace.Error("[DllReference] Log.Trace.Error");
        Log.Trace.Fatal("[DllReference] Log.Trace.Fatal");

        Log.Debug.Trace("[DllReference] Log.Debug.Trace");
        Log.Debug.Debug("[DllReference] Log.Debug.Debug");
        Log.Debug.Info("[DllReference] Log.Debug.Info");
        Log.Debug.Warn("[DllReference] Log.Debug.Warn");
        Log.Debug.Error("[DllReference] Log.Debug.Error");
        Log.Debug.Fatal("[DllReference] Log.Debug.Fatal");

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
