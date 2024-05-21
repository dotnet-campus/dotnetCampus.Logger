using System.Collections.Generic;
using SMath = System.Math;

namespace dotnetCampus.Logging.Properties;

internal static class Compatibility
{
#if NET8_0_OR_GREATER
#else
    internal static ImmutableHashSetString ToImmutableHashSet(this IEnumerable<string> source)
    {
        return [..source];
    }
#endif

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

    public static int Clamp(int value, int min, int max)
    {
        return SMath.Max(min, SMath.Min(max, value));
    }
#endif
}
