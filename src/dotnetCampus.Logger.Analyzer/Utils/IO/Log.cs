using System;
using System.Diagnostics;
using System.IO;

// ReSharper disable CheckNamespace

namespace dotnetCampus.Logger;

internal static class Log
{
    [Conditional("DEBUG")]
    internal static void Debug(string message)
    {
        Debugger.Launch();
    }
}
