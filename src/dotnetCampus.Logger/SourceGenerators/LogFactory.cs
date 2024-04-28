namespace dotnetCampus.Logging.SourceGenerators;

public static class LogFactory
{
    private static ILoggerBridge? _bridge;

    internal static ILoggerBridge? TryGetBridge()
    {
        return _bridge;
    }

    public static void Bridge(ILoggerBridge bridge)
    {
        _bridge = bridge;
    }
}
