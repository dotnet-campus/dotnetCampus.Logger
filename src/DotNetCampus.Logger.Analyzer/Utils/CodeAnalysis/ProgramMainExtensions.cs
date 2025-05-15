using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotNetCampus.Logger.Utils.CodeAnalysis;

public static class ProgramMainExtensions
{
    /// <summary>
    /// 从语法上判断一个方法声明是否符合成为 Main 方法的要求。
    /// </summary>
    /// <param name="methodNode">语法树中的方法声明。</param>
    /// <returns>是否符合成为 Main 方法的要求。</returns>
    public static bool CheckCanBeProgramMain(MethodDeclarationSyntax methodNode)
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
