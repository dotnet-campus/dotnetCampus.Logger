using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using dotnetCampus.Logger.Analyzer.Templates;
using dotnetCampus.Logger.Analyzer.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace dotnetCampus.Logger.Analyzer;

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

    /// <summary>
    /// 从语法上判断一个方法声明是否符合成为 Main 方法的要求。
    /// </summary>
    /// <param name="methodNode">语法树中的方法声明。</param>
    /// <returns>是否符合成为 Main 方法的要求。</returns>
    private static bool CheckCanBeProgramMain(MethodDeclarationSyntax methodNode)
    {
        var methodName = methodNode.Identifier.Text;
        if (methodName != "Main")
        {
            // 名称必须是 Main。
            return false;
        }

        if (methodNode.Modifiers.Any(SyntaxKind.StaticKeyword) == false)
        {
            // 必须是静态方法。
            return false;
        }

        if (methodNode.ParameterList.Parameters.Count > 1)
        {
            // 最多只能有一个参数。
            return false;
        }

        if (methodNode.ParameterList.Parameters.Count == 1)
        {
            var parameter = methodNode.ParameterList.Parameters[0];
            if (parameter.Type is not ArrayTypeSyntax { ElementType: PredefinedTypeSyntax spts })
            {
                // 参数必须是预定义类型。
                return false;
            }

            if (!spts.Keyword.IsKind(SyntaxKind.StringKeyword))
            {
                // 参数类型必须是 string[] 或 System.String[]。
                return false;
            }
        }

        if (methodNode.ReturnType is not PredefinedTypeSyntax ipts)
        {
            // 返回值必须是预定义类型。
            return false;
        }

        if (!ipts.Keyword.IsKind(SyntaxKind.VoidKeyword) && !ipts.Keyword.IsKind(SyntaxKind.IntKeyword))
        {
            // 返回值必须是 void 或 int。
            return false;
        }

        return true;
    }
}
