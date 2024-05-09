using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using dotnetCampus.Logger.Utils.CodeAnalysis;
using dotnetCampus.Logger.Utils.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace dotnetCampus.Logger.Generators;

/// <summary>
/// 生成一组用于记录日志的代码。
/// </summary>
[Generator]
public class GlobalUsingsGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterSourceOutput(context.AnalyzerConfigOptionsProvider, Execute);
    }

    private void Execute(SourceProductionContext context, AnalyzerConfigOptionsProvider provider)
    {
        provider.GlobalOptions.TryGetValue("build_property.OutputType", out var outputType);
        provider.GlobalOptions.TryGetValue("build_property.RootNamespace", out var rootNamespace);
        provider.GlobalOptions.TryGetValue("build_property._DLMainlyUseGeneratedLogger", out var mainlyUseGeneratedLogger);
        if (outputType is null || rootNamespace is null || mainlyUseGeneratedLogger is null)
        {
            context.ReportUnknownError("NuGet 包中应包含 OutputType、RootNamespace 和 _DLMainlyUseGeneratedLogger 属性。");
            return;
        }

        var useGeneratedLogger = CheckIsUsingGeneratedLogger(mainlyUseGeneratedLogger);
        var generatedCode = useGeneratedLogger
            ? GenerateGlobalUsings(rootNamespace)
            : GenerateGlobalUsings("dotnetCampus");

        context.AddSource("GlobalUsings.g.cs", SourceText.From(generatedCode, Encoding.UTF8));
    }

    private static bool CheckIsUsingGeneratedLogger(string mainlyUseGeneratedLogger)
    {
        return mainlyUseGeneratedLogger.Equals("true", StringComparison.OrdinalIgnoreCase);
    }

    private string GenerateGlobalUsings(string rootNamespace)
    {
        var sourceFiles = EmbeddedSourceFiles.Enumerate("Assets/Sources").ToImmutableArray();

        var globalUsingsCode = GenerateGlobalUsingsForTypes(
            rootNamespace,
            [..sourceFiles.Select(x => x.FileName.Replace(".g.cs", ""))]);
        return globalUsingsCode;
    }

    private string GenerateGlobalUsingsForTypes(string rootNamespace, ImmutableArray<string> relativeTypeNames)
    {
        return $"""
global using global::{rootNamespace}.Logging;

{string.Join("\n", relativeTypeNames.Select(GenerateTypeUsing).OfType<string>())}

""";

        string? GenerateTypeUsing(string relativeTypeName)
        {
            if (relativeTypeName.Contains('.'))
            {
                return null;
            }

            return $"global using {relativeTypeName} = global::{rootNamespace}.Logging.{relativeTypeName};";
        }
    }
}
