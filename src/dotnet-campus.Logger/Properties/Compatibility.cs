global using dotnetCampus.Logging.Properties;

namespace dotnetCampus.Logging.Properties;

internal static class Compatibility
{
#if NET6_0_OR_GREATER
#else
    internal static string AsSpan(this string text)
    {
        return text;
    }

    internal static bool Contains(this string text, char value)
    {
        return text.Contains(value.ToString());
    }

    internal static string Slice(this string text, int start, int length)
    {
        return text.Substring(start, length);
    }
#endif
}
