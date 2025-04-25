using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using static Windows.Win32.System.Console.CONSOLE_MODE;
using static Windows.Win32.PInvoke;
using static Windows.Win32.System.Console.STD_HANDLE;

namespace dotnetCampus.Logging.Writers.ConsoleLoggerHelpers;

internal static class ConsoleInitializer
{
    internal static bool Initialize()
    {
#if NET6_0_OR_GREATER
        // 最新的 .NET 可以在编译时判断。
        if (OperatingSystem.IsWindows())
#elif NETFRAMEWORK
        // 更早版本的 .NET Framework 只能运行在 Windows 上。
        if (true)
#else
        // 运行时判断。
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
#endif
        {
            return InitWindows();
        }
        else
        {
            return true;
        }
    }

#if NET6_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool InitWindows()
    {
        var iStdOut = GetStdHandle_SafeHandle(STD_OUTPUT_HANDLE);
        if (!GetConsoleMode(iStdOut, out var outConsoleMode))
        {
            return false;
        }

        outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
        if (!SetConsoleMode(iStdOut, outConsoleMode))
        {
            return false;
        }

        return true;
    }
}
