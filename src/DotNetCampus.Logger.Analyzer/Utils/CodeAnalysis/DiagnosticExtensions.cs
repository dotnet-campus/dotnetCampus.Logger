using Microsoft.CodeAnalysis;

namespace DotNetCampus.Logger.Utils.CodeAnalysis;

public static class DiagnosticExtensions
{
    public static void ReportUnknownError(this SourceProductionContext context, string message)
    {
        context.ReportDiagnostic(Diagnostic.Create(
            Diagnostics.DL0000_UnknownError,
            null,
            message));
    }
}
