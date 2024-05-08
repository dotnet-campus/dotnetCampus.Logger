using System;
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
        var generatedProgramNamespace = programTypeSymbol.ContainingNamespace.ToDisplayString();

        var templatesFolder = templateProgramNamespace.AsSpan().Slice(AssemblyInfo.RootNamespace.Length + 1).ToString();
        foreach (var template in EmbeddedSourceFiles
                     .Enumerate(templatesFolder)
                     .Where(x => x.FileName.StartsWith("Program.")))
        {
            var generatedText = template.Content
                .Replace(templateProgramNamespace, generatedProgramNamespace)
                .Replace("Program", programTypeSymbol.Name);

            context.AddSource(template.FileName, SourceText.From(generatedText, Encoding.UTF8));
        }
    }
}
