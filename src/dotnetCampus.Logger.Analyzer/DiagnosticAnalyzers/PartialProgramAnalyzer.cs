using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace dotnetCampus.Logger.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PartialProgramAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        Diagnostics.DL0101_ProgramIsRecommendedToBePartial
    );

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(AnalyzeProgram, SyntaxKind.MethodDeclaration);
    }

    private void AnalyzeProgram(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is MemberDeclarationSyntax
            {
                Parent: ClassDeclarationSyntax cds,
            } && !cds.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            var spanStart = cds.Modifiers.Count is 0
                ? cds.Keyword.SpanStart
                : cds.Modifiers.Span.Start;
            context.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.DL0101_ProgramIsRecommendedToBePartial,
                Location.Create(context.Node.SyntaxTree, new TextSpan(
                    spanStart,
                    cds.Identifier.Span.End - spanStart
                )),
                cds.Identifier.Text));
        }
    }
}
