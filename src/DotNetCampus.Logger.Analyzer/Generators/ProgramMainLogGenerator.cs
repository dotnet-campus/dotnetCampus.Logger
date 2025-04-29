using System.Text;
using DotNetCampus.Logger.Assets.Templates;
using DotNetCampus.Logger.Utils.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using static DotNetCampus.Logger.Utils.CodeAnalysis.ProgramMainExtensions;

namespace DotNetCampus.Logger.Generators;

/// <summary>
/// 生成 Program.g.cs，为 Main 方法第一行日志生成支持代码。
/// </summary>
[Generator]
public class ProgramMainLogGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider((node, ct) =>
        {
            if (node is not MethodDeclarationSyntax mds)
            {
                // 必须是方法声明。
                return false;
            }

            if (!CheckCanBeProgramMain(mds))
            {
                // 必须符合 Main 方法的要求。
                return false;
            }

            if (mds.Parent is not ClassDeclarationSyntax cds)
            {
                // 必须在类中。
                return false;
            }

            if (!cds.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                // 必须是 partial 类。
                return false;
            }

            return true;
        }, (c, ct) =>
        {
            var mainMethodNode = (MethodDeclarationSyntax)c.Node;
            var programClassNode = (ClassDeclarationSyntax)mainMethodNode.Parent!;
            var programTypeSymbol = c.SemanticModel.GetDeclaredSymbol(programClassNode)!;
            return programTypeSymbol;
        });

        context.RegisterSourceOutput(syntaxProvider.Combine(context.AnalyzerConfigOptionsProvider), Execute);
    }

    private void Execute(SourceProductionContext context, (INamedTypeSymbol programTypeSymbol, AnalyzerConfigOptionsProvider analyzerConfigOptions) tuple)
    {
        var (programTypeSymbol, provider) = tuple;

        if (provider.GlobalOptions
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

        // 生成 Program.Logger.g.cs
        var partialLoggerFile = GeneratorInfo.GetEmbeddedTemplateFile<Program>();
        var generatedLoggerText = ConvertPartialProgramLogger(partialLoggerFile.Content, programTypeSymbol);
        context.AddSource($"{programTypeSymbol.Name}.Logger.g.cs", SourceText.From(generatedLoggerText, Encoding.UTF8));
    }

    private string ConvertPartialProgramLogger(string sourceText, INamedTypeSymbol programTypeSymbol)
    {
        var templateProgramNamespace = typeof(Program).Namespace!;
        var generatedProgramNamespace = programTypeSymbol.ContainingNamespace.ToDisplayString();
        return sourceText
            .Replace("global::DotNetCampus.Logging.", $"global::{generatedProgramNamespace}.Logging.")
            .Replace($"namespace {templateProgramNamespace};", $"namespace {generatedProgramNamespace};")
            .Replace("partial class Program", $"partial class {programTypeSymbol.Name}");
    }
}
