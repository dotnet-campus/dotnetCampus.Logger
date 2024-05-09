using System;
using System.Collections.Immutable;
using System.Diagnostics;
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

        var useGeneratedLogger = mainlyUseGeneratedLogger.Equals("true", StringComparison.OrdinalIgnoreCase);
        var generatedCode = useGeneratedLogger
            ? GenerateGlobalUsings(rootNamespace, useGeneratedLogger)
            : GenerateGlobalUsings("dotnetCampus", useGeneratedLogger);

        context.AddSource("GlobalUsings.g.cs", SourceText.From(generatedCode, Encoding.UTF8));
    }

    private string GenerateGlobalUsings(string rootNamespace, bool useGeneratedLogger)
    {
        var sourceFiles = EmbeddedSourceFiles.Enumerate("Assets/Sources").ToImmutableArray();

        var globalUsingsCode = GenerateGlobalUsingsForTypes(
            rootNamespace,
            [..sourceFiles],
            useGeneratedLogger);
        return globalUsingsCode;
    }

    private string GenerateGlobalUsingsForTypes(string rootNamespace, ImmutableArray<EmbeddedSourceFile> sourceFiles, bool useGeneratedLogger)
    {
        return $"""
global using global::{rootNamespace}.Logging;

{string.Join("\n", sourceFiles.Select(GenerateTypeUsing).OfType<string>())}

""";

        string? GenerateTypeUsing(EmbeddedSourceFile sourceFile)
        {
            if (
                // 如果使用源生成器的日志系统，则所有类型均要导出全局引用。
                useGeneratedLogger
                // 如果使用源生成器的日志系统，则所有类型均要导出全局引用。
                || sourceFile.Namespace.EndsWith("Sources")
            )
            {
                return $"global using {sourceFile.TypeName} = global::{rootNamespace}.Logging.{sourceFile.TypeName};";
            }

            return null;
        }
    }
}
