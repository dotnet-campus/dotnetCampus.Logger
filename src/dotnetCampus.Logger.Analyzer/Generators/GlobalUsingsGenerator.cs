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
        if (provider.GlobalOptions
                .TryGetValue<string>("_DLRootNamespace", out var rootNamespace)
                .TryGetValue<bool>("_DLGenerateSource", out var generateSource)
                .TryGetValue<bool>("_DLGenerateGlobalUsings", out var generateGlobalUsings)
                .TryGetValue<bool>("_DLPreferGeneratedSource", out var preferGeneratedSource)
                is var result
                && !result)
        {
            // 此项目是通过依赖间接引用的，没有 build 因此无法在源生成器中使用编译属性，所以只能选择引用。
            return;
        }

        if (!generateSource || !generateGlobalUsings)
        {
            return;
        }

        var generatedCode = preferGeneratedSource
            ? GenerateGlobalUsings(rootNamespace, preferGeneratedSource)
            : GenerateGlobalUsings("dotnetCampus", preferGeneratedSource);

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
                || sourceFile.Namespace.EndsWith("Sources")
            )
            {
                return $"global using {sourceFile.TypeName} = global::{rootNamespace}.Logging.{sourceFile.TypeName};";
            }

            return null;
        }
    }
}
