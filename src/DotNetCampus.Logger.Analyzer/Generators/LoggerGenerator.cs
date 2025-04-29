using System.Linq;
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
        var analyzerConfigOptionsProvider = context.AnalyzerConfigOptionsProvider;
        var compilationProvider = context.CompilationProvider;

        context.RegisterSourceOutput(analyzerConfigOptionsProvider.Combine(compilationProvider), Execute);
    }

    private void Execute(SourceProductionContext context, (AnalyzerConfigOptionsProvider Left, Compilation Right) args)
    {
        var (provider, compilation) = args;
        if (provider.GlobalOptions
                .TryGetValue<string>("_DLRootNamespace", out var rootNamespace)
                .TryGetValue<bool>("DCUseGeneratedLogger", out var useGeneratedLogger)
                is var result
                && !result)
        {
            // 此项目未设置必要的属性（通常这是不应该出现的，因为 buildTransitive 传递的编译目标会自动生成这些属性）。
            return;
        }


        var logTypes = compilation.GetTypesByMetadataName("DotNetCampus.Logging.Log")
            .Where(x => x.DeclaredAccessibility is Accessibility.Public || x.ContainingAssembly.GivesAccessTo(compilation.Assembly))
            .ToArray();
        var assemblies = logTypes
            .Select(x => x.ContainingAssembly.ToDisplayString())
            .ToArray();
        var isTypeVisible = logTypes.Length is 1;

        var sourceFiles = EmbeddedSourceFiles.Enumerate("Assets/Sources").ToArray();

        foreach (var file in sourceFiles)
        {
            var originalTypeFullType = $"{file.Namespace.Replace("DotNetCampus.Logger", "DotNetCampus.Logging")}.{file.TypeName}";
            var generatedTypeFullName = originalTypeFullType == "DotNetCampus.Logging.Bridges.ILoggerBridge"
                ? $"{rootNamespace}.Logging.{file.TypeName}"
                : originalTypeFullType;

            var code = GenerateSource(file.TypeName, rootNamespace, file.Content);

            code = (useGeneratedLogger, isTypeDefined: isTypeVisible) switch
            {
                (true, true) => $"""
// 此文件所涉及的类型 {originalTypeFullType} 已经在依赖程序集中定义，并被传递给当前程序集；为避免冲突，当前程序集不再生成 {generatedTypeFullName} 类型。
{string.Join("\n", assemblies.Select(x => $"//  - {x}"))}

/*
{code}
*/
""",
                (true, false) => code,
                (false, true) =>  $"""
// 因为 DCUseGeneratedLogger 未被设置为 true，所以不会生成 {generatedTypeFullName} 类型。
// 此文件所涉及的类型已经在依赖程序集中定义，并被传递给当前程序集。
{string.Join("\n", assemblies.Select(x => $"//  - {x}"))}

/*
{code}
*/
""",
                (false, false) =>  $"""
// 因为 DCUseGeneratedLogger 未被设置为 true，所以不会生成 {generatedTypeFullName} 类型。

/*
{code}
*/
"""
            };

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
            // 此类型是 ILoggerBridge，应该修改为 public（引用不公开，源要公开），且变更命名空间。
            return sourceText.ReplaceTypeModifier("public").ReplaceNamespace($"{rootNamespace}.Logging");
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
