using System.Collections.Immutable;
using System.Text;
using DotNetCampus.Logger.Utils.CodeAnalysis;
using DotNetCampus.Logger.Utils.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace DotNetCampus.Logger.Generators;

/// <summary>
/// 生成一组用于记录日志的代码。
/// </summary>
[Generator]
public class LoggerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.AnalyzerConfigOptionsProvider;
        context.RegisterSourceOutput(provider, Execute);
    }

    private void Execute(SourceProductionContext context, AnalyzerConfigOptionsProvider provider)
    {
        if (provider.GlobalOptions
                .TryGetValue<string>("_DLRootNamespace", out var rootNamespace)
                .TryGetValue<bool>("_DLGenerateSource", out var isGenerateSource)
                is var result
                && !result)
        {
            // 此项目未设置必要的属性（通常这是不应该出现的，因为 buildTransitive 传递的编译目标会自动生成这些属性）。
            return;
        }

        if (!isGenerateSource)
        {
            // 属性设置为不生成源代码。
            return;
        }

        var sourceFiles = EmbeddedSourceFiles.Enumerate("Assets/Sources").ToImmutableArray();

        foreach (var file in sourceFiles)
        {
            var code = GenerateSource(file.TypeName, rootNamespace, file.Content);
            context.AddSource($"{file.TypeName}.g.cs", SourceText.From(code, Encoding.UTF8));
        }
    }

    private string GenerateSource(string typeName, string rootNamespace, string sourceText)
    {
        if (typeName == "Log")
        {
            // 源生成器为单独库生成的代码中，默认日志记录器是 BridgeLogger。
            sourceText = sourceText.Replace("new MemoryCacheLogger();", "new BridgeLogger();");
        }

        if (typeName == "ILoggerBridge")
        {
            // 此类型是 ILoggerBridge，应该保持 public 但变更命名空间。
            return sourceText.ReplaceNamespace($"{rootNamespace}.Logging");
        }

        if (typeName == "BridgeLogger")
        {
            // 此类型是 BridgeLogger，里面用到了 ILoggerBridge，要使用新的命名空间。
            return sourceText.Replace("ILoggerBridge", $"global::{rootNamespace}.Logging.ILoggerBridge");

        }

        // 此类型是 public 的，需要修改为 internal。
        return sourceText.ReplaceTypeModifier("internal");
    }
}
