using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using dotnetCampus.Logger.Assets.Templates;
using dotnetCampus.Logger.Utils.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static dotnetCampus.Logger.Utils.CodeAnalysis.ProgramMainExtensions;

namespace dotnetCampus.Logger.Generators;

/// <summary>
/// 生成 Program.g.cs，为 Main 方法第一行日志生成支持代码。
/// </summary>
[Generator]
public class ProgramMainLogGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        return;

        var provider = context.SyntaxProvider.CreateSyntaxProvider((node, ct) =>
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

        context.RegisterSourceOutput(provider, Execute);
    }

    private void Execute(SourceProductionContext context, INamedTypeSymbol programTypeSymbol)
    {
        var templateProgramNamespace = typeof(Program).Namespace!;
        var templatesFolder = templateProgramNamespace.AsSpan().Slice(AssemblyInfo.RootNamespace.Length + 1).ToString();
        var embeddedFiles = EmbeddedSourceFiles.Enumerate(templatesFolder).ToImmutableArray();

        // 生成 Program.Logger.g.cs
        var partialLoggerFile = embeddedFiles.First(x => x.FileName.StartsWith("Program.", StringComparison.Ordinal));
        var generatedLoggerText = ConvertPartialProgramLogger(partialLoggerFile.Content, programTypeSymbol);
        context.AddSource($"{programTypeSymbol.Name}.Logger.g.cs", SourceText.From(generatedLoggerText, Encoding.UTF8));
    }

    private string ConvertPartialProgramLogger(string sourceText, INamedTypeSymbol programTypeSymbol)
    {
        var templateProgramNamespace = typeof(Program).Namespace!;
        var generatedProgramNamespace = programTypeSymbol.ContainingNamespace.ToDisplayString();
        return sourceText
            .Replace(templateProgramNamespace, generatedProgramNamespace)
            .Replace("Program", programTypeSymbol.Name);
    }
}
